using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
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
        public ICommand Home => new RelayCommand(async () =>
        {
            try
            {
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
                Wafer currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                await machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette1);
            }
            catch (Exception ex)
            {
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
                IsCanWorkEFEMTrans = false;
                await machine.Feeder.AlignerL.Home();
                await machine.Feeder.AlignerL.ReleaseWafer();
                await machine.Feeder.WaferAlignerToStandBy();
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
        public ICommand PickWaferFromMicro => new RelayCommand(async () =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                machine.MicroDetection.TableVacuum.Off();
                await machine.Feeder.WaferMicroToStandBy();
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
        public ICommand PickWaferFromMacro => new RelayCommand(async () =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                await machine.Feeder.WaferMacroToStandBy();
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


        public ICommand PutWaferToLoadPort => new RelayCommand(async () =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                RecipeLastArmStation = Model.ArmStation.Cassette1;
                Wafer currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                await machine.Feeder.UnLoadWaferToCassette(currentWafer, true);
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
        public ICommand PutWaferToAligner => new RelayCommand(async () =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                RecipeLastArmStation = Model.ArmStation.Align;
                machine.Feeder.AlignerL.Home().Wait();
                await machine.Feeder.WaferStandByToAligner();
                machine.Feeder.AlignerL.FixWafer().Wait();
                if (machine.Feeder.AlignerL.IsLockOK == false)
                {
                    throw new Exception("EFEMTransCommand 異常!WaferToAligner Aligner真空異常!!");
                }
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
        public ICommand PutWaferToMicro => new RelayCommand(async () =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                await machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                machine.MicroDetection.TableVacuum.On();
                WaferProcessStatus station = new WaferProcessStatus();
                await machine.Feeder.LoadToMicroAsync(station);
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
        public ICommand PutWaferToMacro => new RelayCommand(async () =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                RecipeLastArmStation = Model.ArmStation.Macro;
                machine.Feeder.Macro.FixWafer();
                await machine.Feeder.WaferStandByToMacroAsync();
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
        public ICommand VaccumOn => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(async () =>
                {
                    machine.Feeder.Robot.FixWafer();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand VaccumOff => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(async () =>
                {
                    machine.Feeder.Robot.ReleaseWafer();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
    }
}
