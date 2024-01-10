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
            //this.camera = camera;
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

            InnerRingYawZServo = outputs[43];
            InnerRingYawZOrg = outputs[42];
            InnerRingYawZForward = outputs[38];
            InnerRingYawZBackward = outputs[39];
            InnerRingYawZStop = outputs[40];
            InnerRingYawZAlarmReset = outputs[41];

            InnerRingLiftMotorStart = outputs[29];
            InnerRingLiftMotorM0 = outputs[30];
            InnerRingLiftMotorM1 = outputs[31];
            InnerRingLiftMotorAlarmReset = outputs[27];
            InnerRingLiftMotorServoOff = outputs[28];
            InnerRingLiftMotorStop = outputs[26];


            OuterRingPitchYServo = outputs[36];
            OuterRingPitchYOrg = outputs[37];
            OuterRingPitchYForward = outputs[32];
            OuterRingPitchYBackward = outputs[33];
            OuterRingPitchYStop = outputs[34];
            OuterRingPitchYAlarmReset = outputs[35];

            OuterRingLiftMotorStart = outputs[21];
            OuterRingLiftMotorM0 = outputs[22];
            OuterRingLiftMotorM1 = outputs[23];
            OuterRingLiftMotorAlarmReset = outputs[19];
            OuterRingLiftMotorServoOff = outputs[20];
            OuterRingLiftMotorStop = outputs[18];
            ////Input
            ///
            InnerRingRollXIsORG = inputs[23];
            InnerRingPitchYIsORG = inputs[21];
            InnerRingLiftMotorIsOK = inputs[28];
            InnerRingLiftMotorIsORG = inputs[29];

            OuterRingPitchYMotorIsOK = inputs[17];
            OuterRingPitchYMotorIsORG = inputs[18];

            OuterRingLiftMotorIsOK = inputs[25];
            OuterRingLiftMotorIsORG = inputs[26];





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

        /// <summary>
        /// 內環升降馬達開始運作 M1,M0 00:Down 10:Up
        /// </summary>
        public DigitalOutput InnerRingLiftMotorStart { get; }
        public DigitalOutput InnerRingLiftMotorM0 { get; }
        public DigitalOutput InnerRingLiftMotorM1 { get; }
        public DigitalOutput InnerRingLiftMotorAlarmReset { get; }
        public DigitalOutput InnerRingLiftMotorServoOff { get; }
        public DigitalOutput InnerRingLiftMotorStop { get; }


        public DigitalOutput OuterRingPitchYServo { get; }
        public DigitalOutput OuterRingPitchYOrg { get; }
        public DigitalOutput OuterRingPitchYForward { get; }
        public DigitalOutput OuterRingPitchYBackward { get; }
        public DigitalOutput OuterRingPitchYStop { get; }
        public DigitalOutput OuterRingPitchYAlarmReset { get; }

        /// <summary>
        /// 外環升降馬達開始運作 M1,M0 00:Down 01:UpToDown 11:Up 10:DownToUp
        /// </summary>
        public DigitalOutput OuterRingLiftMotorStart { get; }
        public DigitalOutput OuterRingLiftMotorM0 { get; }
        public DigitalOutput OuterRingLiftMotorM1 { get; }
        public DigitalOutput OuterRingLiftMotorAlarmReset { get; }
        public DigitalOutput OuterRingLiftMotorServoOff { get; }
        public DigitalOutput OuterRingLiftMotorStop { get; }

        public DigitalInput InnerRingRollXIsORG { get; }
        public DigitalInput InnerRingPitchYIsORG { get; }
        public DigitalInput InnerRingLiftMotorIsOK { get; }
        public DigitalInput InnerRingLiftMotorIsORG { get; }

        public DigitalInput OuterRingPitchYMotorIsOK { get; }
        public DigitalInput OuterRingPitchYMotorIsORG { get; }

        public DigitalInput OuterRingLiftMotorIsOK { get; }
        public DigitalInput OuterRingLiftMotorIsORG { get; }


        public async Task HomeAllRing(bool isFirstHome)
        {
            try
            {
                await HomeInnerRing(isFirstHome);
                await HomeOuterRing(isFirstHome);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 外環復歸 isFirstHome=true:一開機的復歸，外環要在下Sensor(若沒有，要請工程師確認)。isFirstHome=false:不再執行外環檢查的復歸，外環要在上、內環要在下
        /// </summary>
        /// <param name="isFirstHome"></param>
        /// <returns></returns>
        public async Task HomeOuterRing(bool isFirstHome)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;

                    if (isFirstHome == true)
                    {
                        //外環抬升軸上來
                        i = 0;
                        if (OuterRingLiftMotorIsORG.IsSignal != true || OuterRingLiftMotorIsOK.IsSignal != true)
                        {
                            throw new Exception($"外環 不在下方，請工程師確認!!");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(25);
                        //看程式這邊有關掉真空 應該是可以不用動作，或者是要加把真空開著
                        await Task.Delay(50);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        await Task.Delay(150);
                        while (OuterRingLiftMotorIsOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 馬達異常Time out");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(50);
                        while (OuterRingLiftMotorIsOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 馬達異常Time out");
                        }
                        OuterRingLiftMotorM0.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        while (OuterRingLiftMotorIsOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 馬達抬升異常Time out");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(25);
                    }
                    //外環旋轉復歸
                    i = 0;
                    if (OuterRingLiftMotorIsORG.IsSignal == true || OuterRingLiftMotorIsOK.IsSignal != true)
                    {
                        throw new Exception($"外環 不在上方，請工程師確認!!");
                    }
                    OuterRingPitchYOrg.Off();
                    await Task.Delay(25);
                    OuterRingPitchYOrg.On();
                    await Task.Delay(150);
                    while (OuterRingPitchYMotorIsOK.IsSignal != true || OuterRingPitchYMotorIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 翻轉馬達復歸異常Time out");
                    }
                    OuterRingPitchYOrg.Off();
                    await Task.Delay(25);
                    //外環抬升軸下降復歸
                    i = 0;
                    if (OuterRingLiftMotorIsORG.IsSignal == true || OuterRingLiftMotorIsOK.IsSignal != true)
                    {
                        throw new Exception($"外環 不在上方，請工程師確認!!");
                    }
                    OuterRingLiftMotorStart.Off();
                    OuterRingLiftMotorM0.Off();
                    OuterRingLiftMotorM1.Off();
                    await Task.Delay(25);
                    OuterRingLiftMotorM0.On();
                    await Task.Delay(25);
                    OuterRingLiftMotorStart.On();
                    await Task.Delay(25);
                    await Task.Delay(150);
                    while (OuterRingLiftMotorIsOK.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.Off();
                    OuterRingLiftMotorM0.Off();
                    await Task.Delay(50);
                    while (OuterRingLiftMotorIsOK.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.On();
                    await Task.Delay(50);
                    while (OuterRingLiftMotorIsOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.Off();
                    await Task.Delay(50);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環復歸 isFirstHome=true:一開機的復歸，沒有一定要在哪個狀態。isFirstHome=false:不再執行內環檢查的復歸，內環要在上、外環要在下
        /// </summary>
        /// <param name="isFirstHome"></param>
        /// <returns></returns>
        public async Task HomeInnerRing(bool isFirstHome)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;

                    //X、Y翻轉復歸
                    i = 0;
                    InnerRingRollXOrg.Off();
                    InnerRingRollXOrg.Off();
                    await Task.Delay(50);
                    InnerRingRollXOrg.On();
                    InnerRingRollXOrg.On();
                    await Task.Delay(150);
                    while (InnerRingRollXIsORG.IsSignal != true || InnerRingPitchYIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"內環 X、Y翻轉復歸Time out");
                    }
                    //內環下降復歸
                    i = 0;
                    await Task.Delay(25);
                    InnerRingLiftMotorStart.Off();
                    InnerRingLiftMotorM0.Off();
                    InnerRingLiftMotorM1.Off();
                    await Task.Delay(25);
                    InnerRingLiftMotorStart.On();
                    while (InnerRingLiftMotorIsOK.IsSignal != true || InnerRingLiftMotorIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"內環 下降復歸Time out");
                    }
                    //內環旋轉馬達歸零
                    i = 0;
                    await Task.Delay(25);
                    InnerRingYawZOrg.Off();
                    await Task.Delay(50);
                    InnerRingYawZOrg.On();
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 外環上升/下降
        /// </summary>
        /// <returns></returns>
        public async Task LiftUpOuterRing()
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;
                    if (CheckCanMoveOuterRing() == CheckCanMove.OuterInOrg || CheckCanMoveOuterRing() == CheckCanMove.OK)
                    {
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(25);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        await Task.Delay(150);
                        while (OuterRingLiftMotorIsOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(50);
                        while (OuterRingLiftMotorIsOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorM0.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        while (OuterRingLiftMotorIsOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(50);

                    }
                    else
                    {
                        throw new Exception($"外環 不能上升!");
                    }
                });

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環上升
        /// </summary>
        /// <returns></returns>
        public async Task LiftUpInnerRing()
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;
                    if (CheckCanMoveInnerRing() == CheckCanMove.InnerInOrg || CheckCanMoveInnerRing() == CheckCanMove.OK)
                    {
                        await Task.Delay(25);
                        InnerRingLiftMotorStart.Off();
                        InnerRingLiftMotorM0.Off();
                        InnerRingLiftMotorM1.Off();
                        await Task.Delay(25);
                        InnerRingLiftMotorM1.On();
                        await Task.Delay(25);
                        InnerRingLiftMotorStart.On();
                        await Task.Delay(150);
                        while (InnerRingLiftMotorIsOK.IsSignal != true || InnerRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"內環 上升異常");
                        }
                    }
                    else
                    {
                        throw new Exception($"內環 不能上升!");
                    }
                });

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private enum CheckCanMove
        {
            OK = 1,
            InnerInOrg = 2,
            InnerInTop = 3,
            OuterInOrg = 4,
            OuterInTop = 5,
            InnerMotorError = 6,
            OuterMotorError = 7
        }
        /// <summary>
        /// 是否可以外環動作
        /// </summary>
        /// <returns></returns>
        private CheckCanMove CheckCanMoveOuterRing()
        {
            //Outer In Top
            //Inner In Org
            try
            {
                if (OuterRingLiftMotorIsORG.IsSignal != false)
                {
                    return CheckCanMove.OuterInOrg;
                }
                if (OuterRingLiftMotorIsOK.IsSignal != true)
                {
                    return CheckCanMove.OuterMotorError;
                }
                if (InnerRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckCanMove.InnerInTop;
                }
                if (InnerRingLiftMotorIsOK.IsSignal != true)
                {
                    return CheckCanMove.InnerMotorError;
                }
                return CheckCanMove.OK;
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
        private CheckCanMove CheckCanMoveInnerRing()
        {
            //Outer In Org
            //Inner In Top
            try
            {
                if (OuterRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckCanMove.OuterInTop;
                }
                if (OuterRingLiftMotorIsOK.IsSignal != true)
                {
                    return CheckCanMove.OuterMotorError;
                }
                if (InnerRingLiftMotorIsORG.IsSignal != false)
                {
                    return CheckCanMove.InnerInOrg;
                }
                if (InnerRingLiftMotorIsOK.IsSignal != true)
                {
                    return CheckCanMove.InnerMotorError;
                }
                return CheckCanMove.OK;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 是否內外環都復歸成功
        /// </summary>
        /// <returns></returns>
        private CheckCanMove CheckRingInOrg()
        {
            //Outer In Org
            //Inner In Top
            try
            {
                if (OuterRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckCanMove.OuterInTop;
                }
                if (OuterRingLiftMotorIsOK.IsSignal != true)
                {
                    return CheckCanMove.OuterMotorError;
                }
                if (InnerRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckCanMove.InnerInTop;
                }
                if (InnerRingLiftMotorIsOK.IsSignal != true)
                {
                    return CheckCanMove.InnerMotorError;
                }
                return CheckCanMove.OK;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        ///// <summary>
        ///// 外環 縱軸/翻滾 X 移動
        ///// </summary>
        ///// <param name="isForward"></param>
        //public void OuterRingRollX_Move(bool isForward)
        //{

        //}
        ///// <summary>
        ///// 外環 縱軸/翻滾 X 停止
        ///// </summary>
        ///// <param name="isForward"></param>
        //public async Task OuterRingRollX_Stop(bool isForward)
        //{

        //}
        /// <summary>
        /// 外環 橫軸/俯仰 Y 移動
        /// </summary>
        /// <param name="isForward"></param>
        public async Task OuterRingPitchY_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveOuterRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        OuterRingPitchYForward.On();
                        OuterRingPitchYBackward.Off();
                    }
                    else
                    {
                        OuterRingPitchYForward.Off();
                        OuterRingPitchYBackward.On();
                    }

                }
                else
                {
                    throw new Exception($"內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 外環 橫軸/俯仰 Y 停止
        /// </summary>
        /// <param name="isForward"></param>
        public async Task OuterRingPitchY_Stop()
        {
            try
            {
                OuterRingPitchYForward.Off();
                OuterRingPitchYBackward.Off();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        ///// <summary>
        ///// 外環 垂軸/偏擺 Z 移動
        ///// </summary>
        ///// <param name="isForward"></param>
        //public async Task OuterRingYawZ_Move(bool isForward)
        //{

        //}
        ///// <summary>
        ///// 外環 垂軸/偏擺 Z 停止
        ///// </summary>
        ///// <param name="isForward"></param>
        //public async Task OuterRingYawZ_Stop(bool isForward)
        //{

        //}
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public void InnerRingRollX_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveInnerRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingRollXForward.On();
                        InnerRingRollXBackward.Off();
                    }
                    else
                    {
                        InnerRingRollXForward.Off();
                        InnerRingRollXBackward.On();
                    }

                }
                else
                {
                    throw new Exception($"內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 停止
        /// </summary>
        public void InnerRingRollX_Stop()
        {
            try
            {
                InnerRingRollXForward.Off();
                InnerRingRollXBackward.Off();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public async Task InnerRingPitchY_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveInnerRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingPitchYForward.On();
                        InnerRingPitchYBackward.Off();
                    }
                    else
                    {
                        InnerRingPitchYForward.Off();
                        InnerRingPitchYBackward.On();
                    }

                }
                else
                {
                    throw new Exception($"內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 停止
        /// </summary>
        public async Task InnerRingPitchY_Stop()
        {
            try
            {
                InnerRingPitchYForward.Off();
                InnerRingPitchYBackward.Off();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環 垂軸/偏擺 移動
        /// </summary>
        /// <param name="isForward"></param>
        public async Task InnerRingYawZ_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveInnerRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingYawZForward.On();
                        InnerRingYawZBackward.Off();
                    }
                    else
                    {
                        InnerRingYawZForward.Off();
                        InnerRingYawZBackward.On();
                    }

                }
                else
                {
                    throw new Exception($"內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環 垂軸/偏擺 停止
        /// </summary>
        /// <param name="isForward"></param>
        public async Task InnerRingYawZ_Stop()
        {
            try
            {
                InnerRingYawZForward.Off();
                InnerRingYawZBackward.Off();
            }
            catch (Exception ex)
            {

                throw ex;
            }
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
