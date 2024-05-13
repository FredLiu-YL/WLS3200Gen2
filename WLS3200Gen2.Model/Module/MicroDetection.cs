using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.CameraLib;
using YuanliCore.Data;
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
        private Subject<DetectionSaveParam> subject = new Subject<DetectionSaveParam>();
        private IDisposable camlive;


        private OpticalAlignment opticalAlignment;


        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes, DigitalOutput[] outputs, DigitalInput[] inputs, Point PixelTable)
        {
            this.Microscope = microscope;
            AxisX = axes[0];
            AxisY = axes[1];
            AxisR = axes[3];
            AxisZ = axes[2];
            TableVacuum = outputs[0];
            LiftPin = outputs[2];
            Camera = camera;
            ObservableDetection();
            IsTableVacuum = inputs[5];

            opticalAlignment = new OpticalAlignment(AxisX, AxisY, Camera);
            opticalAlignment.PixelTable = PixelTable;
            opticalAlignment.FiducialRecord += AlignRecord;
            opticalAlignment.AlignmentManual += AlignmentManual;
        }
        public bool IsInitial { get; private set; } = false;
        public string GrabSaveTime { get; private set; }
        public ICamera Camera { get; }
        public Axis AxisX { get; }
        public Axis AxisY { get; }
        public Axis AxisR { get; }
        public Axis AxisZ { get; }
        public DigitalOutput TableVacuum { get; }
        public DigitalInput IsTableVacuum { get; }
        public DigitalOutput LiftPin { get; }
        public IMicroscope Microscope { get; }
        public DetectionRecipe DetectionRecipe { set; get; }
        public IObservable<DetectionSaveParam> Observable => subject;
        /// <summary>
        /// 流程動作文字記錄
        /// </summary>
        public event Action<string> WriteLog;
        /// <summary>
        /// 對位結果紀錄 (圖像 對位座標(pxel))
        /// </summary>
        public event Action<BitmapSource, Point?, int> FiducialRecord;
        /// <summary>
        /// 
        /// </summary>
        public event Func<PauseTokenSource, CancellationTokenSource, double, double, Task<Point>> AlignmentError;
        /// <summary>
        /// 檢測結果紀錄
        /// </summary>
        public event Action<BitmapSource, DetectionPoint, Wafer, string, string> DetectionRecord;

        /// <summary> 
        /// 
        /// </summary>
        public event Func<PauseTokenSource, CancellationTokenSource, Task<WaferProcessStatus>> MicroReady;
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

                Camera.Open();

                await Task.WhenAll(axisXHome, axisYHome, axisRHome, microscopeHome1);
                IsInitial = true;
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
                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                await TableMoveToAsync(tableWaferCatchPosition);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //如果有lift 或夾持機構 需要做處理
        }
        public async Task FocusZWaferPrepare(double pos, PauseTokenSource pst, CancellationTokenSource ctk)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                await AxisZ.MoveToAsync(pos);
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
            try
            {
                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                await TableMoveToAsync(tableWaferCatchPosition);
                TableVacuum.Off();
                //如果有lift 或夾持機構 需要做處理
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Run(MainRecipe mainRecipe, ProcessSetting processSetting, Wafer currentWafer, PauseTokenSource pst, CancellationTokenSource ctk)
        {
            try
            {
                DetectionRecipe recipe = mainRecipe.DetectRecipe;
                if (IsTableVacuum.IsSignal)
                {
                    if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");

                    if (pst == null || ctk == null)
                    {
                        pst = new PauseTokenSource();
                        ctk = new CancellationTokenSource();
                        this.pauseToken = pst;
                        this.cancelToken = ctk;
                        opticalAlignment.CancelToken = null;
                        opticalAlignment.PauseToken = null;
                    }
                    else
                    {
                        this.pauseToken = pst;
                        this.cancelToken = ctk;
                        opticalAlignment.CancelToken = cancelToken;
                        opticalAlignment.PauseToken = pauseToken;
                    }


                    //入料準備?
                    //await subject.ToTask();

                    cancelToken.Token.ThrowIfCancellationRequested();
                    await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);

                    //對位
                    //ITransform transForm = await opticalAlignment.Alignment(recipe.AlignRecipe);
                    DetectionPoint detectionPoint = new DetectionPoint();
                    detectionPoint.MicroscopeLightValue = 39;
                    detectionPoint.MicroscopeApertureValue = 700;
                    detectionPoint.LensIndex = 1;
                    detectionPoint.CubeIndex = 1;
                    detectionPoint.Filter1Index = 1;
                    detectionPoint.Filter2Index = 1;
                    detectionPoint.Filter3Index = 1;

                    Task alignmentMicroscopeTask = SetMicroscope(detectionPoint);
                    Task alignmentTableTask = TableMoveToAsync(new Point(recipe.AlignRecipe.FiducialDatas[0].GrabPositionX, recipe.AlignRecipe.FiducialDatas[0].GrabPositionY));
                    await Task.WhenAll(alignmentMicroscopeTask, alignmentTableTask);
                    Microscope.AFTrace();
                    if (true)
                    {

                    }
                    else
                    {
                        ITransform transForm = await Alignment(recipe.AlignRecipe);
                        cancelToken.Token.ThrowIfCancellationRequested();
                        await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);

                        var date = DateTime.Now.Date.ToString("yyyyMMdd-HH-mm");
                        string nowTime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                                         DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                        GrabSaveTime = nowTime;
                        int titleIdx = 0;
                        if (processSetting.IsAutoSave)
                        {
                            //每一個座標需要檢查的座標
                            foreach (DetectionPoint point in recipe.DetectionPoints)
                            {
                                titleIdx += 1;
                                WriteLog?.Invoke($"Move To Detection Position :[{point.IndexX} - {point.IndexY}] ");
                                //轉換成對位後實際座標
                                var newPos = new Point(point.Position.X + recipe.AlignRecipe.OffsetX, point.Position.Y + recipe.AlignRecipe.OffsetY);
                                var transPosition = transForm.TransPoint(newPos);
                                await TableMoveToAsync(transPosition); //Offset
                                await SetMicroscope(point);
                                if (processSetting.IsAutoFocus)
                                {
                                    Microscope.AFTrace();
                                }
                                await Task.Delay(200);
                                DetectionSaveParam detectionSaveParam = new DetectionSaveParam();
                                detectionSaveParam.Bmp = Camera.GrabAsync();
                                detectionSaveParam.DetectionPoint = point;
                                detectionSaveParam.Wafer = currentWafer;
                                detectionSaveParam.NowTime = nowTime;
                                detectionSaveParam.TitleIdx = titleIdx.ToString();
                                detectionSaveParam.RecipeName = mainRecipe.Name;

                                var t0 = Thread.CurrentThread.ManagedThreadId;
                                subject.OnNext(detectionSaveParam);//AOI另外丟到其他執行續處理
                                //DetectionRecord?.Invoke(detectionSaveParam.Bmp, point, currentWafer, nowTime, titleIdx.ToString());
                                // pauseToken.IsPaused = true;
                                cancelToken.Token.ThrowIfCancellationRequested();
                                await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);
                            }
                        }
                        else
                        {
                            Task<WaferProcessStatus> micro = MicroReady?.Invoke(pst, ctk);
                            var cc = await micro;
                        }
                    }
                }
                else
                {
                    throw new FlowException("MicroDetection:Fix Wafer Error!!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public async Task TableMoveToAsync(Point pos)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                await Task.WhenAll(AxisX.MoveToAsync(pos.X), AxisY.MoveToAsync(pos.Y));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task TableMoveAsync(Vector distance)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                await Task.WhenAll(AxisX.MoveAsync(distance.X), AxisY.MoveAsync(distance.Y));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }




        /// <summary>
        /// 檢測功能
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public async Task DefectDetection(System.Drawing.Bitmap bmp)
        {



        }




        /// <summary>
        /// 對位功能
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public async Task<ITransform> Alignment(AlignmentRecipe recipe)
        {
            try
            {

                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                opticalAlignment.WriteLog = WriteLog;
                WriteLog?.Invoke("Wafer Alignment Start");
                ITransform transForm = await opticalAlignment.Alignment(recipe.FiducialDatas);
                WriteLog?.Invoke("Wafer Alignment End");
                return transForm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Point> FindFiducial(BitmapSource image, double currentPosX, double currentPosY)
        {
            return await opticalAlignment.FindFiducial(image, currentPosX, currentPosY, 0);
        }

        private async Task SetMicroscope(DetectionPoint detectionPoint)
        {
            await Microscope.ChangeLightAsync(detectionPoint.MicroscopeLightValue);
            await Microscope.ChangeApertureAsync(detectionPoint.MicroscopeApertureValue);
            await Microscope.ChangeLensAsync(detectionPoint.LensIndex);
            await Microscope.ChangeCubeAsync(detectionPoint.CubeIndex);
            await Microscope.ChangeFilter1Async(detectionPoint.Filter1Index);
            await Microscope.ChangeFilter2Async(detectionPoint.Filter2Index);
            await Microscope.ChangeFilter3Async(detectionPoint.Filter3Index);
            if (Microscope.IsAutoFocusTrace == false)//if (Microscope.IsAutoFocusTrace == false)
            {
                await Microscope.MoveToAsync(577952);
                await Microscope.AberrationMoveToAsync(480);
            }
        }
        //預留拿到對位結果後 可以做其他事
        private void AlignRecord(BitmapSource bitmap, Point? pixel, int number)
        {

            FiducialRecord?.Invoke(bitmap, pixel, number);
        }

        private async Task<Point> AlignmentManual(PauseTokenSource pts, CancellationTokenSource cts, double grabPosX, double grabPosY)
        {
            Point actualPos = new Point(0, 0);
            Task<Point> alignmentManual = AlignmentError?.Invoke(pts, cts, grabPosX, grabPosY);
            actualPos = await alignmentManual;
            return actualPos;
        }

        private void ObservableDetection()
        {
            try
            {
                camlive = Observable//.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
                                    // .ObserveOn(DispatcherScheduler.Current).
                    .Select(frame =>
                     {
                         try
                         {
                             var t0 = Thread.CurrentThread.ManagedThreadId;
                             var bmp = frame.Bmp.ToBitmap();

                             string path1 = "C:\\Users\\USER\\Documents\\WLS3200\\Result\\" + frame.NowTime + "_" + frame.RecipeName +
                                                         "\\Wafer" + frame.Wafer.CassetteIndex.ToString().PadLeft(2, '0');

                             if (!Directory.Exists(path1 + "\\MicroPhoto"))
                                 Directory.CreateDirectory(path1 + "\\MicroPhoto");

                             string path = $"{ path1 }\\MicroPhoto\\{ frame.TitleIdx }_{frame.RecipeName}_{frame.NowTime}_{frame.DetectionPoint.IndexX}_{frame.DetectionPoint.IndexY}.bmp";
                             return (bmp, path);
                         }
                         catch (Exception ex)
                         {
                             return (null, "");
                         }
                     })
                    .ObserveOn(TaskPoolScheduler.Default)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
                        .Subscribe(async frame2 =>
                        {
                            try
                            {
                                var t1 = Thread.CurrentThread.ManagedThreadId;
                                if (frame2.bmp != null)
                                {
                                    //Application.Current.Dispatcher.Invoke((Action)delegate
                                    //{
                                    try
                                    {
                                        var t2 = Thread.CurrentThread.ManagedThreadId;

                                        frame2.bmp.Save(frame2.path);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    //});
                                    await DefectDetection(frame2.bmp);
                                }
                            }
                            catch (Exception ex)
                            {


                            }
                        });
            }
            catch (Exception ex)
            {
            }
        }
        public class DetectionSaveParam
        {
            public BitmapSource Bmp { get; set; }
            public DetectionPoint DetectionPoint { get; set; }
            public Wafer Wafer { get; set; }
            public string NowTime { get; set; }
            public string TitleIdx { get; set; }
            public string RecipeName { get; set; }
        }

    }
}
