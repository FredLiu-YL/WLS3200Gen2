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



        public event Action<string> WriteLog;
        /// <summary>
        /// 需要切換Recipe的委派  
        /// </summary>
        public event Func<MainRecipe> ChangeRecipe;

        public event Func<PauseTokenSource, CancellationTokenSource, Task<WaferProcessStatus>> MacroReady;


        public event Action<Wafer> SetWaferStatus;



        public async Task ProcessRunAsync(ProcessSetting processSetting)
        {
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();
                WriteLog?.Invoke("ProcessInitial ");
                Feeder.ProcessInitial(processSetting.Inch, machineSetting, pts, cts);

                //轉換Cassette 與WAFER檢查資訊給流程使用
                //var wafers = processSetting.ProcessStation.Select(s =>
                //{
                //    var w = new Wafer(s.CassetteIndex);
                //    w.ProcessStatus = s;
                //    return w;
                //});
                List<Wafer> wafers = new List<Wafer>();
                for (int i = processSetting.ProcessStation.Length - 1; i >= 0; i--)
                {
                    var s = processSetting.ProcessStation[i];
                    var w = new Wafer(s.CassetteIndex);
                    w.ProcessStatus = s;
                    wafers.Add(w);
                }


                //因為Loadport 先掃完wafer才傳進來 ， 由主流程控制走向(由上往下取)    所有流程前進判斷都靠這個
                Queue<Wafer> processWafers = new Queue<Wafer>(wafers);
                // Queue<Wafer> processWafers = new Queue<Wafer>(wafers.Reverse());


                CreateProcessFolder();
                await Task.Run(async () =>
                {
                    WriteLog?.Invoke("Process Start");

                    Wafer nextWafer = null;
                    Wafer currentWafer = null;
                    Task taskLoad = Task.CompletedTask;
                    Task taskUnLoad = Task.CompletedTask;

                    //第一次執行 避免第一片就沒有要做 所以會搜尋到需要做的WAFER為止             
                    currentWafer = SearchLoadWafer(processWafers);
                    if (currentWafer != null)
                    {
                        //需不需要去Macro
                        if (currentWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select || currentWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
                        {
                            await Feeder.LoadCassetteToMacroAsync(currentWafer.CassetteIndex, processSetting.IsLoadport1);
                        }
                        else
                        {
                            //若沒有人檢查晶圓直接到Aligner
                            await Feeder.LoadCassetteToAlignerAsync(currentWafer.CassetteIndex, processSetting.IsLoadport1);
                        }
                        while (true)
                        {
                            try
                            {
                                //避免未來會先讀MES的資訊 才決定要用哪個Recipe  ，預留擴充的機會
                                if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                                MainRecipe recipe = ChangeRecipe.Invoke();
                                InspectionReport report = new InspectionReport();

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
                                }
                                else
                                {
                                    //等待退片完成
                                    await taskUnLoad;
                                    //等待預載完成
                                    await taskLoad;
                                    //WaferID確認+角度轉到平台角度
                                    await Feeder.AlignerAsync(currentWafer.ProcessStatus.WaferID, recipe.EFEMRecipe);
                                }
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
                                    await Feeder.AlignerToStandByAsync();
                                    await catchWafertask; //等待顯微鏡站準備完成

                                    //wafer送到主設備內 
                                    Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                                    await Feeder.LoadToMicroAsync();// currentWafer = await Feeder.LoadToMicroAsync(currentWafer);

                                    nextWafer = SearchLoadWafer(processWafers);
                                    // nextWafer = processWafers.Dequeue();

                                    //預載一片在Macro上
                                    if (nextWafer != null)
                                    {
                                        if (nextWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select || nextWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
                                        {
                                            taskLoad = Feeder.LoadCassetteToMacroAsync(nextWafer.CassetteIndex, processSetting.IsLoadport1);
                                        }
                                        else
                                        {
                                            taskLoad = Feeder.LoadCassetteToAlignerAsync(nextWafer.CassetteIndex, processSetting.IsLoadport1);
                                        }
                                    }
                                    //執行主設備動作 
                                    await focusZWafertask;
                                    await MicroDetection.Run(recipe, processSetting, currentWafer, pts, cts);
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
                                    await Feeder.AlignerToStandByAsync();
                                    await Feeder.UnLoadWaferToCassette(currentWafer, processSetting.IsLoadport1);
                                    //預載一片在Macro上
                                    if (nextWafer != null)
                                    {
                                        if (nextWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select || nextWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select)
                                        {
                                            await Feeder.LoadCassetteToMacroAsync(nextWafer.CassetteIndex, processSetting.IsLoadport1);
                                        }
                                        else
                                        {
                                            await Feeder.LoadCassetteToAlignerAsync(nextWafer.CassetteIndex, processSetting.IsLoadport1);
                                        }
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

                                WriteLog?.Invoke($"Remaining number of wafers : {waferusableCount} ");
                                //判斷卡匣空了
                                if (nextWafer == null)
                                {
                                    break;
                                }
                                currentWafer = nextWafer; //將下一片的資料轉成當前WAFER資料
                            }
                            catch (FlowException ex)
                            {
                                MessageBox.Show(ex.Message);
                                //pts.IsPaused = true;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
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
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Feeder.ProcessEnd();
                cts.Dispose();
                WriteLog?.Invoke("Process Finish ");
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
        private async Task MacroTopInspection(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            //晶面檢查
            if (station == WaferProcessStatus.Select)
            {
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

            }

        }
        private async Task MacroBackInspection(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe, bool isTestRun)
        {
            //eFEMtionRecipe.MacroBackStartPos
            //晶背檢查
            if (station == WaferProcessStatus.Select)
            {
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
                else
                {
                    await Feeder.TurnBackWafer(-startPos);
                }
                //翻回來
                await Feeder.Macro.HomeOuterRing();
                station = WaferProcessStatus.Complate;
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

        private void CreateProcessFolder()
        {
            var date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            var path = "C:\\WLS3200\\" + date;
            if (!Directory.Exists(path))
            {

                //新增資料夾
                Directory.CreateDirectory(path);
            }

        }

        private void MicroVacuumOn()
        {
            MicroDetection.TableVacuum.On();
        }

        private void SetWaferStatusToUI(Wafer currentWafer)
        {
            SetWaferStatus.Invoke(currentWafer);//Cassette 即時狀態更新
        }
    }
}
