using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WLS3200Gen2.Model;
using YuanliCore.Data;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private int toolLoadPort1Index;
        private bool isMicroVaccum8, isMicroVaccum12, isMacroVaccum8, isMacroVaccum12, isAlignerVaccum12;
        private ObservableCollection<string> toolLoadPort1ComboBox = new ObservableCollection<string>();
        //public ObservableCollection<BincodeInfo> BincodeList { get => bincodeList; set => SetValue(ref bincodeList, value); }
        public ObservableCollection<string> ToolLoadPort1ComboBox { get => toolLoadPort1ComboBox; set => SetValue(ref toolLoadPort1ComboBox, value); }
        public int ToolLoadPort1Index { get => toolLoadPort1Index; set => SetValue(ref toolLoadPort1Index, value); }
        public bool IsMicroVaccum8 { get => isMicroVaccum8; set => SetValue(ref isMicroVaccum8, value); }
        public bool IsMicroVaccum12 { get => isMicroVaccum12; set => SetValue(ref isMicroVaccum12, value); }
        public bool IsMacroVaccum8 { get => isMacroVaccum8; set => SetValue(ref isMacroVaccum8, value); }
        public bool IsMacroVaccum12 { get => isMacroVaccum12; set => SetValue(ref isMacroVaccum12, value); }
        public bool IsAlignerVaccum12 { get => isAlignerVaccum12; set => SetValue(ref isAlignerVaccum12, value); }


        public ICommand Home => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Home");
                IsCanWorkEFEMTrans = false;
                await machine.Feeder.Robot.Home();
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand PickWaferFromLoadPort => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From LoadPort");
                //是否執行取片訊息
                string mesage = "Pick Cassette '" + (ToolLoadPort1Index + 1).ToString() + "' Wafer From Cassette1 ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            Wafer currentWafer = new Wafer(ToolLoadPort1Index + 1);
                            await machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette1);
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PickWaferFromAligner => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From Aligner");
                //是否執行取片訊息
                string mesage = "Pick Wafer From Aligner ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.Feeder.AlignerL.Home();
                            await machine.Feeder.AlignerL.ReleaseWafer();
                            await machine.Feeder.WaferAlignerToStandBy();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PickWaferFromMicro => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From Micro");
                //是否執行取片訊息
                string mesage = "Pick Wafer From Micro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                            machine.MicroDetection.TableVacuum.Off();
                            await machine.Feeder.WaferMicroToStandBy();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PickWaferFromMacro => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From Macro");
                //是否執行取片訊息
                string mesage = "Pick Wafer From Macro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.Feeder.WaferMacroToStandBy();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });


        public ICommand PutWaferToLoadPort => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To LoadPort");
                //是否執行放片訊息
                string mesage = "Put Wafer To LoadPort '" + (ToolLoadPort1Index + 1).ToString() + "' Cassette1 ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認LoadPort沒有片子
                        RecipeLastArmStation = Model.ArmStation.Cassette1;
                        Wafer currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                        await machine.Feeder.UnLoadWaferToCassette(currentWafer, true);
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PutWaferToAligner => new RelayCommand(async () =>
        {
            try
            {
                //是否執行放片訊息
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To Aligner");
                string mesage = "Put Wafer To Aligner ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (await EFEMTransWaferBeforeCheckAlignerHaveWafer())
                        {
                            mesage = "Aligner Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Align;
                            machine.Feeder.AlignerL.Home().Wait();
                            await machine.Feeder.WaferStandByToAligner();
                            machine.Feeder.AlignerL.FixWafer().Wait();
                            if (machine.Feeder.AlignerL.IsLockOK == false)
                            {
                                result = MessageBox.Show("WaferToAligner Aligner真空異常!!", "Info", MessageBoxButton.YesNo);
                            }
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        /// <summary>
        /// 確認手臂有無片
        /// </summary>
        private async Task<bool> EFEMTransWaferBeforeCheckAlignerHaveWafer()
        {
            try
            {
                Task alignerLock = machine.Feeder.AlignerL.FixWafer();
                await alignerLock;
                await Task.Delay(1000);
                bool isHave = false;
                if (machine.Feeder.AlignerL.IsLockOK)
                {
                    isHave = true;
                }
                else
                {
                    await machine.Feeder.AlignerL.ReleaseWafer();
                }
                await Task.Delay(500);
                return isHave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ICommand PutWaferToMicro => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To Micro");
                //是否執行放片訊息
                string mesage = "Put Wafer To Micro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (await EFEMTransWaferBeforeCheckMicroHaveWafer())
                        {
                            mesage = "Micro Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                            machine.MicroDetection.TableVacuum.On();
                            Wafer station = new Wafer(1);
                            await machine.Feeder.LoadToMicroAsync(station);
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        private async Task<bool> EFEMTransWaferBeforeCheckMicroHaveWafer()
        {
            try
            {
                machine.MicroDetection.TableVacuum.On();
                await Task.Delay(1000);
                bool isHave = false;
                if (machine.MicroDetection.IsTableVacuum.IsSignal)
                {
                    isHave = true;
                }
                else
                {
                    machine.MicroDetection.TableVacuum.Off();
                }
                await Task.Delay(500);
                return isHave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand PutWaferToMacro => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To Macro");
                //是否執行放片訊息
                string mesage = "Put Wafer To Macro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (await EFEMTransWaferBeforeCheckMacroHaveWafer())
                        {
                            mesage = "Micro Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Macro;
                            machine.Feeder.Macro.FixWafer();
                            await machine.Feeder.WaferStandByToMacroAsync();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        private async Task<bool> EFEMTransWaferBeforeCheckMacroHaveWafer()
        {
            try
            {
                machine.Feeder.Macro.FixWafer();
                await Task.Delay(1000);
                bool isHave = false;
                if (machine.Feeder.Macro.IsLockOK)
                {
                    isHave = true;
                }
                else
                {
                    machine.Feeder.Macro.ReleaseWafer();
                }
                await Task.Delay(500);
                return isHave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand RobotMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {

                    case "LoadPort1":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
                        break;
                    case "LoadPort2":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);
                        break;
                    case "Aligner1":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                        break;
                    case "Micro":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                        break;
                    case "Macro":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand MicroMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "X":
                        await machine.MicroDetection.AxisX.MoveToAsync(machineSetting.TableWaferCatchPosition.X);
                        break;
                    case "Y":
                        await machine.MicroDetection.AxisY.MoveToAsync(machineSetting.TableWaferCatchPosition.Y);
                        break;
                    case "Z":
                        await machine.MicroDetection.AxisZ.MoveToAsync(machineSetting.TableWaferCatchPositionZ);
                        break;
                    case "R":
                        await machine.MicroDetection.AxisR.MoveToAsync(machineSetting.TableWaferCatchPositionR);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
    }
}
