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
        public static readonly DependencyProperty MacroStatusProperty = DependencyProperty.Register(nameof(MacroStatus), typeof(MacroStatus), typeof(MacroUnitUC),
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
        public MacroStatus MacroStatus
        {
            get => (MacroStatus)GetValue(MacroStatusProperty);
            set => SetValue(MacroStatusProperty, value);
        }

        public ICommand AllHome => new RelayCommand(async () =>
        {
            try
            {
                if (MacroStatus.IsProcessStop == true)
                {
                    MacroStatus.IsProcessStop = false;
                    await Task.Run(async () =>
                    {
                        Macro.HomeAllRing();
                    });
                    MacroStatus.IsProcessStop = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MacroStatus.IsProcessStop = true;
            }
            finally
            {
            }
        });
        public ICommand GoInnerRingCheckPos => new RelayCommand(async () =>
        {
            try
            {
                if (MacroStatus.IsProcessStop == true)
                {
                    MacroStatus.IsProcessStop = false;
                    await Task.Run(async () =>
                    {
                        Macro.GoInnerRingCheckPos();
                    });
                    MacroStatus.IsProcessStop = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MacroStatus.IsProcessStop = true;
            }
            finally
            {
            }
        });

        public ICommand MacroInnerContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                if (MacroStatus.IsProcessStop == true)
                {
                    if (Macro.IsInnerUsing == true)
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
                            default:
                                break;
                        }
                    }
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
                if (MacroStatus.IsProcessStop == true)
                {
                    if (Macro.IsInnerUsing == true)
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
                            default:
                                break;
                        }
                    }
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
                if (MacroStatus.IsProcessStop == true)
                {
                    MacroStatus.IsProcessStop = false;
                    await Task.Run(async () =>
                    {
                        Macro.HomeInnerRing();
                    });
                    MacroStatus.IsProcessStop = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MacroStatus.IsProcessStop = true;
            }
            finally
            {
            }
        });

        public ICommand GoOuterRingCheckPos => new RelayCommand(async () =>
        {
            try
            {
                if (MacroStatus.IsProcessStop == true)
                {
                    MacroStatus.IsProcessStop = false;
                    await Task.Run(async () =>
                    {
                        Macro.GoOuterRingCheckPos();
                    });
                    MacroStatus.IsProcessStop = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MacroStatus.IsProcessStop = true;
            }
            finally
            {

            }
        });
        public ICommand MacroOuterContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                if (MacroStatus.IsProcessStop == true)
                {
                    if (Macro.IsOuterUsing == true)
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
                if (MacroStatus.IsProcessStop == true)
                {
                    if (Macro.IsOuterUsing == true)
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
                if (MacroStatus.IsProcessStop == true)
                {
                    MacroStatus.IsProcessStop = false;
                    await Task.Run(async () =>
                    {
                        Macro.HomeOuterRing();
                    });
                    MacroStatus.IsProcessStop = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MacroStatus.IsProcessStop = true;
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
    public class MacroStatus : INotifyPropertyChanged
    {
        bool isProcessStop = true;
        /// <summary>
        /// 程序是否暫停
        /// </summary>
        public bool IsProcessStop
        {
            get => isProcessStop;
            set => SetValue(ref isProcessStop, value);
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
