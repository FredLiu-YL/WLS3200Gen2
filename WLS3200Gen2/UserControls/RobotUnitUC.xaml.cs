using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Module;

namespace YuanliCore.Model.UserControls
{
    /// <summary>
    /// RobotUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class RobotUnitUC : UserControl
    {
        public RobotUnitUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty FeederProperty = DependencyProperty.Register(nameof(Feeder), typeof(Feeder), typeof(RobotUnitUC),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Feeder Feeder
        {
            get => (Feeder)GetValue(FeederProperty);
            set => SetValue(FeederProperty, value);
        }
        public ICommand Home => new RelayCommand(async () =>
        {
            try
            {
                await Feeder.Robot.Home();
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
                await Feeder.WaferLoadPortToStandBy(0, ArmStation.Cassette1);
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
                await Feeder.WaferAlignerToStandBy();
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
                await Feeder.WaferMicroToStandBy();
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
                await Feeder.WaferMacroToStandBy();
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
                await Feeder.WaferStandByToLoadPort(0, ArmStation.Cassette1);
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
                await Feeder.WaferStandByToAligner();
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
                await Feeder.WaferStandByToMicro();
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
                await Feeder.WaferStandByToMacro();
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
