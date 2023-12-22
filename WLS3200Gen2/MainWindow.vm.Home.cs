using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Model.LoadPort;
using YuanliCore.UserControls;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private ObservableCollection<WorkItem> workItems = new ObservableCollection<WorkItem>();
    
        private string logMessage  ;
        private bool isRunning = false;
        private bool isRunCommand = false; 
        private ProcessSetting processSetting = new ProcessSetting();

        private Visibility informationUIVisibility, workholderUCVisibility;

        private int tabControlSelectedIndex;
   


        public bool IsRunning { get => isRunning; set => SetValue(ref isRunning, value); }
        public ObservableCollection<WorkItem> WorkItems { get => workItems; set => SetValue(ref workItems, value); }
        public string LogMessage { get => logMessage; set => SetValue(ref logMessage, value); }
        public Visibility ProcessVisibility { get => processVisibility; set => SetValue(ref processVisibility, value); }
        public ProcessSetting ProcessSetting { get => processSetting; set => SetValue(ref processSetting, value); }
        public Visibility processVisibility = Visibility.Visible;
        public Visibility InformationUCVisibility { get => informationUIVisibility; set => SetValue(ref informationUIVisibility, value); }
        public Visibility WorkholderUCVisibility { get => workholderUCVisibility; set => SetValue(ref workholderUCVisibility, value); }
        public int TabControlSelectedIndex { get => tabControlSelectedIndex; set => SetValue(ref tabControlSelectedIndex, value); }



        public ICommand RunCommand => new RelayCommand(async () =>
        {
            try
            {
                if (isRunCommand == false)
                {
                    isRunCommand = true;

                    IsRunning = true;

                    await machine.ProcessRunAsync(ProcessSetting);

                    LogMessage = "123454";

                    isRunCommand = false;
                }
                else
                {
                    await machine.ProcessResume();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                isRunCommand = false;
            }
            finally
            {
               
            }
        });


        public ICommand PauseCommand => new RelayCommand(async () =>
        {
            try
            {

                IsRunning = false;
                // ProcessVisibility = Visibility.Hidden;
                await machine.ProcessPause();

                workItems[0].BackGroundBack = Brushes.Red;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ResumeCommand => new RelayCommand(async () =>
        {
            try
            {

                InformationUCVisibility = Visibility.Visible;
                WorkholderUCVisibility = Visibility.Collapsed;
                TabControlSelectedIndex = 0;
                IsRunning = false;
                await machine.ProcessResume();
                ProcessVisibility = Visibility.Visible;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand StopCommand => new RelayCommand(async () =>
        {
            try
            {
                await machine.ProcessStop();
                IsRunning = false;
                ProcessVisibility = Visibility.Visible;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
   
        public ICommand ReadRecipeCommand => new RelayCommand(() =>
        {
            FileInfoWindow win = new FileInfoWindow(false, "WLS3200Gen2", "MainRecipe");
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            bool isDialogResult = (bool)win.ShowDialog();
            if (isDialogResult)
            {
                // machine.BonderRecipe = MicroBonderRecipes.Load(win.FilePathName);
            }
        });
        public ICommand SaveRecipeCommand => new RelayCommand(() =>
        {
            FileInfoWindow win = new FileInfoWindow(false, "WLS3200Gen2", "MainRecipe");
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            bool isDialogResult = (bool)win.ShowDialog();
            if (isDialogResult)
            {
                // machine.BonderRecipe.RecipeID = win.FileName;
                // machine.BonderRecipe.Save(win.FilePathName);
            }
        });



        public ICommand TESTCommand => new RelayCommand(async () =>
        {
            try
            {

                DigitalInputs[0].IsSignal = false;
                DigitalInputs[1].IsSignal = true;
                DigitalInputs[2].IsSignal = true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand TEST1Command => new RelayCommand(async () =>
        {
            try
            {

                DigitalInputs[0].IsSignal = true;
                DigitalInputs[1].IsSignal = false;
                DigitalInputs[2].IsSignal = false;
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
