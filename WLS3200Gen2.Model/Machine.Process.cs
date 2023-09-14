using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Recipe;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {

        public event Func<MainRecipe> ChangeRecipe;


        public async Task ProcessRun()
        {
            
            // do something......
            MainRecipe recipe =  ChangeRecipe?.Invoke();


        }




        public async Task ProcessStop()
        {


        }
    }
}
