using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WLS3200Gen2.Model;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        public ICommand Home => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(async () =>
                {
                    machine.Feeder.Robot.Home();
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

        public ICommand PickWaferFromLoadPort => new RelayCommand(async () =>
        {
            try
            {
                await machine.Feeder.WaferLoadPortToStandBy(0, ArmStation.Cassette1);
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
                await machine.Feeder.WaferAlignerToStandBy();
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
                await machine.Feeder.WaferMicroToStandBy();
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
                await machine.Feeder.WaferMacroToStandBy();
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
                await machine.Feeder.WaferStandByToLoadPort(0, ArmStation.Cassette1);
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
                await machine.Feeder.WaferStandByToAligner();
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
                await machine.Feeder.WaferStandByToMicro();
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
                await machine.Feeder.WaferStandByToMacro();
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

                    case "LoadPort":
                        await machine.Feeder.RobotAxis.MoveToAsync(machine.Feeder.Setting.LoadPortPos);
                        break;
                    case "Aligner":
                        await machine.Feeder.RobotAxis.MoveToAsync(machine.Feeder.Setting.AlignPos);
                        break;
                    case "Micro":
                        await machine.Feeder.RobotAxis.MoveToAsync(machine.Feeder.Setting.MicroPos);
                        break;
                    case "Macro":
                        await machine.Feeder.RobotAxis.MoveToAsync(machine.Feeder.Setting.MacroPos);
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
