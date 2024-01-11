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
using WLS3200Gen2.Model.Module;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// MacroUC.xaml 的互動邏輯
    /// </summary>
    public partial class MacroUC : UserControl, INotifyPropertyChanged
    {
        public MacroUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty MacroDetectionProperty = DependencyProperty.Register(nameof(MacroDetection), typeof(MacroDetection), typeof(MacroUC),
                                                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public MacroDetection MacroDetection
        {
            get => (MacroDetection)GetValue(MacroDetectionProperty);
            set => SetValue(MacroDetectionProperty, value);
        }


        public ICommand AllHome => new RelayCommand(async () =>
        {
            try
            {
                await MacroDetection.HomeAllRing();
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
                if (MacroDetection.CheckMacroCanMoveInnerRing() == CheckMacroCanMove.InnerInOrg || MacroDetection.CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    await MacroDetection.InnerRingLiftUp();
                }
                else
                {
                    MessageBox.Show("內環 不能上升!");
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

        public ICommand MacroInnerContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {

                    case "X+":
                        MacroDetection.InnerRingPitchX_Move(true);
                        break;
                    case "X-":
                        MacroDetection.InnerRingPitchX_Move(false);
                        break;
                    case "Y+":
                        MacroDetection.InnerRingRollY_Move(true);
                        break;
                    case "Y-":
                        MacroDetection.InnerRingRollY_Move(false);
                        break;
                    case "T+":
                        MacroDetection.InnerRingYawT_Move(true);
                        break;
                    case "T-":
                        MacroDetection.InnerRingYawT_Move(false);
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
                        MacroDetection.InnerRingPitchX_Stop();
                        break;
                    case "X-":
                        MacroDetection.InnerRingPitchX_Stop();
                        break;
                    case "Y+":
                        MacroDetection.InnerRingRollY_Stop();
                        break;
                    case "Y-":
                        MacroDetection.InnerRingRollY_Stop();
                        break;
                    case "T+":
                        MacroDetection.InnerRingYawT_Stop();
                        break;
                    case "T-":
                        MacroDetection.InnerRingYawT_Stop();
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
                if (MacroDetection.CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OK)
                {
                    await MacroDetection.HomeInnerRing(false);
                }
                else
                {
                    MessageBox.Show("內環 不能下降!");
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

        public ICommand GoOuterRingCheckPos => new RelayCommand(async () =>
        {
            try
            {
                if (MacroDetection.CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OuterInOrg || MacroDetection.CheckMacroCanMoveOuterRing() == CheckMacroCanMove.OK)
                {
                    await MacroDetection.OuterRingLiftUp();
                }
                else
                {
                    MessageBox.Show("外環 不能上升!");
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
        public ICommand MacroOuterContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "Y+":
                        MacroDetection.OuterRingRollY_Move(true);
                        break;
                    case "Y-":
                        MacroDetection.OuterRingRollY_Move(false);
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
                        MacroDetection.OuterRingRollY_Stop();
                        break;
                    case "Y-":
                        MacroDetection.OuterRingRollY_Stop();
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
                if (MacroDetection.CheckMacroCanMoveInnerRing() == CheckMacroCanMove.OuterInTop)
                {
                    await MacroDetection.HomeOuterRing(false);
                }
                else
                {
                    MessageBox.Show("外環 不能復歸!");
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
