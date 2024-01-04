using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WLS3200Gen2.Model.Component.Adlink
{

    //ADLINK Structure++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_SAMP_DATA_4CH
    {
        public Int32 tick;
        public Int32 data0; //Total channel = 4
        public Int32 data1;
        public Int32 data2;
        public Int32 data3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOVE_PARA
    {
        public Int16 i16_accType;	//Axis parameter
        public Int16 i16_decType;	//Axis parameter
        public Int32 i32_acc;		//Axis parameter
        public Int32 i32_dec;		//Axis parameter
        public Int32 i32_initSpeed;	//Axis parameter
        public Int32 i32_maxSpeed;	//Axis parameter
        public Int32 i32_endSpeed;  //Axis parameter
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT_DATA
    {
        public Int32 i32_pos;       // Position data (relative or absolute) (pulse)
        public Int16 i16_accType;   // Acceleration pattern 0: T-curve,  1: S-curve
        public Int16 i16_decType;   // Deceleration pattern 0: T-curve,  1: S-curve
        public Int32 i32_acc;       // Acceleration rate ( pulse / ss )
        public Int32 i32_dec;       // Deceleration rate ( pulse / ss )
        public Int32 i32_initSpeed; // Start velocity	( pulse / s )
        public Int32 i32_maxSpeed;  // Maximum velocity  ( pulse / s )
        public Int32 i32_endSpeed;  // End velocity		( pulse / s )
        public Int32 i32_angle;     // Arc move angle    ( degree, -360 ~ 360 )
        public Int32 u32_dwell;     // Dwell times       ( unit: ms )
        public Int32 i32_opt;    	// Option //0xABCD , D:0 absolute, 1:relative
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA
    {
        // Point table structure (One dimension)
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.001 degree), [-360000,360000]
        public Int32 i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32 i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32 i32_ve;         // ending velocity (pulse/s), [0,2147484647]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_2D
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
        public Int32 i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32 i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32 i32_ve;         // ending velocity (pulse/s), [0,2147484647]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_2D_F64
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Double f64_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
        public Double f64_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Double f64_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Double f64_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Double f64_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Double f64_ve;         // ending velocity (pulse/s), [0,2147484647]
        public Double f64_sf;              // s-factor [0.0 ~ 1.0]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_4DL
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_z;          // z-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_u;          // u-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32 i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32 i32_ve;         // ending velocity (pulse/s), [0,2147484647]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT_DATA_EX
    {
        public Int32 i32_pos;           //(Center)Position data (could be relative or absolute value) 
        public Int16 i16_accType;       //Acceleration pattern 0: T curve, 1:S curve   
        public Int16 i16_decType;       // Deceleration pattern 0: T curve, 1:S curve 
        public Int32 i32_acc;           //Acceleration rate ( pulse / sec 2 ) 
        public Int32 i32_dec;           //Deceleration rate ( pulse / sec 2  ) 
        public Int32 i32_initSpeed;     //Start velocity ( pulse / s ) 
        public Int32 i32_maxSpeed;      //Maximum velocity    ( pulse / s ) 
        public Int32 i32_endSpeed;      //End velocity  ( pulse / s )     
        public Int32 i32_angle;         //Arc move angle ( degree, -360 ~ 360 ) 
        public UInt32 u32_dwell;        //dwell times ( unit: ms ) *Divided by system cycle time. 
        public Int32 i32_opt;           //Point move option. (*) 
        public Int32 i32_pitch;			// pitch for helical move
        public Int32 i32_totalheight;   // total hight
        public Int16 i16_cw;			// cw or ccw
        public Int16 i16_opt_ext;       // option extend
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINT_DATA2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int32[] i32_pos;                   // Position data (relative or absolute) (pulse)

        public Int32 i32_initSpeed;               // Start velocity	( pulse / s ) 
        public Int32 i32_maxSpeed;                // Maximum velocity  ( pulse / s ) 
        public Int32 i32_angle;                   // Arc move angle    ( degree, -360 ~ 360 ) 
        public UInt32 u32_dwell;                  // Dwell times       ( unit: ms ) 
        public Int32 i32_opt;                     // Option //0xABCD , D:0 absolute, 1:relative
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINT_DATA3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int32[] i32_pos;

        public Int32 i32_maxSpeed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Int32[] i32_endPos;

        public Int32 i32_dir;
        public Int32 i32_opt;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VAO_DATA
    {
        //Param
        public Int32 outputType;    //Output type, [0, 3]
        public Int32 inputType;     //Input type, [0, 1]
        public Int32 config;        //PWM configuration according to output type
        public Int32 inputSrc;      //Input source by axis, [0, 0xf]

        //Mapping table
        public Int32 minVel;                             //Minimum linear speed, [ positive ]
        public Int32 velInterval;                        //Speed interval, [ positive ]
        public Int32 totalPoints;                        //Total points, [1, 32]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] mappingDataArr;   //mapping data array
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTSTS
    {
        public UInt16 BitSts;           //b0: Is PTB work? [1:working, 0:Stopped]
                                        //b1: Is point buffer full? [1:full, 0:not full]
                                        //b2: Is point buffer empty? [1:empty, 0:not empty]
                                        //b3, b4, b5: Reserved for future
                                        //b6~: Be always 0
        public UInt16 PntBufFreeSpace;
        public UInt16 PntBufUsageSpace;
        public UInt32 RunningCnt;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LPSTS
    {
        public UInt32 MotionLoopLoading;
        public UInt32 HostLoopLoading;
        public UInt32 MotionLoopLoadingMax;
        public UInt32 HostLoopLoadingMax;
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct DEBUG_DATA
    {
        public UInt16 ServoOffCondition;
        public Double DspCmdPos;
        public Double DspFeedbackPos;
        public Double FpgaCmdPos;
        public Double FpgaFeedbackPos;
        public Double FpgaOutputVoltage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEBUG_STATE
    {
        public UInt16 AxisState;
        public UInt16 GroupState;
        public UInt16 AxisSuperState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTDWL
    {
        public Double DwTime; //Unit is ms
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTLINE
    {
        public Int32 Dim;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Double[] Pos;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTA2CA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] Index;       //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] Center;  //Center Arr

        public Double Angle;                          //Angle
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)]
    public struct PTA2CE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] Index; //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] Center; //

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] End; // 

        public Int16 Dir; //
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTA3CA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Normal; //Normal Arr

        public Double Angle;                         //Angle
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTA3CE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] End;    //End Arr

        public Int16 Dir; //
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTHCA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Normal; //Normal Arr

        public Double Angle;                         //Angle
        public Double DeltaH;
        public Double FinalR;
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTHCE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Normal; //Normal Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] End;    //End Arr

        public Int16 Dir; //
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PTINFO
    {
        public Int32 Dimension;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Int32[] AxisArr;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct STR_SAMP_DATA_8CH
    {
        public Int32 tick;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Int32[] data; //Total channel = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STR_SAMP_DATA_8CH_ASYNC
    {
        public Int32 tick;
        public Int32 data0;
        public Int32 data1;
        public Int32 data2;
        public Int32 data3;
        public Int32 data4;
        public Int32 data5;
        public Int32 data6;
        public Int32 data7;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAMP_PARAM
    {
        public Int32 rate;  //Sampling rate
        public Int32 edge;  //Trigger edge
        public Int32 level; //Trigger level
        public Int32 trigCh;    //Trigger channel

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int32[] sourceByCh;
        //Sampling source by channel. E.g.,
        // sourceByCh[0] --> Channel 0 sampling source number
        // sourceByCh[1] --> Chaneel 0 sampling axis number
        // sourceByCh[2] --> Channel 1 sampling source number
        // sourceByCh[3] --> Chaneel 1 sampling axis number
        // .....
        // sourceByCh[14] --> Channel 7 sampling source number
        // sourceByCh[15] --> Chaneel 7 sampling axis number 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct STR_SAMP_DATA_ADV
    {
        public Int32 tick;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int32[] data; //Total channel = 16
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STR_SAMP_DATA_ADV_ASYNC
    {
        public Int32 tick;
        public Int32 data0;
        public Int32 data1;
        public Int32 data2;
        public Int32 data3;
        public Int32 data4;
        public Int32 data5;
        public Int32 data6;
        public Int32 data7;
        public Int32 data8;
        public Int32 data9;
        public Int32 data10;
        public Int32 data11;
        public Int32 data12;
        public Int32 data13;
        public Int32 data14;
        public Int32 data15;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAMP_PARAM_ADV
    {
        public Int32 rate;  //Sampling rate
        public Int32 edge;  //Trigger edge
        public Int32 level; //Trigger level
        public Int32 trigCh;    //Trigger channel

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] sourceByCh;
        //Sampling source by channel. E.g.,
        // sourceByCh[0] --> Channel 0 sampling source number
        // sourceByCh[1] --> Chaneel 0 sampling axis number
        // sourceByCh[2] --> Channel 1 sampling source number
        // sourceByCh[3] --> Chaneel 1 sampling axis number
        // .....
        // sourceByCh[30] --> Channel 15 sampling source number
        // sourceByCh[31] --> Chaneel 15 sampling axis number 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOG_DATA
    {
        public Int16 i16_jogMode;	  // Jog mode. 0:Free running mode, 1:Step mode
        public Int16 i16_dir;		  // Jog direction. 0:positive, 1:negative direction
        public Int16 i16_accType;	  // Acceleration pattern 0: T-curve,  1: S-curve
        public Int32 i32_acc;		  // Acceleration rate ( pulse / ss )
        public Int32 i32_dec;		  // Deceleration rate ( pulse / ss )
        public Int32 i32_maxSpeed;	  // Positive value, maximum velocity  ( pulse / s )
        public Int32 i32_offset;	  // Positive value, a step (pulse)
        public Int32 i32_delayTime;  // Delay time, ( range: 0 ~ 65535 millisecond, align by cycle time)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HOME_PARA
    {
        public ushort u8_homeMode;
        public ushort u8_homeDir;
        public ushort u8_curveType;
        public Int32 i32_orgOffset;
        public Int32 i32_acceleration;
        public Int32 i32_startVelocity;
        public Int32 i32_maxVelocity;
        public Int32 i32_OrgVelocity;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct ASYNCALL
    {
        //public void* h_event;
        public Int32 i32_ret;
        public Byte u8_asyncMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TSK_INFO
    {
        public UInt16 State;        // 
        public UInt16 RunTimeErr;     // 
        public UInt16 IP;
        public UInt16 SP;
        public UInt16 BP;
        public UInt16 MsgQueueSts;
    }

    //New ADCNC structure define
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D_F64
    {
        /* This structure extends original point data contents from "I32" to "F64" 
										   for internal computation. It's important to prevent data overflow. */
        public UInt32 u32_opt;        // option, [0x00000000, 0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-9223372036854775808, 9223372036854775807]
        public Double f64_y;          // y-axis component (pulse), [-9223372036854775808, 9223372036854775807]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000, 360000]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D_RPS
    {
        /* This structure adds another variable to record what point was be saved */
        public UInt32 u32_opt;        // option, [0x00000000, 0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648, 2147483647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648, 2147483647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000, 360000]
        public UInt32 crpi;              // current reading point index
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D_F64_RPS
    {
        /* This structure adds another variable to record what point was be saved */
        public UInt32 u32_opt;        // option, [0x00000000, 0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-2147483648, 2147483647]
        public Double f64_y;          // y-axis component (pulse), [-2147483648, 2147483647]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000, 360000]
        public UInt32 crpi;               // current reading point index
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_2D_EXT
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Double f64_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_acc; // acceleration rate (pulse/ss), [0,2147484647]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_dec; // deceleration rate (pulse/ss), [0,2147484647]		

        public Int32 crossover;
        public Int32 Iboundary;     // initial boundary

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_vi; // initial velocity (pulse/s), [0,2147484647]

        public UInt32 vi_cmpr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_vm; // maximum velocity (pulse/s), [0,2147484647]

        public UInt32 vm_cmpr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_ve; // ending velocity (pulse/s), [0,2147484647]

        public UInt32 ve_cmpr;
        public Int32 Eboundary;     // end boundary		
        public Double f64_dist;     // point distance
        public Double f64_angle;        // path angle between previous & current point		
        public Double f64_radius;       // point radiua (used in arc move)
        public Int32 i32_arcstate;
        public UInt32 spt;          // speed profile type

        // unit time measured by DSP sampling period
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] t;

        // Horizontal & Vertical line flag
        public Int32 HorizontalFlag;
        public Int32 VerticalFlag;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DO_DATA_EX
    {
        public UInt32 Do_ValueL;        //bit[0~31]
        public UInt32 Do_ValueH;        //bit[32~63]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DI_DATA_EX
    {
        public UInt32 Di_ValueL;        //bit[0~31]
        public UInt32 Di_ValueH;        //bit[32~63]
    }

    //**********************************************
    // New header functions; 20151102
    //**********************************************
    [StructLayout(LayoutKind.Sequential)]
    public struct MCMP_POINT
    {
        public Double axisX; // x axis data for multi-dimension comparator 0
        public Double axisY; // y axis data for multi-dimension comparator 1
        public Double axisZ; // z axis data for multi-dimension comparator 2
        public Double axisU; // u axis data for multi-dimension comparator 3
        public UInt32 chInBit; // pwm output channel in bit format; 20150609
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    public struct EC_MODULE_INFO
    {
        public Int32 VendorID;
        public Int32 ProductCode;
        public Int32 RevisionNo;
        public Int32 TotalAxisNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Int32[] Axis_ID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Int32[] Axis_ID_manual;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] All_ModuleType;

        public Int32 DI_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] DI_ModuleType;

        public Int32 DO_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] DO_ModuleType;

        public Int32 AI_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] AI_ModuleType;

        public Int32 AO_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] AO_ModuleType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EC_Sub_MODULE_INFO
    {
        public Int32 VendorID;
        public Int32 ProductCode;
        public Int32 RevisionNo;
        public Int32 TotalSubModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] SubModuleID;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EC_Sub_MODULE_OD_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] DataName;

        public Int32 BitLength;
        public Int32 DataType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] DataTypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PDO_OFFSET
    {
        public UInt16 DataType;
        public UInt32 ByteSize;
        public UInt32 ByteOffset;
        public UInt32 Index;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] NameArr;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OD_DESC_ENTRY
    {
        public UInt32 DataTypeNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] DataTypeName;

        public UInt32 BitLen;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] Description;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] Access;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] PdoMapInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] UnitType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] DefaultValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] MinValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] MaxValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Speed_profile
    {
        public Int32 VS;        // start velocity ,range 1 ~ 4,000,000 (pulse)
        public Int32 Vmax;      // Maximum  velocity ,range 1 ~ 4,000,000
        public Int32 Acc;       // Acceleration ,range 1 ~ 500000000
        public Int32 Dec;       // Deceleration ,range 1 ~ 500000000
        public Double s_factor; // range 0 ~ 10

    }
    //	For latch function, 2019.06.10
    [StructLayout(LayoutKind.Sequential)]
    public struct LATCH_POINT
    {
        public Double position; 		// Latched position
        public Int32 ltcSrcInBit; 	// Latch source: bit 0~7: DI; bit 8~11: trigger channel
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct TCMP_EX_POINT
    {
        public Int32 outputPinInBit;    // Trigger output mapping 
        public Double position_f64;     //reserved
        public Int32 position_I32;
    }
    /// <summary>
    /// Adlink-PCI(e)7856 控制卡 Library
    /// </summary>
    public static class APS168Lib
    {
        // System & Initialization
        [DllImport("APS168x64.dll")] public static extern Int32 APS_initial(ref System.Int32 BoardID_InBits, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_close();
        [DllImport("APS168x64.dll")] public static extern Int32 APS_version();
        [DllImport("APS168x64.dll")] public static extern Int32 APS_device_driver_version(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_info(System.Int32 Axis_ID, ref System.Int32 Board_ID, ref System.Int32 Axis_No, ref System.Int32 Port_ID, ref System.Int32 Module_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_board_param(System.Int32 Board_ID, System.Int32 BOD_Param_No, System.Int32 BOD_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_board_param(System.Int32 Board_ID, System.Int32 BOD_Param_No, ref System.Int32 BOD_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_axis_param(System.Int32 Axis_ID, System.Int32 AXS_Param_No, System.Int32 AXS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_param(System.Int32 Axis_ID, System.Int32 AXS_Param_No, ref System.Int32 AXS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_device_info(System.Int32 Board_ID, System.Int32 Info_No, ref System.Int32 Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_card_name(System.Int32 Board_ID, ref System.Int32 CardName);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_disable_device(System.Int32 DeviceName);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_param_from_file(string pXMLFile);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_first_axisId(System.Int32 Board_ID, ref System.Int32 StartAxisID, ref System.Int32 TotalAxisNum);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_system_timer(System.Int32 Board_ID, ref System.Int32 Timer);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_system_loading(System.Int32 Board_ID, ref System.Double Loading1, ref System.Double Loading2, ref System.Double Loading3, ref System.Double Loading4);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_security_key(System.Int32 Board_ID, System.Int32 OldPassword, System.Int32 NewPassword);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_check_security_key(System.Int32 Board_ID, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_security_key(System.Int32 Board_ID);

        //Control driver mode [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_curr_sys_ctrl_mode(System.Int32 Axis_ID, ref System.Int32 Mode);

        //Virtual board settings [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_register_virtual_board(System.Int32 VirCardIndex, System.Int32 Count);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_virtual_board_info(System.Int32 VirCardIndex, ref System.Int32 Count);

        //Parameters setting by float [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_axis_param_f(System.Int32 Axis_ID, System.Int32 AXS_Param_No, System.Double AXS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_param_f(System.Int32 Axis_ID, System.Int32 AXS_Param_No, ref System.Double AXS_Param);

        //[For PCI-7856, MNET series]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_param_to_file(System.Int32 Board_ID, string pXMLFile);

        //Motion queue status [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mq_free_space(System.Int32 Axis_ID, ref System.Int32 Space);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mq_usage(System.Int32 Axis_ID, ref System.Int32 Usage);

        //Motion stop code [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_stop_code(System.Int32 Axis_ID, ref System.Int32 Code);

        //Helical interpolation [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_helix_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Int32 Pitch, System.Int32 TotalHeight, System.Int32 CwOrCcw);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_helix_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_PosOffset_Array, System.Int32 Max_Arc_Speed, System.Int32 Pitch, System.Int32 TotalHeight, System.Int32 CwOrCcw);

        //Helical interpolation [For PCI(e)-8154/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_helical_move(System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32[] End_Pos_Array, System.Int32 Pitch, System.Int32 Dir, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_helical_move(System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32[] End_Offset_Array, System.Int32 Pitch, System.Int32 Dir, System.Int32 Max_Speed);

        //Circular interpolation( Support 2D and 3D ) [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move_3pe(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Pass_Pos_Array, System.Int32[] End_Pos_Array, System.Int32 Max_Arc_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_arc_move_3pe(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Pass_PosOffset_Array, System.Int32[] End_PosOffset_Array, System.Int32 Max_Arc_Speed);

        //Field bus motion interrupt [For PCI-7856, MNET series]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_int_factor_motion(System.Int32 Axis_ID, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_int_factor_motion(System.Int32 Axis_ID, System.Int32 Factor_No, ref System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_int_factor_error(System.Int32 Axis_ID, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_int_factor_error(System.Int32 Axis_ID, System.Int32 Factor_No, ref System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_int_motion(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_field_bus_error_int_motion(System.Int32 Axis_ID, System.Int32 Time_Out);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_int_factor_di(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 bitsOfCheck);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_int_factor_di(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 bitsOfCheck);

        //Flash functions [For PCI-8253/56, PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_parameter_to_flash(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_parameter_from_flash(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_parameter_from_default(System.Int32 Board_ID);

        //SSCNET-3 functions [For PCI-8392(H)] 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_sscnet(System.Int32 Board_ID, ref System.Int32 AxisFound_InBits);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_sscnet(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_param(System.Int32 Axis_ID, System.Int32 Para_No1, ref System.Int32 Para_Dat1, System.Int32 Para_No2, ref System.Int32 Para_Dat2);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_servo_param(System.Int32 Axis_ID, System.Int32 Para_No1, System.Int32 Para_Dat1, System.Int32 Para_No2, System.Int32 Para_Dat2);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_alarm(System.Int32 Axis_ID, ref System.Int32 Alarm_No, ref System.Int32 Alarm_Detail);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_sscnet_servo_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_sscnet_servo_param(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_abs_position(System.Int32 Axis_ID, ref System.Int32 Cyc_Cnt, ref System.Int32 Res_Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_sscnet_servo_abs_position(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_sscnet_servo_abs_position(System.Int32 Axis_ID, System.Int32 Abs_Option, ref System.Int32 Cyc_Cnt, ref System.Int32 Res_Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_link_status(System.Int32 Board_ID, ref System.Int32 Link_Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_servo_monitor_src(System.Int32 Axis_ID, System.Int32 Mon_No, System.Int32 Mon_Src);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_monitor_src(System.Int32 Axis_ID, System.Int32 Mon_No, ref System.Int32 Mon_Src);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_monitor_data(System.Int32 Axis_ID, System.Int32 Arr_Size, System.Int32[] Data_Arr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_control_mode(System.Int32 Axis_ID, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_abs_enable(System.Int32 Board_ID, System.Int32 Option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_abs_enable_by_axis(System.Int32 Axis_ID, System.Int32 Option);

        //Motion IO & motion status functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_status(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_io_status(System.Int32 Axis_ID);

        //Monitor functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command(System.Int32 Axis_ID, ref System.Int32 Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_command(System.Int32 Axis_ID, System.Int32 Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_servo_on(System.Int32 Axis_ID, System.Int32 Servo_On);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_position(System.Int32 Axis_ID, ref System.Int32 Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_position(System.Int32 Axis_ID, System.Int32 Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_velocity(System.Int32 Axis_ID, ref System.Int32 Velocity);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feedback_velocity(System.Int32 Axis_ID, ref System.Int32 Velocity);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_error_position(System.Int32 Axis_ID, ref System.Int32 Err_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_target_position(System.Int32 Axis_ID, ref System.Int32 Targ_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_f(System.Int32 Axis_ID, ref System.Double Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_command_f(System.Int32 Axis_ID, System.Double Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_position_f(System.Int32 Axis_ID, ref System.Double Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_position_f(System.Int32 Axis_ID, System.Double Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_velocity_f(System.Int32 Axis_ID, ref System.Double Velocity);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_target_position_f(System.Int32 Axis_ID, ref System.Double Targ_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_error_position_f(System.Int32 Axis_ID, ref System.Double Err_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feedback_velocity_f(System.Int32 Axis_ID, ref System.Double Velocity);

        // Single axis motion
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_velocity_move(System.Int32 Axis_ID, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_home_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_emg_stop(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move2(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 End_Speed, System.Int32 Acc_Rate, System.Int32 Dec_Rate);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move2(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 End_Speed, System.Int32 Acc_Rate, System.Int32 Dec_Rate);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_home_move2(System.Int32 Axis_ID, System.Int32 Dir, System.Int32 Acc, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 ORG_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_home_escape(System.Int32 Axis_ID);

        //JOG functions [For PCI-8392(H), PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_jog_param(System.Int32 Axis_ID, ref JOG_DATA pStr_Jog, System.Int32 Mask);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_jog_param(System.Int32 Axis_ID, ref JOG_DATA pStr_Jog);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_jog_mode_switch(System.Int32 Axis_ID, System.Int32 Turn_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_jog_start(System.Int32 Axis_ID, System.Int32 STA_On);

        // Interpolation
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_linear_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Int32 Angle);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_arc_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32 Max_Arc_Speed, System.Int32 Angle);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move_f(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Double Angle);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_arc_move_f(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32 Max_Arc_Speed, System.Double Angle);

        // Interrupt functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_int_enable(System.Int32 Board_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_int_factor(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_int_factor(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, ref System.Int32 Enable);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_int_factorH(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_int_no_to_handle(System.Int32 Int_No);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_single_int(System.Int32 Int_No, System.Int32 Time_Out);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_multiple_int(System.Int32 Int_Count, System.Int32[] Int_No_Array, System.Int32 Wait_All, System.Int32 Time_Out);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_int(System.Int32 Int_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_int(System.Int32 Int_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_error_int(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Time_Out);


        //Sampling functions [For PCI-8392(H), PCI-8253/56, PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sampling_param(System.Int32 Board_ID, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_param(System.Int32 Board_ID, System.Int32 ParaNum, ref System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_4CH DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_async(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_4CH DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_count(System.Int32 Board_ID, ref System.Int32 SampCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_wait_sampling(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_auto_sampling(System.Int32 Board_ID, System.Int32 StartStop);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_data(System.Int32 Board_ID, ref System.Int32 Length, [Out] STR_SAMP_DATA_4CH[] DataArr, ref System.Int32 Status);

        //Sampling functions extension [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sampling_param_ex(System.Int32 Board_ID, ref SAMP_PARAM Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_param_ex(System.Int32 Board_ID, ref SAMP_PARAM Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_ex(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, [Out] STR_SAMP_DATA_8CH[] DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_async_ex(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_8CH_ASYNC DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_data_ex(System.Int32 Board_ID, ref System.Int32 Length, [Out] STR_SAMP_DATA_8CH[] DataArr, ref System.Int32 Status);

        // Sampling functions advanced ( For PCIe-833x )
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sampling_param_advanced(System.Int32 Board_ID, ref SAMP_PARAM_ADV Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_param_advanced(System.Int32 Board_ID, ref SAMP_PARAM_ADV Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_advanced(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, [Out] STR_SAMP_DATA_ADV[] DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_async_advanced(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_ADV_ASYNC DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_data_advanced(System.Int32 Board_ID, ref System.Int32 Length, [Out] STR_SAMP_DATA_ADV[] DataArr, ref System.Int32 Status);


        //DIO & AIO functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_d_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_output(System.Int32 Board_ID, System.Int32 DO_Group, ref System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_input(System.Int32 Board_ID, System.Int32 DI_Group, ref System.Int32 DI_Data);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_d_channel_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 Ch_No, System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_channel_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 Ch_No, ref System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_channel_input(System.Int32 Board_ID, System.Int32 DI_Group, System.Int32 Ch_No, ref System.Int32 DI_Data);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_a_input_value(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Double Convert_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_a_input_data(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Int32 Raw_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_a_output_value(System.Int32 Board_ID, System.Int32 Channel_No, System.Double Convert_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_a_output_data(System.Int32 Board_ID, System.Int32 Channel_No, System.Int32 Raw_Data);
        //AIO [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_a_output_value(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Double Convert_Data);

        //Point table move functions [For PCI-8253/56, PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_point_table(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_running_point_index(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_start_point_index(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_end_point_index(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_pause(System.Int32 Axis_ID, System.Int32 Pause_en);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_repeat(System.Int32 Axis_ID, System.Int32 Repeat_en);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_table_move_repeat_count(System.Int32 Axis_ID, ref System.Int32 RepeatCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 StartIndex, System.Int32 EndIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_tableEx(System.Int32 Axis_ID, System.Int32 Index, ref PNT_DATA Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_tableEx_2D(System.Int32 Axis_ID, System.Int32 Axis_ID_2, System.Int32 Index, ref PNT_DATA_2D Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_4DL(System.Int32[] Axis_ID_Array, System.Int32 Index, ref PNT_DATA_4DL Point);

        //Point table + IO - Pause / Resume [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_ex_pause(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_ex_rollback(System.Int32 Axis_ID, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_ex_resume(System.Int32 Axis_ID);

        //Point table with extend option [For PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_ex(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA_EX Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_point_table_ex(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA_EX Point);

        //Point table Feeder [For PCI-8253/56, PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_group(System.Int32 GroupId, System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_group(System.Int32 GroupId, ref System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_free_feeder_group(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_feeder_buffer(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_point_2D(System.Int32 GroupId, ref PNT_DATA_2D PtArray, System.Int32 Size, System.Int32 LastFlag);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_point_2D_ex(System.Int32 GroupId, ref PNT_DATA_2D_F64 PtArray, System.Int32 Size, System.Int32 LastFlag);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_feeder_move(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_status(System.Int32 GroupId, ref System.Int32 State, ref System.Int32 ErrCode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_running_index(System.Int32 GroupId, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_feed_index(System.Int32 GroupId, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_ex_pause(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_ex_rollback(System.Int32 GroupId, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_ex_resume(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_cfg_acc_type(System.Int32 GroupId, System.Int32 Type);

        //Point table functions [For MNET-4XMO-C]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_mode2(System.Int32 Axis_ID, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table2(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Index, ref POINT_DATA2 Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_continuous_move2(System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_single_move2(System.Int32 Axis_ID, System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_running_point_index2(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_status2(System.Int32 Axis_ID, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_setting_continuous_move2(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 TotalPoints, ref POINT_DATA2 Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table2_maximum_speed_check(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Index, ref POINT_DATA2 Point);

        //Point table functions [For HSL-4XMO]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table3(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Index, ref POINT_DATA3 Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_move3(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 StartIndex, System.Int32 EndIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_param3(System.Int32 FirstAxid, System.Int32 ParaNum, System.Int32 ParaDat);

        //Digital filter functions [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_filter_param(System.Int32 Axis_ID, System.Int32 Filter_paramNo, System.Int32 param_val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_filter_param(System.Int32 Axis_ID, System.Int32 Filter_paramNo, ref System.Int32 param_val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_device_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Info_No, ref System.Int32 Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_first_axisno(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 AxisNo, ref System.Int32 TotalAxes);

        //Field bus DIO slave functions [For PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_channel_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_channel_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_channel_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Int32 DI_Value);

        //Field bus AIO slave function
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_a_output_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value, System.Int16 RunStep);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_a_input_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value, System.Int16 RunStep);

        //Field bus comparing trigger functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_linear(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, System.Int32 StartPoint, System.Int32 RepeatTimes, System.Int32 Interval);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_table(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, System.Int32[] DataArr, System.Int32 ArraySize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_manual(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_manual_s(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgChInBit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_table_cmp(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_linear_cmp(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh, ref System.Int32 TrgCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_trigger_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_linear_cmp_remain_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_table_cmp_remain_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_encoder(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 EncCh, ref System.Int32 EncCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_encoder(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 EncCh, System.Int32 EncCnt);
        // Only support [For PCIe-8338 + EtherCAT 4xMO]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_timer_counter(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TmrCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_timer_counter(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TmrCh, System.Int32 Cnt);

        //Field bus latch functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_field_bus_ltc_fifo(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_point(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 ArraySize, ref LATCH_POINT LatchPoint);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_ltc_fifo_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_ltc_fifo(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_usage(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 Usage);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_free_space(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 FreeSpace);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_status(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 Status);



        // Comparing trigger functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_trigger_count(System.Int32 Board_ID, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_trigger_fifo_cmp(System.Int32 Board_ID, System.Int32 FCmpCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_fifo_cmp(System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_fifo_status(System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 FifoSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_fifo_data(System.Int32 Board_ID, System.Int32 FCmpCh, System.Int32[] DataArr, System.Int32 ArraySize, System.Int32 ShiftFlag);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_encoder_counter(System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 TrgCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_encoder_counter(System.Int32 Board_ID, System.Int32 TrgCh, ref System.Int32 TrgCnt);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_timer(System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 Start);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_timer_counter(System.Int32 Board_ID, System.Int32 TmrCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_timer_counter(System.Int32 Board_ID, System.Int32 TmrCh, System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_trigger_timer(System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 Start);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_timer_counter(System.Int32 Board_ID, System.Int32 TmrCh, ref System.Int32 TmrCnt);


        //VAO functions [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_param(System.Int32 Board_ID, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_vao_param(System.Int32 Board_ID, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_table(System.Int32 Board_ID, System.Int32 Table_No, System.Int32 MinVelocity, System.Int32 VelInterval, System.Int32 TotalPoints, System.Int32[] MappingDataArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_switch_vao_table(System.Int32 Board_ID, System.Int32 Table_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_vao(System.Int32 Board_ID, System.Int32 Output_Ch, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_vao_status(System.Int32 Board_ID, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_check_vao_param(System.Int32 Board_ID, System.Int32 Table_No, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_param_ex(System.Int32 Board_ID, System.Int32 Table_No, ref VAO_DATA VaoData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_vao_param_ex(System.Int32 Board_ID, System.Int32 Table_No, ref VAO_DATA VaoData);

        //Simultaneous move
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_relative_simultaneous_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32[] Max_Speed_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_absolute_simultaneous_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32[] Max_Speed_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_velocity_simultaneous_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Max_Speed_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_Release_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_release_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_emg_stop_simultaneous_move(System.Int32 Axis_ID);

        //Override functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_speed_override(System.Int32 Axis_ID, System.Int32 MaxSpeed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_ovrd(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_ovrd(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed);

        //Point table functions [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_dwell(System.Int32 Board_ID, System.Int32 PtbId, ref PTDWL Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_line(System.Int32 Board_ID, System.Int32 PtbId, ref PTLINE Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc2_ca(System.Int32 Board_ID, System.Int32 PtbId, ref PTA2CA Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc2_ce(System.Int32 Board_ID, System.Int32 PtbId, ref PTA2CE Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc3_ca(System.Int32 Board_ID, System.Int32 PtbId, ref PTA3CA Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc3_ce(System.Int32 Board_ID, System.Int32 PtbId, ref PTA3CE Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_spiral_ca(System.Int32 Board_ID, System.Int32 PtbId, ref PTHCA Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_spiral_ce(System.Int32 Board_ID, System.Int32 PtbId, ref PTHCE Prof, ref PTSTS Status);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_enable(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 Dimension, System.Int32[] AxisArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_disable(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pt_info(System.Int32 Board_ID, System.Int32 PtbId, ref PTINFO Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_vs(System.Int32 Board_ID, System.Int32 PtbId, System.Double Vs);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_get_vs(System.Int32 Board_ID, System.Int32 PtbId, ref System.Double Vs);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_start(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_stop(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pt_status(System.Int32 Board_ID, System.Int32 PtbId, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_pt_buffer(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_roll_back(System.Int32 Board_ID, System.Int32 PtbId, System.Double Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_get_error(System.Int32 Board_ID, System.Int32 PtbId, ref System.Int32 ErrCode);

        //Cmd buffer setting
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_ext_set_do_ch(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 Channel, System.Int32 OnOff);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_ext_set_table_no(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 CtrlNo, System.Int32 TableNo);

        //Profile buffer setting
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_absolute(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_relative(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_buffered(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_inp(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_blend_dec(System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_blend_dist(System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_blend_pcnt(System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_acc(System.Int32 Board_ID, System.Int32 PtbId, System.Double Acc);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_dec(System.Int32 Board_ID, System.Int32 PtbId, System.Double Dec);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_acc_dec(System.Int32 Board_ID, System.Int32 PtbId, System.Double AccDec);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_s(System.Int32 Board_ID, System.Int32 PtbId, System.Double Sf);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_vm(System.Int32 Board_ID, System.Int32 PtbId, System.Double Vm);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_ve(System.Int32 Board_ID, System.Int32 PtbId, System.Double Ve);

        //Program download functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_vmc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_vmc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_amc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_amc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_task_mode(System.Int32 Board_ID, System.Int32 TaskNum, System.Byte Mode, System.UInt16 LastIP);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_task_mode(System.Int32 Board_ID, System.Int32 TaskNum, ref System.Byte Mode, ref System.UInt16 LastIP);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_task(System.Int32 Board_ID, System.Int32 TaskNum, System.Int32 CtrlCmd);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_task_info(System.Int32 Board_ID, System.Int32 TaskNum, ref TSK_INFO Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_task_msg(System.Int32 Board_ID, System.UInt16 QueueSts, ref System.UInt16 ActualSize, System.Byte[] CharArr);

        //Latch functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_encoder(System.Int32 Axis_ID, ref System.Int32 Encoder);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_latch_counter(System.Int32 Axis_ID, System.Int32 Src, ref System.Int32 Counter);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_latch_event(System.Int32 Axis_ID, System.Int32 Src, ref System.Int32 Event);

        //Raw command counter [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_counter(System.Int32 Axis_ID, ref System.Int32 Counter);

        //Reset raw command counter [For PCIe-8338]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_command_counter(System.Int32 Axis_ID);

        //Watch dog timer 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_start(System.Int32 Board_ID, System.Int32 TimerNo, System.Int32 TimeOut);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_get_timeout_period(System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 TimeOut);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_reset_counter(System.Int32 Board_ID, System.Int32 TimerNo);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_get_counter(System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 Counter);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_set_action_event(System.Int32 Board_ID, System.Int32 TimerNo, System.Int32 EventByBit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_get_action_event(System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 EventByBit);

        //Multi-axes simultaneuos move start/stop [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_move_trigger(System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_move_multi(System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_emg_stop_multi(System.Int32 Dimension, System.Int32[] Axis_ID_Array);

        //Gear/Gantry functions [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_gear(System.Int32 Axis_ID, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gear_status(System.Int32 Axis_ID, ref System.Int32 Status);

        //Multi-latch functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_ltc_counter(System.Int32 Board_ID, System.Int32 CntNum, System.Int32 CntValue);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_counter(System.Int32 Board_ID, System.Int32 CntNum, ref System.Int32 CntValue);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_ltc_fifo_param(System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_param(System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_latch(System.Int32 Board_ID, System.Int32 LatchSignalInBits);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_ltc_fifo(System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_ltc_fifo(System.Int32 Board_ID, System.Int32 FLtcCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_data(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_usage(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Usage);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_free_space(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 FreeSpace);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_status(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_point(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 ArraySize, [In, Out] LATCH_POINT[] LatchPoint);

        //Single latch functions 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_latch2(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_latch_data2(System.Int32 Axis_ID, System.Int32 LatchNum, ref System.Int32 LatchData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_backlash_en(System.Int32 Axis_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_backlash_en(System.Int32 Axis_ID, ref System.Int32 Enable);

        //ODM functions for Mechatrolink
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_mlink(System.Int32 Board_ID, ref System.Int32 AxisFound_InBits);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_mlink(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_mlink_servo_param(System.Int32 Axis_ID, System.Int32 Para_No, System.Int32 Para_Dat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_servo_param(System.Int32 Axis_ID, System.Int32 Para_No, ref System.Int32 Para_Dat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_config_mlink(System.Int32 Board_ID, System.Int32 TotalAxes, ref System.Int32 AxesArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_rv_ptr(System.Int32 Axis_ID, out IntPtr rptr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_sd_ptr(System.Int32 Axis_ID, out IntPtr sptr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_servo_alarm(System.Int32 Axis_ID, System.Int32 Alarm_No, ref System.Int32 Alarm_Detail);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_mlink_servo_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_mlink_pulse_per_rev(System.Int32 Axis_ID, System.Int32 PPR);

        //Apply smooth servo off [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_smooth_servo_off(System.Int32 Axis_ID, System.Double Decay_Rate);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_smooth_servo_off(System.Int32 Board_ID, System.Int32 Axis_ID, System.Int32 cnt_Max, ref System.Int32 cnt_Err);

        //ODM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_wait(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_wait(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_linear_move_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_non_wait(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_non_wait(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_linear_move_non_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move_non_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_move_done(System.Int32 Axis_ID, ref System.Int32 MotionSts);

        //ODM functions [For MNET-4XMO-C]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move_ex(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32[] End_Pos_Array, System.Int32 CwOrCcw, System.Int32 Max_Arc_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_status_ex(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_io_status_ex(System.Int32 Axis_ID);

        //Gantry functions [For PCI-8392(H), PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_gantry_param(System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_param(System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 ParaNum, ref System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_gantry_axis(System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 Master_Axis_ID, System.Int32 Slave_Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_axis(System.Int32 Board_ID, System.Int32 GroupNum, ref System.Int32 Master_Axis_ID, ref System.Int32 Slave_Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_error(System.Int32 Board_ID, System.Int32 GroupNum, ref System.Int32 GentryError);

        //Field bus master functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 BUS_Param_No, System.Int32 BUS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 BUS_Param_No, ref System.Int32 BUS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_field_bus(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 Start_Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_scan_field_bus(System.Int32 Board_ID, System.Int32 BUS_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_field_bus(System.Int32 Board_ID, System.Int32 BUS_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_master_status(System.Int32 Board_ID, System.Int32 BUS_No, ref System.UInt32 Status);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_last_scan_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32[] Info_Array, System.Int32 Array_Size, ref System.Int32 Info_Count);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_master_type(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 BUS_Type);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_type(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 MOD_Type);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_name(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 MOD_Name);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_serialID(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int16 Serial_ID);

        // Diagnostic functions [Only for PCIe-833x]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_frame_loss_diagnostic(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Result);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_frame_loss_diagnostic(System.Int32 Board_ID, System.Int32 BUS_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_connecting_diagnostic(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Result, ref System.UInt16 NumOfDisconnect, ref IntPtr DisconnectIDArray);

        //Field bus slave functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_slave_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, ref System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_slave_connect_quality(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 Sts_data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_slave_online_status(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 Live);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_slave_recovery(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No);


        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ESC_register(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 RegOffset, System.Int32 DataSize, ref System.Int32 DataValue);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_ESC_register(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 RegOffset, System.Int32 DataSize, ref System.Int32 DataValue);


        //Field bus DIO slave functions [For PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DI_Value);

        //Modules be 64 bits gpio
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_output_ex(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, DO_DATA_EX DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_output_ex(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref DO_DATA_EX DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_input_ex(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref DI_DATA_EX DI_Value);

        //Field bus AIO slave functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_a_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_a_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_a_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value);

        //ODM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_vao_by_mode(System.Int32 Board_ID, System.Int32 ChannelInBit, System.Int32 Mode, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_pwm_burst_count(System.Int32 Board_ID, System.Int32 Table_No, System.Int32 Count);

        //PWM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pwm_width(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 Width);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pwm_width(System.Int32 Board_ID, System.Int32 PWM_Ch, ref System.Int32 Width);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pwm_frequency(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 Frequency);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pwm_frequency(System.Int32 Board_ID, System.Int32 PWM_Ch, ref System.Int32 Frequency);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pwm_on(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 PWM_On);

        // Comparing trigger functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_param(System.Int32 Board_ID, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_param(System.Int32 Board_ID, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_linear(System.Int32 Board_ID, System.Int32 LCmpCh, System.Int32 StartPoint, System.Int32 RepeatTimes, System.Int32 Interval);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_table(System.Int32 Board_ID, System.Int32 TCmpCh, System.Int32[] DataArr, System.Int32 ArraySize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_manual(System.Int32 Board_ID, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_manual_s(System.Int32 Board_ID, System.Int32 TrgChInBit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_table_cmp(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_linear_cmp(System.Int32 Board_ID, System.Int32 LCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_count(System.Int32 Board_ID, System.Int32 TrgCh, ref System.Int32 TrgCnt);

        //Pulser counter functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pulser_counter(System.Int32 Board_ID, ref System.Int32 Counter);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pulser_counter(System.Int32 Board_ID, System.Int32 Counter);

        //Reserved functions [Legacy functions]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_slave_set_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_slave_get_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, ref System.Int32 ParaDat);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_d_set_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_d_get_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_d_get_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DI_Value);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_set_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_get_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_get_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_set_output_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value, System.Int16 RunStep);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_get_input_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value, System.Int16 RunStep);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_eep_curr_drv_ctrl_mode(System.Int32 Board_ID, ref System.Int32 ModeInBit);

        //DPAC functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_rescan_CF(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_battery_status(System.Int32 Board_ID, ref System.Int32 Battery_status);

        //DPAC display & Display button
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_display_data(System.Int32 Board_ID, System.Int32 displayDigit, ref System.Int32 displayIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_display_data(System.Int32 Board_ID, System.Int32 displayDigit, System.Int32 displayIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_button_status(System.Int32 Board_ID, ref System.Int32 buttonstatus);

        //NV RAM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_nv_ram(System.Int32 Board_ID, System.Int32 RamNo, System.Int32 DataWidth, System.Int32 Offset, System.Int32 Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_nv_ram(System.Int32 Board_ID, System.Int32 RamNo, System.Int32 DataWidth, System.Int32 Offset, ref System.Int32 Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_clear_nv_ram(System.Int32 Board_ID, System.Int32 RamNo);

        //Advanced single move & interpolation [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_ptp(System.Int32 Axis_ID, System.Int32 Option, System.Double Position, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_ptp_v(System.Int32 Axis_ID, System.Int32 Option, System.Double Position, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_ptp_all(System.Int32 Axis_ID, System.Int32 Option, System.Double Position, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_vel(System.Int32 Axis_ID, System.Int32 Option, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_vel_all(System.Int32 Axis_ID, System.Int32 Option, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_line(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_line_v(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_line_all(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);

        //Ring counter functions [For PCI-8154/8]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_ring_counter(System.Int32 Axis_ID, System.Int32 RingVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ring_counter(System.Int32 Axis_ID, ref System.Int32 RingVal);



        //**********************************************
        // New header functions; 20151102
        //**********************************************

        //Pitch error compensation [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pitch_table(System.Int32 Axis_ID, System.Int32 Comp_Type, System.Int32 Total_Points, System.Int32 MinPosition, System.UInt32 Interval, System.Int32[] Comp_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pitch_table(System.Int32 Axis_ID, ref System.Int32 Comp_Type, ref System.Int32 Total_Points, ref System.Int32 MinPosition, ref System.UInt32 Interval, System.Int32[] Comp_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_pitch_comp(System.Int32 Axis_ID, System.Int32 Enable);

        //2D compensation [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_2d_compensation_table(System.Int32[] AxisIdArray, System.UInt32 CompType, System.UInt32[] TotalPointArray, System.Double[] StartPosArray, System.Double[] IntervalArray, System.Double[] CompDataArrayX, System.Double[] CompDataArrayY);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_2d_compensation_table(System.Int32[] AxisIdArray, ref System.UInt32 CompType, System.UInt32[] TotalPointArray, System.Double[] StartPosArray, System.Double[] IntervalArray, System.Double[] CompDataArrayX, System.Double[] CompDataArrayY);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_2d_compensation(System.Int32 Axis_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move_2d_compensation(System.Int32[] Axis_ID_Array, System.Double[] Position_Array, System.Double Max_Linear_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_2d_compensation_command_position(System.Int32 Axis_ID, ref System.Double CommandX, ref System.Double CommandY, ref System.Double PositionX, ref System.Double PositionY);

        //20200120
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_table_data(System.Int32 Board_ID, System.Int32 TCmpCh, System.Int32[] DataArr, Int32 ArraySize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_table_status(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 FreeSpace, ref System.Int32 FifoSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_cmp_value(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_trigger_table(System.Int32 Board_ID, System.Int32 TCmpCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_trigger_table(System.Int32 Board_ID, System.Int32 TCmpCh);



        //Multi-dimension comparator functions [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_multi_trigger_table(System.Int32 Board_ID, System.Int32 Dimension, MCMP_POINT[] DataArr, System.Int32 ArraySize, System.Int32 Window);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_multi_trigger_table_cmp(System.Int32 Board_ID, System.Int32 Dimension, ref MCMP_POINT CmpVal);

        //Pulser functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_start(System.Int32 Board_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_velocity_move(System.Int32 Axis_ID, System.Double SpeedLimit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_relative_move(System.Int32 Axis_ID, System.Double Distance, System.Double SpeedLimit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_home_move(System.Int32 Axis_ID);

        // [Wei-Li suggests to remove]
        //**********************************************
        // 2D arc-interpolation for 3-point
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ct_all(System.Int32[] Axis_ID_Array, System.Int32 APS_Option, System.Double[] AnyArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        //**********************************************

        // [Reserved for unknown usage]
        //**********************************************
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_watch_timer(System.Int32 Board_ID, ref System.Int32 Timer);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_wdt(System.Int32 Board_ID, System.Int32 WDT_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_mapto_AxisID(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 AxisID);
        //**********************************************

        //for 8338 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_module_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, [In, Out] EC_MODULE_INFO[] Module_info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_alarm(System.Int32 Axis_ID, ref System.UInt32 AlarmCode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_pdo_offset(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, out IntPtr PPTx, ref System.UInt32 NumOfTx, out IntPtr PPRx, ref System.UInt32 NumOfRx);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_pdo(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt16 ByteOffset, System.UInt16 Size, ref System.UInt32 Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_pdo(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt16 ByteOffset, System.UInt16 Size, System.UInt32 Value);
        [DllImport("APS168x64.dll")]
        public static extern Int32 APS_get_field_bus_sdo(
                                                         System.Int32 Board_ID,
                                                         System.Int32 BUS_No,
                                                         System.Int32 MOD_No,
                                                         System.UInt16 ODIndex,
                                                         System.UInt16 ODSubIndex,
                                                         System.Byte[] Data,
                                                         System.UInt32 DataLen,
                                                         ref System.UInt32 OutDatalen,
                                                         System.UInt32 Timeout,
                                                         System.UInt32 Flags
                                                        );

        [DllImport("APS168x64.dll")]
        public static extern Int32 APS_set_field_bus_sdo(
                                                         System.Int32 Board_ID,
                                                         System.Int32 BUS_No,
                                                         System.Int32 MOD_No,
                                                         System.UInt16 ODIndex,
                                                         System.UInt16 ODSubIndex,
                                                         System.Byte[] Data,
                                                         System.UInt32 DataLen,
                                                         System.UInt32 Timeout,
                                                         System.UInt32 Flags
                                                        );

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_num(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.UInt16 Num, out IntPtr ODList);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_desc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.UInt16 ODIndex, ref System.UInt16 MaxNumSubIndex, System.Byte[] Description, System.UInt32 Size);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_desc_entry(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.UInt16 ODIndex, System.UInt16 ODSubIndex, [In, Out] OD_DESC_ENTRY[] pOD_DESC_ENTRY);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_actual_torque(System.Int32 Axis_ID, ref System.Int32 Torque);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_port_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Port_No, System.UInt32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_port_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Port_No, ref System.UInt32 DI_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_port_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Port_No, ref System.UInt32 DO_Value);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_circular_limit(System.Int32 Axis_A, System.Int32 Axis_B, System.Double Center_A, System.Double Center_B, System.Double Radius, System.Int32 Stop_Mode, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_circular_limit(System.Int32 Axis_A, System.Int32 Axis_B, ref System.Double Center_A, ref System.Double Center_B, ref System.Double Radius, ref System.Int32 Stop_Mode, ref System.Int32 Enable);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_loss_package(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Loss_Count);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_od_data(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 ODIndex, System.UInt32 RawData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_data(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 ODIndex, ref System.UInt32 RawData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_module_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, [In, Out] EC_Sub_MODULE_INFO[] Sub_Module_info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_number(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, ref System.Int32 TxODNum, ref System.Int32 RxODNum);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_tx(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 TxODIndex, [In, Out] EC_Sub_MODULE_OD_INFO[] Sub_MODULE_OD_INFO);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_rx(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 RxODIndex, [In, Out] EC_Sub_MODULE_OD_INFO[] Sub_MODULE_OD_INFO);

        // PVT function;
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_add_point(System.Int32 Axis_ID, System.Int32 ArraySize, System.Double[] PositionArray, System.Double[] VelocityArray, System.Double[] TimeArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_get_status(System.Int32 Axis_ID, ref System.Int32 FreeSize, ref System.Int32 PointCount, ref System.Int32 State);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_start(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_reset(System.Int32 Axis_ID);

        // PT functions;
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_add_point(System.Int32 Axis_ID, System.Int32 ArraySize, System.Double[] PositionArray, System.Double[] TimeArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_get_status(System.Int32 Axis_ID, ref System.Int32 FreeSize, ref System.Int32 PointCount, ref System.Int32 State);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_start(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_reset(System.Int32 Axis_ID);

        //Get speed profile calculation
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_profile(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed, ref System.Int32 StrVel, ref System.Int32 MaxVel, ref System.Double Tacc, ref System.Double Tdec, ref System.Double Tconst);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_profile(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed, ref System.Int32 StrVel, ref System.Int32 MaxVel, ref System.Double Tacc, ref System.Double Tdec, ref System.Double Tconst);

        //ASYNC mode
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_error_code(System.Int32 Axis_ID, System.UInt32 Index, ref System.Int32 ErrorCode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_cmd_fifo_usage(System.Int32 Axis_ID, ref System.UInt32 Number);

        //Get fpga latch value [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_latch_data(System.Int32 Axis_ID, System.Int32 latch_channel, ref System.Int32 latch_data);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_register_emx(System.Int32 emx_online, System.Int32 option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_deviceIP(System.Int32 Board_ID, ref string option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_emx_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_check_motion_profile_emx(System.Int32 Axis_ID, ref Speed_profile profile_input, ref Speed_profile profile_output, ref System.Int32 MinDis);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_module_map(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt32[] MOD_No_Arr, System.UInt32 Size);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_module_map(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt32[] MOD_No_Arr, System.UInt32 Size);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_analysis_topology(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Error_Slave_No, [In, Out] EC_MODULE_INFO[] Current_slave_info, ref System.Int32 Current_slave_num, [In, Out] EC_MODULE_INFO[] Past_slave_info, ref System.Int32 Past_slave_num);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_number(System.Int32 Axis_ID, ref System.Int32 SlaveAxisIDSize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_info(System.Int32 Axis_ID, System.Int32 SlaveAxisIDSize, System.Int32[] SlaveAxisIDArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_deviation(System.Int32 Axis_ID, System.Int32 SlaveAxisIDSize, System.Int32[] SlaveAxisIDArray, System.Double[] DeviationArray);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_state(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 State);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_slave_state(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 State);
        // Coordinate transform 20190624
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_coordTransform2D_config(System.Int32 Board_ID, System.Int32 AxisID_X, System.Int32 AxisID_Y, System.Double XYAngle, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_coordTransform2D_config(System.Int32 Board_ID, ref System.Int32 AxisID_X, ref System.Int32 AxisID_Y, ref System.Double XYAngle, ref System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_coordTransform2D_position(System.Int32 Board_ID, ref System.Double Cmd_transform_X, ref System.Int32 Cmd_transform_Y, ref System.Double Fbk_transform_X, ref System.Double Fbk_transform_Y);

        // Torque control
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_torque_command(System.Int32 Axis_ID, ref System.Int32 TorqueCmd);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_command_control_mode(System.Int32 Axis_ID, System.Byte Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_control_mode(System.Int32 Axis_ID, ref System.Byte Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_torque_move(System.Int32 Axis_ID, System.Int16 TorqueValue, System.UInt32 Slope, System.UInt16 Option, ref ASYNCALL Wait);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void callback_func();
        [DllImport("APS168x64.dll")] public static extern Int32 APS_register_int_callback(System.Int32 ISR_No, System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, callback_func SuccessHandler, callback_func FailHandler, Int32 Action);

        // For asynchronous function
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_status_async(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_io_status_async(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_f_async(System.Int32 Axis_ID, ref System.Double Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_position_f_async(System.Int32 Axis_ID, ref System.Double Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_last_error(System.Int32 Axis_ID, ref System.Int32 ErrorCode);

        // for AMP-304C
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_table_data_ex(System.Int32 Board_ID, System.Int32 TCmpCh, TCMP_EX_POINT[] TcmpDataArr, System.Int32 ArraySize, System.Int32 Option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_table_remain_count(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_linear_remain_count(System.Int32 Board_ID, System.Int32 LCmpCh, ref System.Int32 Cnt);
    }
    public enum APS_Define
    {

        // Initial option
        INIT_AUTO_CARD_ID = (0x00),   // (Bit 0) CardId assigned by system, Input parameter of APS_initial( cardId, "MODE" )
        INIT_MANUAL_ID = (0x1),    //CardId manual by dip switch, Input parameter of APS_initial=( cardId, "MODE" )
        INIT_PARALLEL_FIXED = (0x02),   // (Bit 1) Fixed axis indexing mode in Parallel type
        INIT_SERIES_FIXED = (0x04),   // (Bit 2) Fixed axis indexing mode in Series type
        INIT_NOT_RESET_DO = (0x08),   // (Bit 3) HSL Digital output not reset, (DO status will follow the slave status.)
        INIT_PARAM_IGNORE = (0x00),   // (Bit 4-5) Load parameter method - ignore, keep current value
        INIT_PARAM_LOAD_DEFAULT = (0x10),   // (Bit 4-5) Load parameter method - load parameter as default value 
        INIT_PARAM_LOAD_FLASH = (0x20),   // (Bit 4-5) Load parameter method - load parameter from flash memory
        INIT_MNET_INTERRUPT = (0x40),   // (Bit 6) Enable MNET interrupt mode. (Support motion interrupt for MotionNet series)

        // Board parameter define =(General),
        PRB_EMG_LOGIC = (0x0),  // Board EMG logic
        PRB_WDT0_LIMIT = (0x10),  // Set / Get watch dog limit.
        PRB_WDT0_COUNTER = (0x11),  // Reset Wdt / Get Wdt_Count_Value
        PRB_WDT0_UNIT = (0x12),  // wdt_unit
        PRB_WDT0_ACTION = (0x13),  // wdt_action   
        PRB_DO_LOGIC = (0x14),  //DO logic, 0: no invert; 1: invert
        PRB_DI_LOGIC = (0x15),  //DI logic, 0: no invert; 1: invert 
        MHS_GET_SERVO_OFF_INFO = (0x16), //
        MHS_RESET_SERVO_OFF_INFO = (0x0017),
        MHS_GET_ALL_STATE = (0x0018),
        PRB_TMR0_BASE = (0x20),  // Set TMR Value
        PRB_TMR0_VALUE = (0x21),  // Get timer System.Int32 count value
        PRB_SYS_TMP_MONITOR = (0x30),  // Get system temperature monitor data
        PRB_CPU_TMP_MONITOR = (0x31),  // Get CPU temperature monitor data
        PRB_AUX_TMP_MONITOR = (0x32),  // Get AUX temperature monitor data
        PRB_UART_MULTIPLIER = (0x40),  // Set UART Multiplier
        PRB_PSR_MODE = (0x90),  // Config pulser mode
        PRB_PSR_EA_LOGIC = (0x91),  // Set EA inverted
        PRB_PSR_EB_LOGIC = (0x92),  // Set EB inverted
        PRB_EMG_MODE = (0x101),  // Set EMG condition mode
        PRB_ECAT_MODE = (0x102),  // Set EtherCAT master operation mode
        PRB_ECAT_RESTORE_OUTPUT = (0x104),  // Keeps status setting for EtherCAT DIO/AIO slave devices.
        PRB_DI_EMG_FILTER_ENABLE = (0x105), //Low-pass filter switch setting for on-board DI and EMG signal
        PRB_DI_EMG_FILTER_RANGE = (0x106), //Low-pass filter bandwidth setting for on-board DI and EMG signal
        PRB_PULSER_FILTER_RANGE = (0x107), //Low-pass filter bandwidth setting for on-board pulser signal. (The filter is always enable)
        PRB_PULSER_FILTER_ENABLE = (0x108),
        PRB_ECAT_AUTO_RECOVERY = (0x109),
        PRB_IO_ACCESS_SEL = (0x16),
        PRB_ECAT_SYNC_MODE = (0x17),  // 0: DC(default); 1: FreeRun;
        PRB_ECAT_OP_RETRY_COUNT = (0x18),
        PRB_ECAT_SERVO_ON_MODE = (0x19),    // 0 : Standard mode (Default ) , 1 : Fast mode (no check status word)
        PRB_ECAT_SERVO_ON_NO_RESET_ALARM = (0x1A), // 0 : Disable that remove reset alarm while servo on( Default ), 1 : Enable that remove reset alarm while servo on
        PRB_ECAT_SYNC_OFFSET = (0x20),



        // Board parameter define =(For PCI-8253/56),
        PRB_DENOMINATOR = (0x80),  // Floating number denominator
                                   //   PRB_PSR_MODE   =(0x90),  // Config pulser mode
        PRB_PSR_ENABLE = (0x91),  // Enable/disable pulser mode
        PRB_BOOT_SETTING = (0x100), // Load motion parameter method when DSP boot     
        PRB_PWM0_MAP_DO = (0x110),  // Enable & Map PWM0 to Do channels
        PRB_PWM1_MAP_DO = (0x111),  // Enable & Map PWM1 to Do channels
        PRB_PWM2_MAP_DO = (0x112),  // Enable & Map PWM2 to Do channels
        PRB_PWM3_MAP_DO = (0x113),  // Enable & Map PWM3 to Do channels

        // PTP buffer mode define
        PTP_OPT_ABORTING = (0x00000000),
        PTP_OPT_BUFFERED = (0x00001000),
        PTP_OPT_BLEND_LOW = (0x00002000),
        PTP_OPT_BLEND_PREVIOUS = (0x00003000),
        PTP_OPT_BLEND_NEXT = (0x00004000),
        PTP_OPT_BLEND_HIGH = (0x00005000),

        ITP_OPT_ABORT_BLEND = (0x00000000),
        ITP_OPT_ABORT_FORCE = (0x00001000),
        ITP_OPT_ABORT_STOP = (0x00002000),
        ITP_OPT_BUFFERED = (0x00003000),
        ITP_OPT_BLEND_DEC_EVENT = (0x00004000),
        ITP_OPT_BLEND_RES_DIST = (0x00005000),
        ITP_OPT_BLEND_RES_DIST_PERCENT = (0x00006000),

        //Latch parameter number define. [Only for PCI-8158A]
        //////////////////////////////////////
        LTC_ENC_IPT_MODE = (0x00),
        LTC_ENC_EA_INV = (0x01),
        LTC_ENC_EB_INV = (0x02),
        LTC_ENC_EZ_CLR_LOGIC = (0x03),
        LTC_ENC_EZ_CLR_EN = (0x04),
        LTC_ENC_SIGNAL_FILITER_EN = (0x05),
        LTC_FIFO_HIGH_LEVEL = (0x06),
        LTC_SIGNAL_FILITER_EN = (0x07),
        LTC_SIGNAL_TRIG_LOGIC = (0x08),

        //Latch parameter number define. [For PCI-8258 & PCIe-8334/8]
        LTC_IPT = (0x10),
        LTC_ENC = (0x11),
        LTC_LOGIC = (0x12),

        // Board parameter define =(For PCI-8392 SSCNET), 
        PRB_SSC_APPLICATION = (0x10000), // Reserved
        PRB_SSC_CYCLE_TIME = (0x10000), // SSCNET cycle time selection=(vaild befor start sscnet),
        PRB_PARA_INIT_OPT = (0x00020), // Initial boot mode.
        PRB_WATCH_DOG_LIMIT = (0x00010), // Set / Get watch dog limit.
        PRB_WATCH_DOG_COUNTER = (0x00011), //Watch dog counter

        // Board parameter define =(For DPAC), 
        PRB_DPAC_DISPLAY_MODE = (0x10001), //DPAC Display mode
        PRB_DPAC_DI_MODE = (0x10002), //Set DI pin modes

        //Board Parameter define (EMX-100) 
        PRB_ASYNC_MODE = (0x50),
        PRB_DISCONNET_HANDLING = (0x51),

        PRB_DPAC_THERMAL_MONITOR_NO = (0x20001), //DPAC TEST
        PRB_DPAC_THERMAL_MONITOR_VALUE = (0x20002), //DPAC TEST

        // move option define
        OPT_ABSOLUTE = (0x00000000),
        OPT_RELATIVE = (0x00000001),
        OPT_WAIT = (0x00000100),
        OPT_FORCE_ABORT = (0x00000200),

        // Axis parameter define =(General),
        PRA_EL_LOGIC = (0x00),  // EL logic
        PRA_ORG_LOGIC = (0x01),  // ORG logic
        PRA_EL_MODE = (0x02),  // EL stop mode
        PRA_MDM_CONDI = (0x03),  // Motion done condition
        PRA_EL_EXCHANGE = (0x04),  //PEL, MEL exchange enable

        PRA_ALM_LOGIC = (0x04),  // ALM logic [PCI-8253/56 only]
        PRA_ZSP_LOGIC = (0x05), // ZSP logic [PCI-8253/56 only]
        PRA_EZ_LOGIC = (0x06),  // EZ logic  [PCI-8253/56 only]
        PRA_STP_DEC = (0x07),  // Stop deceleration
        PRA_SD_DEC = (0x07),  // Stop deceleration
        PRA_SPEL_EN = (0x08),  // SPEL Enable
        PRA_SMEL_EN = (0x09),  // SMEL Enable
        PRA_EFB_POS0 = (0x0A),  // EFB position 0
        PRA_SPEL_POS = (0x0A),  // EFB position 0
        PRA_EFB_POS1 = (0x0B),  // EFB position 1
        PRA_SMEL_POS = (0x0B),  // EFB position 1
        PRA_EFB_CONDI0 = (0x0C),  // EFB position 0 condition 
        PRA_EFB_CONDI1 = (0x0D),  // EFB position 1 condition 
        PRA_EFB_SRC0 = (0x0E),  // EFB position 0 source
        PRA_EFB_SRC1 = (0x0F),  // EFB position 1 source 
        PRA_HOME_MODE = (0x10),  // home mode
        PRA_HOME_DIR = (0x11),  // homing direction
        PRA_HOME_CURVE = (0x12),  // homing curve parten=(T or s curve),
        PRA_HOME_ACC = (0x13),  // Acceleration deceleration rate 
        PRA_HOME_VS = (0x14),  // homing start velocity
        PRA_HOME_VM = (0x15),  // homing max velocity
        PRA_HOME_VA = (0x16),  // homing approach velocity [PCI-8253/56 only]
        PRA_HOME_SHIFT = (0x17),  // The shift from ORG [PCI-8254/58 only]
        PRA_HOME_EZA = (0x18),  // EZ alignment enable
        PRA_HOME_VO = (0x19),  // Homing leave ORG velocity
        PRA_HOME_OFFSET = (0x1A),  // The escape pulse amounts=(Leaving home by position),
        PRA_HOME_POS = (0x1B),  // The position from ORG [PCI-8254/58 only]
        PRA_HOME_TORQUE = (0x1C),  //Torque-Limit value setting for home move
        PRA_HOME_Search_Target = (0x1D),  // Select Home move search target signal
        PRA_CURVE = (0x20),  // Move curve pattern
        PRA_SF = (0x20),  // Move s-factor
        PRA_ACC = (0x21), // Move acceleration
        PRA_DEC = (0x22),// Move deceleration
        PRA_VS = (0x23),  // Move start velocity
        PRA_VM = (0x24),  // Move max velocity
        PRA_VE = (0x25),  // Move end velocity
        PRA_SACC = (0x26),  // S curve acceleration
        PRA_SDEC = (0x27),  // S curve deceleration
        PRA_ACC_SR = (0x28),  // S curve ratio in acceleration( S curve with linear acceleration)
        PRA_DEC_SR = (0x29),  // S curve ratio in deceleration( S curve with linear deceleration)
        PRA_PRE_EVENT_DIST = (0x2A), //Pre-event distance
        PRA_POST_EVENT_DIST = (0x2B), //Post-event distance

        //following only for V2...
        PRA_DIST = (0x30),  // Move distance
        PRA_MAX_VELOCITY = (0x31),  // Maximum velocity
        PRA_SCUR_PERCENTAGE = (0x32),  // Scurve percentage
        PRA_BLENDING_MODE = (0x33),  // Blending mode
        PRA_STOP_MODE = (0x34),  // Stop mode
        PRA_STOP_DELRATE = (0x35),  // Stop function deceleration rate 
        PRA_PT_STOP_ENDO = (0x32),  // Disable do when point table stopping.
        PRA_PT_STP_DO_EN = (0x32),  // Disable do when point table stopping.
        PRA_PT_STOP_DO = (0x33),  // Set do value when point table stopping.
        PRA_PT_STP_DO = (0x33),  // Set do value when point table stopping.		 
        PRA_PWM_OFF = (0x34),  // Disable specified PWM output when ASTP input signal is active.
        PRA_DO_OFF = (0x35),  // Set DO value when ASTP input signal is active.		 
        PRA_MOVE_RATIO = (0x88),  //Move ratio

        PRA_JG_MODE = (0x40),  // Jog mode
        PRA_JG_DIR = (0x41),  // Jog move direction
        PRA_JG_CURVE = (0x42),  // Jog curve parten=(T or s curve),
        PRA_JG_SF = (0x42),  // Jog curve parten=(T or s curve)
        PRA_JG_ACC = (0x43),  // Jog move acceleration
        PRA_JG_DEC = (0x44),  // Jog move deceleration
        PRA_JG_VM = (0x45),  // Jog move max velocity
        PRA_JG_STEP = (0x46),  // Jog offset =(For step mode),
        PRA_JG_OFFSET = (0x46),  // Jog offset =(For step mode),
        PRA_JG_DELAY = (0x47),  // Jog delay =(For step mode),
        PRA_JG_MAP_DI_EN = (0x48), // (I32) Enable Digital input map to jog command signal
        PRA_JG_P_JOG_DI = (0x49), // (I32) Mapping configuration for positive jog and digital input.
        PRA_JG_N_JOG_DI = (0x4A), // (I32) Mapping configuration for negative jog and digital input.
        PRA_JG_JOG_DI = (0x4B), // (I32) Mapping configuration for jog and digital input.
        PRA_JG_STOP = (0x4C),// just for EMX100

        PRA_MDN_DELAY = (0x50),  // NSTP delay setting
        PRA_SINP_WDW = (0x51),  // Soft INP window setting
        PRA_SINP_STBL = (0x52),  // Soft INP stable cycle
        PRA_SINP_STBT = (0x52),  // Soft INP stable cycle
        PRA_SERVO_LOGIC = (0x53), //  SERVO logic

        PRA_GEAR_MASTER = (0x60),  // (I32) Select gearing master
        PRA_GEAR_ENGAGE_RATE = (0x61),  // (F64) Gear engage rate
        PRA_GEAR_RATIO = (0x62),  // (F64) Gear ratio
        PRA_GANTRY_PROTECT_1 = (0x63),  // (F64) E-gear gantry mode protection level 1
        PRA_GANTRY_PROTECT_2 = (0x64),  // (F64) E-gear gantry mode protection level 2
        PRA_EGEAR_MASTER = (0x65),    //Select gearing master axis

        // Axis parameter define =(For PCI-8253/56),
        PRA_PLS_IPT_MODE = (0x80),  // Pulse input mode setting
        PRA_PLS_OPT_MODE = (0x81), // Pulse output mode setting
        PRA_MAX_E_LIMIT = (0x82),  // Maximum encoder count limit
        PRA_ENC_FILTER = (0x83),  // Encoder filter
        PRA_ENCODER_FILTER = (0x83),  // Encoder filter

        PRA_EGEAR = (0x84),  // E-Gear ratio
        PRA_ENCODER_DIR = (0x85),  // Encoder direction
        PRA_POS_UNIT_FACTOR = (0x86),  // position unit factor setting 
        PRA_KP_SHIFT = (0x9B), //Proportional control result shift
        PRA_KI_SHIFT = (0x9c), //Integral control result shift 
        PRA_KD_SHIFT = (0x9D), // Derivative control result shift
        PRA_KVFF_SHIFT = (0x9E),       //Velocity feed-forward control result shift
        PRA_KAFF_SHIFT = (0x9F),       //Acceleration feed-forward control result shift		 

        PRA_PID_SHIFT = (0xA0),        //PID control result shift 
        PRA_KP_GAIN = (0x90),  // PID controller Kp gain
        PRA_KI_GAIN = (0x91),  // PID controller Ki gain
        PRA_KD_GAIN = (0x92),  // PID controller Kd gain
        PRA_KFF_GAIN = (0x93),  // Feed forward Kff gain
        PRA_KVFF_GAIN = (0x93),  // Feed forward Kff gain
        PRA_KVGTY_GAIN = (0x94),  // Gantry controller Kvgty gain
        PRA_KPGTY_GAIN = (0x95),  // Gantry controller Kpgty gain
        PRA_IKP_GAIN = (0x96),  // PID controller Kp gain in torque mode
        PRA_IKI_GAIN = (0x97),  // PID controller Ki gain in torque mode
        PRA_IKD_GAIN = (0x98),  // PID controller Kd gain in torque mode
        PRA_IKFF_GAIN = (0x99),  // Feed forward Kff gain in torque mode
        PRA_KAFF_GAIN = (0x9A),  // Acceleration feedforward Kaff gain

        //following only for V2...
        PRA_VOLTAGE_MAX = (0x9B),  // Maximum output limit
        PRA_VOLTAGE_MIN = (0x9C),  // Minimum output limit
        PRA_M_INTERFACE = (0x100), // Motion System.Int32erface 
        PRA_M_VOL_RANGE = (0x110), // Motor voltage input range
        PRA_M_MAX_SPEED = (0x111), // Motor maximum speed 
        PRA_M_ENC_RES = (0x112), // Motor encoder resolution

        PRA_V_OFFSET = (0x120), // Voltage offset
        PRA_SERVO_V_BIAS = (0x120), // Voltage offset
        PRA_DZ_LOW = (0x121), // Dead zone low side
        PRA_DZ_UP = (0x122), // Dead zone up side
        PRA_SAT_LIMIT = (0x123), // Voltage saturation output limit
        PRA_SERVO_V_LIMIT = (0x123), // Voltage saturation output limit
        PRA_ERR_C_LEVEL = (0x124), // Error counter check level
        PRA_ERR_POS_LEVEL = (0x124), // Error counter check level
        PRA_V_INVERSE = (0x125), // Output voltage inverse
        PRA_SERVO_V_INVERSE = (0x125), // Output voltage inverse
        PRA_DZ_VAL = (0x126), // Dead zone output value
        PRA_IW_MAX = (0x127), // Integral windup maximum value
        PRA_IW_MIN = (0x128), // Integral windup minimum value
        PRA_BKL_DIST = (0x129), // Backlash distance
        PRA_BKL_CNSP = (0x12a),// Backlash consumption
        PRA_INTEGRAL_LIMIT = (0x12B), // (I32) Integral limit
        PRA_D_SAMPLE_TIME = (0x12C), // (I32) Derivative Sample Time

        PRA_PSR_LINK = (0x130), // Connect pulser number
        PRA_PSR_RATIO = (0X131), // Set pulser ratio  
        PRA_BIQUAD0_A1 = (0x132), // (F64) Biquad filter0 coefficient A1
        PRA_BIQUAD0_A2 = (0x133),// (F64) Biquad filter0 coefficient A2
        PRA_BIQUAD0_B0 = (0x134), // (F64) Biquad filter0 coefficient B0
        PRA_BIQUAD0_B1 = (0x135), // (F64) Biquad filter0 coefficient B1
        PRA_BIQUAD0_B2 = (0x136), // (F64) Biquad filter0 coefficient B2
        PRA_BIQUAD0_DIV = (0x137), // (F64) Biquad filter0 divider
        PRA_BIQUAD1_A1 = (0x138), // (F64) Biquad filter1 coefficient A1
        PRA_BIQUAD1_A2 = (0x139), // (F64) Biquad filter1 coefficient A2
        PRA_BIQUAD1_B0 = (0x13A), // (F64) Biquad filter1 coefficient B0
        PRA_BIQUAD1_B1 = (0x13B), // (F64) Biquad filter1 coefficient B1
        PRA_BIQUAD1_B2 = (0x13C), // (F64) Biquad filter1 coefficient B2
        PRA_BIQUAD1_DIV = (0x13D), // (F64) Biquad filter1 divider
        PRA_FRIC_GAIN = (0x13E), // (F64) Friction voltage compensation

        PRA_DA_TYPE = (0x140), // DAC output type
        PRA_CONTROL_MODE = (0x141), // Closed loop control mode

        //Pulser function
        PRA_PSR_IPT_MODE = (0x160), // all
        PRA_PSR_IPT_LOGIC = (0x161), // dsp
        PRA_PSR_IPT_DIR = (0x162), // all
        PRA_PSR_RATIO_VALUE = (0x163), // dsp
        PRA_PSR_PDV = (0x164), // asic
        PRA_PSR_PMG = (0x165), // asic
        PRA_PSR_HOME_TYPE = (0x166), // asic
        PRA_PSR_HOME_SPD = (0x167), // asic
        PRA_PSR_ACC = (0x168), // dsp
        PRA_PSR_JERK = (0x169), // dsp

        // Axis parameter define =(For PCI-8154/58),
        // Input/Output Mode
        PRA_PLS_IPT_LOGIC = (0x200), //Reverse pulse input counting
        PRA_FEEDBACK_SRC = (0x201), //Select feedback conter
                                    //IO Config
        PRA_ALM_MODE = (0x210), //ALM Mode
        PRA_INP_LOGIC = (0x211), //INP Logic
        PRA_SD_EN = (0x212), //SD Enable -- Bit 8
        PRA_SD_MODE = (0x213), //SD Mode
        PRA_SD_LOGIC = (0x214), //SD Logic
        PRA_SD_LATCH = (0x215), //SD Latch
        PRA_ERC_MODE = (0x216), //ERC Mode
        PRA_ERC_LOGIC = (0x217), //ERC logic
        PRA_ERC_LEN = (0x218), //ERC pulse width
        PRA_RESET_COUNTER = (0x219), //Reset counter when home move is complete
        PRA_PLS_IPT_FLT = (0x21B), //EA/EB Filter Enable
        PRA_INP_MODE = (0x21C), //INP Mode
        PRA_LTC_LOGIC = (0x21D), //LTC LOGIC
        PRA_IO_FILTER = (0x21E), //+-EZ, SD, ORG, ALM, INP filter
        PRA_COMPENSATION_PULSE = (0x221), //BACKLASH PULSE
        PRA_COMPENSATION_MODE = (0x222), //BACKLASH MODE
        PRA_LTC_SRC = (0x223), //LTC Source
        PRA_LTC_DEST = (0x224), //LTC Destination
        PRA_LTC_DATA = (0x225), //Get LTC DATA
        PRA_GCMP_EN = (0x226), // CMP Enable
        PRA_GCMP_POS = (0x227), // Get CMP position
        PRA_GCMP_SRC = (0x228), // CMP source
        PRA_GCMP_ACTION = (0x229), // CMP Action
        PRA_GCMP_STS = (0x22A), // CMP Status
        PRA_VIBSUP_RT = (0x22B), // Vibration Reverse Time
        PRA_VIBSUP_FT = (0x22C), // Vibration Forward Time
        PRA_LTC_DATA_SPD = (0x22D), // Choose latch data for current speed or error position
        PRA_GPDO_SEL = (0x230), //Select DO/CMP Output mode
        PRA_GPDI_SEL = (0x231), //Select DO/CMP Output mode
        PRA_GPDI_LOGIC = (0x232), //Set gpio input logic
        PRA_RDY_LOGIC = (0x233), //RDY logic
        PRA_ECMP_EN = (0x280), // Error CMP Enable
        PRA_ECMP_POS = (0x281), // Get CMP position
        PRA_ECMP_SRC = (0x282), // Set CMP source
        PRA_ECMP_ACTION = (0x283), // CMP Status
        PRA_ECMP_STS = (0x284), //Error CMP logic
        PRA_ERR_RESCOUNT = (0x285), // Reset Counter 3 (Error Counter)
        PRA_ERR_COUNTER = (0x290), // Counter 3(Error Counter)
        PRA_TCMP_EN = (0x270), // trigger CMP Enable
        PRA_TCMP_POS = (0x271), // Get CMP position
        PRA_TCMP_SRC = (0x272), // Set CMP source
        PRA_TCMP_STS = (0x273), // CMP Status
        PRA_TCMP_LOGIC = (0x274), // CMP logic
        PRA_TCMP_ACTION = (0x275), // CMP Action

        //Fixed Speed
        PRA_SPD_LIMIT = (0x240), // Set Fixed Speed
        PRA_MAX_ACCDEC = (0x241), // Get max acceleration by fixed speed
        PRA_MIN_ACCDEC = (0x242), // Get max acceleration by fixed speed
        PRA_ENABLE_SPD = (0x243), // Disable/Enable Fixed Speed only for HSL-4XMO.

        //Continuous Move
        PRA_CONTI_MODE = (0x250), // Continuous Mode
        PRA_CONTI_BUFF = (0x251), // Continuous Buffer 
                                  //Simultaneous Move
        PRA_SYNC_STOP_MODE = (0x260), // Sync Mode

        //PCS
        PRA_PCS_EN = (0x2A0),        // PCS Enable
        PRA_PCS_LOGIC = (0x2A1),     // PCS Logic

        // PCI-8144 axis parameter define
        PRA_CMD_CNT_EN = (0x10000),
        PRA_MIO_SEN = (0x10001),
        PRA_START_STA = (0x10002),
        PRA_SPEED_CHN = (0x10003),
        PRA_ORG_STP = (0x1A),

        // Axis parameter define =(For PCI-8392 SSCNET),
        PRA_SSC_SERVO_PARAM_SRC = (0x10000), //Servo parameter source
        PRA_SSC_SERVO_ABS_POS_OPT = (0x10001), //Absolute position system option
        PRA_SSC_SERVO_ABS_CYC_CNT = (0x10002), //Absolute cycle counter of servo driver
        PRA_SSC_SERVO_ABS_RES_CNT = (0x10003), //Absolute resolution counter of servo driver
        PRA_SSC_TORQUE_LIMIT_P = (0x10004), //Torque limit positive =(0.1%),
        PRA_SSC_TORQUE_LIMIT_N = (0x10005), //Torque limit negative =(0.1%),
        PRA_SSC_TORQUE_CTRL = (0x10006), //Torque control
        PRA_SSC_RESOLUTION = (0x10007), //resolution =(E-gear),
        PRA_SSC_GMR = (0x10008), //resolution (New E-gear)
        PRA_SSC_GDR = (0x10009), //resolution (New E-gear)

        // Axis parameter define (For EMX-100) 
        PRA_SOFT_EL_EN = (0xB0),
        PRA_SOFT_EL_SRC = (0xB1),
        PRA_PLS_IPT_NEG_DRIVE = (0xB2),
        PRA_PLS_OPT_NEG_DRIVE = (0xB3),
        PRA_PLS_OPT_DIR = (0xB4),
        PRA_TRIG_VEL_PREVENTION_EN = (0xB5),
        PRA_PLS_IPT_DIR_PIN = (0x87),
        PRA_PLS_OPT_DIR_PIN = (0x88),

        //PCI-8353
        PRA_HOME_LATCH = (0x900), //Select Home latch source
                                  // Sampling parameter define
        SAMP_PA_RATE = (0x0), //Sampling rate
        SAMP_PA_EDGE = (0x2), //Edge select
        SAMP_PA_LEVEL = (0x3), //Level select
        SAMP_PA_TRIGCH = (0x5), //Select trigger channel
        SAMP_PA_SEL = (0x6),
        SAMP_PA_SRC_CH0 = (0x10), //Sample source of channel 0
        SAMP_PA_SRC_CH1 = (0x11), //Sample source of channel 1
        SAMP_PA_SRC_CH2 = (0x12), //Sample source of channel 2
        SAMP_PA_SRC_CH3 = (0x13), //Sample source of channel 3

        // Sampling source
        SAMP_AXIS_MASK = (0xF00),
        SAMP_PARAM_MASK = (0xFF),
        SAMP_COM_POS = (0x00), //command position
        SAMP_FBK_POS = (0x01), //feedback position
        SAMP_CMD_VEL = (0x02), //command velocity
        SAMP_FBK_VEL = (0x03), //feedback velocity
        SAMP_MIO = (0x04), //motion IO
        SAMP_MSTS = (0x05), //motion status
        SAMP_MSTS_ACC = (0x06), //motion status acc
        SAMP_MSTS_MV = (0x07), //motion status at max velocity
        SAMP_MSTS_DEC = (0x08), //motion status at dec
        SAMP_MSTS_CSTP = (0x09), //motion status CSTP
        SAMP_MSTS_NSTP = (0x0A), //motion status NSTP
        SAMP_MSTS_MDN = (0x0A), //motion status NSTP
        SAMP_MIO_INP = (0x0B), //motion status INP
        SAMP_MIO_ZERO = (0x0C), //motion status ZERO
        SAMP_MIO_ORG = (0x0D), //motion status OGR
        SAMP_CONTROL_VOL = (0x20),  // Control command voltage
        SAMP_GTY_DEVIATION = (0x21), // Gantry deviation
        SAMP_ENCODER_RAW = (0x22), // Encoder raw data
        SAMP_ERROR_COUNTER = (0x23), // Error counter data
        SAMP_ERROR_POS = (0x23), //Error position [PCI-8254/58]
        SAMP_PTBUFF_RUN_INDEX = (0x24), //Point table running index

        //Only for PCI-8392
        SAMP_SSC_MON_0 = (0x10),  // SSCNET servo monitor ch0
        SAMP_SSC_MON_1 = (0x11),  // SSCNET servo monitor ch1
        SAMP_SSC_MON_2 = (0x12),  // SSCNET servo monitor ch2
        SAMP_SSC_MON_3 = (0x13),  // SSCNET servo monitor ch3		 
                                  //Only for PCI-8254/8, AMP-204/8C
        SAMP_COM_POS_F64 = (0x10), // Command position
        SAMP_FBK_POS_F64 = (0x11), // Feedback position
        SAMP_CMD_VEL_F64 = (0x12), // Command velocity
        SAMP_FBK_VEL_F64 = (0x13), // Feedback velocity
        SAMP_CONTROL_VOL_F64 = (0x14), // Control command voltage
        SAMP_ERR_POS_F64 = (0x15), // Error position
        SAMP_PWM_FREQUENCY_F64 = (0x18), // PWM frequency (Hz)
        SAMP_PWM_DUTY_CYCLE_F64 = (0x19), // PWM duty cycle (%)
        SAMP_PWM_WIDTH_F64 = (0x1A), // PWM width (ns)
        SAMP_VAO_COMP_VEL_F64 = (0x1B), // Composed velocity for Laser power control (pps)
        SAMP_PTBUFF_COMP_VEL_F64 = (0x1C), // Composed velocity of point table
        SAMP_PTBUFF_COMP_ACC_F64 = (0x1D), // Composed acceleration of point table

        //FieldBus parameter define
        PRF_COMMUNICATION_TYPE = (0x00),// FiledBus Communication Type=(Full/half duplex),
        PRF_TRANSFER_RATE = (0x01),// FiledBus Transfer Rate
        PRF_HUB_NUMBER = (0x02),// FiledBus Hub Number
        PRF_INITIAL_TYPE = (0x03),// FiledBus Initial Type(Clear/Reserve Do area)
        PRF_CHKERRCNT_LAYER = (0x04),// Set the check error count layer.

        //Gantry parameter number define [Only for PCI-8392, PCI-8253/56]
        GANTRY_MODE = (0x0),
        GENTRY_DEVIATION = (0x1),
        GENTRY_DEVIATION_STP = (0x2),

        // Filter parameter number define [Only for PCI-8253/56]
        FTR_TYPE_ST0 = (0x00),  // Station 0 filter type
        FTR_FC_ST0 = (0x01), // Station 0 filter cutoff frequency
        FTR_BW_ST0 = (0x02),  // Station 0 filter bandwidth
        FTR_ENABLE_ST0 = (0x03),  // Station 0 filter enable/disable
        FTR_TYPE_ST1 = (0x10),  // Station 1 filter type
        FTR_FC_ST1 = (0x11),  // Station 1 filter cutoff frequency
        FTR_BW_ST1 = (0x12),  // Station 1 filter bandwidth
        FTR_ENABLE_ST1 = (0x13),  // Station 1 filter enable/disable


        // Device name define
        DEVICE_NAME_NULL = (0xFFFF),
        DEVICE_NAME_PCI_8392 = (0),
        DEVICE_NAME_PCI_825X = (1),
        DEVICE_NAME_PCI_8154 = (2),
        DEVICE_NAME_PCI_785X = (3),
        DEVICE_NAME_PCI_8158 = (4),
        DEVICE_NAME_PCI_7856 = (5),
        DEVICE_NAME_ISA_DPAC1000 = (6),
        DEVICE_NAME_ISA_DPAC3000 = (7),
        DEVICE_NAME_PCI_8144 = (8),
        DEVICE_NAME_PCI_825458 = (9),
        DEVICE_NAME_PCI_8102 = (10),
        DEVICE_NAME_PCI_V8258 = (11),
        DEVICE_NAME_PCI_V8254 = (12),
        DEVICE_NAME_PCI_8158A = (13),
        DEVICE_NAME_AMP_20408C = (14),
        DEVICE_NAME_PCI_8353 = (15),
        DEVICE_NAME_PCI_8392F = (16),
        DEVICE_NAME_PCI_C154 = (17),
        DEVICE_NAME_PCI_C154_PLUS = (18),
        DEVICE_NAME_PCI_8353_RTX = (19),
        DEVICE_NAME_PCIE_8338 = (20),
        DEVICE_NAME_PCIE_8154 = (21),
        DEVICE_NAME_PCIE_8158 = (22),
        DEVICE_NAME_ENET_EMX100 = (23),
        DEVICE_NAME_PCIE_8334 = (24),
        DEVICE_NAME_PCIE_8332 = (25),
        DEVICE_NAME_PCIE_8331 = (26),


        ///////////////////////////////////////////////
        //   HSL Slave module definition
        ///////////////////////////////////////////////
        SLAVE_NAME_UNKNOWN = (0x000),
        SLAVE_NAME_HSL_DI32 = (0x100),
        SLAVE_NAME_HSL_DO32 = (0x101),
        SLAVE_NAME_HSL_DI16DO16 = (0x102),
        SLAVE_NAME_HSL_AO4 = (0x103),
        SLAVE_NAME_HSL_AI16AO2_VV = (0x104),
        SLAVE_NAME_HSL_AI16AO2_AV = (0x105),
        SLAVE_NAME_HSL_DI16UL = (0x106),
        SLAVE_NAME_HSL_DI16RO8 = (0x107),
        SLAVE_NAME_HSL_4XMO = (0x108),
        SLAVE_NAME_HSL_DI16_UCT = (0x109),
        SLAVE_NAME_HSL_DO16_UCT = (0x10A),
        SLAVE_NAME_HSL_DI8DO8 = (0x10B),
        ///////////////////////////////////////////////
        //   MNET Slave module definition
        ///////////////////////////////////////////////
        SLAVE_NAME_MNET_1XMO = (0x200),
        SLAVE_NAME_MNET_4XMO = (0x201),
        SLAVE_NAME_MNET_4XMO_C = (0x202),

        ///////////////////////////////////////////////
        //   PCIe-8338 Slave module definition
        ///////////////////////////////////////////////
        SLAVE_ADLINK_ECAT_EPS_1132 = (0x1132),
        SLAVE_ADLINK_ECAT_EPS_2032 = (0x2032),
        SLAVE_ADLINK_ECAT_EPS_2132 = (0x2132),
        SLAVE_ADLINK_ECAT_EPS_3032 = (0x3032),
        SLAVE_ADLINK_ECAT_EPS_3216 = (0x3216),
        SLAVE_ADLINK_ECAT_EPS_3504 = (0x3504),
        SLAVE_ADLINK_ECAT_EPS_4008 = (0x4008),
        SLAVE_ADLINK_ECAT_EPS_2308 = (0x2308),
        SLAVE_ADLINK_ECAT_EPS_7002 = (0x7002),
        SLAVE_ADLINK_ECAT_EPS_1032 = (0x1032),
        SLAVE_ADLINK_EU_1008 = (0x6),    //DI8, 8 Channels, PNP 
        SLAVE_ADLINK_EU_1108 = (0x9),    //DI8, 8 Channels, NPN 
        SLAVE_ADLINK_EU_2008 = (0xB),    //11 DO8, 8 Channels, PNP 
        SLAVE_ADLINK_EU_2108 = (0xC),    //12 DO8, 8 Channels, NPN
        SLAVE_ADLINK_EU_3104 = (0x31),   //49 AI4, Voltage(0-10V), 4 Channels, 16 Bit 
        SLAVE_ADLINK_EU_3304 = (0x29),   //41 AI4, Current(4-20mA), 4 Channels, 16 Bit 
        SLAVE_ADLINK_EU_4104 = (0x32),   //50 AO4, Voltage(0-10V), 4 Channels, 16 Bit 
        SLAVE_ADLINK_EU_4304 = (0x35),   //53 AO4, Current(4-20mA), 4 Channels, 16 Bit

        ///////////////////////////////////////////////
        //   PCIe-8338 EtherCAT master/slave status definition
        ///////////////////////////////////////////////
        EC_STATE_NOT_RDY = (0x0000),
        EC_STATE_RDY = (0x0001),
        EC_STATE_BUS_SCAN = (0x0002),
        EC_STATE_INIT = (0x0003),
        EC_STATE_PREOP = (0x0004),
        EC_STATE_SAFEOP = (0x0005),
        EC_STATE_OP = (0x0006),

        //Trigger parameter number define. [Only for DB-8150]
        TG_PWM0_PULSE_WIDTH = (0x00),
        TG_PWM1_PULSE_WIDTH = (0x01),
        TG_PWM0_MODE = (0x02),
        TG_PWM1_MODE = (0x03),
        TG_TIMER0_INTERVAL = (0x04),
        TG_TIMER1_INTERVAL = (0x05),
        TG_ENC0_CNT_DIR = (0x06),
        TG_ENC1_CNT_DIR = (0x07),
        TG_IPT0_MODE = (0x08),
        TG_IPT1_MODE = (0x09),
        TG_EZ0_CLEAR_EN = (0x0A),
        TG_EZ1_CLEAR_EN = (0x0B),
        TG_EZ0_CLEAR_LOGIC = (0x0C),
        TG_EZ1_CLEAR_LOGIC = (0x0D),
        TG_CNT0_SOURCE = (0x0E),
        TG_CNT1_SOURCE = (0x0F),
        TG_FTR0_EN = (0x10),
        TG_FTR1_EN = (0x11),
        TG_DI_LATCH0_EN = (0x12),
        TG_DI_LATCH1_EN = (0x13),
        TG_DI_LATCH0_EDGE = (0x14),
        TG_DI_LATCH1_EDGE = (0x15),
        TG_DI_LATCH0_VALUE = (0x16),
        TG_DI_LATCH1_VALUE = (0x17),
        TG_TRGOUT_MAP = (0x18),
        TG_TRGOUT_LOGIC = (0x19),
        TG_FIFO_LEVEL = (0x1A),
        TG_PWM0_SOURCE = (0x1B),
        TG_PWM1_SOURCE = (0x1C),

        //trigger only for EMX100
        TGR0_CMP_SRC = (0x00),
        TGR1_CMP_SRC = (0x01),
        TGR0_CMP_COND = (0x02),
        TGR1_CMP_COND = (0x03),
        TGR0_CMP_VALUE = (0x04),
        TGR1_CMP_VALUE = (0x05),
        TGR0_PULSE_WIDTH = (0x06),
        TGR1_PULSE_WIDTH = (0x07),
        TGR0_PULSE_LOGIC = (0x08),
        TGR1_PULSE_LOGIC = (0x09),
        TGR0_CMP_EN = (0x0A),
        TGR1_CMP_EN = (0x0B),
        TGR0_CMP_MODE = (0x0C),
        TGR1_CMP_MODE = (0x0D),
        TGR0_LCMP_INTER = (0x0E),
        TGR1_LCMP_INTER = (0x0F),
        TGR0_LCMP_RETIME = (0x10),
        TGR1_LCMP_RETIME = (0x11),

        //Trigger parameter number define. [Only for PCI-8253/56]
        TG_LCMP0_SRC = (0x00),
        TG_LCMP1_SRC = (0x01),
        TG_TCMP0_SRC = (0x02),
        TG_TCMP1_SRC = (0x03),
        TG_LCMP0_EN = (0x04),
        TG_LCMP1_EN = (0x05),
        TG_TCMP0_EN = (0x06),
        TG_TCMP1_EN = (0x07),
        TG_TRG0_SRC = (0x10),
        TG_TRG1_SRC = (0x11),
        TG_TRG2_SRC = (0x12),
        TG_TRG3_SRC = (0x13),
        TG_TRG0_PWD = (0x14),
        TG_TRG1_PWD = (0x15),
        TG_TRG2_PWD = (0x16),
        TG_TRG3_PWD = (0x17),
        TG_TRG0_CFG = (0x18),
        TG_TRG1_CFG = (0x19),
        TG_TRG2_CFG = (0x1A),
        TG_TRG3_CFG = (0x1B),
        TMR_ITV = (0x20),
        TMR_EN = (0x21),

        //Trigger parameter number define. [Only for MNET-4XMO-C & HSL-4XMO]
        TG_CMP0_SRC = (0x00),
        TG_CMP1_SRC = (0x01),
        TG_CMP2_SRC = (0x02),
        TG_CMP3_SRC = (0x03),
        TG_CMP0_EN = (0x04),
        TG_CMP1_EN = (0x05),
        TG_CMP2_EN = (0x06),
        TG_CMP3_EN = (0x07),
        TG_CMP0_TYPE = (0x08),
        TG_CMP1_TYPE = (0x09),
        TG_CMP2_TYPE = (0x0A),
        TG_CMP3_TYPE = (0x0B),
        TG_CMPH_EN = (0x0C), //Not for HSL-4XMO
        TG_CMPH_DIR_EN = (0x0D),//Not for HSL-4XMO
        TG_CMPH_DIR = (0x0E), //Not for HSL-4XMO
        TG_ENCH_CFG = (0x20),//Not for HSL-4XMO
        TG_TRG0_CMP_DIR = (0x21), //Only for HSL-4XMO
        TG_TRG1_CMP_DIR = (0x22), //Only for HSL-4XMO
        TG_TRG2_CMP_DIR = (0x23), //Only for HSL-4XMO
        TG_TRG3_CMP_DIR = (0x24), //Only for HSL-4XMO

        //Trigger parameter number define. [Only for PCI-8258, ECAT-4XMO]
        TGR_LCMP0_SRC = (0x00),
        TGR_LCMP1_SRC = (0x01),
        TGR_TCMP0_SRC = (0x02),
        TGR_TCMP1_SRC = (0x03),
        TGR_TCMP0_DIR = (0x04),
        TGR_TCMP1_DIR = (0x05),
        TGR_TRG_EN = (0x06),

        TGR_TCMP0_REUSE = (0x07),   // ECAT-4XMO
        TGR_TCMP1_REUSE = (0x08),   // ECAT-4XMO
        TGR_TCMP0_TRANSFER_DONE = (0x09),   // ECAT-4XMO
        TGR_TCMP1_TRANSFER_DONE = (0x0A),   // ECAT-4XMO
        TGR_TRG0_SRC = (0x10),
        TGR_TRG1_SRC = (0x11),
        TGR_TRG2_SRC = (0x12),
        TGR_TRG3_SRC = (0x13),
        TGR_TRG0_PWD = (0x14),
        TGR_TRG1_PWD = (0x15),
        TGR_TRG2_PWD = (0x16),
        TGR_TRG3_PWD = (0x17),
        TGR_TRG0_LOGIC = (0x18),
        TGR_TRG1_LOGIC = (0x19),
        TGR_TRG2_LOGIC = (0x1A),
        TGR_TRG3_LOGIC = (0x1B),
        TGR_TRG0_TGL = (0x1C),
        TGR_TRG1_TGL = (0x1D),
        TGR_TRG2_TGL = (0x1E),
        TGR_TRG3_TGL = (0x1F),
        TIMR_ITV = (0x20),
        TIMR_DIR = (0x21),
        TIMR_RING_EN = (0x22),
        TIMR_EN = (0x23),

        //Trigger parameter number define. [Only for PCI-8158A & PCI-C154(+)]
        TIG_ENC_IPT_MODE0 = (0x00),
        TIG_ENC_IPT_MODE1 = (0x01),
        TIG_ENC_IPT_MODE2 = (0x02),
        TIG_ENC_IPT_MODE3 = (0x03),
        TIG_ENC_IPT_MODE4 = (0x04),
        TIG_ENC_IPT_MODE5 = (0x05),
        TIG_ENC_IPT_MODE6 = (0x06),
        TIG_ENC_IPT_MODE7 = (0x07),
        TIG_ENC_EA_INV0 = (0x08),
        TIG_ENC_EA_INV1 = (0x09),
        TIG_ENC_EA_INV2 = (0x0A),
        TIG_ENC_EA_INV3 = (0x0B),
        TIG_ENC_EA_INV4 = (0x0C),
        TIG_ENC_EA_INV5 = (0x0D),
        TIG_ENC_EA_INV6 = (0x0E),
        TIG_ENC_EA_INV7 = (0x0F),
        TIG_ENC_EB_INV0 = (0x10),
        TIG_ENC_EB_INV1 = (0x11),
        TIG_ENC_EB_INV2 = (0x12),
        TIG_ENC_EB_INV3 = (0x13),
        TIG_ENC_EB_INV4 = (0x14),
        TIG_ENC_EB_INV5 = (0x15),
        TIG_ENC_EB_INV6 = (0x16),
        TIG_ENC_EB_INV7 = (0x17),
        TIG_ENC_SIGNAL_FILITER_EN0 = (0x28),
        TIG_ENC_SIGNAL_FILITER_EN1 = (0x29),
        TIG_ENC_SIGNAL_FILITER_EN2 = (0x2A),
        TIG_ENC_SIGNAL_FILITER_EN3 = (0x2B),
        TIG_ENC_SIGNAL_FILITER_EN4 = (0x2C),
        TIG_ENC_SIGNAL_FILITER_EN5 = (0x2D),
        TIG_ENC_SIGNAL_FILITER_EN6 = (0x2E),
        TIG_ENC_SIGNAL_FILITER_EN7 = (0x2F),
        TIG_TIMER8_DIR = (0x30),
        TIG_TIMER8_ITV = (0x31),
        TIG_CMP0_SRC = (0x32),
        TIG_CMP1_SRC = (0x33),
        TIG_CMP2_SRC = (0x34),
        TIG_CMP3_SRC = (0x35),
        TIG_CMP4_SRC = (0x36),
        TIG_CMP5_SRC = (0x37),
        TIG_CMP6_SRC = (0x38),
        TIG_CMP7_SRC = (0x39),
        TIG_TRG0_SRC = (0x3A),
        TIG_TRG1_SRC = (0x3B),
        TIG_TRG2_SRC = (0x3C),
        TIG_TRG3_SRC = (0x3D),
        TIG_TRG4_SRC = (0x3E),
        TIG_TRG5_SRC = (0x3F),
        TIG_TRG6_SRC = (0x40),
        TIG_TRG7_SRC = (0x41),
        TIG_TRGOUT0_MAP = (0x42),
        TIG_TRGOUT1_MAP = (0x43),
        TIG_TRGOUT2_MAP = (0x44),
        TIG_TRGOUT3_MAP = (0x45),
        TIG_TRGOUT4_MAP = (0x46),
        TIG_TRGOUT5_MAP = (0x47),
        TIG_TRGOUT6_MAP = (0x48),
        TIG_TRGOUT7_MAP = (0x49),
        TIG_TRGOUT0_LOGIC = (0x4A),
        TIG_TRGOUT1_LOGIC = (0x4B),
        TIG_TRGOUT2_LOGIC = (0x4C),
        TIG_TRGOUT3_LOGIC = (0x4D),
        TIG_TRGOUT4_LOGIC = (0x4E),
        TIG_TRGOUT5_LOGIC = (0x4F),
        TIG_TRGOUT6_LOGIC = (0x50),
        TIG_TRGOUT7_LOGIC = (0x51),
        TIG_PWM0_PULSE_WIDTH = (0x52),
        TIG_PWM1_PULSE_WIDTH = (0x53),
        TIG_PWM2_PULSE_WIDTH = (0x54),
        TIG_PWM3_PULSE_WIDTH = (0x55),
        TIG_PWM4_PULSE_WIDTH = (0x56),
        TIG_PWM5_PULSE_WIDTH = (0x57),
        TIG_PWM6_PULSE_WIDTH = (0x58),
        TIG_PWM7_PULSE_WIDTH = (0x59),
        TIG_PWM0_MODE = (0x5A),
        TIG_PWM1_MODE = (0x5B),
        TIG_PWM2_MODE = (0x5C),
        TIG_PWM3_MODE = (0x5D),
        TIG_PWM4_MODE = (0x5E),
        TIG_PWM5_MODE = (0x5F),
        TIG_PWM6_MODE = (0x60),
        TIG_PWM7_MODE = (0x61),
        TIG_TIMER0_ITV = (0x62),
        TIG_TIMER1_ITV = (0x63),
        TIG_TIMER2_ITV = (0x64),
        TIG_TIMER3_ITV = (0x65),
        TIG_TIMER4_ITV = (0x66),
        TIG_TIMER5_ITV = (0x67),
        TIG_TIMER6_ITV = (0x68),
        TIG_TIMER7_ITV = (0x69),
        TIG_FIFO_LEVEL0 = (0x6A),
        TIG_FIFO_LEVEL1 = (0x6B),
        TIG_FIFO_LEVEL2 = (0x6C),
        TIG_FIFO_LEVEL3 = (0x6D),
        TIG_FIFO_LEVEL4 = (0x6E),
        TIG_FIFO_LEVEL5 = (0x6F),
        TIG_FIFO_LEVEL6 = (0x70),
        TIG_FIFO_LEVEL7 = (0x71),
        TIG_OUTPUT_EN0 = (0x72),
        TIG_OUTPUT_EN1 = (0x73),
        TIG_OUTPUT_EN2 = (0x74),
        TIG_OUTPUT_EN3 = (0x75),
        TIG_OUTPUT_EN4 = (0x76),
        TIG_OUTPUT_EN5 = (0x77),
        TIG_OUTPUT_EN6 = (0x78),
        TIG_OUTPUT_EN7 = (0x79),

        // Motion IO status bit number define.
        MIO_ALM = (0),   // Servo alarm.
        MIO_PEL = (1),   // Positive end limit.
        MIO_MEL = (2),   // Negative end limit.
        MIO_ORG = (3),   // ORG (Home)
        MIO_EMG = (4),   // Emergency stop
        MIO_EZ = (5),   // EZ.
        MIO_INP = (6),   // In position.
        MIO_SVON = (7),   // Servo on signal.
        MIO_RDY = (8),   // Ready.
        MIO_WARN = (9),   // Warning.
        MIO_ZSP = (10),  // Zero speed.
        MIO_SPEL = (11),  // Soft positive end limit.
        MIO_SMEL = (12),  // Soft negative end limit.
        MIO_TLC = (13),  // Torque is limited by torque limit value.
        MIO_ABSL = (14),  // Absolute position lost.
        MIO_STA = (15),  // External start signal.
        MIO_PSD = (16),  // Positive slow down signal
        MIO_MSD = (17),  // Negative slow down signal
        MIO_SCL = (10),  // Circular limit.
        MIO_OP = (24),  // Not all slaves are in operation mode.

        // Motion status bit number define.
        MTS_CSTP = (0),     // Command stop signal. 
        MTS_VM = (1),     // At maximum velocity.
        MTS_ACC = (2),     // In acceleration.
        MTS_DEC = (3),     // In deceleration.
        MTS_DIR = (4),     // (Last)Moving direction.
        NSTP = (5),     // Normal stop(Motion done).
        MTS_HMV = (6),     // In home operation.
        MTS_SMV = (7),     // Single axis move( relative, absolute, velocity move).
        MTS_LIP = (8),     // Linear interpolation.
        MTS_CIP = (9),     // Circular interpolation.
        MTS_VS = (10),    // At start velocity.
        MTS_PMV = (11),    // Point table move.
        MTS_PDW = (12),    // Point table dwell move.
        MTS_PPS = (13),    // Point table pause state.
        MTS_SLV = (14),    // Slave axis move.
        MTS_JOG = (15),    // Jog move.
        MTS_ASTP = (16),    // Abnormal stop.
        MTS_SVONS = (17),    // Servo off stopped.
        MTS_EMGS = (18),    // EMG / SEMG stopped.
        MTS_ALMS = (19),    // Alarm stop.
        MTS_WANS = (20),    // Warning stopped.
        MTS_PELS = (21),    // PEL stopped.
        MTS_MELS = (22),    // MEL stopped.
        MTS_ECES = (23),    // Error counter check level reaches and stopped.
        MTS_SPELS = (24),    // Soft PEL stopped.
        MTS_SMELS = (25),    // Soft MEL stopped.
        MTS_STPOA = (26),    // Stop by others axes.
        MTS_GDCES = (27),    // Gantry deviation error level reaches and stopped.
        MTS_GTM = (28),    // Gantry mode turn on.
        MTS_PAPB = (29),    // Pulsar mode turn on.

        //Following definition for PCI-8254/8
        MTS_MDN = (5),         // Motion done. 0: In motion, 1: Motion done ( It could be abnormal stop)
        MTS_WAIT = (10),        // Axis is in waiting state. ( Wait move trigger )
        MTS_PTB = (11),        // Axis is in point buffer moving. ( When this bit on, MDN and ASTP will be cleared )
        MTS_BLD = (17),        // Axis (Axes) in blending moving
        MTS_PRED = (18),        // Pre-distance event, 1: event arrived. The event will be clear when axis start moving 
        MTS_POSTD = (19),        // Post-distance event. 1: event arrived. The event will be clear when axis start moving
        MTS_GER = (28),        // 1: In geared ( This axis as slave axis and it follow a master specified in axis parameter. )

        //Following definition for PCI-8334/8
        MTS_PSR = (29),
        MTS_GRY = (30),
        //Following definition for EMX-100
        MTS_EZS = (28),
        MTS_HMES = (29),
        MTS_ORGS = (30),
        //define error code
        ERR_NoError = (0),      //No Error
        ERR_OSVersion = (-1),   // Operation System type mismatched
        ERR_OpenDriverFailed = (-2),    // Open device driver failed - Create driver interface failed
        ERR_InsufficientMemory = (-3),  // System memory insufficiently
        ERR_DeviceNotInitial = (-4),    // Cards not be initialized
        ERR_NoDeviceFound = (-5),   // Cards not found(No card in your system)
        ERR_CardIdDuplicate = (-6), // Cards' ID is duplicated. 
        ERR_DeviceAlreadyInitialed = (-7),  // Cards have been initialed
        ERR_InterruptNotEnable = (-8),  // Cards' interrupt events not enable or not be initialized
        ERR_TimeOut = (-9), // Function time out
        ERR_ParametersInvalid = (-10),  // Function input parameters are invalid
        ERR_SetEEPROM = (-11),  // Set data to EEPROM (or nonvolatile memory) failed
        ERR_GetEEPROM = (-12),  // Get data from EEPROM (or nonvolatile memory) failed
        ERR_FunctionNotAvailable = (-13),   // Function is not available in this step, The device is not support this function or Internal process failed
        ERR_FirmwareError = (-14),   // Firmware error, please reboot the system
        ERR_CommandInProcess = (-15),   // Previous command is in process
        ERR_AxisIdDuplicate = (-16),    // Axes' ID is duplicated.
        ERR_ModuleNotFound = (-17),   // Slave module not found.
        ERR_InsufficientModuleNo = (-18),   // System ModuleNo insufficiently
        ERR_HandShakeFailed = (-19),   // HandSake with the DSP out of time.
        ERR_FILE_FORMAT = (-20),    // Config file format error.(cannot be parsed)
        ERR_ParametersReadOnly = (-21), // Function parameters read only.
        ERR_DistantNotEnough = (-22),   // Distant is not enough for motion.
        ERR_FunctionNotEnable = (-23),  // Function is not enabled.
        ERR_ServerAlreadyClose = (-24), // Server already closed.
        ERR_DllNotFound = (-25),    // Related dll is not found, not in correct path.
        ERR_TrimDAC_Channel = (-26),
        ERR_Satellite_Type = (-27),
        ERR_Over_Voltage_Spec = (-28),
        ERR_Over_Current_Spec = (-29),
        ERR_SlaveIsNotAI = (-30),
        ERR_Over_AO_Channel_Scope = (-31),
        ERR_DllFuncFailed = (-32),  // Failed to invoke dll function. Extension Dll version is wrong.
        ERR_FeederAbnormalStop = (-33), //Feeder abnormal stop, External stop or feeding stop
        ERR_AreadyClose = (-34),
        ERR_NullObject = (-35), // Null object is detected
        ERR_PreMoveErr = (-36), // last move is on error stop
        ERR_PreMoveNotDone = (-37), // last move not be done
        ERR_MismatchState = (-38),  // there is a mismatch state
        ERR_Read_ModuleType_Dismatch = (-39),
        ERR_DoubleOverflow = (-40), // Double format parameter is overflow
        ERR_SlaveNumberErr = (-41),
        ERR_SlaveStatusErr = (-42),
        ERR_KernelUpdateError = (-50), //For Kernel update
        ERR_KernelGeneralFunc = (-51),  //for general functions
        ERR_Win32Error = (-1000), // No such INT number, or WIN32_API error, contact with ADLINK's FAE staff.
        ERR_DspStart = (-2000), // The base for DSP error


        //For EtherCAT, PCIe-8334_8 ( -1001 ~ -1999 )
        ERR_NoENIFile = (-1001),
        ERR_TimeOut_SetVoltageEnable = (-1002),
        ERR_TimeOut_SetReadyToSwitch = (-1003),
        ERR_TimeOut_SetShutdown = (-1004),
        ERR_TimeOut_SetSwitchOn = (-1005),
        ERR_TimeOut_SetOperationEnable = (-1006),
        ERR_RegistryPath = (-1007),
        ERR_MasterNotOPState = (-1008),
        ERR_SlaveNotOPState = (-1009),
        ERR_SlaveTotalAxisNumber = (-1010), // The scanned number of EtherCAT slaves' axes exceeds the number of max.
        ERR_MissESIFileOrMissENIPath = (-1011), // No ESI file or ESI file path miss result .
        ERR_MissConfig_1_Xml = (-1012),  // No config_1 xml.
        ERR_MissConfig_1_Xml_fail = (-1013),  // No config_1 xml.



        EC_NO_ERROR = (0),
        EC_INIT_MASTER_ERR = (-4001),
        EC_GET_SLV_NUM_ERR = (-4011),
        EC_CONFIG_MASTER_ERR = (-4012),
        EC_BUSCONFIG_MISMATCH = (-4013),
        EC_CONFIGDATA_READ_ERR = (-4014),
        EC_ENI_NO_SAFEOP_OP_SUPPORT = (-4015),
        EC_CONFIG_DC_ERR = (-4021),
        EC_DCM_MODE_NO_SUPPORT = (-4022),
        EC_CONFIG_DCM_FEATURE_DISABLED = (-4023),
        EC_CONFIG_DCM_ERR = (-4024),
        EC_REG_CLIENT_ERR = (-4031),
        EC_SET_INIT_STATE_ERR = (-4041),
        EC_SET_PREOP_STATE_ERR = (-4042),
        EC_SET_SAVEOP_STATE_ERR = (-4043),
        EC_SET_OP_STATE_ERR = (-4044),
        EC_DE_INIT_MASTER_ERR = (-4051),
        EC_ENI_FOPEN_ERR = (-4061),
        EC_ENI_FREAD_ERR = (-4062),
        EC_GEN_EBI_BUSSCAN_ERR = (-4063),
        EC_GEN_EBI_FOPEN_ERR = (-4064),
        EC_GET_EBI_FOPEN_ERR = (-4065),
        EC_GET_EBI_FREAD_ERR = (-4066),
        EC_WRITE_DO_OUT_ERR = (-4071),
        EC_READ_DI_INP_ERR = (-4072),
        EC_CONNECT_SLAVES_ERR = (-4073),
        EC_WRONG_PORT_NO = (-4081),
        EC_GET_SLAVE_INFO_ERR = (-4091),
        EC_COE_SDO_UPLOAD_ERR = (-4101),
        EC_WRONG_SLAVE_NO = (-4201),
        EC_WRONG_MODULE_NO = (-4202),
        EC_WRONG_AI_CHANNEL_NO = (-4203),
        EC_WRONG_AO_CHANNEL_NO = (-4204),
        EC_COE_SDO_DOWNLOAD_ERR = (-4205),

        EC_COE_OD_INIT_ERR = (-4301),
        EC_COE_GET_OD_NUM_ERR = (-4302),
        EC_COE_GET_OD_NUM_LAST = (-4303),
        EC_COE_GET_OD_DESC_ERR = (-4304),
        EC_COE_GET_OD_DESC_ENTRY_ERR = (-4305),
        EC_COE_GET_OD_STATUS_PEND = (-4306),
        EC_DUPLICATE_SLAVE_ID_ERR = (-4503),
        EC_GET_SLAVE_REGISTER_ERR = (-4504),
        EC_SET_SLAVE_REGISTER_ERR = (-4505),


        // Motion IO status bit value define.
        MIO_ALM_V = (0x1),   // Servo alarm.
        MIO_PEL_V = (0x2),   // Positive end limit.
        MIO_MEL_V = (0x4),   // Negative end limit.
        MIO_ORG_V = (0x8),   // ORG (Home).
        MIO_EMG_V = (0x10),  // Emergency stop.
        MIO_EZ_V = (0x20),  // EZ.
        MIO_INP_V = (0x40),  // In position.
        MIO_SVON_V = (0x80),  // Servo on signal.
        MIO_RDY_V = (0x100), // Ready.
        MIO_WARN_V = (0x200), // Warning.
        MIO_ZSP_V = (0x400), // Zero speed.
        MIO_SPEL_V = (0x800), // Soft positive end limit.
        MIO_SMEL_V = (0x1000),  // Soft negative end limit.
        MIO_TLC_V = (0x2000),  // Torque is limited by torque limit value.
        MIO_ABSL_V = (0x4000),  // Absolute position lost.
        MIO_STA_V = (0x8000),  // External start signal.
        MIO_PSD_V = (0x10000), // Positive slow down signal.
        MIO_MSD_V = (0x20000), // Negative slow down signal.

        // Motion status bit define.
        MTS_CSTP_V = (0x1),     // Command stop signal. 
        MTS_VM_V = (0x2),     // At maximum velocity.
        MTS_ACC_V = (0x4),     // In acceleration.
        MTS_DEC_V = (0x8),     // In deceleration.
        MTS_DIR_V = (0x10),    // (Last)Moving direction.
        MTS_NSTP_V = (0x20),    // Normal stop(Motion done).
        MTS_HMV_V = (0x40),    // In home operation.
        MTS_SMV_V = (0x80),    // Single axis move( relative, absolute, velocity move).
        MTS_LIP_V = (0x100),   // Linear interpolation.
        MTS_CIP_V = (0x200),   // Circular interpolation.
        MTS_VS_V = (0x400),   // At start velocity.
        MTS_PMV_V = (0x800),   // Point table move.
        MTS_PDW_V = (0x1000),    // Point table dwell move.
        MTS_PPS_V = (0x2000),    // Point table pause state.
        MTS_SLV_V = (0x4000),    // Slave axis move.
        MTS_JOG_V = (0x8000),    // Jog move.
        MTS_ASTP_V = (0x10000),   // Abnormal stop.
        MTS_SVONS_V = (0x20000),   // Servo off stopped.
        MTS_EMGS_V = (0x40000),   // EMG / SEMG stopped.
        MTS_ALMS_V = (0x80000),   // Alarm stop.
        MTS_WANS_V = (0x100000),  // Warning stopped.
        MTS_PELS_V = (0x200000),  // PEL stopped.
        MTS_MELS_V = (0x400000),  // MEL stopped.
        MTS_ECES_V = (0x800000),  // Error counter check level reaches and stopped.
        MTS_SPELS_V = (0x1000000), // Soft PEL stopped.
        MTS_SMELS_V = (0x2000000), // Soft MEL stopped.
        MTS_STPOA_V = (0x4000000), // Stop by others axes.
        MTS_GDCES_V = (0x8000000), // Gantry deviation error level reaches and stopped.
        MTS_GTM_V = (0x10000000),  // Gantry mode turn on.
        MTS_PAPB_V = (0x20000000), // Pulsar mode turn on.

        // PointTable, option
        PT_OPT_ABS = (0x00000000),    // move, absolute
        PT_OPT_REL = (0x00000001),   // move, relative
        PT_OPT_LINEAR = (0x00000000),  // move, linear
        PT_OPT_ARC = (0x00000004),    // move, arc
        PT_OPT_FC_CSTP = (0x00000000),   // signal, command stop (finish condition)
        PT_OPT_FC_INP = (0x00000010),    // signal, in position
        PT_OPT_LAST_POS = (0x00000020),    // last point index
        PT_OPT_DWELL = (0x00000040),    // dwell
        PT_OPT_RAPID = (0x00000080),    // rapid positioning
        PT_OPT_NOARC = (0x00010000),    // do not add arc
        PT_OPT_SCUVE = (0x00000002),    // s-curve





    }
}
