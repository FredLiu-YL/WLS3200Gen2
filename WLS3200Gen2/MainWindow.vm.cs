﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Account;
using YuanliCore.Interface;
using YuanliCore.Model.LoadPort;
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
        private AxisConfig tableXConfig, tableYConfig, tableRConfig, robotAxisConfig;
        private double tablePosX, tablePosY, tablePosR;

        private ObservableCollection<CassetteUC> cassetteUC = new ObservableCollection<CassetteUC>();
        private WriteableBitmap mainImage;
        private DigitalInput[] digitalInputs;
        private DigitalOutput[] digitalOutputs;


        private bool isRefresh;


        public ObservableCollection<CassetteUC> CassetteUC
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
        public AxisConfig TableRConfig { get => tableRConfig; set => SetValue(ref tableRConfig, value); }
        public AxisConfig RobotAxisConfig { get => robotAxisConfig; set => SetValue(ref robotAxisConfig, value); }
        public DigitalInput[] DigitalInputs { get => digitalInputs; set => SetValue(ref digitalInputs, value); }
        public DigitalOutput[] DigitalOutputs { get => digitalOutputs; set => SetValue(ref digitalOutputs, value); }


        //刷新座標
        public double TablePosX { get => tablePosX; set => SetValue(ref tablePosX, value); }
        public double TablePosY { get => tablePosY; set => SetValue(ref tablePosY, value); }
        public double TablePosR { get => tablePosR; set => SetValue(ref tablePosR, value); }

        public string Version { get => version; set => SetValue(ref version, value); }
        public UserAccount Account { get => account; set => SetValue(ref account, value); }

        public WriteableBitmap MainImage { get => mainImage; set => SetValue(ref mainImage, value); }
        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeAction { get; set; }


        public ICommand WindowLoadedCommand => new RelayCommand(async () =>
        {
            try
            {


                //
                //大部分都會在這裡初始化  有些因為寫法問題必須移動到MainViewModel.cs
                //
                LogMessage = "Initial ．．．";
                machine.Initial();
                LogMessage = "Home ．．．";
                machine.Home();
                TableX = machine.MicroDetection.AxisX;
                TableY = machine.MicroDetection.AxisY;
                TableR = machine.MicroDetection.AxisR;
                RobotAxis = machine.Feeder.RobotAxis;

                DigitalInputs = machine.GetInputs();
                DigitalOutputs = machine.GetOutputs();

                TableXConfig = machineSetting.TableXConfig;
                TableYConfig = machineSetting.TableYConfig;
                TableRConfig = machineSetting.TableRConfig;
                RobotAxisConfig = machineSetting.RobotAxisConfig;





                InformationUCVisibility = Visibility.Visible;
                WorkholderUCVisibility = Visibility.Collapsed;
                TabControlSelectedIndex = 0;

                for (int i = 0; i < 5; i++)
                    LoadPort1Wafers.Add(new WaferUIData { WaferStates = ExistStates.Exist, SN = (i + 1).ToString() });
                for (int i = 5; i < 10; i++)
                    LoadPort1Wafers.Add(new WaferUIData { WaferStates = ExistStates.Select, SN = (i + 1).ToString() });
                for (int i = 10; i < 25; i++)
                    LoadPort1Wafers.Add(new WaferUIData { WaferStates = ExistStates.None, SN = (i + 1).ToString() });

                LogMessage = "Equipment Ready．．．";
                isRefresh = true;
                taskRefresh1 = Task.Run(RefreshPos);

                SampleFind = SampleFindAction;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand WindowClosingCommand => new RelayCommand(() =>
       {
           try
           {
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
                    await Task.Delay(300);
                }

            }
            catch (Exception ex)
            {

                // throw ex;
            }

        }
    }
}
