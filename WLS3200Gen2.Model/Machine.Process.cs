﻿using Nito.AsyncEx;
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




        /// <summary>
        /// 需要切換Recipe的委派  
        /// </summary>
        public event Func<MainRecipe> ChangeRecipe;

        public event Action MacroReady;

        public async Task ProcessRunAsync(ProcessSetting processSetting)
        {
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();

                Feeder.ProcessInitial(processSetting.Inch, pts, cts);


                await Task.Run(async () =>
                {
                    //先放一片在macro上
                    await Feeder.LoadToReadyAsync();

                    while (true)
                    {


                        if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                        MainRecipe recipe = ChangeRecipe?.Invoke();

                        //委派到上層 去執行macro動作
                        MacroReady?.Invoke();

                        // ProcessPause();
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token); //暫停在Macro上

                        //顯微鏡站準備接WAFER
                        Task catchWafertask = MicroDetection.CatchWaferPrepare(machineSetting.TableWaferCatchPosition, pts, cts);

                        //到預備位置準備進片
                        await Feeder.LoadToMicroReadyAsync();
                        await catchWafertask; //等待顯微鏡站準備完成

                        //wafer送到主設備內 
                        Feeder.MicroFixed = MicroVacuumOn;//委派 顯微鏡的固定方式
                        Wafer waferInside = await Feeder.LoadToMicroAsync();

                        Task taskLoad = Task.CompletedTask;
                        var waferusable = Feeder.Cassette.Wafers.Select(w => w.ProcessStatus == WaferProcessStatus.Usable);
                        if (waferusable.Count() > 0)//如果還有片
                        {
                            //預載一片在Macro上
                            taskLoad = Feeder.LoadToReadyAsync();
                        }




                        //執行主設備動作
                        await MicroDetection.Run(recipe.DetectRecipe, processSetting.AutoSave, pts, cts);

                        await MicroDetection.PutWaferPrepare(machineSetting.TableWaferCatchPosition);
                        //退片
                        await Feeder.UnLoadAsync(waferInside);


                        await Task.Delay(300);
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);

                        //等待預載完成
                        await taskLoad;

                        //判斷卡匣空了
                        if (waferusable.Count()==0)
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


            }
            finally
            {
                Feeder.ProcessEnd();


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
