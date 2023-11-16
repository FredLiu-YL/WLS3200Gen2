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

        public event Func<MainRecipe> ChangeRecipe;
        private PauseTokenSource pts = new PauseTokenSource();
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async Task ProcessRunAsync()
        {
            try
            {
                pts = new PauseTokenSource();
                cts = new CancellationTokenSource();


                // do something......
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
