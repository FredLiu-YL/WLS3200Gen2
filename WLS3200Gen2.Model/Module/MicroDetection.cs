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

        }
        public bool IsInitial { get; set; } = false;
        /// <summary>
        /// 存圖檔案路徑
        /// </summary>
        public string GrabSaveFolder { get; private set; }
        /// <summary>
        /// 存圖圖片路徑
        /// </summary>
        public string GrabSavePicturTime { get; private set; }
        /// <summary>
        /// 存圖圖片Index
        /// </summary>
        public int GrabTitleIdx { get; set; }
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
        public event Action<YuanliCore.Logger.LogType, string> WriteLog;
        /// <summary>
        /// 對位結果紀錄 (圖像 對位座標(pxel))
        /// </summary>
        public event Action<BitmapSource, Point?, int> FiducialRecord;
        /// <summary>
        /// 手動對位 
        /// </summary>
        public event Func<PauseTokenSource, CancellationTokenSource, double, double, Task<Point>> AlignManual;

        /// <summary>
        /// 檢測結果紀錄
        /// </summary>
        public event Action<BitmapSource, DetectionPoint> DetectionRecord;

        /// <summary> 
        /// 
        /// </summary>
        public event Func<PauseTokenSource, CancellationTokenSource, Die[], Task<(WaferProcessStatus, Die[])>> MicroReady;

        public ITransform TransForm { get; set; }
        public async Task Home()
        {
            try
            {
                //Task axisZHome = Task.Run(async () =>
                //{
                //    await AxisZ.HomeAsync();
                //});
                //await Task.Run(() => { });
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Micro Homing Start");

                await AxisZ.HomeAsync();

                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "AxisZ Homing End");

                Task axisXHome = AxisX.HomeAsync();

                Task axisYHome = AxisY.HomeAsync();

                Task axisRHome = AxisR.HomeAsync();

                Task microscopeHome1 = Microscope.HomeAsync();

                await Task.WhenAll(axisXHome, axisYHome, axisRHome, microscopeHome1);
                IsInitial = true;
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Micro Homing End");
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
                await Task.Delay(100);//真空解除要一段時間
                //如果有lift 或夾持機構 需要做處理
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Run(Wafer currentWafer, InspectionReport report, MainRecipe mainRecipe, MicroscopeLens[] lensSetting, ProcessSetting processSetting, string savePath, PauseTokenSource pst, CancellationTokenSource ctk)
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Micro Start");
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

                    //設定顯微鏡參數
                    DetectionPoint detectionPoint = new DetectionPoint();
                    detectionPoint.MicroscopeLightValue = recipe.AlignRecipe.FiducialDatas[0].MicroscopeLightValue;
                    detectionPoint.MicroscopeApertureValue = recipe.AlignRecipe.FiducialDatas[0].MicroscopeApertureValue;
                    detectionPoint.LensIndex = recipe.AlignRecipe.FiducialDatas[0].LensIndex;
                    detectionPoint.CubeIndex = recipe.AlignRecipe.FiducialDatas[0].CubeIndex;
                    detectionPoint.Filter1Index = recipe.AlignRecipe.FiducialDatas[0].Filter1Index;
                    detectionPoint.Filter2Index = recipe.AlignRecipe.FiducialDatas[0].Filter2Index;
                    detectionPoint.Filter3Index = recipe.AlignRecipe.FiducialDatas[0].Filter3Index;
                    Task alignmentMicroscopeTask = SetMicroscope(detectionPoint);
                    //移動到第一個定位點，以便先第一次自動AF
                    Task alignmentTableTask = TableMoveToAsync(new Point(recipe.AlignRecipe.FiducialDatas[0].GrabPositionX, recipe.AlignRecipe.FiducialDatas[0].GrabPositionY));
                    await Task.WhenAll(alignmentMicroscopeTask, alignmentTableTask);
                    Microscope.AFTrace();


                    //對位
                    ITransform transForm = await Alignment(recipe.AlignRecipe, lensSetting[detectionPoint.LensIndex]);
                    TransForm = transForm;
                    cancelToken.Token.ThrowIfCancellationRequested();
                    await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);


                    //創建存檔各片存檔的路徑
                    GrabSaveFolder = savePath;
                    GrabTitleIdx = 0;
                    if (processSetting.IsAutoSave || processSetting.IsTestRun)
                    {
                        //每一個座標需要檢查的座標
                        foreach (DetectionPoint point in recipe.DetectionPoints)
                        {
                            GrabTitleIdx += 1;
                            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, $"Move To Detection Position :[{point.IndexX} - {point.IndexY}] ");
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
                            detectionSaveParam.SavePath = $"{ GrabSaveFolder }\\MicroPhoto\\{ GrabTitleIdx }_{mainRecipe.Name}_{GrabSavePicturTime}_{point.IndexX}_{point.IndexY}.bmp";

                            var t0 = Thread.CurrentThread.ManagedThreadId;
                            subject.OnNext(detectionSaveParam);//AOI另外丟到其他執行續處理
                                                               //DetectionRecord?.Invoke(detectionSaveParam.Bmp, point, currentWafer, nowTime, titleIdx.ToString());
                                                               // pauseToken.IsPaused = true;
                            cancelToken.Token.ThrowIfCancellationRequested();
                            await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);
                            currentWafer.ProcessStatus.Micro = WaferProcessStatus.Pass;
                        }
                    }
                    else
                    {
                        Task<(WaferProcessStatus, Die[])> micro = MicroReady?.Invoke(pst, ctk, report.WaferMapping.Dies);
                        await micro;
                        currentWafer.ProcessStatus.Micro = micro.Result.Item1;
                        report.WaferMapping.Dies = micro.Result.Item2;
                        SaveSinfResult(GrabSaveFolder + "\\Result.txt", report.WaferMapping.Dies, mainRecipe.DetectRecipe.WaferMap);
                    }
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Micro End");
                }
                else
                {
                    throw new FlowException("MicroDetection:Fix Wafer Error!!");
                }
            }
            catch (FlowException ex)
            {
                currentWafer.ProcessStatus.Micro = WaferProcessStatus.Reject;
                throw ex;
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.Micro = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        /// <summary>
        /// 儲存成sinf格式(orgWaferMapping要拿原始的WaferMapping)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dies"></param>
        /// <param name="orgWaferMapping"></param>
        private void SaveSinfResult(string path, Die[] dies, WaferMapping orgWaferMapping)
        {
            try
            {
                SinfWaferMapping sinfWaferMapping = (SinfWaferMapping)orgWaferMapping;
                SinfWaferMapping sinfWaferMapping2 = sinfWaferMapping.Copy();
                sinfWaferMapping2.Dies = dies;
                sinfWaferMapping2.SaveWaferFile(GrabSaveFolder + "\\Result.txt", false, false);
            }
            catch (Exception ex)
            {
            }
        }
        private string CreateMicroFolder(string nowTime, string recipeName, Wafer currentWafer)
        {
            try
            {
                string pathfolder = "C:\\Users\\USER\\Documents\\WLS3200\\Result\\" + nowTime + "_" + recipeName +
                                                         "\\Wafer" + currentWafer.CassetteIndex.ToString().PadLeft(2, '0');
                if (!Directory.Exists(pathfolder + "\\MicroPhoto"))
                    Directory.CreateDirectory(pathfolder + "\\MicroPhoto");
                return pathfolder;
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
        public async Task<ITransform> Alignment(AlignmentRecipe recipe, MicroscopeLens lensSetting)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("MicroDetection:Is Not Initial!!");
                opticalAlignment.WriteLog = WriteLog;
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Wafer Alignment Start");
                ITransform transForm = await opticalAlignment.Alignment(recipe.FiducialDatas, lensSetting);
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Wafer Alignment End");
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
        public void AlignmentManualAdd()
        {
            opticalAlignment.AlignmentManual += AlignManual;
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
                             var path = frame.SavePath;
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
            public string SavePath { get; set; }
        }

    }
}
