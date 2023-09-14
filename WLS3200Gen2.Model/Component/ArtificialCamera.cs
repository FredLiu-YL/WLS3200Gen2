using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WLS3200Gen2.Model.Component
{
    public class ArtificialCamera : ICamera
    {
        private string imagePath;
        private bool isGrabbing;
        private BitmapSource bitmapSource;

        public ArtificialCamera()
        {

            isGrabbing = false;
        }

        public double ExposureTime { get  ; set  ; }
        public double Gain { get  ; set  ; }

        public bool IsGrabbing => isGrabbing;

        public Task GrabAsync()
        {
            return Task.Run(() => bitmapSource);

        }


        public void Grabbing()
        {
          
        }

        public void Initial()
        {
            
        }

        public void Stop()
        {
            
        }
    }
}
