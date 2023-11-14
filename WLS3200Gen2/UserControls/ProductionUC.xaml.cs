﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using YuanliCore.Model.LoadPort;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// Informaiton.xaml 的互動邏輯
    /// </summary>
    public partial class ProductionUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AutoSaveIsCheckedProperty = DependencyProperty.Register(nameof(AutoSaveIsChecked), typeof(bool), typeof(ProductionUC),
                                                                                    new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CircleIsCheckedProperty = DependencyProperty.Register(nameof(CircleIsChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DieInsideAllCheckedProperty = DependencyProperty.Register(nameof(DieInsideAllChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AddButtonActionProperty = DependencyProperty.Register(nameof(AddButtonAction), typeof(ICommand), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //public static readonly DependencyProperty CassetteUCProperty = DependencyProperty.Register(nameof(CassetteUC), typeof(ObservableCollection<CassetteUC>), typeof(ProductionUC),
        //                                                                                             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty WorkItemsProperty = DependencyProperty.Register(nameof(WorkItems), typeof(IEnumerable<WorkItem>), typeof(ProductionUC),
                                                                                   new FrameworkPropertyMetadata(new WorkItem[] { }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ProductionUC()
        {
            InitializeComponent();

            try
            {
                //CassetteUC = new ObservableCollection<CassetteUC>();
                AddButtonAction = new RelayCommand<(int, List<int>)>(key =>
                {
                    int variable1 = key.Item1;
                    List<int> variable2 = key.Item2;
                    AddButton(variable1, variable2);
                });

                List<int> RowsDisable = new List<int>();
                for (int i = 5; i < 20; i++)
                {
                    RowsDisable.Add(i);
                }
                AddButton(25, RowsDisable);
            }
            catch (Exception)
            {
                throw;
            }

        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //var list = WorkItems.ToList();
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool AutoSaveIsChecked
        {
            get => (bool)GetValue(AutoSaveIsCheckedProperty);
            set => SetValue(AutoSaveIsCheckedProperty, value);
        }
        public bool CircleIsChecked
        {
            get => (bool)GetValue(CircleIsCheckedProperty);
            set => SetValue(CircleIsCheckedProperty, value);
        }
        public bool DieInsideAllChecked
        {
            get => (bool)GetValue(DieInsideAllCheckedProperty);
            set => SetValue(DieInsideAllCheckedProperty, value);
        }
        public ICommand AddButtonAction
        {
            get => (ICommand)GetValue(AddButtonActionProperty);
            set => SetValue(AddButtonActionProperty, value);
        }
        public IEnumerable<WorkItem> WorkItems
        {
            get => (IEnumerable<WorkItem>)GetValue(WorkItemsProperty);
            set
            {
                var list = WorkItems.ToList();
                for (int i = 0; i < WorkItems.Count(); i++)
                {
                    CassetteUC[i].WorkStatus = list[i];
                }
                SetValue(WorkItemsProperty, value);
            }
        }
        //public ObservableCollection<CassetteUC> CassetteUC
        //{
        //    get => (ObservableCollection<CassetteUC>)GetValue(CassetteUCProperty);
        //    set => SetValue(CassetteUCProperty, value);
        //}

        public ObservableCollection<CassetteUC> CassetteUC
        { get => cassetteUC; set => SetValue(ref cassetteUC, value); }

        public ObservableCollection<CassetteUC> cassetteUC = new ObservableCollection<CassetteUC>();

        public void AddButton(int Rows, List<int> RowsDisable)
        {
            try
            {
                CassetteUC.Clear();
                for (int i = Rows - 1; i >= 0; i--)
                {
                    bool IsDisable = false;
                    for (int j = RowsDisable.Count - 1; j >= 0; j--)
                    {
                        if (i == RowsDisable[j])
                        {
                            IsDisable = true;
                            RowsDisable.RemoveAt(j);
                            break;
                        }
                    }

                    CassetteUC add_CassetteUC;

                    if (IsDisable == true)
                    {
                        add_CassetteUC = new CassetteUC(false, (i + 1).ToString());
                    }
                    else
                    {
                        add_CassetteUC = new CassetteUC(true, (i + 1).ToString());
                    }
                    //add_CassetteUC.WorkItemChange += (workItem) =>
                    //{

                    //};
                    add_CassetteUC.WorkItemChange += (workItem) =>
                    {
                        WorkItemChange(workItem);
                    };
                    CassetteUC.Add(add_CassetteUC);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WorkItemChange(WorkItem workItem)
        {
            try
            {
                //List<WorkItem> aaaaaa = new List<WorkItem>();
                //foreach (var item in CassetteUC)
                //{
                //    aaaaaa.Add(item.WorkStatus);
                //}
                //WorkItems = aaaaaa;
                WorkItems = CassetteUC.Select(c => c.WorkStatus).ToArray();
            }
            catch (Exception)
            {

                throw;
            }
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
