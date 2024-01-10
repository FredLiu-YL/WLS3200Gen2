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
        public MacroDetection(DigitalOutput[] outputs, DigitalInput[] inputs)
        {
            //this.camera = camera;
            InnerRingPitchXServo = outputs[52];
            InnerRingPitchXOrg = outputs[53];
            InnerRingPitchXForward = outputs[48];
            InnerRingPitchXBackward = outputs[49];
            InnerRingPitchXStop = outputs[50];
            InnerRingPitchXAlarmReset = outputs[51];

            InnerRingRollYServo = outputs[58];
            InnerRingRollYOrg = outputs[59];
            InnerRingRollYForward = outputs[54];
            InnerRingRollYBackward = outputs[55];
            InnerRingRollYStop = outputs[56];
            InnerRingRollYAlarmReset = outputs[57];

            InnerRingYawTServo = outputs[43];
            InnerRingYawTOrg = outputs[42];
            InnerRingYawTForward = outputs[38];
            InnerRingYawTBackward = outputs[39];
            InnerRingYawTStop = outputs[40];
            InnerRingYawTAlarmReset = outputs[41];

            InnerRingLiftMotorStart = outputs[29];
            InnerRingLiftMotorM0 = outputs[30];
            InnerRingLiftMotorM1 = outputs[31];
            InnerRingLiftMotorAlarmReset = outputs[27];
            InnerRingLiftMotorServoOff = outputs[28];
            InnerRingLiftMotorStop = outputs[26];


            OuterRingRollYServo = outputs[36];
            OuterRingRollYOrg = outputs[37];
            OuterRingRollYForward = outputs[32];
            OuterRingRollYBackward = outputs[33];
            OuterRingRollYStop = outputs[34];
            OuterRingRollYAlarmReset = outputs[35];

            OuterRingLiftMotorStart = outputs[21];
            OuterRingLiftMotorM0 = outputs[22];
            OuterRingLiftMotorM1 = outputs[23];
            OuterRingLiftMotorAlarmReset = outputs[19];
            OuterRingLiftMotorServoOff = outputs[20];
            OuterRingLiftMotorStop = outputs[18];
            ////Input
            ///
            InnerRingPitchXIsORG = inputs[23];
            InnerRingRollYIsORG = inputs[21];
            InnerRingLiftMotorIsOK = inputs[28];
            InnerRingLiftMotorIsORG = inputs[29];

            OuterRingRollYMotorIsOK = inputs[17];
            OuterRingRollYMotorIsORG = inputs[18];

            OuterRingLiftMotorIsOK = inputs[25];
            OuterRingLiftMotorIsORG = inputs[26];





            //ObservableDetection();
        }
        private DigitalOutput InnerRingPitchXServo { get; }
        private DigitalOutput InnerRingPitchXOrg { get; }
        private DigitalOutput InnerRingPitchXForward { get; }
        private DigitalOutput InnerRingPitchXBackward { get; }
        private DigitalOutput InnerRingPitchXStop { get; }
        private DigitalOutput InnerRingPitchXAlarmReset { get; }

        private DigitalOutput InnerRingRollYServo { get; }
        private DigitalOutput InnerRingRollYOrg { get; }
        private DigitalOutput InnerRingRollYForward { get; }
        private DigitalOutput InnerRingRollYBackward { get; }
        private DigitalOutput InnerRingRollYStop { get; }
        private DigitalOutput InnerRingRollYAlarmReset { get; }

        private DigitalOutput InnerRingYawTServo { get; }
        private DigitalOutput InnerRingYawTOrg { get; }
        private DigitalOutput InnerRingYawTForward { get; }
        private DigitalOutput InnerRingYawTBackward { get; }
        private DigitalOutput InnerRingYawTStop { get; }
        private DigitalOutput InnerRingYawTAlarmReset { get; }

        /// <summary>
        /// 內環升降馬達開始運作 M1,M0 00:Down 10:Up
        /// </summary>
        private DigitalOutput InnerRingLiftMotorStart { get; }
        private DigitalOutput InnerRingLiftMotorM0 { get; }
        private DigitalOutput InnerRingLiftMotorM1 { get; }
        private DigitalOutput InnerRingLiftMotorAlarmReset { get; }
        private DigitalOutput InnerRingLiftMotorServoOff { get; }
        private DigitalOutput InnerRingLiftMotorStop { get; }


        private DigitalOutput OuterRingRollYServo { get; }
        private DigitalOutput OuterRingRollYOrg { get; }
        private DigitalOutput OuterRingRollYForward { get; }
        private DigitalOutput OuterRingRollYBackward { get; }
        private DigitalOutput OuterRingRollYStop { get; }
        private DigitalOutput OuterRingRollYAlarmReset { get; }

        /// <summary>
        /// 外環升降馬達開始運作 M1,M0 00:Down 01:UpToDown 11:Up 10:DownToUp
        /// </summary>
        private DigitalOutput OuterRingLiftMotorStart { get; }
        private DigitalOutput OuterRingLiftMotorM0 { get; }
        private DigitalOutput OuterRingLiftMotorM1 { get; }
        private DigitalOutput OuterRingLiftMotorAlarmReset { get; }
        private DigitalOutput OuterRingLiftMotorServoOff { get; }
        private DigitalOutput OuterRingLiftMotorStop { get; }

        private DigitalInput InnerRingPitchXIsORG { get; }
        private DigitalInput InnerRingRollYIsORG { get; }
        private DigitalInput InnerRingLiftMotorIsOK { get; }
        private DigitalInput InnerRingLiftMotorIsORG { get; }

        private DigitalInput OuterRingRollYMotorIsOK { get; }
        private DigitalInput OuterRingRollYMotorIsORG { get; }

        private DigitalInput OuterRingLiftMotorIsOK { get; }
        private DigitalInput OuterRingLiftMotorIsORG { get; }


        public async Task HomeAllRing()
        {
            try
            {
                bool isFirstHome = true;
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
                    OuterRingRollYOrg.Off();
                    await Task.Delay(25);
                    OuterRingRollYOrg.On();
                    await Task.Delay(150);
                    while (OuterRingRollYMotorIsOK.IsSignal != true || OuterRingRollYMotorIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 翻轉馬達復歸異常Time out");
                    }
                    OuterRingRollYOrg.Off();
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
                    InnerRingPitchXOrg.Off();
                    InnerRingPitchXOrg.Off();
                    await Task.Delay(50);
                    InnerRingPitchXOrg.On();
                    InnerRingPitchXOrg.On();
                    await Task.Delay(150);
                    while (InnerRingPitchXIsORG.IsSignal != true || InnerRingRollYIsORG.IsSignal != true)
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
                    InnerRingYawTOrg.Off();
                    await Task.Delay(50);
                    InnerRingYawTOrg.On();
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 外環上升
        /// </summary>
        /// <returns></returns>
        public async Task OuterRingLiftUp()
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
        public async Task InnerRingLiftUp()
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
        //public void OuterRingPitchX_Move(bool isForward)
        //{

        //}
        ///// <summary>
        ///// 外環 縱軸/翻滾 X 停止
        ///// </summary>
        ///// <param name="isForward"></param>
        //public async Task OuterRingPitchX_Stop(bool isForward)
        //{

        //}
        /// <summary>
        /// 外環 橫軸/俯仰 Y 移動
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingRollY_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveOuterRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        OuterRingRollYForward.On();
                        OuterRingRollYBackward.Off();
                    }
                    else
                    {
                        OuterRingRollYForward.Off();
                        OuterRingRollYBackward.On();
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
        public void OuterRingRollY_Stop()
        {
            try
            {
                OuterRingRollYForward.Off();
                OuterRingRollYBackward.Off();
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
        //public async Task OuterRingYawT_Move(bool isForward)
        //{

        //}
        ///// <summary>
        ///// 外環 垂軸/偏擺 Z 停止
        ///// </summary>
        ///// <param name="isForward"></param>
        //public async Task OuterRingYawT_Stop(bool isForward)
        //{

        //}
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public void InnerRingPitchX_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveInnerRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingPitchXForward.On();
                        InnerRingPitchXBackward.Off();
                    }
                    else
                    {
                        InnerRingPitchXForward.Off();
                        InnerRingPitchXBackward.On();
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
        public void InnerRingPitchX_Stop()
        {
            try
            {
                InnerRingPitchXForward.Off();
                InnerRingPitchXBackward.Off();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public void InnerRingRollY_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveInnerRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingRollYForward.On();
                        InnerRingRollYBackward.Off();
                    }
                    else
                    {
                        InnerRingRollYForward.Off();
                        InnerRingRollYBackward.On();
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
        public void InnerRingRollY_Stop()
        {
            try
            {
                InnerRingRollYForward.Off();
                InnerRingRollYBackward.Off();
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
        public void InnerRingYawT_Move(bool isForward)
        {
            try
            {
                if (CheckCanMoveInnerRing() == CheckCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingYawTForward.On();
                        InnerRingYawTBackward.Off();
                    }
                    else
                    {
                        InnerRingYawTForward.Off();
                        InnerRingYawTBackward.On();
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
        public void InnerRingYawT_Stop()
        {
            try
            {
                InnerRingYawTForward.Off();
                InnerRingYawTBackward.Off();
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
