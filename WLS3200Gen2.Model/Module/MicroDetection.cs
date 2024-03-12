using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Model;
using YuanliCore.Model.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class MicroDetection
    {

        private PauseTokenSource pauseToken;
        private CancellationTokenSource cancelToken;
        private Subject<BitmapSource> subject = new Subject<BitmapSource>();
        private IDisposable camlive;


        private OpticalAlignment opticalAlignment;


        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes, DigitalOutput[] outputs, DigitalInput[] inputs)
        {

            this.Microscope = microscope;
            AxisX = axes[0];
            AxisY = axes[1];
            AxisR = axes[3];
            AxisZ = axes[2];
            TableVacuum = outputs[1];
            LiftPin = outputs[2];
            Camera = camera;
            ObservableDetection();


            opticalAlignment = new OpticalAlignment(AxisX, AxisY, Camera);
            opticalAlignment.FiducialRecord += AlignRecord;
        }
        public ICamera Camera { get; }
        public Axis AxisX { get; }
        public Axis AxisY { get; }
        public Axis AxisR { get; }
        public Axis AxisZ { get; }
        public DigitalOutput TableVacuum { get; }
        public DigitalOutput LiftPin { get; }
        public IMicroscope Microscope { get; }

        public DetectionRecipe DetectionRecipe { set; get; }
        public IObservable<BitmapSource> Observable => subject;
        /// <summary>
        /// 流程動作文字記錄
        /// </summary>
        public event Action<string> WriteLog;
        /// <summary>
        /// 對位結果紀錄 (圖像 對位座標(pxel))
        /// </summary>
        public event Action<BitmapSource , Point?,int> FiducialRecord;
        /// <summary>
        /// 檢測結果紀錄
        /// </summary>
        public event Action<BitmapSource> DetectionRecord;

        public async Task Home()
        {
            try
            {
                //Task axisZHome = Task.Run(async () =>
                //{
                //    await AxisZ.HomeAsync();
                //});
                //await Task.Run(() => { });
                WriteLog?.Invoke("Micro Homing Start");

                await AxisZ.HomeAsync();

                WriteLog?.Invoke("AxisZ Homing End");

                Task axisXHome = AxisX.HomeAsync();

                Task axisYHome = AxisY.HomeAsync();

                Task axisRHome = AxisR.HomeAsync();

                Task microscopeHome1 = Microscope.HomeAsync();

                await Task.WhenAll(axisXHome, axisYHome, axisRHome, microscopeHome1);

                WriteLog?.Invoke("Micro Homing End");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 接料準備動作
        /// </summary>
        /// <param name="tableWaferCatchPosition"></param>
        /// <param name="pst"></param>
        /// <param name="ctk"></param>
        /// <returns></returns>
        public async Task CatchWaferPrepare(Point tableWaferCatchPosition, PauseTokenSource pst, CancellationTokenSource ctk)
        {
            try
            {
                await TableMoveToAsync(tableWaferCatchPosition);
            }
            catch (Exception ex)
            {

                throw ex;
            }


            //如果有lift 或夾持機構 需要做處理

        }

        /// <summary>
        /// 出料準備動作
        /// </summary>
        /// <param name="tableWaferCatchPosition"></param>
        /// <param name="pst"></param>
        /// <param name="ctk"></param>
        /// <returns></returns>
        public async Task PutWaferPrepare(Point tableWaferCatchPosition)
        {

            await TableMoveToAsync(tableWaferCatchPosition);
            TableVacuum.Off();
            //如果有lift 或夾持機構 需要做處理

        }


        public async Task Run(DetectionRecipe recipe, bool isAutoSave, PauseTokenSource pst, CancellationTokenSource ctk)
        {
            this.pauseToken = pst;
            this.cancelToken = ctk;
            opticalAlignment.CancelToken = cancelToken;
            opticalAlignment.PauseToken = pauseToken;

            //入料準備?
            //await subject.ToTask();

            cancelToken.Token.ThrowIfCancellationRequested();
            await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);


          
            //對位
            //ITransform transForm = await opticalAlignment.Alignment(recipe.AlignRecipe);
            ITransform transForm = await Alignment(recipe.AlignRecipe);
            cancelToken.Token.ThrowIfCancellationRequested();
            await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);

            //每一個座標需要檢查的座標
            foreach (DetectionPoint point in recipe.DetectionPoints)
            {

                WriteLog?.Invoke($"Move To Detection Position :[{point.IndexX} - {point.IndexY}] ");
                //轉換成對位後實際座標
                var transPosition = transForm.TransPoint(point.Position);

                await TableMoveToAsync(transPosition);
                SetMicroscope(point);

                await Task.Delay(200);
                BitmapSource bmp = Camera.GrabAsync();
             
                if (isAutoSave)
                {
                    subject.OnNext(bmp);//AOI另外丟到其他執行續處理
                }
                else
                {

                }
                DetectionRecord?.Invoke(bmp);
                // pauseToken.IsPaused = true;

                cancelToken.Token.ThrowIfCancellationRequested();
                await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);

            }



        }



        public async Task TableMoveToAsync(Point pos)
        {
            await Task.WhenAll(AxisX.MoveToAsync(pos.X),
                    AxisY.MoveToAsync(pos.Y));
        }
        public async Task TableMoveAsync(Vector distance)
        {
            await Task.WhenAll(AxisX.MoveAsync(distance.X),
                    AxisY.MoveAsync(distance.Y));
        }




        /// <summary>
        /// 檢測功能
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public async Task DefectDetection(BitmapSource bmp)
        {



        }




        /// <summary>
        /// 對位功能
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public async Task<ITransform> Alignment(AlignmentRecipe recipe)
        {
            opticalAlignment.WriteLog = WriteLog;
            WriteLog?.Invoke("Wafer Alignment Start");
            ITransform transForm = await opticalAlignment.Alignment(recipe.FiducialDatas);

            WriteLog?.Invoke("Wafer Alignment End");
            return transForm;


        }

        public async Task<Point> FindFiducial(BitmapSource image, double currentPosX, double currentPosY)
        {
            return await opticalAlignment.FindFiducial(image, currentPosX, currentPosY,0);

        }

        private void SetMicroscope(DetectionPoint detectionPoint)
        {

   
        }
        

        //預留拿到對位結果後 可以做其他事
        private void AlignRecord(BitmapSource bitmap  , Point? pixel,int number)
        {

            FiducialRecord?.Invoke(bitmap, pixel, number);
        }

        private void ObservableDetection()
        {
            camlive = Observable//.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
                    .ObserveOn(TaskPoolScheduler.Default)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
                    .Subscribe(async frame =>
                    {

                        frame.Save("C:\\TEST", ImageFileFormats.Bmp);


                        await DefectDetection(frame);
                    });
        }

    }
}
