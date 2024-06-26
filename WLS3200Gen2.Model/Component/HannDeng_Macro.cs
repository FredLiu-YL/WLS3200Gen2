﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Data;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Component
{
    public class HannDeng_Macro : IMacro
    {
        private readonly object lockObj = new object();
        private bool isCanMoveAllHome = false;

        private bool isInnerRingPitchXBusy = false;
        private bool isInnerRingPitchXStop = false;
        private bool isInnerRingRollYBusy = false;
        private bool isInnerRingRollYStop = false;
        private bool isInnerRingYawTBusy = false;
        private bool isInnerRingYawTStop = false;

        private bool isInnerCanMoveStartPos = false;
        private bool isInnerUsing = false;
        private bool isOuterCanMoveStartPos = false;
        private bool isOuterUsing = false;

        private bool isInnerPitchXForward = false;
        private bool isInnerPitchXBackward = false;

        private bool isInnerRollYForward = false;
        private bool isInnerRollYBackward = false;

        private bool isInnerYawTForward = false;
        private bool isInnerYawTBackward = false;

        private bool isOuterRingRollYBusy = false;
        private bool isOuterRingRollYStop = false;

        private bool isOuterRollYForward = false;
        private bool isOuterRollYBackward = false;


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
        /// 
        private DigitalOutput OuterRingVacuum { get; }
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

            OuterRingVacuum = outputs[2];

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
            InnerRingIsVacuumOn = inputs[6];

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

        public bool IsLockOK => InnerRingIsVacuumOn.IsSignal;

        public double InnerPitchXPosition { get; private set; }
        public double InnerRollYPosition { get; private set; }
        public double InnerYawTPosition { get; private set; }
        public double OuterRollYPosition { get; private set; }
        public double InnerRingPitchXPositionPEL { get; set; }
        public double InnerRingPitchXPositionNEL { get; set; }
        public double InnerRingYawTPositionPEL { get; set; }
        public double InnerRingYawTPositionNEL { get; set; }
        public double InnerRingRollYPositionPEL { get; set; }
        public double InnerRingRollYPositionNEL { get; set; }
        public double OuterRingRollYPositionPEL { get; set; }
        public double OuterRingRollYPositionNEL { get; set; }

        public Task Home(bool isMacroHaveWafer)
        {
            try
            {
                return Task.Run(async () =>
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
                    await HomeOuterRing(isFirstHome, isMacroHaveWafer);
                    IsCanMoveAllHome = true;
                    IsInnerCanMoveStartPos = true;
                    IsOuterCanMoveStartPos = true;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void Initial()
        {
            ReflashPosition();
        }
        private Task ReflashPosition()
        {
            try
            {
                return Task.Run(async () =>
                {
                    Stopwatch stopwatchReflash = new Stopwatch();
                    stopwatchReflash.Start();
                    while (true)
                    {
                        //await Task.Delay(1);
                        stopwatchReflash.Restart();
                        while (true)
                        {
                            if (stopwatchReflash.ElapsedMilliseconds >= 2)
                            {
                                break;
                            }
                        }
                        //內環位置++
                        if (isInnerPitchXForward)
                        {
                            InnerPitchXPosition += 2;
                        }
                        else if (isInnerPitchXBackward)
                        {
                            InnerPitchXPosition -= 2;
                        }

                        if (isInnerRollYForward)
                        {
                            InnerRollYPosition += 2;
                        }
                        else if (isInnerRollYBackward)
                        {
                            InnerRollYPosition -= 2;
                        }

                        if (isInnerYawTForward)
                        {
                            InnerYawTPosition += 2;
                        }
                        else if (isInnerYawTBackward)
                        {
                            InnerYawTPosition -= 2;
                        }

                        //外環位置++
                        if (isOuterRollYForward)
                        {
                            OuterRollYPosition += 2;
                        }
                        else if (isOuterRollYBackward)
                        {
                            OuterRollYPosition -= 2;
                        }
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void FixWafer()
        {
            try
            {
                InnerRingVacuum.On();
            }
            catch (Exception ex)
            {

                throw new Exception("Macro FixWafer:" + ex);
            }
        }

        public void ReleaseWafer()
        {
            try
            {
                InnerRingVacuum.Off();
            }
            catch (Exception ex)
            {

                throw new Exception("Macro ReleaseWafer:" + ex);
            }
        }

        public Task HomeInnerRing()
        {
            try
            {
                Task task = Task.CompletedTask;
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    task = HomeInnerRing(false);
                }
                else
                {
                    throw new FlowException("內環 不能下降!");
                }
                return task;
            }
            catch (Exception ex)
            {

                throw new Exception("Macro HomeInnerRing:" + ex);
            }
        }
        public void InnerYawTReset()
        {
            try
            {
                InnerYawTPosition = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Macro InnerYawTReset:" + ex);
            }
        }

        public Task HomeOuterRing()
        {
            try
            {
                Task task = Task.CompletedTask;
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OuterInTop)
                {
                    task = HomeOuterRing(false, true);
                }
                else
                {
                    throw new FlowException("外環 不能復歸!");
                }
                return task;
            }
            catch (Exception ex)
            {

                throw new Exception("Macro HomeOuterRing:" + ex);
            }
        }

        public Task GoInnerRingCheckPos()
        {
            try
            {
                Task task = Task.CompletedTask;
                if ((CheckMacroCanMoveInnerRing() == CheckMacroCanMove.InnerInOrg || CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK) && IsInnerCanMoveStartPos == true)
                {
                    task = InnerRingLiftUp();
                }
                else
                {
                    throw new FlowException("內環 不能上升!");
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Macro GoInnerRingCheckPos:" + ex);
            }
        }

        public Task GoOuterRingCheckPos()
        {
            try
            {
                Task task = Task.CompletedTask;
                if ((CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OuterInOrg || CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK) && IsOuterCanMoveStartPos == true)
                {
                    task = OuterRingLiftUp();
                }
                else
                {
                    throw new FlowException("外環 不能上升!");
                }
                return task;
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
                if (CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK && IsOuterUsing == true)
                {
                    if (isForward == true)
                    {
                        OuterRingRollYForward.On();
                        OuterRingRollYBackward.Off();
                        isOuterRollYForward = true;
                        isOuterRollYBackward = false;
                    }
                    else
                    {
                        OuterRingRollYForward.Off();
                        OuterRingRollYBackward.On();
                        isOuterRollYForward = false;
                        isOuterRollYBackward = true;
                    }

                }
                else
                {
                    throw new FlowException("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Macro GoInnerRingCheckPos:" + ex);
            }
        }
        public async Task OuterRingRollYMoveToAsync(double postion)
        {
            if (isOuterRingRollYBusy) throw new FlowException("InnerRingRollY is Busy");
            if (postion >= OuterRingRollYPositionPEL) postion = OuterRingRollYPositionPEL;
            if (postion <= OuterRingRollYPositionNEL) postion = OuterRingRollYPositionNEL;
            try
            {
                isOuterRingRollYBusy = true;
                await Task.Run(async () =>
                {
                    isOuterRingRollYStop = false;
                    await Task.Delay(300);
                    //System.Threading.Thread.Sleep(500);
                    double movePos = postion - OuterRollYPosition;
                    Stopwatch stopwatchRollY = new Stopwatch();
                    stopwatchRollY.Start();
                    if (Math.Abs(movePos) > 0)
                    {
                        if (movePos > 0)
                        {
                            OuterRingRollY_Move(true);
                        }
                        else
                        {
                            OuterRingRollY_Move(false);
                        }
                        stopwatchRollY.Restart();
                        while (true)
                        {
                            if (stopwatchRollY.ElapsedMilliseconds >= Math.Abs(movePos) || isOuterRingRollYStop)
                            {
                                OuterRingRollY_Stop();
                                break;
                            }
                        }
                        await Task.Delay(1);
                        stopwatchRollY.Stop();
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                isOuterRingRollYBusy = false;
                isOuterRingRollYStop = false;
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
                isOuterRingRollYStop = true;
                OuterRingRollYForward.Off();
                OuterRingRollYBackward.Off();
                isOuterRollYForward = false;
                isOuterRollYBackward = false;
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
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK && IsInnerUsing == true)
                {
                    if (isForward == true)
                    {
                        InnerRingPitchXBackward.Off();
                        InnerRingPitchXForward.On();
                        isInnerPitchXForward = true;
                        isInnerPitchXBackward = false;
                    }
                    else
                    {
                        InnerRingPitchXForward.Off();
                        InnerRingPitchXBackward.On();
                        isInnerPitchXForward = false;
                        isInnerPitchXBackward = true;
                    }

                }
                else
                {
                    throw new FlowException("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Macro OuterRingRollY_Stop:" + ex);
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 移動
        /// </summary>
        /// <param name="postion"></param>
        /// <returns></returns>
        public Task InnerRingPitchXMoveToAsync(double postion)
        {
            if (isInnerRingPitchXBusy) throw new FlowException("InnerRingRollY is Busy");
            if (postion >= InnerRingPitchXPositionPEL) postion = InnerRingPitchXPositionPEL;
            if (postion <= InnerRingPitchXPositionNEL) postion = InnerRingPitchXPositionNEL;
            try
            {
                isInnerRingPitchXBusy = true;
                return Task.Run(async () =>
               {
                   isInnerRingPitchXStop = false;
                   await Task.Delay(300);
                   //System.Threading.Thread.Sleep(500);
                   double movePos = postion - InnerPitchXPosition;
                   Stopwatch stopwatchPitchX = new Stopwatch();
                   stopwatchPitchX.Start();
                   if (Math.Abs(movePos) > 0)
                   {
                       if (movePos > 0)
                       {
                           InnerRingPitchX_Move(true);
                       }
                       else
                       {
                           InnerRingPitchX_Move(false);
                       }
                       stopwatchPitchX.Restart();
                       while (true)
                       {
                           if (movePos > 0)
                           {
                               if (postion - InnerPitchXPosition <= 1 || isInnerRingPitchXStop)//if (stopwatchPitchX.ElapsedMilliseconds >= Math.Abs(movePos) || isInnerRingPitchXStop)
                               {
                                   InnerRingPitchX_Stop();
                                   break;
                               }
                           }
                           else
                           {
                               if (postion - InnerPitchXPosition >= 1 || isInnerRingPitchXStop)//if (stopwatchPitchX.ElapsedMilliseconds >= Math.Abs(movePos) || isInnerRingPitchXStop)
                               {
                                   InnerRingPitchX_Stop();
                                   break;
                               }
                           }
                       }
                       await Task.Delay(1);
                       stopwatchPitchX.Stop();
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                isInnerRingPitchXBusy = false;
                isInnerRingPitchXStop = false;
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 停止
        /// </summary>
        public void InnerRingPitchX_Stop()
        {
            try
            {
                isInnerRingPitchXStop = true;
                isInnerPitchXForward = false;
                isInnerPitchXBackward = false;
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
                if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK && IsInnerUsing == true)
                {
                    if (isForward == true)
                    {
                        InnerRingRollYBackward.Off();
                        InnerRingRollYForward.On();
                        isInnerRollYForward = true;
                        isInnerRollYBackward = false;
                    }
                    else
                    {
                        InnerRingRollYForward.Off();
                        InnerRingRollYBackward.On();
                        isInnerRollYForward = false;
                        isInnerRollYBackward = true;
                    }

                }
                else
                {
                    throw new FlowException("內環 不能移動!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Macro InnerRingRollY_Move:" + ex);
            }
        }
        public Task InnerRingRollYMoveToAsync(double postion)
        {
            if (isInnerRingRollYBusy) throw new FlowException("InnerRingRollY is Busy");
            if (postion >= InnerRingRollYPositionPEL) postion = InnerRingRollYPositionPEL;
            if (postion <= InnerRingRollYPositionNEL) postion = InnerRingRollYPositionNEL;
            try
            {
                isInnerRingRollYBusy = true;
                return Task.Run(async () =>
               {
                   isInnerRingRollYStop = false;
                   await Task.Delay(300);
                   //System.Threading.Thread.Sleep(500);
                   double movePos = postion - InnerRollYPosition;
                   Stopwatch stopwatchRollY = new Stopwatch();
                   stopwatchRollY.Start();
                   if (Math.Abs(movePos) > 0)
                   {
                       if (movePos > 0)
                       {
                           InnerRingRollY_Move(true);
                       }
                       else
                       {
                           InnerRingRollY_Move(false);
                       }
                       stopwatchRollY.Restart();
                       while (true)
                       {
                           if (movePos > 0)
                           {
                               if (postion - InnerRollYPosition <= 1 || isInnerRingRollYStop)//if (stopwatchRollY.ElapsedMilliseconds >= Math.Abs(movePos) || isInnerRingRollYStop)
                               {
                                   InnerRingRollY_Stop();
                                   break;
                               }
                           }
                           else
                           {
                               if (postion - InnerRollYPosition >= 1 || isInnerRingRollYStop)//if (stopwatchRollY.ElapsedMilliseconds >= Math.Abs(movePos) || isInnerRingRollYStop)
                               {
                                   InnerRingRollY_Stop();
                                   break;
                               }
                           }

                       }
                       await Task.Delay(1);
                       InnerRollYPosition = postion;
                       stopwatchRollY.Stop();
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                isInnerRingRollYBusy = false;
                isInnerRingRollYStop = false;
            }
        }
        /// <summary>
        /// 內環 縱軸/翻滾 停止
        /// </summary>
        public void InnerRingRollY_Stop()
        {
            try
            {
                isInnerRingRollYStop = true;
                InnerRingRollYForward.Off();
                InnerRingRollYBackward.Off();
                isInnerRollYForward = false;
                isInnerRollYBackward = false;
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
                if (isForward == true)
                {
                    InnerRingYawTBackward.Off();
                    InnerRingYawTForward.On();
                    isInnerYawTForward = true;
                    isInnerYawTBackward = false;
                }
                else
                {
                    InnerRingYawTForward.Off();
                    InnerRingYawTBackward.On();
                    isInnerYawTForward = false;
                    isInnerYawTBackward = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Macro InnerRingYawT_Move:" + ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Task InnerRingYawTMoveToAsync(double pos)
        {
            if (isInnerRingYawTBusy) throw new FlowException("InnerRingYawT is Busy");
            if (pos >= InnerRingYawTPositionPEL) pos = InnerRingYawTPositionPEL;
            if (pos <= InnerRingYawTPositionNEL) pos = InnerRingYawTPositionNEL;
            try
            {
                isInnerRingYawTBusy = true;
                return Task.Run(async () =>
               {
                   isInnerRingYawTStop = false;
                   await Task.Delay(300);
                   //System.Threading.Thread.Sleep(500);
                   double movePos = pos - InnerYawTPosition;
                   Stopwatch stopwatchYawT = new Stopwatch();
                   stopwatchYawT.Start();
                   if (Math.Abs(movePos) > 0)
                   {
                       if (movePos > 0)
                       {
                           InnerRingYawT_Move(true);
                       }
                       else
                       {
                           InnerRingYawT_Move(false);
                       }
                       stopwatchYawT.Restart();
                       while (true)
                       {
                           if (movePos > 0)
                           {
                               if (pos - InnerYawTPosition <= 1 || isInnerRingYawTStop)//if (stopwatchYawT.ElapsedMilliseconds >= Math.Abs(movePos) || isInnerRingYawTStop)
                               {
                                   InnerRingYawT_Stop();
                                   break;
                               }
                           }
                           else
                           {
                               if (pos - InnerYawTPosition >= 1 || isInnerRingYawTStop)//if (stopwatchYawT.ElapsedMilliseconds >= Math.Abs(movePos) || isInnerRingYawTStop)
                               {
                                   InnerRingYawT_Stop();
                                   break;
                               }
                           }
                       }
                       await Task.Delay(1);
                       stopwatchYawT.Stop();
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                isInnerRingYawTBusy = false;
                isInnerRingYawTStop = false;
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
                isInnerRingYawTStop = true;
                InnerRingYawTForward.Off();
                InnerRingYawTBackward.Off();
                isInnerYawTForward = false;
                isInnerYawTBackward = false;
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
        private Task HomeOuterRing(bool isFirstHome, bool isHaveWafer)
        {
            try
            {
                return Task.Run(() =>
                {
                    int i = 0;

                    //外環抬升軸上來
                    System.Threading.Thread.Sleep(200);
                    OuterRingRollYForward.Off();
                    OuterRingRollYBackward.Off();
                    System.Threading.Thread.Sleep(300);
                    if (isFirstHome == true)
                    {
                        i = 0;
                        OuterRingLiftMotorStart.Off();
                        if (OuterRingLiftMotorIsORG.IsSignal != true)
                        {
                            throw new Exception($"外環 不在下方，請工程師確認!!");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        InnerRingVacuum.Off();
                        OuterRingVacuum.On();
                        System.Threading.Thread.Sleep(50);
                        OuterRingLiftMotorM1.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorStart.On();
                        System.Threading.Thread.Sleep(150);
                        //上升第一段
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"外環 馬達異常Time out");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM1.Off();
                        System.Threading.Thread.Sleep(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"外環 馬達異常Time out");
                        }
                        System.Threading.Thread.Sleep(50);
                        //上升第二段
                        OuterRingLiftMotorM0.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorM1.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorStart.On();
                        System.Threading.Thread.Sleep(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"外環 馬達抬升異常Time out");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        System.Threading.Thread.Sleep(25);
                    }
                    IsOuterUsing = false;
                    //外環旋轉復歸
                    i = 0;
                    if (OuterRingLiftMotorIsORG.IsSignal == true || OuterRingLiftMotorIsMoveOK.IsSignal != true)
                    {
                        throw new Exception($"外環 不在上方，請工程師確認!!");
                    }
                    OuterRingRollYOrg.Off();
                    System.Threading.Thread.Sleep(25);
                    OuterRingRollYOrg.On();
                    System.Threading.Thread.Sleep(150);
                    while (OuterRingRollYMotorIsOK.IsSignal != true || OuterRingRollYMotorIsORG.IsSignal != true)
                    {
                        i++;
                        System.Threading.Thread.Sleep(50);
                        if (i >= 400) throw new Exception($"外環 翻轉馬達復歸異常Time out");
                    }
                    OuterRingRollYOrg.Off();
                    System.Threading.Thread.Sleep(25);
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
                    System.Threading.Thread.Sleep(25);
                    OuterRingLiftMotorM0.On();
                    System.Threading.Thread.Sleep(25);
                    OuterRingLiftMotorStart.On();
                    System.Threading.Thread.Sleep(300);
                    while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                    {
                        i++;
                        System.Threading.Thread.Sleep(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    if (isFirstHome != true || isHaveWafer)
                    {
                        InnerRingVacuum.On();
                    }
                    OuterRingVacuum.Off();
                    System.Threading.Thread.Sleep(50);
                    //外環下降第二段
                    OuterRingLiftMotorStart.Off();
                    OuterRingLiftMotorM0.Off();
                    System.Threading.Thread.Sleep(50);
                    while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                    {
                        i++;
                        System.Threading.Thread.Sleep(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.On();
                    System.Threading.Thread.Sleep(150);
                    while (OuterRingLiftMotorIsMoveOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal != true)
                    {
                        i++;
                        System.Threading.Thread.Sleep(50);
                        if (i >= 200) throw new Exception($"外環 升降馬達復歸異常Time out");
                    }
                    OuterRingLiftMotorStart.Off();
                    System.Threading.Thread.Sleep(50);

                    if (isFirstHome == false)
                    {
                        IsCanMoveAllHome = true;
                        IsInnerCanMoveStartPos = true;
                        IsOuterCanMoveStartPos = true;
                    }
                    OuterRollYPosition = 0;
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
        private Task HomeInnerRing(bool isFirstHome)
        {
            try
            {
                return Task.Run(() =>
                {
                    IsInnerUsing = false;
                    int i = 0;
                    //全部關閉
                    System.Threading.Thread.Sleep(200);
                    InnerRingPitchXForward.Off();
                    InnerRingPitchXBackward.Off();
                    InnerRingRollYForward.Off();
                    InnerRingRollYBackward.Off();
                    InnerRingYawTForward.Off();
                    InnerRingYawTBackward.Off();
                    //X、Y翻轉復歸
                    i = 0;
                    System.Threading.Thread.Sleep(300);
                    InnerRingPitchXOrg.Off();
                    InnerRingRollYOrg.Off();
                    System.Threading.Thread.Sleep(50);
                    InnerRingPitchXOrg.On();
                    System.Threading.Thread.Sleep(50);
                    InnerRingRollYOrg.On();
                    System.Threading.Thread.Sleep(200);
                    while (InnerRingPitchXIsORG.IsSignal != true || InnerRingRollYIsORG.IsSignal != true)
                    {
                        i++;
                        System.Threading.Thread.Sleep(50);
                        if (i >= 200) throw new Exception($"內環 X、Y翻轉復歸Time out");
                    }
                    InnerRingPitchXOrg.Off();
                    InnerRingRollYOrg.Off();
                    //內環下降復歸
                    i = 0;
                    System.Threading.Thread.Sleep(25);
                    InnerRingLiftMotorStart.Off();
                    InnerRingLiftMotorM0.Off();
                    InnerRingLiftMotorM1.Off();
                    System.Threading.Thread.Sleep(25);
                    InnerRingLiftMotorStart.On();
                    System.Threading.Thread.Sleep(150);
                    while (InnerRingLiftMotorIsMoveOK.IsSignal != true || InnerRingLiftMotorIsORG.IsSignal != true)
                    {
                        i++;
                        System.Threading.Thread.Sleep(50);
                        if (i >= 200) throw new Exception($"內環 下降復歸Time out");
                    }
                    InnerRingLiftMotorStart.Off();
                    //內環旋轉馬達歸零
                    i = 0;
                    System.Threading.Thread.Sleep(25);
                    InnerRingYawTOrg.Off();
                    System.Threading.Thread.Sleep(50);
                    InnerRingYawTOrg.On();

                    if (isFirstHome == false)
                    {
                        IsCanMoveAllHome = true;
                        IsInnerCanMoveStartPos = true;
                        IsOuterCanMoveStartPos = true;
                    }
                    InnerPitchXPosition = 0;
                    InnerRollYPosition = 0;
                    InnerYawTPosition = 0;
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
        private Task OuterRingLiftUp()
        {
            try
            {
                return Task.Run(() =>
                {
                    int i = 0;
                    if (CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OuterInOrg)
                    {
                        IsCanMoveAllHome = false;
                        IsInnerCanMoveStartPos = false;
                        IsOuterCanMoveStartPos = false;

                        //外環上升第一段
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        InnerRingVacuum.Off();
                        OuterRingVacuum.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorM1.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorStart.On();
                        System.Threading.Thread.Sleep(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        System.Threading.Thread.Sleep(50);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        System.Threading.Thread.Sleep(300);
                        //外環上升第二段
                        OuterRingLiftMotorM0.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorM1.On();
                        System.Threading.Thread.Sleep(25);
                        OuterRingLiftMotorStart.On();
                        System.Threading.Thread.Sleep(300);
                        while (OuterRingLiftMotorIsMoveOK.IsSignal != true || OuterRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"外環 上升異常");
                        }
                        OuterRingLiftMotorStart.Off();
                        OuterRingLiftMotorM0.Off();
                        OuterRingLiftMotorM1.Off();
                        System.Threading.Thread.Sleep(50);

                        IsOuterUsing = true;
                    }
                    else if (CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK)
                    {
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
        private Task InnerRingLiftUp()
        {
            try
            {
                return Task.Run(() =>
                {
                    int i = 0;
                    if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.InnerInOrg)
                    {
                        IsCanMoveAllHome = false;
                        IsInnerCanMoveStartPos = false;
                        IsOuterCanMoveStartPos = false;

                        System.Threading.Thread.Sleep(25);
                        InnerRingLiftMotorStart.Off();
                        InnerRingLiftMotorM0.Off();
                        InnerRingLiftMotorM1.Off();
                        System.Threading.Thread.Sleep(25);
                        InnerRingLiftMotorM1.On();
                        System.Threading.Thread.Sleep(25);
                        InnerRingLiftMotorStart.On();
                        System.Threading.Thread.Sleep(150);
                        while (InnerRingLiftMotorIsMoveOK.IsSignal != true || InnerRingLiftMotorIsORG.IsSignal == true)
                        {
                            i++;
                            System.Threading.Thread.Sleep(50);
                            if (i >= 200) throw new Exception($"內環 上升異常");
                        }
                        InnerRingLiftMotorStart.Off();
                        InnerRingLiftMotorM1.Off();

                        IsInnerUsing = true;
                    }
                    else if (CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                    {
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
            /// <summary>
            /// 內環在下
            /// </summary>
            InnerInOrg = 2,
            /// <summary>
            /// 內環在上
            /// </summary>
            InnerInTop = 3,
            /// <summary>
            /// 外環在下
            /// </summary>
            OuterInOrg = 4,
            /// <summary>
            /// 外環在上
            /// </summary>
            OuterInTop = 5,
            /// <summary>
            /// 內環馬達異常
            /// </summary>
            InnerMotorError = 6,
            /// <summary>
            /// 外環馬達異常
            /// </summary>
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
