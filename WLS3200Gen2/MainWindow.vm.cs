﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using YuanliCore.Account;
using YuanliCore.Model.LoadPort;
using YuanliCore.Motion;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private Task taskRefresh1 = Task.CompletedTask;

        private UserAccount account;

        private string version;

        private Axis tableX;

        private Axis tableY;

        private double tablePosX, tablePosY;

        private ObservableCollection<CassetteUC> cassetteUC;

        private ObservableCollection<WorkItem> workItems;

        private Visibility informationUIVisibility, workholderUCVisibility;

        private int tabControlSelectedIndex;

        private bool isRefresh;

        public ObservableCollection<CassetteUC> CassetteUC
        {
            get => cassetteUC;
            set { SetValue(ref cassetteUC, value); }
        }
        public ICommand AddButtonAction { get; set; }
        public ObservableCollection<WorkItem> WorkItems
        { get => workItems; set => SetValue(ref workItems, value); }
        public Axis TableX { get => tableX; set => SetValue(ref tableX, value); }
        public Axis TableY { get => tableY; set => SetValue(ref tableY, value); }
        public double TablePosX { get => tablePosX; set => SetValue(ref tablePosX, value); }
        public double TablePosY { get => tablePosY; set => SetValue(ref tablePosY, value); }
        public string Version { get => version; set => SetValue(ref version, value); }
        public UserAccount Account { get => account; set => SetValue(ref account, value); }
        public Visibility InformationUCVisibility { get => informationUIVisibility; set => SetValue(ref informationUIVisibility, value); }
        public Visibility WorkholderUCVisibility { get => workholderUCVisibility; set => SetValue(ref workholderUCVisibility, value); }
        public int TabControlSelectedIndex { get => tabControlSelectedIndex; set => SetValue(ref tabControlSelectedIndex, value); }



        public ICommand WindowLoadedCommand => new RelayCommand(async () =>
        {
            try
            {
                machine.Initial();
                machine.Home();
                TableX = machine.MicroDetection.AxisX;
                TableY = machine.MicroDetection.AxisY;

                InformationUCVisibility = Visibility.Visible;
                WorkholderUCVisibility = Visibility.Collapsed;
                TabControlSelectedIndex = 0;

                isRefresh = true;
                taskRefresh1 = Task.Run(RefreshPos);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand WindowClosingCommand => new RelayCommand(() =>
       {
           try
           {
               isRefresh = false;

           }
           catch (Exception ex)
           {

               MessageBox.Show(ex.Message);
           }
           finally
           {

           }
       });
        private async Task RefreshPos()
        {
            try
            {

                while (isRefresh)
                {

                    //var pos = atfMachine.Table_Module.GetPostion();
                    TablePosX = machine.MicroDetection.AxisX.Position;
                    TablePosY = machine.MicroDetection.AxisY.Position;

                    //if (atfMachine.AFModule.AFSystem != null)
                    //    PositionZ = (int)atfMachine.AFModule.AFSystem.AxisZPosition;
                    await Task.Delay(300);
                }

            }
            catch (Exception ex)
            {

                // throw ex;
            }

        }
    }
}
