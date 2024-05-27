using System;
using System.Collections.Generic;
using System.Drawing;
using Cognex.InSight;
using Cognex.InSight.Net;
using Cognex.InSight.Controls.Display;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Model.Interface.Component;
using System.Diagnostics;

namespace WLS3200Gen2.Model.Component
{
    public class CognexWaferID : IReader
    {
        private CvsInSightDisplay cvsInSightDisplay;
        private CvsNetworkMonitor cvsMonitor;
        private CvsHostSensor cvsHostSensor;
        private CvsInSight cvsInSight = new CvsInSight();
        public CognexWaferID()
        {
            try
            {
                cvsInSightDisplay = new Cognex.InSight.Controls.Display.CvsInSightDisplay();
                cvsInSightDisplay.DefaultTextScaleMode = CvsInSightDisplay.TextScaleModeType.Proportional;
                cvsInSightDisplay.PreferredCropScaleMode = CvsInSightDisplayCropScaleMode.Default;
                cvsInSightDisplay.InSightChanged += new System.EventHandler(this.cvsInSightDisplay_InSightChanged);
                cvsInSight = cvsInSightDisplay.InSight;

                cvsMonitor = new CvsNetworkMonitor();
                cvsMonitor.PingInterval = 0;
                cvsMonitor.Enabled = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Bitmap Image { get; private set; }
        public double Score { get; private set; }
        public void Initial()
        {
            try
            {
                if (!cvsInSightDisplay.Connected)
                {
                    int cnt = 0;
                    while (true)
                    {
                        if (cvsMonitor.Hosts.Count > 0)
                        {
                            break;
                        }
                        else if (cnt > 100)
                        {
                            throw new Exception("CognexWaferID Initial Error Hosts Count <= 0 !!");
                        }
                        System.Threading.Thread.Sleep(100);
                        cnt++;
                    }
                    cvsHostSensor = cvsMonitor.Hosts[cvsMonitor.Hosts[0].Name];
                    cvsInSightDisplay.Connect(cvsHostSensor.IPAddressString, "admin", "", false);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void cvsInSightDisplay_InSightChanged(object sender, EventArgs e)
        {
            try
            {
                cvsInSight = cvsInSightDisplay.InSight;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<string> ReadAsync()
        {
            try
            {
                return Task.Run(() =>
                {
                    if (cvsInSight != null)
                    {
                        //Stopwatch sw = new Stopwatch();
                        //sw.Start();
                        //sw.Stop();
                        cvsInSight.ManualAcquire();
                        var cc = cvsInSight.JobInfo.ActiveJobFile;
                        Image = cvsInSight.Results.Image.ToBitmap();
                        Score = Convert.ToDouble(cvsInSight.Results.Cells[11].ToString()) * 100;
                        if (Score >= 100)
                        {
                            return cvsInSight.Results.Cells[12].ToString();
                        }
                        else
                        {
                            return "";
                        }
                        //_InSight.Results.Image.Save("", System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else
                    {
                        throw new Exception("WaferID is Null !!");
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Close()
        {
            try
            {
                cvsInSightDisplay.Disconnect();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void SetParam(int paramID)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
