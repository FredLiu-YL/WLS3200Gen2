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

        public async Task ProcessRunAsync(ProcessSetting processSetting)
        {
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();
                WriteLog("ProcessInitial ");
                Feeder.ProcessInitial(processSetting.Inch, machineSetting, pts, cts);

                await Task.Run(async () =>
                {
                    WriteLog("Process Start");


                    //先放一片在macro上
                    Wafer currentWafer = await Feeder.LoadToReadyAsync();

                    while (true)
                    {


                        if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                        MainRecipe recipe = ChangeRecipe?.Invoke();


                        //讀取 使用的Cassette  片子數量資訊
                        IEnumerable<Wafer> waferusable = Feeder.Cassette.Wafers.Where(w =>
                        w.ProcessStatus.MacroTop == WaferProcessStatus.Select ||
                        w.ProcessStatus.MacroBack == WaferProcessStatus.Select ||
                        w.ProcessStatus.WaferID == WaferProcessStatus.Select ||
                        w.ProcessStatus.Micro == WaferProcessStatus.Select);

                        //對應Cassette位置的 需要檢查站 CassetteIndex 可能要+1  或-1 才是實際位置
                        ProcessStation processStation = processSetting.ProcessStation[currentWafer.CassetteIndex];


                        //晶面檢查
                        await MacroTopInspection(processStation.MacroTop, recipe.EFEMRecipe);

                        //晶背檢查
                        await MacroBackInspection(processStation.MacroBack, recipe.EFEMRecipe.MacroBackStartPos);


                        //到Align
                        await Feeder.LoadToAlignerAsync(processStation.WaferID, recipe.EFEMRecipe);


                        // ProcessPause();
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);


                        Task<Wafer> taskLoad = null;


                        if (processStation.Micro == WaferProcessStatus.Select)//判斷如果有需要進顯微鏡
                        {
                            //顯微鏡站準備接WAFER
                            Task catchWafertask = MicroDetection.CatchWaferPrepare(machineSetting.TableWaferCatchPosition, pts, cts);
                            //到預備位置準備進片
                            await Feeder.AlignerToStandByAsync();
                            await catchWafertask; //等待顯微鏡站準備完成

                            //wafer送到主設備內 
                            Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                            currentWafer = await Feeder.LoadToMicroAsync(currentWafer);

                            if (waferusable.Count() > 0)//如果還有片
                            {
                                //預載一片在Macro上
                                taskLoad = Feeder.LoadToReadyAsync();

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
                        await Feeder.UnLoadWaferToCassette(currentWafer);


                        await Task.Delay(300);
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);

                        //等待預載完成
                        currentWafer = await taskLoad;

                        WriteLog($"Remaining number of wafers : {waferusable.Count()} ");
                        //判斷卡匣空了
                        if (waferusable.Count() == 0)
                        {
                            break;
                        }

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
            }


        }
        private void MicroVacuumOn()
        {

            MicroDetection.TableVacuum.On();

        }


    }
}
