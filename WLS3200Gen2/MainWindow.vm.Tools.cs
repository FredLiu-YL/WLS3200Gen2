using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WLS3200Gen2.Model;
using YuanliCore.Data;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private int toolLoadPort1Index, toolAutoUnLoadAlignerIndex, toolAutoUnLoadMacroIndex, toolAutoUnLoadMicroIndex;
        private int alignerSuggestSlot, microSuggestSlot, macroSuggestSlot;
        private bool isMicroVaccum8, isMicroVaccum12, isMacroVaccum8, isMacroVaccum12, isAlignerVaccum12;
        private ObservableCollection<string> toolLoadPort1ComboBox = new ObservableCollection<string>();
        //public ObservableCollection<BincodeInfo> BincodeList { get => bincodeList; set => SetValue(ref bincodeList, value); }
        public ObservableCollection<string> ToolLoadPort1ComboBox { get => toolLoadPort1ComboBox; set => SetValue(ref toolLoadPort1ComboBox, value); }
        public int ToolLoadPort1Index { get => toolLoadPort1Index; set => SetValue(ref toolLoadPort1Index, value); }
        public int ToolAutoUnLoadAlignerIndex { get => toolAutoUnLoadAlignerIndex; set => SetValue(ref toolAutoUnLoadAlignerIndex, value); }
        public int ToolAutoUnLoadMacroIndex { get => toolAutoUnLoadMacroIndex; set => SetValue(ref toolAutoUnLoadMacroIndex, value); }
        public int ToolAutoUnLoadMicroIndex { get => toolAutoUnLoadMicroIndex; set => SetValue(ref toolAutoUnLoadMicroIndex, value); }
        public bool IsMicroVaccum8 { get => isMicroVaccum8; set => SetValue(ref isMicroVaccum8, value); }
        public bool IsMicroVaccum12 { get => isMicroVaccum12; set => SetValue(ref isMicroVaccum12, value); }
        public bool IsMacroVaccum8 { get => isMacroVaccum8; set => SetValue(ref isMacroVaccum8, value); }
        public bool IsMacroVaccum12 { get => isMacroVaccum12; set => SetValue(ref isMacroVaccum12, value); }
        public bool IsAlignerVaccum12 { get => isAlignerVaccum12; set => SetValue(ref isAlignerVaccum12, value); }

        public int AlignerSuggestSlot { get => alignerSuggestSlot; set => SetValue(ref alignerSuggestSlot, value); }
        public int MicroSuggestSlot { get => microSuggestSlot; set => SetValue(ref microSuggestSlot, value); }
        public int MacroSuggestSlot { get => macroSuggestSlot; set => SetValue(ref macroSuggestSlot, value); }

        public ICommand AlignerWaferToLoadPortCommand => new RelayCommand(async () =>
        {
            try
            {
                //是否執行移動片子訊息
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools AlignerWaferToLoadPort Start");
                var result = MessageBox.Show("AlignerWaferToLoadPort?", "Info", MessageBoxButton.YesNo);

                if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                {
                    IsCanWorkEFEMTrans = false;
                    bool isSelectLoadport1 = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        if (isSelectLoadport1)
                        {
                            RecipeLastArmStation = Model.ArmStation.Cassette1;
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Cassette2;
                        }
                        bool?[] wafers;
                        if (IsLoadport1)
                        {
                            isSelectLoadport1 = true;
                        }
                        else if (IsLoadport2)
                        {
                        }
                        else
                        {
                            throw new FlowException("LoadPort Select Error!");
                        }

                        var processStation = ProcessStations.Where(p => p.CassetteIndex == ToolAutoUnLoadAlignerIndex + 1).FirstOrDefault();
                        if (processStation == null ||
                        processStation.MacroTop != WaferProcessStatus.None ||
                        processStation.MacroBack != WaferProcessStatus.None ||
                        processStation.WaferID != WaferProcessStatus.None ||
                        processStation.Micro != WaferProcessStatus.None)
                        {
                            throw new FlowException("LoadPort Select Slot Have Wafer!!Please Select Another!");
                        }


                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                        }
                        WriteLog(YuanliCore.Logger.LogType.PROCESS, "Tools AlignerWaferToLoadPort Start");
                        await ToolAutoUnloadAlignerWaferToLoadPort(ToolAutoUnLoadAlignerIndex + 1, isSelectLoadport1);
                        //更改狀態變成有片
                        processStation.MacroTop = WaferProcessStatus.NotSelect;
                        processStation.MacroBack = WaferProcessStatus.NotSelect;
                        processStation.WaferID = WaferProcessStatus.NotSelect;
                        processStation.Micro = WaferProcessStatus.NotSelect;
                        processStation.IsCanChangeSelect = true;
                        LoadPort1Wafers.Clear();
                        foreach (var item in ProcessStations)
                        {
                            var w = new WaferUIData();
                            w.SN = item.CassetteIndex.ToString();
                            //只要不是空的 就是有片
                            if (item.MacroBack != WaferProcessStatus.None || item.MacroTop != WaferProcessStatus.None || item.Micro != WaferProcessStatus.None)
                                w.WaferStates = WaferProcessStatus.NotSelect;
                            LoadPort1Wafers.Add(w);
                        }

                        WriteLog(YuanliCore.Logger.LogType.PROCESS, "Tools AlignerWaferToLoadPort End");
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (FlowException ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                isInitialComplete = false;
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        /// <summary>
        /// Aligner端先吸真空->FindNotch->退到LoadPort
        /// </summary>
        /// <param name="CassetteIndex"></param>
        /// <param name="isSelectLoadport1"></param>
        public async Task ToolAutoUnloadAlignerWaferToLoadPort(int CassetteIndex, bool isSelectLoadport1)
        {
            try
            {
                //固定住Wafer在Aligner上
                await machine.Feeder.AlignerL.FixWafer();
                if (machine.Feeder.AlignerL.IsLockOK == false)
                {
                    throw new FlowException("Aligner真空異常!!");
                }
                //AlignerHome
                await machine.Feeder.AlignerL.Home();
                //轉到出貨角度
                double secondAngle = 0.0;
                if (DegreeUnLoad == Model.Recipe.Degree.Degree0)
                {
                    secondAngle = machineSetting.AlignerUnLoadOffset;
                }
                else if (DegreeUnLoad == Model.Recipe.Degree.Degree90)
                {
                    secondAngle = machineSetting.AlignerUnLoadOffset + 90;
                }
                else if (DegreeUnLoad == Model.Recipe.Degree.Degree180)
                {
                    secondAngle = machineSetting.AlignerUnLoadOffset + 180;
                }
                else if (DegreeUnLoad == Model.Recipe.Degree.Degree270)
                {
                    secondAngle = machineSetting.AlignerUnLoadOffset + 270;
                }
                if (secondAngle > 360)
                {
                    secondAngle = secondAngle % 360;
                }
                await machine.Feeder.AlignerL.Run(secondAngle);
                await machine.Feeder.AlignerL.ReleaseWafer();
                await machine.Feeder.WaferAlignerToStandBy();
                if (isSelectLoadport1)
                {
                    var currentWafer = new Wafer(CassetteIndex);
                    await machine.Feeder.UnLoadWaferToCassette(currentWafer, true);
                }
                else
                {
                    var currentWafer = new Wafer(CassetteIndex);
                    await machine.Feeder.UnLoadWaferToCassette(currentWafer, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand MicroWaferToLoadPortCommand => new RelayCommand(async () =>
        {
            try
            {
                //是否執行移動片子訊息
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools MicroWaferToLoadPort");
                var result = MessageBox.Show("MicroWaferToLoadPort?", "Info", MessageBoxButton.YesNo);

                if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                {
                    IsCanWorkEFEMTrans = false;
                    bool isSelectLoadport1 = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //isSelectLoadport1
                        if (isSelectLoadport1)
                        {
                            RecipeLastArmStation = Model.ArmStation.Cassette1;
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Cassette2;
                        }
                        bool?[] wafers;
                        if (IsLoadport1)
                        {
                            isSelectLoadport1 = true;
                        }
                        else if (IsLoadport2)
                        {
                        }
                        else
                        {
                            throw new FlowException("LoadPort Select Error!");
                        }

                        var processStation = ProcessStations.Where(p => p.CassetteIndex == ToolAutoUnLoadMicroIndex + 1).FirstOrDefault();
                        if (processStation == null ||
                        processStation.MacroTop != WaferProcessStatus.None ||
                        processStation.MacroBack != WaferProcessStatus.None ||
                        processStation.WaferID != WaferProcessStatus.None ||
                        processStation.Micro != WaferProcessStatus.None)
                        {
                            throw new FlowException("LoadPort Select Slot Have Wafer!!Please Select Another!");
                        }
                        //確認Aligner有無片
                        if (await EFEMTransWaferBeforeCheckAlignerHaveWafer())
                        {
                            throw new FlowException("EFEMTransCommand Error!Aligner Have Wafer!");
                        }
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                        }
                        WriteLog(YuanliCore.Logger.LogType.PROCESS, "Tools AlignerWaferToLoadPort Start");

                        await machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                        machine.MicroDetection.TableVacuum.Off();
                        await machine.Feeder.WaferMicroToStandBy();
                        await machine.Feeder.AlignerL.Home();
                        await machine.Feeder.WaferStandByToAligner();

                        await ToolAutoUnloadAlignerWaferToLoadPort(ToolAutoUnLoadMicroIndex + 1, isSelectLoadport1);
                        //更改狀態變成有片
                        processStation.MacroTop = WaferProcessStatus.NotSelect;
                        processStation.MacroBack = WaferProcessStatus.NotSelect;
                        processStation.WaferID = WaferProcessStatus.NotSelect;
                        processStation.Micro = WaferProcessStatus.NotSelect;
                        processStation.IsCanChangeSelect = true;
                        LoadPort1Wafers.Clear();
                        foreach (var item in ProcessStations)
                        {
                            var w = new WaferUIData();
                            w.SN = item.CassetteIndex.ToString();
                            //只要不是空的 就是有片
                            if (item.MacroBack != WaferProcessStatus.None || item.MacroTop != WaferProcessStatus.None || item.Micro != WaferProcessStatus.None)
                                w.WaferStates = WaferProcessStatus.NotSelect;
                            LoadPort1Wafers.Add(w);
                        }

                        WriteLog(YuanliCore.Logger.LogType.PROCESS, "Tools AlignerWaferToLoadPort End");
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (FlowException ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                isInitialComplete = false;
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand MacroWaferToLoadPortCommand => new RelayCommand(async () =>
        {
            try
            {
                //是否執行移動片子訊息
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools MacroWaferToLoadPort");
                var result = MessageBox.Show("MicroWaferToLoadPort?", "Info", MessageBoxButton.YesNo);

                if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                {
                    IsCanWorkEFEMTrans = false;
                    bool isSelectLoadport1 = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //isSelectLoadport1
                        if (isSelectLoadport1)
                        {
                            RecipeLastArmStation = Model.ArmStation.Cassette1;
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Cassette2;
                        }

                        bool?[] wafers;
                        if (IsLoadport1)
                        {
                            isSelectLoadport1 = true;
                        }
                        else if (IsLoadport2)
                        {
                        }
                        else
                        {
                            throw new FlowException("LoadPort Select Error!");
                        }

                        var processStation = ProcessStations.Where(p => p.CassetteIndex == ToolAutoUnLoadMacroIndex + 1).FirstOrDefault();
                        if (processStation == null ||
                        processStation.MacroTop != WaferProcessStatus.None ||
                        processStation.MacroBack != WaferProcessStatus.None ||
                        processStation.WaferID != WaferProcessStatus.None ||
                        processStation.Micro != WaferProcessStatus.None)
                        {
                            throw new FlowException("LoadPort Select Slot Have Wafer!!Please Select Another!");
                        }
                        //確認Aligner有無片
                        if (await EFEMTransWaferBeforeCheckAlignerHaveWafer())
                        {
                            throw new FlowException("EFEMTransCommand Error!Aligner Have Wafer!");
                        }
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                        }
                        WriteLog(YuanliCore.Logger.LogType.PROCESS, "Tools MacroWaferToLoadPort Start");

                        await machine.Feeder.WaferMacroToStandBy();
                        await machine.Feeder.AlignerL.Home();
                        await machine.Feeder.WaferStandByToAligner();

                        await ToolAutoUnloadAlignerWaferToLoadPort(ToolAutoUnLoadMacroIndex + 1, isSelectLoadport1);
                        //更改狀態變成有片
                        processStation.MacroTop = WaferProcessStatus.NotSelect;
                        processStation.MacroBack = WaferProcessStatus.NotSelect;
                        processStation.WaferID = WaferProcessStatus.NotSelect;
                        processStation.Micro = WaferProcessStatus.NotSelect;
                        processStation.IsCanChangeSelect = true;
                        LoadPort1Wafers.Clear();
                        foreach (var item in ProcessStations)
                        {
                            var w = new WaferUIData();
                            w.SN = item.CassetteIndex.ToString();
                            //只要不是空的 就是有片
                            if (item.MacroBack != WaferProcessStatus.None || item.MacroTop != WaferProcessStatus.None || item.Micro != WaferProcessStatus.None)
                                w.WaferStates = WaferProcessStatus.NotSelect;
                            LoadPort1Wafers.Add(w);
                        }

                        WriteLog(YuanliCore.Logger.LogType.PROCESS, "Tools MacroWaferToLoadPort End");
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (FlowException ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                isInitialComplete = false;
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand HomeCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Home");
                IsCanWorkEFEMTrans = false;
                await machine.Feeder.Robot.Home();
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand PickWaferFromLoadPortCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From LoadPort");
                //是否執行取片訊息
                string mesage = "Pick Cassette '" + (ToolLoadPort1Index + 1).ToString() + "' Wafer From Cassette1 ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            Wafer currentWafer = new Wafer(ToolLoadPort1Index + 1);
                            await machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette1);
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PickWaferFromAlignerCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From Aligner");
                //是否執行取片訊息
                string mesage = "Pick Wafer From Aligner ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.Feeder.AlignerL.Home();
                            await machine.Feeder.AlignerL.ReleaseWafer();
                            await machine.Feeder.WaferAlignerToStandBy();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                await machine.Feeder.AlignerL.FixWafer();
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PickWaferFromMicroCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From Micro");
                //是否執行取片訊息
                string mesage = "Pick Wafer From Micro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                            machine.MicroDetection.TableVacuum.Off();
                            await machine.Feeder.WaferMicroToStandBy();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                machine.MicroDetection.TableVacuum.On();
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PickWaferFromMacroCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Pick Wafer From Macro");
                //是否執行取片訊息
                string mesage = "Pick Wafer From Macro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                        {
                            mesage = "Robot Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.Feeder.WaferMacroToStandBy();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });


        public ICommand PutWaferToLoadPortCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To LoadPort");
                //是否執行放片訊息
                string mesage = "Put Wafer To LoadPort '" + (ToolLoadPort1Index + 1).ToString() + "' Cassette1 ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認LoadPort沒有片子
                        RecipeLastArmStation = Model.ArmStation.Cassette1;
                        Wafer currentWafer = new Wafer(ToolLoadPort1Index + 1);
                        await machine.Feeder.UnLoadWaferToCassette(currentWafer, true);
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand PutWaferToAlignerCommand => new RelayCommand(async () =>
        {
            try
            {
                //是否執行放片訊息
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To Aligner");
                string mesage = "Put Wafer To Aligner ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (await EFEMTransWaferBeforeCheckAlignerHaveWafer())
                        {
                            mesage = "Aligner Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Align;
                            machine.Feeder.AlignerL.Home().Wait();
                            await machine.Feeder.WaferStandByToAligner();
                            machine.Feeder.AlignerL.FixWafer().Wait();
                            if (machine.Feeder.AlignerL.IsLockOK == false)
                            {
                                result = MessageBox.Show("WaferToAligner Aligner真空異常!!", "Info", MessageBoxButton.YesNo);
                            }
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        /// <summary>
        /// 確認手臂有無片
        /// </summary>
        private async Task<bool> EFEMTransWaferBeforeCheckAlignerHaveWafer()
        {
            try
            {
                Task alignerLock = machine.Feeder.AlignerL.FixWafer();
                await alignerLock;
                await Task.Delay(1000);
                bool isHave = false;
                if (machine.Feeder.AlignerL.IsLockOK)
                {
                    isHave = true;
                }
                else
                {
                    await machine.Feeder.AlignerL.ReleaseWafer();
                }
                await Task.Delay(500);
                return isHave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ICommand PutWaferToMicroCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To Micro");
                //是否執行放片訊息
                string mesage = "Put Wafer To Micro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (await EFEMTransWaferBeforeCheckMicroHaveWafer())
                        {
                            mesage = "Micro Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            await machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                            machine.MicroDetection.TableVacuum.On();
                            Wafer station = new Wafer(1);
                            await machine.Feeder.LoadToMicroAsync(station);
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        private async Task<bool> EFEMTransWaferBeforeCheckMicroHaveWafer()
        {
            try
            {
                machine.MicroDetection.TableVacuum.On();
                await Task.Delay(1000);
                bool isHave = false;
                if (machine.MicroDetection.IsTableVacuum.IsSignal)
                {
                    isHave = true;
                }
                else
                {
                    machine.MicroDetection.TableVacuum.Off();
                }
                await Task.Delay(500);
                return isHave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand PutWaferToMacroCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Tools Put Wafer To Macro");
                //是否執行放片訊息
                string mesage = "Put Wafer To Macro ?";
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    if (result == MessageBoxResult.Yes)
                    {
                        //確認手臂有無片
                        if (await EFEMTransWaferBeforeCheckMacroHaveWafer())
                        {
                            mesage = "Micro Have Wafer!!";
                            result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                        }
                        else
                        {
                            RecipeLastArmStation = Model.ArmStation.Macro;
                            machine.Feeder.Macro.FixWafer();
                            await machine.Feeder.WaferStandByToMacroAsync();
                        }
                    }
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        private async Task<bool> EFEMTransWaferBeforeCheckMacroHaveWafer()
        {
            try
            {
                machine.Feeder.Macro.FixWafer();
                await Task.Delay(1000);
                bool isHave = false;
                if (machine.Feeder.Macro.IsLockOK)
                {
                    isHave = true;
                }
                else
                {
                    machine.Feeder.Macro.ReleaseWafer();
                }
                await Task.Delay(500);
                return isHave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand RobotMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {

                    case "LoadPort1":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
                        break;
                    case "LoadPort2":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);
                        break;
                    case "Aligner1":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                        break;
                    case "Micro":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                        break;
                    case "Macro":
                        await machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand MicroMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "X":
                        await machine.MicroDetection.AxisX.MoveToAsync(machineSetting.TableWaferCatchPosition.X);
                        break;
                    case "Y":
                        await machine.MicroDetection.AxisY.MoveToAsync(machineSetting.TableWaferCatchPosition.Y);
                        break;
                    case "Z":
                        await machine.MicroDetection.AxisZ.MoveToAsync(machineSetting.TableWaferCatchPositionZ);
                        break;
                    case "R":
                        await machine.MicroDetection.AxisR.MoveToAsync(machineSetting.TableWaferCatchPositionR);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
    }
}
