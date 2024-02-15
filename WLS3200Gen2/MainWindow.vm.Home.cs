using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.UserControls;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private ObservableCollection<WorkItem> workItems = new ObservableCollection<WorkItem>();
        private ObservableCollection<ProcessStationAssign> processStations = new ObservableCollection<ProcessStationAssign>();
        private string logMessage;
        private bool isRunning = false;
        private bool isRunCommand = false;
        private ProcessSetting processSetting = new ProcessSetting();

        private Visibility informationUIVisibility, workholderUCVisibility;

        private int tabControlSelectedIndex; // 0:Process Infomation   1:Alignment  2:Micro  3 :Macro
        private bool isOperateUI =true;
        private double manualPosX, manualPosY;
        private MachineStates machinestatus = MachineStates.IDLE;
        private MacroJudge macroJudgeOperation  ;


        public bool IsRunning { get => isRunning; set => SetValue(ref isRunning, value); }
        /// <summary>
        /// 在很多情況下 流程進行到一半需要人為操作 ，此時需要卡控不必要按鈕鎖住
        /// </summary>
        public bool IsOperateUI { get => isOperateUI; set => SetValue(ref isOperateUI, value); }
        public ObservableCollection<WorkItem> WorkItems { get => workItems; set => SetValue(ref workItems, value); }
        public string LogMessage { get => logMessage; set => SetValue(ref logMessage, value); }
        public Visibility ProcessVisibility { get => processVisibility; set => SetValue(ref processVisibility, value); }
        public ProcessSetting ProcessSetting { get => processSetting; set => SetValue(ref processSetting, value); }
        public Visibility processVisibility = Visibility.Visible;
        public Visibility InformationUCVisibility { get => informationUIVisibility; set => SetValue(ref informationUIVisibility, value); }
        public Visibility WorkholderUCVisibility { get => workholderUCVisibility; set => SetValue(ref workholderUCVisibility, value); }
        public int TabControlSelectedIndex { get => tabControlSelectedIndex; set => SetValue(ref tabControlSelectedIndex, value); }
        public double ManualPosX { get => manualPosX; set => SetValue(ref manualPosX, value); }
        public double ManualPosY { get => manualPosY; set => SetValue(ref manualPosY, value); }
        public MachineStates Machinestatus { get => machinestatus; set => SetValue(ref machinestatus, value); }
        public ObservableCollection<ProcessStationAssign> ProcessStations { get => processStations; set => SetValue(ref processStations, value); }


        public ICommand RunCommand => new RelayCommand(async () =>
        {
            try
            {
                if (Machinestatus == MachineStates.Emergency)
                {
                    MessageBox.Show("Not available in emergencies", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (isRunCommand == false)
                {
                    isRunCommand = true;

                    IsRunning = true;
                    WriteLog("Process Start");
                    Machinestatus = MachineStates.RUNNING;
                    await machine.ProcessRunAsync(ProcessSetting);
                    Machinestatus = MachineStates.IDLE;


                    isRunCommand = false;
                }
                else
                {
                    await machine.ProcessResume();
                }

            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                MessageBox.Show(ex.Message);
                isRunCommand = false;
                Machinestatus = MachineStates.Alarm;
            }
            finally
            {
                WriteLog("Process Finish");

            }
        });


        public ICommand PauseCommand => new RelayCommand(async () =>
        {
            try
            {

                IsRunning = false;
                // ProcessVisibility = Visibility.Hidden;
                Machinestatus = MachineStates.PAUSED;
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
                Machinestatus = MachineStates.RUNNING;
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
                Machinestatus = MachineStates.IDLE;
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

            try
            {
                string path = $"{systemPath}\\Recipe\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }


                FileInfoWindow win = new FileInfoWindow(false, "WLS3200Gen2", path);
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                bool isDialogResult = (bool)win.ShowDialog();
                if (isDialogResult)
                {
                    var recipename = win.FileName;
                    mainRecipe.Load(path, recipename);
                    SetRecipeToLocateParam(mainRecipe.DetectRecipe);
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand SaveRecipeCommand => new RelayCommand(() =>
        {
            try
            {

                string path = $"{systemPath}\\Recipe\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }

                FileInfoWindow win = new FileInfoWindow(true, "WLS3200Gen2", path);
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                bool isDialogResult = (bool)win.ShowDialog();
                if (isDialogResult)
                {

                    var recipename = win.FileName;
                    // machine.BonderRecipe.Save(win.FilePathName);
                    mainRecipe.RecipeSave(path, recipename);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        });



        public ICommand TESTCommand => new RelayCommand(async () =>
        {
            try
            {


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ManualReFindCommand => new RelayCommand(async () =>
       {
           try
           {
               await machine.MicroDetection.FindFiducial(MainImage, TablePosX, TablePosY);

           }
           catch (Exception ex)
           {

               MessageBox.Show(ex.Message);
           }
           finally
           {
           }
       });
        public ICommand ManualMoveCommand => new RelayCommand(async () =>
        {
            try
            {

                await Task.WhenAll(TableX.MoveToAsync(ManualPosX), TableY.MoveToAsync(ManualPosY));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ManualGetPosCommand => new RelayCommand(() =>
        {
            try
            {
                ManualPosX = TablePosX;
                ManualPosY = TablePosY;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand MacroPASSOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                macroJudgeOperation = MacroJudge.Pass;
                await machine.ProcessResume();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand MacroRejectOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                macroJudgeOperation = MacroJudge.Reject;
                await machine.ProcessResume();

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


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });




        private async Task<MacroJudge> MacroOperate(PauseTokenSource pts, CancellationTokenSource cts)
        {
            machine.ProcessPause();//暫停
            
            //切到Macro 頁面
            TabControlSelectedIndex = 3;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            //切到Infomation頁面
            TabControlSelectedIndex = 0;
            IsOperateUI = true;
            return macroJudgeOperation;

        }

    }
}
