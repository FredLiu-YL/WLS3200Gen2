using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.Module;
using WLS3200Gen2.UserControls;
using YuanliCore.Account;
using YuanliCore.Data;
using YuanliCore.Interface;
using YuanliCore.Machine.Base;
using YuanliCore.Model.Interface;
using YuanliCore.Model.UserControls;
using YuanliCore.Motion;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private Task taskRefresh1 = Task.CompletedTask;
        private UserAccount account;
        private string version;
        private Axis tableX, tableY, tableR;
        private Axis robotAxis;
        private AxisConfig tableXConfig, tableYConfig, tableZConfig, tableRConfig, robotAxisConfig;
        private double tablePosX, tablePosY, tablePosR;
        private StackLight stackLight;
        private ObservableCollection<BincodeInfo> bincodeList = new ObservableCollection<BincodeInfo>();

        private ObservableCollection<CassetteUnitUC> cassetteUC = new ObservableCollection<CassetteUnitUC>();
        private WriteableBitmap mainImage, mapImage;
        private DigitalInput[] digitalInputs;
        private DigitalOutput[] digitalOutputs;
        private IDisposable camlive;
        private System.Windows.Point mapmousePixcel;
        //WLS3200的文件都放在這 (Recipe、 Log setting)
        private string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WLS3200";
        private bool isRefresh, isInitialComplete, isWaferInSystem;
        private LoadPortQuantity loadportQuantity;

        public ObservableCollection<CassetteUnitUC> CassetteUC
        {
            get => cassetteUC;
            set { SetValue(ref cassetteUC, value); }
        }
        public ICommand AddButtonAction { get; set; }

        public Axis TableX { get => tableX; set => SetValue(ref tableX, value); }
        public Axis TableY { get => tableY; set => SetValue(ref tableY, value); }
        public Axis TableR { get => tableR; set => SetValue(ref tableR, value); }
        public Axis RobotAxis { get => robotAxis; set => SetValue(ref robotAxis, value); }
        public AxisConfig TableXConfig { get => tableXConfig; set => SetValue(ref tableXConfig, value); }
        public AxisConfig TableYConfig { get => tableYConfig; set => SetValue(ref tableYConfig, value); }
        public AxisConfig TableZConfig { get => tableZConfig; set => SetValue(ref tableZConfig, value); }
        public AxisConfig TableRConfig { get => tableRConfig; set => SetValue(ref tableRConfig, value); }
        public AxisConfig RobotAxisConfig { get => robotAxisConfig; set => SetValue(ref robotAxisConfig, value); }
        public DigitalInput[] DigitalInputs { get => digitalInputs; set => SetValue(ref digitalInputs, value); }
        public DigitalOutput[] DigitalOutputs { get => digitalOutputs; set => SetValue(ref digitalOutputs, value); }
        public ObservableCollection<BincodeInfo> BincodeList { get => bincodeList; set => SetValue(ref bincodeList, value); }

        //刷新座標
        public double TablePosX { get => tablePosX; set => SetValue(ref tablePosX, value); }
        public double TablePosY { get => tablePosY; set => SetValue(ref tablePosY, value); }
        public double TablePosR { get => tablePosR; set => SetValue(ref tablePosR, value); }

        public string Version { get => version; set => SetValue(ref version, value); }
        public UserAccount Account { get => account; set => SetValue(ref account, value); }

        public WriteableBitmap MainImage { get => mainImage; set => SetValue(ref mainImage, value); }
        public WriteableBitmap MapImage { get => mapImage; set => SetValue(ref mapImage, value); }

        public System.Windows.Point MapMousePixcel { get => mapmousePixcel; set => SetValue(ref mapmousePixcel, value); }

        public LoadPortQuantity LoadportQuantity { get => loadportQuantity; set => SetValue(ref loadportQuantity, value); }

        private ObservableCollection<RobotAddress> customers = new ObservableCollection<RobotAddress>();
        public ObservableCollection<RobotAddress> Customers { get => customers; set => SetValue(ref customers, value); }
        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeAction { get; set; }

        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddMapShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearMapShapeAction { get; set; }

        public ICommand WindowLoadedCommand => new RelayCommand(async () =>
        {
            try
            {
                Assembly thisAssem = typeof(MainViewModel).Assembly;
                AssemblyName thisAssemName = thisAssem.GetName();

                Version ver = thisAssemName.Version;
                Version = $"WLS3100/3200  {ver}";
                //
                //大部分都會在這裡初始化  有些因為寫法問題必須移動到MainViewModel.cs
                //machineSetting 的讀取放在MainViewModel.cs
                //
                LogMessage = "Initial ．．．";
                machine.Initial();

                //加入 LOG功能到各模組 一定要放在  machine.Initial()後面
                machine.MicroDetection.WriteLog += WriteLog;
                machine.Feeder.WriteLog += WriteLog;
                await Task.Delay(10);//顯示UI 
                isWaferInSystem = true;
                isWaferInSystem = await machine.BeforeHomeCheck();
                MessageBoxResult result = MessageBoxResult.Yes;

                if (isWaferInSystem)
                {
                    result = MessageBox.Show("Wafer In System!! StartHome??", "StartHome", MessageBoxButton.YesNo);
                }

                if (result == MessageBoxResult.Yes)
                {
                    await machine.Home();
                }
                else
                {
                    //直接關掉程式!
                }

                BincodeInfo bincode1 = new BincodeInfo
                {
                    Code = "B01",
                    Describe = "TES_TEST123",
                    Color = Brushes.Blue
                };
                BincodeInfo bincode2 = new BincodeInfo
                {
                    Code = "B02",
                    Describe = "ABc_TEST123",
                    Color = Brushes.GreenYellow
                };
                BincodeList.Add(bincode1);
                BincodeList.Add(bincode2);
                BincodeList.Add(new BincodeInfo());


                TableX = machine.MicroDetection.AxisX;
                TableY = machine.MicroDetection.AxisY;
                TableR = machine.MicroDetection.AxisR;
                RobotAxis = machine.Feeder.RobotAxis;

                Microscope = machine.MicroDetection.Microscope;

                DigitalInputs = machine.GetInputs();
                DigitalOutputs = machine.GetOutputs();

                TableXConfig = machineSetting.TableXConfig;
                TableYConfig = machineSetting.TableYConfig;
                TableZConfig = machineSetting.TableZConfig;
                TableRConfig = machineSetting.TableRConfig;
                RobotAxisConfig = machineSetting.RobotAxisConfig;



                InformationUCVisibility = Visibility.Visible;
                WorkholderUCVisibility = Visibility.Collapsed;
                TabControlSelectedIndex = 0;
    

                WriteLog("Equipment Ready．．．");

                isRefresh = true;
                taskRefresh1 = Task.Run(RefreshPos);

                SampleFind = SampleFindAction;


                CameraLive();


               
                isInitialComplete = true;

                //載入機台設定LoadPort 數量  給UI作卡控
                LoadportQuantity = machineSetting.LoadPortCount;





                if (!File.Exists("MAP1.bmp")) throw new Exception("模擬情境下需要放一張圖片到執行檔資料夾 取名MAP1.bmp");
                // 讀取BMP檔案
                BitmapImage bitmap = new BitmapImage(new Uri("MAP1.bmp", UriKind.RelativeOrAbsolute));

                MapImage = new WriteableBitmap(bitmap);
                //MapImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);

                Customers = new ObservableCollection<RobotAddress>()
            {
                new RobotAddress() { Name = "LoadPort1 Step1", Address = "110" },
                new RobotAddress() { Name = "LoadPort1 Step2", Address = "111" },
                new RobotAddress() { Name = "LoadPort1 Step3", Address = "112" },
                new RobotAddress() { Name = "LoadPort1 Step4", Address = "113" },
                new RobotAddress() { Name = "LoadPort1 Step5", Address = "114" },
                new RobotAddress() { Name = "LoadPort1 Step1", Address = "110" },
                new RobotAddress() { Name = "LoadPort1 Step2", Address = "111" },
                new RobotAddress() { Name = "LoadPort1 Step3", Address = "112" },
                new RobotAddress() { Name = "LoadPort1 Step4", Address = "113" },
                new RobotAddress() { Name = "LoadPort1 Step5", Address = "114" },
            };


                Customers.Add(new RobotAddress() { Name = "LoadPort1 Step1", Address = "110" });
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand WindowClosingCommand => new RelayCommand<CancelEventArgs>(async e =>
        {
            try
            {
                // 取消預設的視窗關閉行為
                e.Cancel = true;
                var result = MessageBox.Show("是否關閉程式?", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    // 如果使用者選擇 "否"，則取消視窗關閉
                    e.Cancel = true;
                    return;

                }



                // 如果使用者選擇 "是"，則允許視窗關閉
                e.Cancel = false;



                isRefresh = false;


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });

        private void SwitchStates(MachineStates states)
        {
            Machinestatus = states;
            if(stackLight!=null)
                stackLight.SwitchStates(states);
        }

        //Security進入會執行
        private void LoadSecurityPage()
        {



            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right)
            {
                IsMainSecurityPageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }

            WriteLog("Enter the SecurityPage");
        }


        //離開Security頁面會執行
        private void UnLoadSecurityPage()
        {
            if (isInitialComplete)
            {
                //軸 修改參數存檔
                machineSetting.Save(machineSettingPath);
            }

        }

        private async Task RefreshPos()
        {
            try
            {

                while (isRefresh)
                {

                    //var pos = atfMachine.Table_Module.GetPostion();
                    TablePosX = machine.MicroDetection.AxisX.Position;
                    TablePosY = machine.MicroDetection.AxisY.Position;
                    TablePosR = machine.MicroDetection.AxisR.Position;
                    //if (atfMachine.AFModule.AFSystem != null)
                    //    PositionZ = (int)atfMachine.AFModule.AFSystem.AxisZPosition;
                    

                    //刷IO 得到EMO 軸停止


                    if (machine.Feeder.LoadPortL != null)
                    {
                        LoadPortStatus loadPortStatus = new LoadPortStatus();
                        loadPortStatus = await machine.Feeder.LoadPortL.GetStatus();
                        LoadPortUIShow.ErrorStatus = loadPortStatus.ErrorStatus;
                        LoadPortUIShow.DeviceStatus = loadPortStatus.DeviceStatus;
                        LoadPortUIShow.ErrorCode = loadPortStatus.ErrorCode;
                        LoadPortUIShow.IsCassettePutOK = loadPortStatus.IsCassettePutOK;
                        LoadPortUIShow.IsClamp = loadPortStatus.IsClamp;
                        LoadPortUIShow.IsSwitchDoor = loadPortStatus.IsSwitchDoor;
                        LoadPortUIShow.IsVaccum = loadPortStatus.IsVaccum;
                        LoadPortUIShow.IsDoorOpen = loadPortStatus.IsDoorOpen;
                        LoadPortUIShow.IsSensorCheckDoorOpen = loadPortStatus.IsSensorCheckDoorOpen;
                        LoadPortUIShow.IsDock = loadPortStatus.IsDock;
                    }
                    if (machine.Feeder.AlignerL != null)
                    {
                        AlignerStatus alignerStatus = new AlignerStatus();
                        alignerStatus = await machine.Feeder.AlignerL.GetStatus();
                        AlignerUIShow.DeviceStatus = alignerStatus.DeviceStatus;
                        AlignerUIShow.ErrorCode = alignerStatus.ErrorCode;
                        AlignerUIShow.NotchStatus = alignerStatus.NotchStatus;
                        AlignerUIShow.IsWafer = alignerStatus.IsWafer;
                        AlignerUIShow.IsOrg = alignerStatus.IsOrg;
                        AlignerUIShow.IsVaccum = alignerStatus.IsVaccum;
                    }
                    if (machine.Feeder.Robot != null)
                    {
                        RobotStatus robotStatus = new RobotStatus();
                        robotStatus = await machine.Feeder.Robot.GetStatus();
                        RobotStaus.Mode = robotStatus.Mode;
                        RobotStaus.IsStopSignal = robotStatus.IsStopSignal;
                        RobotStaus.IsEStopSignal = robotStatus.IsEStopSignal;
                        RobotStaus.IsCommandDoneSignal = robotStatus.IsCommandDoneSignal;
                        RobotStaus.IsMovDoneSignal = robotStatus.IsMovDoneSignal;
                        RobotStaus.IsRunning = robotStatus.IsRunning;
                        RobotStaus.ErrorCode = robotStatus.ErrorCode;
                        RobotStaus.ErrorXYZWRC = Convert.ToInt32("" + robotStatus.ErrorX + robotStatus.ErrorY + robotStatus.ErrorZ + robotStatus.ErrorW + robotStatus.ErrorR + robotStatus.ErrorC);

                        RobotStaus.IsHavePiece = await machine.Feeder.Robot.IsHavePiece();
                        RobotStaus.IsLockOK = await machine.Feeder.Robot.IsLockOK();
                        //RobotUIIShow
                    }

                    if (machine.MicroDetection.Microscope != null)
                    {
                        double nowPos = 0;
                        nowPos = await machine.MicroDetection.Microscope.GetZPosition();
                        BXFMUIShow.FocusZ = Convert.ToInt32(nowPos);
                        BXFMUIShow.ApertureValue = machine.MicroDetection.Microscope.ApertureValue;
                        BXFMUIShow.LightValue = machine.MicroDetection.Microscope.LightValue;
                    }

                    await Task.Delay(300);
                }

            }
            catch (Exception ex)
            {

           
                SwitchStates(MachineStates.Emergency);
                throw ex;
            }

        }


        private void CameraLive()
        {
            var camera = machine.MicroDetection.Camera;
            var dis = machine.MicroDetection.Camera.Grab();


            try
            {


                MainImage = new WriteableBitmap(camera.Width, camera.Height, 96, 96, camera.PixelFormat, null);


                camlive = camera.Frames.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
                             .ObserveOn(DispatcherScheduler.Current)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
                             .Subscribe(frame =>
                             {

                                 var a = System.Threading.Thread.CurrentThread.ManagedThreadId;
                                 if (frame != null) MainImage.WritePixels(frame);
                                 //  Image = new WriteableBitmap(frame.Width, frame.Height, frame.dP, double dpiY, PixelFormat pixelFormat, BitmapPalette palette);
                             });

            }
            catch (Exception ex)
            {


            }
        }
    }


    /// <summary>
    /// 如果是雙Port 就把loadport有關功能打開
    /// </summary>
    public class IsLoadPortEnableConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((LoadPortQuantity)value)
            {
                case LoadPortQuantity.Single:
                    return false;

                case LoadPortQuantity.Pair:
                    return true;

                default:
                    return false;

            }



        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
