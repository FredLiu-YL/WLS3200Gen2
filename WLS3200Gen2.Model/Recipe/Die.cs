using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YuanliCore.Data
{
    public  class Die
    {
        public int IndexX { get; set; }
        public int IndexY { get; set; }

        public Point PosX { get; set; }
        public Point PosY { get; set; }

        public Size DieSize { get; set; }
    }
}
