using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Module;


namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// MacroUC.xaml 的互動邏輯
    /// </summary>
    public partial class MacroUnitUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty MacroProperty = DependencyProperty.Register(nameof(Macro), typeof(IMacro), typeof(MacroUnitUC),
                                                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public MacroUnitUC()
        {
            InitializeComponent();
        }

        public IMacro Macro
        {
            get => (IMacro)GetValue(MacroProperty);
            set => SetValue(MacroProperty, value);
        }


        public ICommand AllHome => new RelayCommand(async () =>
        {
            try
            {
                await Macro.HomeAllRing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand GoInnerRingCheckPos => new RelayCommand(async () =>
        {
            try
            {
                await Macro.GoInnerRingCheckPos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand MacroInnerContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {

                    case "X+":
                        Macro.InnerRingPitchX_Move(true);
                        break;
                    case "X-":
                        Macro.InnerRingPitchX_Move(false);
                        break;
                    case "Y+":
                        Macro.InnerRingRollY_Move(true);
                        break;
                    case "Y-":
                        Macro.InnerRingRollY_Move(false);
                        break;
                    case "T+":
                        Macro.InnerRingYawT_Move(true);
                        break;
                    case "T-":
                        Macro.InnerRingYawT_Move(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand MacroInnerMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "X+":
                        Macro.InnerRingPitchX_Stop();
                        break;
                    case "X-":
                        Macro.InnerRingPitchX_Stop();
                        break;
                    case "Y+":
                        Macro.InnerRingRollY_Stop();
                        break;
                    case "Y-":
                        Macro.InnerRingRollY_Stop();
                        break;
                    case "T+":
                        Macro.InnerRingYawT_Stop();
                        break;
                    case "T-":
                        Macro.InnerRingYawT_Stop();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });


        public ICommand InnerRingHome => new RelayCommand(async () =>
        {
            try
            {
                await Macro.HomeInnerRing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand GoOuterRingCheckPos => new RelayCommand(async () =>
        {
            try
            {
                await Macro.GoOuterRingCheckPos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand MacroOuterContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "Y+":
                        Macro.OuterRingRollY_Move(true);
                        break;
                    case "Y-":
                        Macro.OuterRingRollY_Move(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand MacroOuterMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "Y+":
                        Macro.OuterRingRollY_Stop();
                        break;
                    case "Y-":
                        Macro.OuterRingRollY_Stop();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand OuterRingHome => new RelayCommand(async () =>
        {
            try
            {
                await Macro.HomeOuterRing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });





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

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
