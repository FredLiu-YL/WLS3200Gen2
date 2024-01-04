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
        private ICamera camera;
        private PauseTokenSource pauseToken;
        private CancellationTokenSource cancelToken;
        private Subject<(BitmapSource, bool)> subject = new Subject<(BitmapSource, bool)>();
        private IDisposable camlive;


        private OpticalAlignment opticalAlignment;


        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes, DigitalOutput[] outputs, DigitalInput[] inputs)
        {
            this.camera = camera;
            this.Microscope = microscope;
            AxisX = axes[0];
            AxisY = axes[1];
            AxisR = axes[3];
            AxisZ = axes[2];
            TableVacuum = outputs[1];
            LiftPin = outputs[2];

            ObservableDetection();


            opticalAlignment = new OpticalAlignment(AxisX, AxisY, this.camera);
        }

        public Axis AxisX { get; }
        public Axis AxisY { get; }
        public Axis AxisR { get; }
        public Axis AxisZ { get; }
        public DigitalOutput TableVacuum { get; }
        public DigitalOutput LiftPin { get; }
        public IMicroscope Microscope { get; }

        public DetectionRecipe detectionRecipe;
        public IObservable<(BitmapSource image, bool isAutoSave)> Observable => subject;

        public async Task Home()
        {
            try
            {
                //Task axisZHome = Task.Run(async () =>
                //{
                //    await AxisZ.HomeAsync();
                //});
                //await Task.Run(() => { });
                await AxisZ.HomeAsync();

                Task axisXHome = AxisX.HomeAsync();

                Task axisYHome = AxisY.HomeAsync();

                Task axisRHome = AxisR.HomeAsync();

                await Task.WhenAll(axisXHome, axisYHome, axisRHome);
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

            await TableMoveToAsync(tableWaferCatchPosition);

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

            //入料準備?


            cancelToken.Token.ThrowIfCancellationRequested();
            await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);


            //對位
            opticalAlignment.CancelToken = cancelToken;
            opticalAlignment.PauseToken = pauseToken;
            //ITransform transForm = await opticalAlignment.Alignment(recipe.AlignRecipe);
            ITransform transForm = await Alignment(recipe.AlignRecipe);
            cancelToken.Token.ThrowIfCancellationRequested();
            await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);

            //每一個座標需要檢查的座標
            foreach (DetectionPoint point in recipe.DetectionPoints)
            {


                //轉換成對位後實際座標
                var transPosition = transForm.TransPoint(point.Position);

                await TableMoveToAsync(transPosition);
                await Task.Delay(200);
                BitmapSource bmp = camera.GrabAsync();
                subject.OnNext((bmp, isAutoSave));//AOI另外丟到其他執行續處理

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

            ITransform transForm = await opticalAlignment.Alignment(recipe);
            return transForm;
        }


        private void ObservableDetection()
        {
            camlive = Observable//.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
                    .ObserveOn(TaskPoolScheduler.Default)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
                    .Subscribe(async frame =>
                    {
                        if (frame.isAutoSave)
                        {
                            frame.image.Save("C:\\TEST", ImageFileFormats.Bmp);
                        }
                        await DefectDetection(frame.image);
                    });
        }

    }
}
