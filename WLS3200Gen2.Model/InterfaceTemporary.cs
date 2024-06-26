﻿using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WLS3200Gen2.Model.Component;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public class LoadPortStatus
    {
        /// <summary>
        /// Error的狀態 正常、可復原的錯誤、不可復原的錯誤
        /// </summary>
        public string ErrorStatus { get; set; }
        /// <summary>
        /// LoadPort目前運作的位置
        /// </summary>
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Cassette放置是否正確
        /// </summary>
        public bool IsCassettePutOK { get; set; }
        /// <summary>
        /// Cassette是否夾住
        /// </summary>
        public bool IsClamp { get; set; }
        /// <summary>
        /// 轉開門機構，是否有轉開
        /// </summary>
        public bool IsSwitchDoor { get; set; }
        /// <summary>
        /// 吸附門真空，是否開啟
        /// </summary>
        public bool IsVaccum { get; set; }
        /// <summary>
        /// 門是否打開了
        /// </summary>
        public bool IsDoorOpen { get; set; }
        /// <summary>
        /// Sensor確認門是否打開
        /// </summary>
        public bool IsSensorCheckDoorOpen { get; set; }
        /// <summary>
        ///  移動Cassette前進後退的平台，是否前進到可以運作的位置
        /// </summary>
        public bool IsDock { get; set; }
    }
    public class LoadPortParam
    {
        /// <summary>
        /// Wafer厚薄度(um)
        /// </summary>
        public int WaferThickness { get; set; }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        public int CassettePitch { get; set; }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        public int StarOffset { get; set; }
        /// <summary>
        /// Wafer間距容忍值
        /// </summary>
        public int WaferPitchTolerance { get; set; }
        /// <summary>
        /// Wafer位置容忍值
        /// </summary>
        public int WaferPositionTolerance { get; set; }
    }
    public interface ILampControl
    {
        /// <summary>
        /// 目前光強度
        /// </summary>
        int LightValue { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 關閉
        /// </summary>
        void Close();
        /// <summary>
        /// 更換光亮度
        /// </summary>
        /// <param name="LigntValue"></param>
        /// <returns></returns>
        Task ChangeLightAsync(int LigntValue);
    }
    public interface ILoadPort
    {
        /// <summary>
        /// LoadPort是否有開門
        /// </summary>
        bool IsDoorOpen { get; }
        /// <summary>
        /// null:沒片子 true:有片子 false:片子異常 (陣列0是最上面那片)
        /// </summary>
        bool?[] Slot { get; }
        /// <summary>
        /// TimeOut重送次數
        /// </summary>
        int TimeOutRetryCount { get; set; }
        /// <summary>
        /// 取得狀態
        /// </summary>
        /// <returns></returns>
        Task<LoadPortStatus> GetStatus();
        /// <summary>
        /// 取得設定參數
        /// </summary>
        /// <returns></returns>
        Task<LoadPortParam> GetParam();
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 關閉
        /// </summary>
        void Close();
        /// <summary>
        /// 打開門，會自動SlotMapping
        /// </summary>
        Task Load();
        /// <summary>
        /// 回歸到關門狀態，也可以應用在UnLoad
        /// </summary>
        Task Home();
        /// <summary>
        /// 異常復原
        /// </summary>
        Task AlarmReset();
        /// <summary>
        /// 參數設定
        /// </summary>
        Task SetParam(LoadPortParam loadPortParam);
    }
    public class RobotPoint
    {
        public double X;
        public double Y;
        public double Z;
        public double W;
        public RobotPoint()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.W = 0;
        }
    }
    public class RobotStatus
    {
        /// <summary>
        /// 目前手臂的運作模式
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// 停止訊號
        /// </summary>
        public bool IsStopSignal { get; set; }
        /// <summary>
        /// 急停訊號
        /// </summary>
        public bool IsEStopSignal { get; set; }
        /// <summary>
        /// 指令訊號完成
        /// </summary>
        public bool IsCommandDoneSignal { get; set; }
        /// <summary>
        /// 移動完成訊號完成
        /// </summary>
        public bool IsMovDoneSignal { get; set; }
        /// <summary>
        /// 是否還在運作
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// 發生什麼樣的異常
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// X軸狀態 0:正常 1~3要看對應ErrorCode
        /// </summary>
        public int ErrorX { get; set; }
        /// <summary>
        /// Y軸狀態 0:正常 1~3要看對應ErrorCode
        /// </summary>
        public int ErrorY { get; set; }
        /// <summary>
        /// Z軸狀態 0:正常 1~3要看對應ErrorCode
        /// </summary>
        public int ErrorZ { get; set; }
        /// <summary>
        /// W軸狀態 0:正常 1~3要看對應ErrorCode
        /// </summary>
        public int ErrorW { get; set; }
        /// <summary>
        /// R軸狀態 0:正常 1~3要看對應ErrorCode
        /// </summary>
        public int ErrorR { get; set; }
        /// <summary>
        /// C軸狀態 0:正常 1~3要看對應ErrorCode
        /// </summary>
        public int ErrorC { get; set; }
    }
    public interface IRobot
    {
        /// <summary>
        /// 手臂是否正常開啟
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// 移動軸到位容許範圍
        /// </summary>
        double MoveTolerance { get; }
        /// <summary>
        /// Cassette裡的Wafer間距
        /// </summary>
        double CassetteWaferPitch { get; set; }
        /// <summary>
        /// 軸速度百分比(Hirata 4軸可以設定兩個速度:XYW同一個速度、Z單獨一個速度)
        /// </summary>
        /// <returns></returns>
        int SpeedPercent { get; }
        /// <summary>
        /// 手臂是否固定住Wafer
        /// </summary>
        /// <returns></returns>
        bool IsLockOK { get; }
        /// <summary>
        /// TimeOut重送次數
        /// </summary>
        int TimeOutRetryCount { get; set; }
        /// <summary>
        /// 手臂初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// Robot回Home
        /// </summary>
        Task Home();
        /// <summary>
        /// Robot停止動作
        /// </summary>
        Task Continue();
        /// <summary>
        /// Robot停止動作
        /// </summary>
        Task Stop();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="zShift"></param>
        /// <returns></returns>
        Task MovAddress(int address, double zShift);
        /// <summary>
        /// 取得Robot位置
        /// </summary>
        /// <returns></returns>
        Task<RobotPoint> GetPositionCommand();
        /// <summary>
        /// 取得目前Robot狀態
        /// </summary>
        /// <returns></returns>
        Task<RobotStatus> GetStatus();
        /// <summary>
        /// 設定軸速度(Hirata 4軸可以設定兩個速度:XYW同一個速度、Z單獨一個速度)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motionVelocity"></param>
        Task SetSpeedPercentCommand(int motionPercent);
        /// <summary>
        /// 手臂固定Wafer(真空/夾持)
        /// </summary>
        /// <returns></returns>
        Task FixWafer();
        /// <summary>
        /// 手臂解開Wafer(真空/夾持)
        /// </summary>
        /// <returns></returns>
        Task ReleaseWafer();
    }
    public interface IEFEMRobot : IRobot
    {

        PauseTokenSource pauseToken { get; set; }
        CancellationTokenSource cancelToken { get; set; }


        /// <summary>
        /// 取片從卡匣 手臂到安全位置
        /// </summary>
        /// <returns></returns>
        Task PickWafer_Safety(ArmStation armStation);
        /// <summary>
        /// 取片從卡匣 手臂到準備位置
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PickWafer_Standby(ArmStation armStation, int layer);
        /// <summary>
        /// 取片從卡匣 手臂伸進去卡匣裡
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PickWafer_GoIn(ArmStation armStation, int layer);
        /// <summary>
        /// 取片從卡匣 手臂抬起接片
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PickWafer_LiftUp(ArmStation armStation, int layer);
        /// <summary>
        /// 取片從卡匣 手臂縮回來
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PickWafer_Retract(ArmStation armStation, int layer);


        /// <summary>
        /// 取片從Aligner 手臂到準備位置
        /// </summary>
        /// <returns></returns>
        Task PickWafer_Standby(ArmStation armStation);
        /// <summary>
        /// 取片從Aligner 手臂伸進去Aligner裡
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PickWafer_GoIn(ArmStation armStation);
        /// <summary>
        /// 取片從Aligner 手臂抬起接片
        /// </summary>
        /// <returns></returns>
        Task PickWafer_LiftUp(ArmStation armStation);
        /// <summary>
        /// 取片從Aligner 手臂縮回來
        /// </summary>
        /// <returns></returns>
        Task PickWafer_Retract(ArmStation armStation);







        /// <summary>
        /// 放片至卡匣 手臂到安全位置
        /// </summary>
        /// <returns></returns>
        Task PutWafer_Safety(ArmStation armStation);
        /// <summary>
        /// 放片至卡匣 手臂到準備位置
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PutWafer_Standby(ArmStation armStation, int layer);
        /// <summary>
        /// 放片至卡匣 手臂伸進去卡匣裡
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PutWafer_GoIn(ArmStation armStation, int layer);
        /// <summary>
        /// 放片至卡匣 手臂放下片子
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PutWafer_PutDown(ArmStation armStation, int layer);
        /// <summary>
        /// 放片至卡匣 手臂縮回來
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Task PutWafer_Retract(ArmStation armStation, int layer);


        /// <summary>
        /// 放片至Aligner 手臂到準備位置
        /// </summary>
        /// <returns></returns>
        Task PutWafer_Standby(ArmStation armStation);
        /// <summary>
        /// 放片至Aligner 手臂伸進去Aligner裡
        /// </summary>
        /// <returns></returns>
        Task PutWafer_GoIn(ArmStation armStation);
        /// <summary>
        /// 放片至Aligner 手臂放下片子
        /// </summary>
        /// <returns></returns>
        Task PutWafer_PutDown(ArmStation armStation);
        /// <summary>
        /// 放片至Aligner 手臂縮回來
        /// </summary>
        /// <returns></returns>
        Task PutWafer_Retract(ArmStation armStation);

    }
    public enum ArmStation
    {
        Cassette1,
        Cassette2,
        Align,
        Macro,
        Micro
    }
    public class AlignerStatus
    {
        /// <summary>
        /// Aligner目前運作的狀態
        /// </summary>
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// AlignerNotch偵測狀態
        /// </summary>
        public string NotchStatus { get; set; }
        /// <summary>
        /// 是否有Wafer
        /// </summary>
        public bool IsWafer { get; set; }
        /// <summary>
        /// 是否在原點
        /// </summary>
        public bool IsOrg { get; set; }
        /// <summary>
        /// 是否真空建立
        /// </summary>
        public bool IsVaccum { get; set; }
    }
    public interface IAligner
    {
        /// <summary>
        /// 手臂是否固定住Wafer
        /// </summary>
        /// <returns></returns>
        bool IsLockOK { get; }
        /// <summary>
        /// TimeOut重送次數
        /// </summary>
        int TimeOutRetryCount { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 關閉
        /// </summary>
        void Close();
        /// <summary>
        /// 回到原點
        /// </summary>
        /// <returns></returns>
        Task Home();
        /// <summary>
        /// 找到Notch後，偏移多少角度(degree)
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        Task Run(double degree);
        /// <summary>
        /// 移動到WaferID idReaderDegree角度(但是兩個角度要先設定)
        /// </summary>
        /// <param name="idReaderDegree"></param>
        /// <param name="notchDegree"></param>
        /// <returns></returns>
        Task WaferIDRun1(double idReaderDegree, double notchDegree);
        /// <summary>
        /// 移動到WaferID第二個角度(要先WaferIDRun1移動過完成後再來下)
        /// </summary>
        /// <returns></returns>
        Task WaferIDRun2();
        /// <summary>
        /// 鎖住Aligner Wafer
        /// </summary>
        Task FixWafer();
        /// <summary>
        /// 解開Aligner Wafer
        /// </summary>
        Task ReleaseWafer();
        /// <summary>
        /// 異常重置
        /// </summary>
        /// <returns></returns>
        Task AlarmReset();
        /// <summary>
        /// 取得目前狀態
        /// </summary>
        /// <returns></returns>
        Task<AlignerStatus> GetStatus();
    }
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
    public interface IMacro
    {
        /// <summary>
        /// 是否可以復歸
        /// </summary>
        bool IsCanMoveAllHome { get; }
        /// <summary>
        /// 內環是否可以到起始位置
        /// </summary>
        bool IsInnerCanMoveStartPos { get; }
        /// <summary>
        /// 內環是否正在使用
        /// </summary>
        bool IsInnerUsing { get; }
        /// <summary>
        /// 外環是否可以到起始位置
        /// </summary>
        bool IsOuterCanMoveStartPos { get; }
        /// <summary>
        /// 外環是否正在使用
        /// </summary>
        bool IsOuterUsing { get; }
        /// <summary>
        /// Wafer是否鎖住
        /// </summary>
        bool IsLockOK { get; }
        /// <summary>
        /// 內環橫軸X
        /// </summary>
        double InnerPitchXPosition { get; }
        /// <summary>
        /// 內環縱軸Y
        /// </summary>
        double InnerRollYPosition { get; }
        /// <summary>
        /// 內環旋轉T
        /// </summary>
        double InnerYawTPosition { get; }
        /// <summary>
        /// 外環縱軸Y
        /// </summary>
        double OuterRollYPosition { get; }
        /// <summary>
        /// 內環橫軸正極限
        /// </summary>
        double InnerRingPitchXPositionPEL { get; set; }
        /// <summary>
        /// 內環橫軸負極限
        /// </summary>
        double InnerRingPitchXPositionNEL { get; set; }
        /// <summary>
        /// 內環旋轉軸正極限
        /// </summary>
        double InnerRingYawTPositionPEL { get; set; }
        /// <summary>
        /// 內環旋轉軸負極限
        /// </summary>
        double InnerRingYawTPositionNEL { get; set; }
        /// <summary>
        /// 內環翻轉正極限
        /// </summary>
        double InnerRingRollYPositionPEL { get; set; }
        /// <summary>
        /// 內環翻轉負極限
        /// </summary>
        double InnerRingRollYPositionNEL { get; set; }
        /// <summary>
        /// 外環翻轉正極限
        /// </summary>
        double OuterRingRollYPositionPEL { get; set; }
        /// <summary>
        /// 外環翻轉負極限
        /// </summary>
        double OuterRingRollYPositionNEL { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 鎖住Macro Wafer
        /// </summary>
        void FixWafer();
        /// <summary>
        /// 解開Macro Wafer
        /// </summary>
        void ReleaseWafer();

        /// <summary>
        /// 全部復歸
        /// </summary>
        /// <returns></returns>
        Task Home(bool isHaveWafer);
        /// <summary>
        /// 內圈復歸
        /// </summary>
        /// <returns></returns>
        Task HomeInnerRing();
        /// <summary>
        /// 外圈復歸
        /// </summary>
        /// <returns></returns>
        Task HomeOuterRing();
        /// <summary>
        /// 內圈 到 檢查位置
        /// </summary>
        /// <returns></returns>
        Task GoInnerRingCheckPos();
        /// <summary>
        /// 外圈 到 檢查位置
        /// </summary>
        /// <returns></returns>
        Task GoOuterRingCheckPos();
        /// <summary>
        /// 側面左右翻 isForwards = true(右翻)/false(左翻)
        /// </summary>
        /// <param name="isForward"></param>
        void InnerRingPitchX_Move(bool isForward);
        /// <summary>
        /// 金面左右翻絕對位置
        /// </summary>
        /// <param name="isForward"></param>
        Task InnerRingPitchXMoveToAsync(double pos);
        /// <summary>
        /// 正面前後倒 isForwards = true(前傾)/false(後仰)
        /// </summary>
        /// <param name="isForward"></param>
        void InnerRingRollY_Move(bool isForward);
        /// <summary>
        /// 金面前後倒絕對位置
        /// </summary>
        /// <param name="isForward"></param>
        Task InnerRingRollYMoveToAsync(double pos);
        /// <summary>
        /// 平面旋轉 isForwards = true(順時針)/false(逆時針)
        /// </summary>
        /// <param name="isForwards"></param>
        void InnerRingYawT_Move(bool isForwards);
        /// <summary>
        /// 金面旋轉絕對位置
        /// </summary>
        /// <param name="isForward"></param>
        Task InnerRingYawTMoveToAsync(double pos);


        void OuterRingRollY_Move(bool isForwards);
        /// <summary>
        /// 金背前後倒絕對位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Task OuterRingRollYMoveToAsync(double pos);

        void InnerRingPitchX_Stop();
        void InnerRingRollY_Stop();
        void InnerRingYawT_Stop();
        void OuterRingRollY_Stop();
        /// <summary>
        /// 重置YawT數值
        /// </summary>
        void InnerYawTReset();
    }


}
