﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuanliCore.UserControls;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private string logMessage;
        private bool isRunning = false;
        public Visibility processVisibility =  Visibility.Visible;

        public bool IsRunning { get => isRunning; set => SetValue(ref isRunning, value); }

        public string LogMessage { get => logMessage; set => SetValue(ref logMessage, value); }
        public Visibility ProcessVisibility { get => processVisibility; set => SetValue(ref processVisibility, value); }
    
        public ICommand RunCommand => new RelayCommand(async () =>
        {
            try
            {
                IsRunning = true;
                await machine.ProcessRunAsync();


                LogMessage = "123454";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
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
                ProcessVisibility = Visibility.Hidden;
                await machine.ProcessPause();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

                CassetteUC[1].Top_Background = Brushes.Black;
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

    }
}
