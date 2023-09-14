using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WLS3200Gen2.Model.Component
{
    public class DummyCamera : ICamera
    {
        private string imagePath;
        private bool isGrabbing;
        private BitmapSource bitmapSource;

        public DummyCamera(string path)
        {
            imagePath = path;

        }

        public double ExposureTime { get; set; }
        public double Gain { get; set; }

      
        public bool IsGrabbing => isGrabbing;

 
        public Task GrabAsync()
        {
            return Task.Run(() => bitmapSource);
           
        }

        public void Grabbing()
        {
            isGrabbing = true;
        }

        public void Initial()
        {
             
        }

        public void Stop()
        {
            isGrabbing=false;
        }
    }
}
