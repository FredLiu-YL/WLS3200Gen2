using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Recipe
{
    public  class MainRecipe:AbstractRecipe
    {

        public DetectionRecipe DetectRecipe { get; set; } = new DetectionRecipe();

    }
}
