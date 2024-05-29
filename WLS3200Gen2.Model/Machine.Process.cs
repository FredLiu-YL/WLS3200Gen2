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



        public event Action<YuanliCore.Logger.LogType, string> WriteLog;
        /// <summary>
        /// 需要切換Recipe的委派  
        /// </summary>
        public event Func<MainRecipe> ChangeRecipe;

        public event Func<PauseTokenSource, CancellationTokenSource, Task<WaferProcessStatus>> MacroReady;

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
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "ProcessInitial ");
                Feeder.ProcessInitial(processSetting.Inch, machineSetting, pts, cts);

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



                await Task.Run(async () =>
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Process Start");
                    Wafer nextWafer = null;
                    Task taskLoad = Task.CompletedTask;
                    Task taskUnLoad = Task.CompletedTask;
                    //第一次執行 避免第一片就沒有要做 所以會搜尋到需要做的WAFER為止             
                    currentWafer = SearchLoadWafer(processWafers);
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
                            currentSavePath = CreateProcessFolder(recipe.Name, currentWafer.CassetteIndex);
                            currentWafer.Dies = recipe.DetectRecipe.WaferMap.Dies;
                            //需不需要在Macro檢查
                            if (currentWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select || currentWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
                            {
                                //晶面檢查
                                await MacroTopInspection(currentWafer.ProcessStatus.MacroTop, recipe.EFEMRecipe, processSetting.IsTestRun);
                                SetWaferStatusToUI(currentWafer);
                                //晶背檢查
                                await MacroBackInspection(currentWafer.ProcessStatus.MacroBack, recipe.EFEMRecipe, processSetting.IsTestRun);
                                SetWaferStatusToUI(currentWafer);

                                //關閉光源
                                await CloseLampControl();

                                //等待退片完成
                                await taskUnLoad;
                                //等待預載完成
                                await taskLoad;
                                //到Align
                                await Feeder.LoadMacroToAlignerAsync(currentWafer.ProcessStatus.WaferID, recipe.EFEMRecipe);
                                SetWaferStatusToUI(currentWafer);
                            }
                            //等待退片完成
                            await taskUnLoad;
                            //等待預載完成
                            await taskLoad;
                            //WaferID確認+角度轉到平台角度
                            await Feeder.AlignerAsync(currentWafer.ProcessStatus.WaferID, recipe.EFEMRecipe);
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
                                await Feeder.AlignerToStandByAsync(currentWafer.ProcessStatus.Micro);
                                await catchWafertask; //等待顯微鏡站準備完成

                                //wafer送到主設備內 
                                Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                                await Feeder.LoadToMicroAsync(currentWafer.ProcessStatus.Micro);// currentWafer = await Feeder.LoadToMicroAsync(currentWafer);

                                nextWafer = SearchLoadWafer(processWafers);
                                // nextWafer = processWafers.Dequeue();

                                //預載一片在Macro上
                                if (nextWafer != null)
                                {
                                    taskLoad = PreLoad(nextWafer, processSetting);
                                }
                                //執行主設備動作 
                                await focusZWafertask;
                                await MicroDetection.Run(currentWafer, recipe, machineSetting.MicroscopeLensDefault.ToArray(), processSetting, nextSavePath, pts, cts);
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
                                taskUnLoad = UnLoadWafer(currentWafer, processSetting.IsLoadport1);
                                //await Feeder.MicroUnLoadToStandByAsync();
                                //await Feeder.UnLoadWaferToCassette(currentWafer, processSetting.IsLoadport1);
                                if (nextWafer == null)
                                {
                                    await taskUnLoad;
                                }
                            }
                            else
                            {
                                nextWafer = SearchLoadWafer(processWafers);
                                // nextWafer = processWafers.Dequeue();
                                //退片
                                await Feeder.AlignerToStandByAsync(currentWafer.ProcessStatus.WaferID);
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
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Process Finish ");
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

        private Wafer SearchLoadWafer(Queue<Wafer> processWafers)
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
        private async Task MacroTopInspection(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            try
            {
                //晶面檢查
                if (station == WaferProcessStatus.Select)
                {
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
                        var judgeResult = await macro;
                    }
                    await Feeder.Macro.HomeInnerRing();
                    station = WaferProcessStatus.Complate;
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Macro Top Inspection End");
                }
            }
            catch (Exception ex)
            {
                station = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task MacroBackInspection(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            try
            {
                //eFEMtionRecipe.MacroBackStartPos
                //晶背檢查
                if (station == WaferProcessStatus.Select)
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
                        var judgeResult = await macro;
                    }
                    await Feeder.TurnBackWafer(0);
                    //翻回來
                    await Feeder.Macro.HomeOuterRing();
                    station = WaferProcessStatus.Complate;
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Macro Back Inspection End");
                }
            }
            catch (Exception ex)
            {
                station = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task UnLoadWafer(Wafer unLoadWafer, bool unLoadIsLoadport1)
        {
            await Feeder.MicroUnLoadToStandByAsync();
            await Feeder.UnLoadWaferToCassette(unLoadWafer, unLoadIsLoadport1);
        }
        private async Task CloseLampControl()
        {
            Task taskLampControl1 = Feeder.LampControl1.ChangeLightAsync(0);
            Task taskLampControl2 = Feeder.LampControl2.ChangeLightAsync(0);
            await Task.WhenAll(taskLampControl1, taskLampControl2);
        }

        private string CreateProcessFolder(string recipeName, int waferIndex)
        {
            try
            {
                if (!Directory.Exists(machineSetting.ResultPath))
                {
                    //新增資料夾
                    Directory.CreateDirectory(machineSetting.ResultPath);
                }
                var date = DateTime.Now.ToString("yyyyMMddHHmm");
                var path = machineSetting.ResultPath + "\\" + date + "_" + recipeName;
                if (!Directory.Exists(path))
                {
                    //新增資料夾
                    Directory.CreateDirectory(path);
                }
                path = machineSetting.ResultPath + "\\" + date + "_" + recipeName + "\\Wafer" + waferIndex.ToString().PadLeft(2, '0');
                if (!Directory.Exists(path))
                {
                    //新增資料夾
                    Directory.CreateDirectory(path);
                }


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
