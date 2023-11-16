using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Recipe;

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

                await Task.Run(async () =>
                {
                    //先放一片在macro上
                    await Feeder.LoadToReadyAsync(processSetting.Inch, pts, cts);

                    while (true)
                    {


                        if (ChangeRecipe == null) throw new NotImplementedException("ChangeRecipe  Not Implemented");
                        MainRecipe recipe = ChangeRecipe?.Invoke();

                        //委派到上層 去執行macro動作
                        MacroReady.Invoke();
                        // ProcessPause();
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token); //暫停在Macro上

                        //macro動作完成後 ，載到主設備內 
                        await Feeder.LoadAsync();

                        //預載一片在Macro上
                        Task taskLoad = Feeder.LoadToReadyAsync(processSetting.Inch, pts, cts);

                        //執行主設備動作
                        await MicroDetection.Run(recipe.DetectRecipe, pts, cts);


                        await Task.Delay(6000);
                        cts.Token.ThrowIfCancellationRequested();
                        await pts.Token.WaitWhilePausedAsync(cts.Token);

                        //等待預載完成
                        await taskLoad;

                    }
                });


            }
            catch (OperationCanceledException canceleEx)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw;
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
    }
}
