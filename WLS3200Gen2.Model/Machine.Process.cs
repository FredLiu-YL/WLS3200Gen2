using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                WriteLog("ProcessInitial ");

                Feeder.ProcessInitial(processSetting.Inch, machineSetting, pts, cts);

                //轉換Cassette 與WAFER檢查資訊給流程使用
                var wafers = processSetting.ProcessStation.Select(s =>
                {
                    var w = new Wafer(s.CassetteIndex);
                    w.ProcessStatus = s;
                    return w;
                });

                //因為Loadport 先掃完wafer才傳進來 ， 由主流程控制走向(由上往下取)    所有流程前進判斷都靠這個
                Queue<Wafer> processWafers = new Queue<Wafer>(wafers);
                // Queue<Wafer> processWafers = new Queue<Wafer>(wafers.Reverse());



                await Task.Run(async () =>
                {
                    WriteLog("Process Start");

                    Wafer nextWafer = null;
                    Wafer currentWafer = null;


                    //第一次執行 避免第一片就沒有要做 所以會搜尋到需要做的WAFER為止
                    while (true)
                    {

                        currentWafer = processWafers.Dequeue();
                        if (currentWafer.ProcessStatus.WaferID == WaferProcessStatus.Select || currentWafer.ProcessStatus.MacroTop == WaferProcessStatus.Select
                        || currentWafer.ProcessStatus.MacroBack == WaferProcessStatus.Select || currentWafer.ProcessStatus.Micro == WaferProcessStatus.Select)
                        {
                            await Feeder.LoadToReadyAsync(currentWafer.CassetteIndex, processSetting.IsLoadport1);
                            break;
                        }

                    }



                    while (true)
                    {


                        if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                        MainRecipe recipe = ChangeRecipe?.Invoke();


                        //讀取 使用的Cassette  還剩下片子數量資訊 ， 只要任何一站要做就要算
                        int waferusableCount = processWafers.Where(w =>
                        w.ProcessStatus.MacroTop == WaferProcessStatus.Select ||
                        w.ProcessStatus.MacroBack == WaferProcessStatus.Select ||
                        w.ProcessStatus.WaferID == WaferProcessStatus.Select ||
                        w.ProcessStatus.Micro == WaferProcessStatus.Select).Count();


                        //晶面檢查
                        await MacroTopInspection(currentWafer.ProcessStatus.MacroTop, recipe.EFEMRecipe);                
                        SetWaferStatusToUI(currentWafer);

                        //晶背檢查
                        await MacroBackInspection(currentWafer.ProcessStatus.MacroBack, recipe.EFEMRecipe.MacroBackStartPos);                     
                        SetWaferStatusToUI(currentWafer);


                        //到Align
                        await Feeder.LoadToAlignerAsync(currentWafer.ProcessStatus.WaferID, recipe.EFEMRecipe);


                        // ProcessPause();
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);


                        Task taskLoad = Task.CompletedTask;


                        if (currentWafer.ProcessStatus.Micro == WaferProcessStatus.Select)//判斷如果有需要進顯微鏡
                        {
                            //顯微鏡站準備接WAFER
                            Task catchWafertask = MicroDetection.CatchWaferPrepare(machineSetting.TableWaferCatchPosition, pts, cts);
                            //到預備位置準備進片
                            await Feeder.AlignerToStandByAsync();
                            await catchWafertask; //等待顯微鏡站準備完成

                            //wafer送到主設備內 
                            Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                            currentWafer = await Feeder.LoadToMicroAsync(currentWafer);

                            if (waferusableCount > 0)//如果還有片
                            {
                                nextWafer = processWafers.Dequeue();
                                //預載一片在Macro上
                                taskLoad = Feeder.LoadToReadyAsync(nextWafer.CassetteIndex, processSetting.IsLoadport1);

                            }

                            //執行主設備動作 
                            await MicroDetection.Run(recipe.DetectRecipe, processSetting.AutoSave, pts, cts);
                            await MicroDetection.PutWaferPrepare(machineSetting.TableWaferCatchPosition);
                            //退片
                            await Feeder.MicroUnLoadToStandByAsync();

                        }
                        else
                        {
                            //退片
                            await Feeder.AlignerToStandByAsync();

                        }

                        //退片
                        await Feeder.UnLoadWaferToCassette(currentWafer, processSetting.IsLoadport1);


                        await Task.Delay(300);
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);

                        //等待預載完成
                        await taskLoad;


                        WriteLog($"Remaining number of wafers : {waferusableCount} ");
                        //判斷卡匣空了
                        if (waferusableCount == 0)
                        {
                            break;
                        }
                        currentWafer = nextWafer; //將下一片的資料轉成當前WAFER資料
                    }


                });


            }
            catch (OperationCanceledException canceleEx)
            {


            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                Feeder.ProcessEnd();
                cts.Dispose();
                WriteLog("Process Finish ");
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


        private async Task MacroTopInspection(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {

            //晶面檢查
            if (station == WaferProcessStatus.Select)
            {
                await Feeder.TurnWafer(eFEMtionRecipe);
                //委派到ui 去執行macro人工檢
                Task<WaferProcessStatus> macro = MacroReady?.Invoke(pts, cts);
                var judgeResult = macro.Result;
                await Feeder.Macro.HomeInnerRing();


                station = WaferProcessStatus.Complate;

            }

        }
        private async Task MacroBackInspection(WaferProcessStatus station, int MacroBackStartPos)
        {
            //晶背檢查
            if (station == WaferProcessStatus.Select)
            {
                //做翻面動作  可能Robot 取走翻轉完再放回 ，或Macro 機構本身能翻
                await Feeder.TurnBackWafer(MacroBackStartPos);
                //委派到ui層 去執行macro人工檢
                Task<WaferProcessStatus> macro = MacroReady?.Invoke(pts, cts);
                var judgeResult = macro.Result;

                //翻回來
                await Feeder.Macro.HomeOuterRing();
          
                station= WaferProcessStatus.Complate;
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
