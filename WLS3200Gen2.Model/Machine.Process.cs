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
        /// 需要切換Recipe的委派  ，不需要不用實作
        /// </summary>
        public event Func<MainRecipe> ChangeRecipe;

        public async Task ProcessRunAsync(ProcessSetting processSetting)
        {
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();


                await Feeder.LoadToReadyAsync(processSetting.Inch);


                MainRecipe recipe = ChangeRecipe?.Invoke();







                await Task.Delay(6000);
                cts.Token.ThrowIfCancellationRequested();
                await Task.Delay(3000);
                await pts.Token.WaitWhilePausedAsync(cts.Token);
                await Task.Delay(3000);
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
