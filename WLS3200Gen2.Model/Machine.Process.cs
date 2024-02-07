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

        public event Func<PauseTokenSource, CancellationTokenSource, Task<MacroJudge>> MacroReady;

        public async Task ProcessRunAsync(ProcessSetting processSetting)
        {
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();
                WriteLog("ProcessInitial ");
                Feeder.ProcessInitial(processSetting.Inch, pts, cts);

                await Task.Run(async () =>
                {
                    WriteLog("Process Start");
 

                    //先放一片在macro上
                    await Feeder.LoadToReadyAsync();

                    while (true)
                    {


                        if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                        MainRecipe recipe = ChangeRecipe?.Invoke();

                        //晶面檢查
                        if (processSetting.IsMacroTop)
                        {

                            //委派到ui 去執行macro人工檢
                            Task<MacroJudge> macro = MacroReady?.Invoke(pts, cts);
                            var judgeResult = macro.Result;



                        }


                        //晶背檢查
                        if (processSetting.IsMacroBack)
                        {

                            //做翻面動作  可能Robot 取走翻轉完再放回 ，或Macro 機構本身能翻
                            await Feeder.TurnWafer();

                            //委派到ui層 去執行macro人工檢
                            Task<MacroJudge> macro = MacroReady?.Invoke(pts, cts);
                            var judgeResult = macro.Result;


                            //翻回來
                            await Feeder.TurnBackWafer();

                        }

                        //到Align
                        await Feeder.LoadToAlignerAsync(processSetting.IsReadWaferID);


                        // ProcessPause();
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);

                        Wafer waferInside = null;
                        Task taskLoad = Task.CompletedTask;
                        var waferusable = Feeder.Cassette.Wafers.Select(w => w.ProcessStatus == WaferProcessStatus.Usable);


                        if (processSetting.IsMicro)//判斷如果有需要進顯微鏡
                        {
                            //顯微鏡站準備接WAFER
                            Task catchWafertask = MicroDetection.CatchWaferPrepare(machineSetting.TableWaferCatchPosition, pts, cts);
                            //到預備位置準備進片
                            await Feeder.AlignerToStandByAsync();
                            await catchWafertask; //等待顯微鏡站準備完成

                            //wafer送到主設備內 
                            Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                            waferInside = await Feeder.LoadToMicroAsync();

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
                        await Feeder.UnLoadWaferToCassette(waferInside);




                        await Task.Delay(300);
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);

                        //等待預載完成
                        await taskLoad;

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

        private void MicroVacuumOn()
        {

            MicroDetection.TableVacuum.On();

        }


    }
}
