using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Model;
using YuanliCore.Model.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class MacroDetection
    {
        private ICamera camera;
        private PauseTokenSource pauseToken;
        private CancellationTokenSource cancelToken;
        private Subject<(BitmapSource, bool)> subject = new Subject<(BitmapSource, bool)>();
        private IDisposable camlive;
        public MacroDetection(ICamera camera, DigitalOutput[] outputs, DigitalInput[] inputs)
        {
            this.camera = camera;
            InnerRingRollXServo = outputs[52];
            InnerRingRollXOrg = outputs[53];
            InnerRingRollXForward = outputs[48];
            InnerRingRollXBackward = outputs[49];
            InnerRingRollXStop = outputs[50];
            InnerRingRollXAlarmReset = outputs[51];

            InnerRingPitchYServo = outputs[58];
            InnerRingPitchYOrg = outputs[59];
            InnerRingPitchYForward = outputs[54];
            InnerRingPitchYBackward = outputs[55];
            InnerRingPitchYStop = outputs[56];
            InnerRingPitchYAlarmReset = outputs[57];

            InnerRingLiftMotorStart = outputs[29];
            InnerRingLiftMotorM0 = outputs[30];
            InnerRingLiftMotorM1 = outputs[31];
            InnerRingLiftMotorAlarmReset = outputs[27];
            InnerRingLiftMotorServoOff = outputs[28];
            InnerRingLiftMotorStop = outputs[26];

            InnerRingLiftMotorStart = outputs[21];
            InnerRingLiftMotorM0 = outputs[22];
            InnerRingLiftMotorM1 = outputs[23];
            InnerRingLiftMotorAlarmReset = outputs[19];
            InnerRingLiftMotorServoOff = outputs[20];
            InnerRingLiftMotorStop = outputs[18];

            OuterRingYawZServo = outputs[43];
            OuterRingYawZOrg = outputs[42];
            OuterRingYawZForward = outputs[38];
            OuterRingYawZBackward = outputs[39];
            OuterRingYawZStop = outputs[40];
            OuterRingYawZAlarmReset = outputs[41];

            OuterRingYawZServo = outputs[36];
            OuterRingYawZOrg = outputs[37];
            OuterRingYawZForward = outputs[32];
            OuterRingYawZBackward = outputs[33];
            OuterRingYawZStop = outputs[34];
            OuterRingYawZAlarmReset = outputs[35];
            //ObservableDetection();
        }
        public DigitalOutput InnerRingRollXServo { get; }
        public DigitalOutput InnerRingRollXOrg { get; }
        public DigitalOutput InnerRingRollXForward { get; }
        public DigitalOutput InnerRingRollXBackward { get; }
        public DigitalOutput InnerRingRollXStop { get; }
        public DigitalOutput InnerRingRollXAlarmReset { get; }

        public DigitalOutput InnerRingPitchYServo { get; }
        public DigitalOutput InnerRingPitchYOrg { get; }
        public DigitalOutput InnerRingPitchYForward { get; }
        public DigitalOutput InnerRingPitchYBackward { get; }
        public DigitalOutput InnerRingPitchYStop { get; }
        public DigitalOutput InnerRingPitchYAlarmReset { get; }

        public DigitalOutput InnerRingYawZServo { get; }
        public DigitalOutput InnerRingYawZOrg { get; }
        public DigitalOutput InnerRingYawZForward { get; }
        public DigitalOutput InnerRingYawZBackward { get; }
        public DigitalOutput InnerRingYawZStop { get; }
        public DigitalOutput InnerRingYawZAlarmReset { get; }

        public DigitalOutput InnerRingLiftMotorStart { get; }
        public DigitalOutput InnerRingLiftMotorM0 { get; }
        public DigitalOutput InnerRingLiftMotorM1 { get; }
        public DigitalOutput InnerRingLiftMotorAlarmReset { get; }
        public DigitalOutput InnerRingLiftMotorServoOff { get; }
        public DigitalOutput InnerRingLiftMotorStop { get; }


        public DigitalOutput OuterRingYawZServo { get; }
        public DigitalOutput OuterRingYawZOrg { get; }
        public DigitalOutput OuterRingYawZForward { get; }
        public DigitalOutput OuterRingYawZBackward { get; }
        public DigitalOutput OuterRingYawZStop { get; }
        public DigitalOutput OuterRingYawZAlarmReset { get; }

        public DigitalOutput OuterRingLiftMotorStart { get; }
        public DigitalOutput OuterRingLiftMotorM0 { get; }
        public DigitalOutput OuterRingLiftMotorM1 { get; }
        public DigitalOutput OuterRingLiftMotorAlarmReset { get; }
        public DigitalOutput OuterRingLiftMotorServoOff { get; }
        public DigitalOutput OuterRingLiftMotorStop { get; }

        public DigitalInput InnerRingRollXORG { get; }
        public DigitalInput InnerRingPitchYORG { get; }
        public DigitalInput InnerRingLiftMotorOK { get; }
        public DigitalInput InnerRingLiftMotorORG { get; }

        public DigitalInput OuterRingYawZMotorOK { get; }
        public DigitalInput OuterRingYawZMotorORG { get; }

        public DigitalInput OuterRingLiftMotorOK { get; }
        public DigitalInput OuterRingLiftMotorORG { get; }
        /// <summary>
        /// 外環上升/下降
        /// </summary>
        /// <returns></returns>
        private bool LiftOuterRing(bool isUp)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內上升/下降
        /// </summary>
        /// <returns></returns>
        private bool LiftInnerRing(bool isUp)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 是否可以外環動作
        /// </summary>
        /// <returns></returns>
        private bool CheckCanMoveOuterRing()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 是否可以內環動作
        /// </summary>
        /// <returns></returns>
        private bool CheckCanMoveInnerRing()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 外環 縱軸/翻滾 X 移動
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingRollX_Move(bool isForward)
        {

        }
        /// <summary>
        /// 外環 縱軸/翻滾 X 停止
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingRollX_Stop(bool isForward)
        {

        }
        /// <summary>
        /// 外環 橫軸/俯仰 Y 移動
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingPitchY_Move(bool isForward)
        {

        }
        /// <summary>
        /// 外環 橫軸/俯仰 Y 停止
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingPitchY_Stop(bool isForward)
        {

        }
        /// <summary>
        /// 外環 垂軸/偏擺 Z 移動
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingYawZ_Move(bool isForward)
        {

        }
        /// <summary>
        /// 外環 垂軸/偏擺 Z 停止
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingYawZ_Stop(bool isForward)
        {

        }
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public void InnerRingRollX_Move(bool isForward)
        {

        }
        /// <summary>
        /// 內環 縱軸/翻滾 停止
        /// </summary>
        public void InnerRingRollX_Stop(bool isForward)
        {

        }
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public void InnerRingPitchY_Move(bool isForward)
        {

        }
        /// <summary>
        /// 內環 縱軸/翻滾 停止
        /// </summary>
        public void InnerRingPitchY_Stop(bool isForward)
        {

        }
        /// <summary>
        /// 內環 垂軸/偏擺 移動
        /// </summary>
        /// <param name="isForward"></param>
        public void InnerRingYawZ_Move(bool isForward)
        {

        }
        /// <summary>
        /// 內環 垂軸/偏擺 停止
        /// </summary>
        /// <param name="isForward"></param>
        public void InnerRingYawZ_Stop(bool isForward)
        {

        }


        /// <summary>
        /// 檢測功能
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public async Task DefectDetection(BitmapSource bmp)
        {



        }
        //private void ObservableDetection()
        //{
        //    camlive = Observable//.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
        //            .ObserveOn(TaskPoolScheduler.Default)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
        //            .Subscribe(async frame =>
        //            {
        //                if (frame.isAutoSave)
        //                {
        //                    frame.image.Save("C:\\TEST", ImageFileFormats.Bmp);
        //                }
        //                await DefectDetection(frame.image);
        //            });
        //}
    }
}
