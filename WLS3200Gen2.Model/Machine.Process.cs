using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Data;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {

        private PauseTokenSource pts = new PauseTokenSource();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool isAbort = false;


        public event Action<YuanliCore.Logger.LogType, string> WriteLog;
        /// <summary>
        /// 需要切換Recipe的委派  
        /// </summary>
        public event Func<MainRecipe> ChangeRecipe;

        public event Func<PauseTokenSource, CancellationTokenSource, Task<WaferProcessStatus>> MacroReady;

        public event Func<PauseTokenSource, CancellationTokenSource, Task<ProcessStation>> MacroDoneReady;

        public event Func<PauseTokenSource, CancellationTokenSource, double, double, Task<Point>> AlignmentReady;


        public event Action<Wafer> SetWaferStatus;



        public async Task ProcessRunAsync(ProcessSetting processSetting)
        {

            Wafer currentWafer = null;
            string currentSavePath = "";
            string nextSavePath = "";
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();
                isAbort = false;
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "ProcessInitial ");
                Feeder.ProcessInitial(processSetting.Inch, machineSetting, pts, cts);
                int testCount = 1;
                var copiedProcessSetting = processSetting.Copy();
                if (processSetting.IsTestRun)
                {
                    testCount = 20;
                }
                for (int i = 0; i < testCount; i++)
                {
                    //測試跑，重新Reset要運作的項目
                    for (int j = 0; j < processSetting.ProcessStation.Length; j++)
                    {
                        processSetting.ProcessStation[j].MacroTop = copiedProcessSetting.ProcessStation[j].MacroTop;
                        processSetting.ProcessStation[j].MacroBack = copiedProcessSetting.ProcessStation[j].MacroBack;
                        processSetting.ProcessStation[j].WaferID = copiedProcessSetting.ProcessStation[j].WaferID;
                        processSetting.ProcessStation[j].Micro = copiedProcessSetting.ProcessStation[j].Micro;
                    }
                    //processSetting = copiedProcessSetting.Copy();
                    //轉換Cassette 與WAFER檢查資訊給流程使用
                    var wafers = processSetting.ProcessStation.Select(s =>
                    {
                        var w = new Wafer(s.CassetteIndex);
                        w.ProcessStatus = s;
                        return w;
                    });
                    //因為Loadport 先掃完wafer才傳進來 ， 由主流程控制走向(由上往下取)    所有流程前進判斷都靠這個
                    //Queue<Wafer> processWafers = new Queue<Wafer>(wafers);//主流程走向(由上往下取)
                    Queue<Wafer> processWafers = new Queue<Wafer>(wafers.Reverse());//主流程走向(由下往上取)


                    var resultTime = DateTime.Now.ToString("yyyyMMddHHmm");
                    await Task.Run(async () =>
                    {
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Process Start");
                        Wafer nextWafer = null;
                        Task taskLoad = Task.CompletedTask;
                        Task taskUnLoad = Task.CompletedTask;
                        //第一次執行 避免第一片就沒有要做 所以會搜尋到需要做的WAFER為止             
                        currentWafer = SearchLoadWafer(processWafers, false);
                        if (currentWafer != null)
                        {
                            //先載一片
                            await PreLoad(currentWafer, processSetting);
                            while (true)
                            {
                                //避免未來會先讀MES的資訊 才決定要用哪個Recipe  ，預留擴充的機會
                                if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                                MainRecipe recipe = ChangeRecipe.Invoke();
                                InspectionReport report = new InspectionReport();
                                report.WaferMapping = recipe.DetectRecipe.WaferMap.Copy();

                                currentSavePath = CreateResultFolder(recipe.Name, resultTime, currentWafer.CassetteIndex);
                                currentWafer.Dies = recipe.DetectRecipe.WaferMap.Dies;
                                //需不需要在Macro檢查
                                if (currentWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select || currentWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
                                {
                                    //晶面檢查
                                    await MacroTopInspection(currentWafer, recipe.EFEMRecipe, processSetting.IsTestRun);
                                    SetWaferStatusToUI(currentWafer);
                                    //晶背檢查
                                    await MacroBackInspection(currentWafer, recipe.EFEMRecipe, processSetting.IsTestRun);
                                    SetWaferStatusToUI(currentWafer);

                                    //是否還要回頭檢查
                                    await MacroDone(currentWafer, recipe.EFEMRecipe, processSetting.IsTestRun);
                                    //關閉光源
                                    await CloseLampControl();

                                    //等待退片完成
                                    await taskUnLoad;
                                    //等待預載完成
                                    await taskLoad;
                                    //到Align
                                    await Feeder.LoadMacroToAlignerAsync(currentWafer, recipe.EFEMRecipe);
                                    SetWaferStatusToUI(currentWafer);
                                }
                                //等待退片完成
                                await taskUnLoad;
                                //等待預載完成
                                await taskLoad;
                                //WaferID確認+角度轉到平台角度
                                await Feeder.AlignerAsync(currentWafer, recipe.EFEMRecipe);
                                SetWaferStatusToUI(currentWafer);
                                // ProcessPause();
                                cts.Token.ThrowIfCancellationRequested();
                                await pts.Token.WaitWhilePausedAsync(cts.Token);

                                taskLoad = Task.CompletedTask;


                                if (currentWafer.ProcessStatus.Micro == WaferProcessStatus.Select)//判斷如果有需要進顯微鏡
                                {
                                    //顯微鏡站準備接WAFER
                                    Task catchWafertask = MicroDetection.CatchWaferPrepare(machineSetting.TableWaferCatchPosition, pts, cts);
                                    //大Z的位置
                                    Task focusZWafertask = MicroDetection.FocusZWaferPrepare(machineSetting.TableWaferCatchPositionZ, pts, cts);
                                    //到預備位置準備進片
                                    await Feeder.AlignerToStandByAsync(currentWafer);
                                    await catchWafertask; //等待顯微鏡站準備完成

                                    //wafer送到主設備內 
                                    Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                                    await Feeder.LoadToMicroAsync(currentWafer);// currentWafer = await Feeder.LoadToMicroAsync(currentWafer);

                                    nextWafer = SearchLoadWafer(processWafers, isAbort);

                                    //預載一片在Macro上
                                    if (nextWafer != null)
                                    {
                                        taskLoad = PreLoad(nextWafer, processSetting);
                                    }
                                    //執行主設備動作 
                                    await focusZWafertask;
                                    await MicroDetection.Run(currentWafer, report, recipe, machineSetting.MicroscopeLensDefault.ToArray(), processSetting, currentSavePath, pts, cts);
                                    SetWaferStatusToUI(currentWafer);
                                    if (pts.IsPaused)
                                    {
                                        await taskLoad;
                                        cts.Token.ThrowIfCancellationRequested();
                                        await pts.Token.WaitWhilePausedAsync(cts.Token);
                                    }
                                    await MicroDetection.PutWaferPrepare(machineSetting.TableWaferCatchPosition);
                                    //等待預載完成
                                    await taskLoad;
                                    //退片
                                    //Wafer unLoadWafer = currentWafer;
                                    //bool unLoadIsLoadport1 = processSetting.IsLoadport1;
                                    taskUnLoad = UnLoadWafer(currentWafer, processSetting.IsLoadport1, nextWafer);
                                    //await Feeder.MicroUnLoadToStandByAsync();
                                    //await Feeder.UnLoadWaferToCassette(currentWafer, processSetting.IsLoadport1);
                                    if (nextWafer == null)
                                    {
                                        await taskUnLoad;
                                    }
                                }
                                else
                                {
                                    nextWafer = SearchLoadWafer(processWafers, isAbort);
                                    // nextWafer = processWafers.Dequeue();
                                    //退片
                                    await Feeder.AlignerToStandByAsync(currentWafer);
                                    await Feeder.UnLoadWaferToCassette(currentWafer, processSetting.IsLoadport1);
                                    //預載一片在Macro上
                                    if (nextWafer != null)
                                    {
                                        taskLoad = PreLoad(nextWafer, processSetting);
                                    }
                                }

                                await Task.Delay(300);
                                cts.Token.ThrowIfCancellationRequested();
                                await pts.Token.WaitWhilePausedAsync(cts.Token);

                                //讀取 使用的Cassette  還剩下片子數量資訊 ， 只要任何一站要做就要算
                                int waferusableCount = processWafers.Where(w =>
                                    w.ProcessStatus.MacroTop == WaferProcessStatus.Select ||
                                    w.ProcessStatus.MacroBack == WaferProcessStatus.Select ||
                                    w.ProcessStatus.WaferID == WaferProcessStatus.Select ||
                                    w.ProcessStatus.Micro == WaferProcessStatus.Select).Count();
                                //ShowDetectionHomeNewMapImgae(mainRecipe.DetectRecipe);
                                //ResetDetectionRunningPointList();
                                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, $"Remaining number of wafers : {waferusableCount} ");
                                //判斷卡匣空了
                                if (nextWafer == null)
                                {
                                    break;
                                }
                                currentWafer = nextWafer; //將下一片的資料轉成當前WAFER資料

                            }
                        }
                    });
                }
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Process Finish ");
            }
            catch (OperationCanceledException canceleEx)
            {
                throw canceleEx;
            }
            catch (FlowException ex)
            {
                WriteLog(YuanliCore.Logger.LogType.ALARM, ex.Message);
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                Feeder.IsInitial = false;
                MicroDetection.IsInitial = false;
                throw ex;
            }
            finally
            {
                SetWaferStatusToUI(currentWafer);
                Feeder.ProcessEnd();
                cts.Dispose();
            }
        }
        public async Task ProcessPause()
        {
            pts.IsPaused = true;
        }
        public async Task ProcessResume()
        {
            pts.IsPaused = false;
        }
        public async Task ProcessStop()
        {
            cts.Cancel();
        }
        public async Task Abort()
        {
            isAbort = true;
        }
        private Wafer SearchLoadWafer(Queue<Wafer> processWafers, bool isAbort)
        {
            //Abort全部退片
            if (isAbort)
            {
                return null;
            }
            else
            {
                //避免沒有要做 所以會搜尋到需要做的WAFER為止
                while (true)
                {
                    if (processWafers.Count == 0) return null;

                    var currentWafer = processWafers.Dequeue();
                    if (currentWafer.ProcessStatus.WaferID == WaferProcessStatus.Select || currentWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select
                    || currentWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select || currentWafer.ProcessStatus.Micro == WaferProcessStatus.Select)
                    {
                        return currentWafer;
                    }
                }
            }
        }
        private async Task PreLoad(Wafer wafer, ProcessSetting processSetting)
        {
            if (wafer.ProcessStatus.MacroTop == WaferProcessStatus.Select || wafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
            {
                await Feeder.LoadCassetteToMacroAsync(wafer.ProcessStatus.MacroTop, wafer.CassetteIndex, processSetting.IsLoadport1);
            }
            else
            {
                //若沒有人檢查晶圓直接到Aligner
                await Feeder.LoadCassetteToAlignerAsync(wafer.ProcessStatus.WaferID, wafer.CassetteIndex, processSetting.IsLoadport1);
            }
        }
        private async Task MacroTopInspection(Wafer currentWafer, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            try
            {
                //晶面檢查
                if (currentWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select)
                {
                    currentWafer.ProcessStatus.MacroTop = WaferProcessStatus.Pass;
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Macro Top Inspection Start");
                    Task taskLampControl1 = Feeder.LampControl1.ChangeLightAsync(eFEMtionRecipe.MacroTopLeftLightValue);
                    Task taskLampControl2 = Feeder.LampControl2.ChangeLightAsync(eFEMtionRecipe.MacroTopRightLightValue);
                    await Feeder.Macro.GoInnerRingCheckPos();
                    await Feeder.TurnWafer(eFEMtionRecipe);
                    await Task.WhenAll(taskLampControl1, taskLampControl2);
                    //委派到ui 去執行macro人工檢
                    if (isTestRun == false)
                    {
                        Task<WaferProcessStatus> macro = MacroReady?.Invoke(pts, cts);
                        currentWafer.ProcessStatus.MacroTop = await macro;
                    }
                    else
                    {
                        currentWafer.ProcessStatus.MacroTop = WaferProcessStatus.Pass;
                    }
                    await Feeder.Macro.HomeInnerRing();
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Macro Top Inspection End");
                }
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.MacroTop = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task MacroBackInspection(Wafer currentWafer, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            try
            {
                //eFEMtionRecipe.MacroBackStartPos
                //晶背檢查
                if (currentWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Macro Back Inspection Start");
                    Task taskLampControl1 = Feeder.LampControl1.ChangeLightAsync(eFEMtionRecipe.MacroBackLeftLightValue);
                    Task taskLampControl2 = Feeder.LampControl2.ChangeLightAsync(eFEMtionRecipe.MacroBackRightLightValue);
                    await Feeder.Macro.GoOuterRingCheckPos();
                    //做翻面動作  可能Robot 取走翻轉完再放回 ，或Macro 機構本身能翻
                    var startPos = eFEMtionRecipe.MacroBackStartPos;
                    await Feeder.TurnBackWafer(startPos);
                    await Task.WhenAll(taskLampControl1, taskLampControl2);
                    //委派到ui層 去執行macro人工檢
                    if (isTestRun == false)
                    {
                        Task<WaferProcessStatus> macro = MacroReady?.Invoke(pts, cts);
                        currentWafer.ProcessStatus.MacroBack = await macro;
                    }
                    else
                    {
                        currentWafer.ProcessStatus.MacroBack = WaferProcessStatus.Pass;
                    }
                    await Feeder.TurnBackWafer(0);
                    //翻回來
                    await Feeder.Macro.HomeOuterRing();
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Macro Back Inspection End");
                }
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.MacroBack = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        //MacroDone
        private async Task MacroDone(Wafer currentWafer, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            try
            {
                ProcessStation macroDoneProcessStation = new ProcessStation(1);
                while (true)
                {
                    if (isTestRun == false)
                    {
                        Task<ProcessStation> macroDone = MacroDoneReady?.Invoke(pts, cts);
                        macroDoneProcessStation = await macroDone;

                        if (macroDoneProcessStation.MacroTop == WaferProcessStatus.Select)
                        {
                            currentWafer.ProcessStatus.MacroTop = WaferProcessStatus.Select;
                            //晶面檢查
                            await MacroTopInspection(currentWafer, eFEMtionRecipe, isTestRun);
                            SetWaferStatusToUI(currentWafer);
                        }
                        else if (macroDoneProcessStation.MacroBack == WaferProcessStatus.Select)
                        {
                            currentWafer.ProcessStatus.MacroBack = WaferProcessStatus.Select;
                            //晶背檢查
                            await MacroBackInspection(currentWafer, eFEMtionRecipe, isTestRun);
                            SetWaferStatusToUI(currentWafer);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.MacroBack = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task UnLoadWafer(Wafer unLoadWafer, bool unLoadIsLoadport1, Wafer nextWafer)
        {
            await Feeder.MicroUnLoadToStandByAsync();
            await Feeder.UnLoadWaferToCassette(unLoadWafer, unLoadIsLoadport1);
            if (nextWafer != null)
            {
                if (nextWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select ||
                    nextWafer.ProcessStatus.MacroBack == WaferProcessStatus.Pass ||
                    nextWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select ||
                    nextWafer.ProcessStatus.MacroTop == WaferProcessStatus.Pass)
                {
                    await Feeder.UnLoadWaferMacroPrePare(unLoadWafer);
                }
            }
        }
        private async Task CloseLampControl()
        {
            Task taskLampControl1 = Feeder.LampControl1.ChangeLightAsync(0);
            Task taskLampControl2 = Feeder.LampControl2.ChangeLightAsync(0);
            await Task.WhenAll(taskLampControl1, taskLampControl2);
        }

        private string CreateResultFolder(string recipeName, string resultTime, int waferIndex)
        {
            try
            {
                var path = machineSetting.ResultPath + "\\" + resultTime + "_" + recipeName + "\\Wafer" + waferIndex.ToString().PadLeft(2, '0') + "\\MicroPhoto";
                if (!Directory.Exists(path))
                {
                    //新增資料夾
                    Directory.CreateDirectory(path);
                }
                path = machineSetting.ResultPath + "\\" + resultTime + "_" + recipeName + "\\Wafer" + waferIndex.ToString().PadLeft(2, '0');
                return path;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void MicroVacuumOn()
        {
            MicroDetection.TableVacuum.On();
        }

        private void SetWaferStatusToUI(Wafer currentWafer)
        {
            if (currentWafer != null)
            {
                SetWaferStatus.Invoke(currentWafer);//Cassette 即時狀態更新
            }
        }
    }
}
