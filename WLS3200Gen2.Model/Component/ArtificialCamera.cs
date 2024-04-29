using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuanliCore.Interface;

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

        public double ExposureTime { get; set; }
        public double Gain { get; set; }

        public bool IsGrabbing => isGrabbing;

        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public IObservable<Frame<byte[]>> Frames => throw new NotImplementedException();

        public PixelFormat PixelFormat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Close()
        {

        }

        public IDisposable Grab()
        {
            throw new NotImplementedException();
        }

        public Task GrabAsync()
        {
            return Task.Run(() => bitmapSource);

        }




        public void Open()
        {

        }

        public void Stop()
        {

        }

        BitmapSource ICamera.GrabAsync()
        {
            return bitmapSource;
        }
    }
}
