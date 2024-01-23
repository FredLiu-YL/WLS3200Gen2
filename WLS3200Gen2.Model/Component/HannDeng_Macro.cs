using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Component
{
    public class HannDeng_Macro : IMacro
    {
        private bool isCanMoveAllHome = false;
        private bool isInnerCanMoveStartPos = false;
        private bool isInnerUsing = false;
        private bool isOuterCanMoveStartPos = false;
        private bool isOuterUsing = false;
        private DigitalOutput InnerRingVacuum { get; }

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
        /// 外環升降馬達開始運作 M1,M0 00:Down 10:DownToUp 11:Up 01:UpToDown
        /// </summary>
        private DigitalOutput OuterRingLiftMotorStart { get; }
        private DigitalOutput OuterRingLiftMotorM0 { get; }
        private DigitalOutput OuterRingLiftMotorM1 { get; }
        private DigitalOutput OuterRingLiftMotorAlarmReset { get; }
        private DigitalOutput OuterRingLiftMotorServoOff { get; }
        private DigitalOutput OuterRingLiftMotorStop { get; }

        private DigitalInput InnerRingIsVacuumOn { get; }

        private DigitalInput InnerRingPitchXIsORG { get; }
        private DigitalInput InnerRingRollYIsORG { get; }
        private DigitalInput InnerRingLiftMotorIsOK { get; }
        private DigitalInput InnerRingLiftMotorIsMoveOK { get; }
        private DigitalInput InnerRingLiftMotorIsORG { get; }

        private DigitalInput OuterRingRollYMotorIsOK { get; }
        private DigitalInput OuterRingRollYMotorIsORG { get; }

        private DigitalInput OuterRingLiftMotorIsOK { get; }
        private DigitalInput OuterRingLiftMotorIsMoveOK { get; }
        private DigitalInput OuterRingLiftMotorIsORG { get; }

        public HannDeng_Macro(DigitalOutput[] outputs, DigitalInput[] inputs)
        {
            //this.camera = camera;
            IsCanMoveAllHome = true;
            IsInnerCanMoveStartPos = true;
            IsOuterCanMoveStartPos = true;

            InnerRingVacuum = outputs[1];

            InnerRingPitchXServo = outputs[58];//58
            InnerRingPitchXOrg = outputs[59];//59
            InnerRingPitchXForward = outputs[54];//54
            InnerRingPitchXBackward = outputs[55];//55
            InnerRingPitchXStop = outputs[56];//56
            InnerRingPitchXAlarmReset = outputs[57];//57

            InnerRingRollYServo = outputs[52];//52
            InnerRingRollYOrg = outputs[53];//53
            InnerRingRollYForward = outputs[48];//48
            InnerRingRollYBackward = outputs[49];//49
            InnerRingRollYStop = outputs[50];//50
            InnerRingRollYAlarmReset = outputs[51];//51

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
            InnerRingIsVacuumOn = inputs[5];

            InnerRingPitchXIsORG = inputs[23];
            InnerRingRollYIsORG = inputs[21];

            InnerRingLiftMotorIsOK = inputs[27];
            InnerRingLiftMotorIsMoveOK = inputs[28];
            InnerRingLiftMotorIsORG = inputs[29];

            OuterRingRollYMotorIsOK = inputs[18];
            OuterRingRollYMotorIsORG = inputs[17];

            OuterRingLiftMotorIsOK = inputs[24];
            OuterRingLiftMotorIsMoveOK = inputs[25];
            OuterRingLiftMotorIsORG = inputs[26];





            //ObservableDetection();
        }
        public bool IsCanMoveAllHome { get => isCanMoveAllHome; set => SetValue(ref isCanMoveAllHome, value); }
        public bool IsInnerCanMoveStartPos { get => isInnerCanMoveStartPos; set => SetValue(ref isInnerCanMoveStartPos, value); }
        public bool IsInnerUsing { get => isInnerUsing; set => SetValue(ref isInnerUsing, value); }
        public bool IsOuterCanMoveStartPos { get => isOuterCanMoveStartPos; set => SetValue(ref isOuterCanMoveStartPos, value); }
        public bool IsOuterUsing { get => isOuterUsing; set => SetValue(ref isOuterUsing, value); }

        public async Task HomeAllRing()
        {
            try
            {
                bool isFirstHome = true;

                string returnStr = "";
                if (InnerRingLiftMotorIsOK.IsSignal != true)
                {
                    returnStr += " 內環抬升馬達異常";
                }
                if (OuterRingRollYMotorIsOK.IsSignal != true)
                {
                    returnStr += " 外環翻轉馬達異常";
                }
                if (OuterRingLiftMotorIsOK.IsSignal != true)
                {
                    returnStr += " 外環抬升馬達異常";
                }
                if (returnStr != "")
                {
                    throw new Exception("Macro復歸異常" + returnStr);
                }
                IsCanMoveAllHome = false;
                IsInnerCanMoveStartPos = false;
                IsOuterCanMoveStartPos = false;
                await HomeInnerRing(isFirstHome);
                await HomeOuterRing(isFirstHome);
                IsCanMoveAllHome = true;
                IsInnerCanMoveStartPos = true;
                IsOuterCanMoveStartPos = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void Initial()
        {
            throw new NotImplementedException();
        }

        public void FixWafer()
        {
            throw new NotImplementedException();
        }

        public void ReleaseWafer()
        {
            throw new NotImplementedException();
        }

        public async Task HomeInnerRing()
        {
            try
            {
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    await HomeInnerRing(false);
                }
                else
                {
                    throw new Exception("內環 不能下降!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Macro HomeInnerRing:" + ex);
            }
        }

        public async Task HomeOuterRing()
        {
            try
            {
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OuterInTop)
                {
                    await HomeOuterRing(false);
                }
                else
                {
                    throw new Exception("外環 不能復歸!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Macro HomeOuterRing:" + ex);
            }
        }

        public async Task GoInnerRingCheckPos()
        {
            try
            {
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.InnerInOrg || CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    await InnerRingLiftUp();
                }
                else
                {
                    throw new Exception("內環 不能上升!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Macro GoInnerRingCheckPos:" + ex);
            }
        }

        public async Task GoOuterRingCheckPos()
        {
            try
            {
                if (CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OuterInOrg || CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK)
                {
                    await OuterRingLiftUp();
                }
                else
                {
                    throw new Exception("外環 不能上升!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Macro GoOuterRingCheckPos:" + ex);
            }
        }
        //}
        /// <summary>
        /// 外環 橫軸/俯仰 Y 移動
        /// </summary>
        /// <param name="isForward"></param>
        public void OuterRingRollY_Move(bool isForward)
        {
            try
            {
                if (CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK)
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
                    throw new Exception("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Macro GoInnerRingCheckPos:" + ex);
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

                throw new Exception("Macro OuterRingRollY_Stop:" + ex);
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
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingPitchXBackward.Off();
                        InnerRingPitchXForward.On();
                    }
                    else
                    {
                        InnerRingPitchXForward.Off();
                        InnerRingPitchXBackward.On();
                    }

                }
                else
                {
                    throw new Exception("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Macro OuterRingRollY_Stop:" + ex);
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
                throw new Exception("Macro InnerRingPitchX_Stop:" + ex);
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        public void InnerRingRollY_Move(bool isForward)
        {
            try
            {
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingRollYBackward.Off();
                        InnerRingRollYForward.On();
                    }
                    else
                    {
                        InnerRingRollYForward.Off();
                        InnerRingRollYBackward.On();
                    }

                }
                else
                {
                    throw new Exception("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Macro InnerRingRollY_Move:" + ex);
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
                throw new Exception("Macro InnerRingRollY_Stop:" + ex);
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
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    if (isForward == true)
                    {
                        InnerRingYawTBackward.Off();
                        InnerRingYawTForward.On();
                    }
                    else
                    {
                        InnerRingYawTForward.Off();
                        InnerRingYawTBackward.On();
                    }

                }
                else
                {
                    throw new Exception("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Macro InnerRingYawT_Move:" + ex);
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
                throw new Exception("Macro InnerRingYawT_Stop:" + ex);
            }
        }

        /// <summary>
        /// 外環復歸 isFirstHome=true:一開機的復歸，外環要在下Sensor(若沒有，要請工程師確認)。isFirstHome=false:不再執行外環檢查的復歸，外環要在上、內環要在下
        /// </summary>
        /// <param name="isFirstHome"></param>
        /// <returns></returns>
        private async Task HomeOuterRing(bool isFirstHome)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;

                    //外環抬升軸上來
                    if (isFirstHome == true)
                    {
                        i = 0;
                        if (OuterRingLiftMotorIsORG.IsSignal != true || OuterRingLiftMotorIsMoveOK.IsSignal != true)
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
                        //上升第一段
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 馬達異常Time out");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 馬達異常Time out");
                        }
                        //上升第二段
                        OuterRingLiftMotorM0.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        await Task.Delay(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal == true)
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
                    if (isFirstHome == false)
                    {
                        IsOuterUsing = false;
                    }
                    //外環旋轉復歸
                    i = 0;
                    if (OuterRingLiftMotorIsORG.IsSignal == true || OuterRingLiftMotorIsMoveOK.IsSignal != true)
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
                    if (OuterRingLiftMotorIsORG.IsSignal == true || OuterRingLiftMotorIsMoveOK.IsSignal != true)
                    {
                        throw new Exception($"外環 不在上方，請工程師確認!!");
                    }
                    //外環下降第一段
                    OuterRingLiftMotorStart.Off();
                    OuterRingLiftMotorM0.Off();
                    OuterRingLiftMotorM1.Off();
                    await Task.Delay(25);
                    OuterRingLiftMotorM0.On();
                    await Task.Delay(25);
                    OuterRingLiftMotorStart.On();
                    await Task.Delay(300);
                    while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    if (isFirstHome != true)
                    {
                        InnerRingVacuum.On();
                        await Task.Delay(50);
                    }
                    //外環下降第二段
                    OuterRingLiftMotorStart.Off();
                    OuterRingLiftMotorM0.Off();
                    await Task.Delay(50);
                    while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.On();
                    await Task.Delay(150);
                    while (OuterRingLiftMotorIsMoveOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.Off();
                    await Task.Delay(50);

                    if (isFirstHome == false)
                    {
                        IsCanMoveAllHome = true;
                        IsInnerCanMoveStartPos = true;
                        IsOuterCanMoveStartPos = true;
                    }
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
        private async Task HomeInnerRing(bool isFirstHome)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (isFirstHome == false)
                    {
                        IsInnerUsing = false;
                    }
                    int i = 0;

                    //X、Y翻轉復歸
                    i = 0;
                    InnerRingPitchXOrg.Off();
                    InnerRingRollYOrg.Off();
                    await Task.Delay(50);
                    InnerRingPitchXOrg.On();
                    await Task.Delay(50);
                    InnerRingRollYOrg.On();
                    await Task.Delay(200);
                    while (InnerRingPitchXIsORG.IsSignal != true || InnerRingRollYIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"內環 X、Y翻轉復歸Time out");
                    }
                    InnerRingPitchXOrg.Off();
                    InnerRingRollYOrg.Off();
                    //內環下降復歸
                    i = 0;
                    await Task.Delay(25);
                    InnerRingLiftMotorStart.Off();
                    InnerRingLiftMotorM0.Off();
                    InnerRingLiftMotorM1.Off();
                    await Task.Delay(25);
                    InnerRingLiftMotorStart.On();
                    await Task.Delay(150);
                    while (InnerRingLiftMotorIsMoveOK.IsSignal != true || InnerRingLiftMotorIsORG.IsSignal != true)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"內環 下降復歸Time out");
                    }
                    InnerRingLiftMotorStart.Off();
                    //內環旋轉馬達歸零
                    i = 0;
                    await Task.Delay(25);
                    InnerRingYawTOrg.Off();
                    await Task.Delay(50);
                    InnerRingYawTOrg.On();

                    if (isFirstHome == false)
                    {
                        IsCanMoveAllHome = true;
                        IsInnerCanMoveStartPos = true;
                        IsOuterCanMoveStartPos = true;
                    }
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
        private async Task OuterRingLiftUp()
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;
                    if (CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OuterInOrg || CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK)
                    {
                        IsCanMoveAllHome = false;
                        IsInnerCanMoveStartPos = false;
                        IsOuterCanMoveStartPos = false;

                        //外環上升第一段
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(25);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        await Task.Delay(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(50);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        InnerRingVacuum.Off();
                        await Task.Delay(50);
                        //外環上升第二段
                        OuterRingLiftMotorM0.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorM1.On();
                        await Task.Delay(25);
                        OuterRingLiftMotorStart.On();
                        await Task.Delay(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        await Task.Delay(50);

                        IsOuterUsing = true;
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
        private async Task InnerRingLiftUp()
        {
            try
            {
                await Task.Run(async () =>
                {
                    int i = 0;
                    if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.InnerInOrg || CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                    {
                        IsCanMoveAllHome = false;
                        IsInnerCanMoveStartPos = false;
                        IsOuterCanMoveStartPos = false;

                        await Task.Delay(25);
                        InnerRingLiftMotorStart.Off();
                        InnerRingLiftMotorM0.Off();
                        InnerRingLiftMotorM1.Off();
                        await Task.Delay(25);
                        InnerRingLiftMotorM1.On();
                        await Task.Delay(25);
                        InnerRingLiftMotorStart.On();
                        await Task.Delay(150);
                        while (InnerRingLiftMotorIsMoveOK.IsSignal != true || InnerRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            await Task.Delay(50);
                            if (i >= 200) throw new Exception($"內環 上升異常");
                        }
                        InnerRingLiftMotorStart.Off();
                        InnerRingLiftMotorM1.Off();

                        IsInnerUsing = true;
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

        /// <summary>
        /// 是否可以外環動作
        /// </summary>
        /// <returns></returns>
        private CheckMacroCanMove CheckMacroCanMoveOuterRing()
        {
            //Outer In Top
            //Inner In Org
            try
            {
                if (OuterRingLiftMotorIsORG.IsSignal != false)
                {
                    return CheckMacroCanMove.OuterInOrg;
                }
                if (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                {
                    return CheckMacroCanMove.OuterMotorError;
                }
                if (InnerRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckMacroCanMove.InnerInTop;
                }
                if (InnerRingLiftMotorIsMoveOK.IsSignal != true)
                {
                    return CheckMacroCanMove.InnerMotorError;
                }
                return CheckMacroCanMove.OK;
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
        private CheckMacroCanMove CheckMacroCanMoveInnerRing()
        {
            //Outer In Org
            //Inner In Top
            try
            {
                if (OuterRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckMacroCanMove.OuterInTop;
                }
                if (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                {
                    return CheckMacroCanMove.OuterMotorError;
                }
                if (InnerRingLiftMotorIsORG.IsSignal != false)
                {
                    return CheckMacroCanMove.InnerInOrg;
                }
                if (InnerRingLiftMotorIsMoveOK.IsSignal != true)
                {
                    return CheckMacroCanMove.InnerMotorError;
                }
                return CheckMacroCanMove.OK;
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
        private CheckMacroCanMove CheckRingInOrg()
        {
            //Outer In Org
            //Inner In Top
            try
            {
                if (OuterRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckMacroCanMove.OuterInTop;
                }
                if (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                {
                    return CheckMacroCanMove.OuterMotorError;
                }
                if (InnerRingLiftMotorIsORG.IsSignal != true)
                {
                    return CheckMacroCanMove.InnerInTop;
                }
                if (InnerRingLiftMotorIsMoveOK.IsSignal != true)
                {
                    return CheckMacroCanMove.InnerMotorError;
                }
                return CheckMacroCanMove.OK;
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

        public enum CheckMacroCanMove
        {
            OK = 1,
            InnerInOrg = 2,
            InnerInTop = 3,
            OuterInOrg = 4,
            OuterInTop = 5,
            InnerMotorError = 6,
            OuterMotorError = 7
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
        }

        protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
        {
            // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
