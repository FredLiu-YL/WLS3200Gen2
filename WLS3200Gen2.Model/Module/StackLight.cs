using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;
using YuanliCore.Machine.Base;

namespace WLS3200Gen2.Module
{
    /// <summary>
    /// 三色燈
    /// </summary>
    public class StackLight
    {
        private MachineStates machinestatus;
        private DigitalOutput ledRed, ledGreen, ledBlue, ledYellow, buzzer;
        private bool isVolume;//蜂鳴器功能開
        private bool isReflash;
        public Task ReflashTask;
        public StackLight(DigitalOutput[] outputs)
        {
            isVolume = false;
            this.ledRed = outputs[11];
            this.ledYellow = outputs[12];
            this.ledGreen = outputs[13];
            this.ledBlue = outputs[14];
            this.buzzer = outputs[15];
            SwitchStates(MachineStates.IDLE);
            isReflash = true;
            ReflashTask = Task.Run(Reflash);
        }

        public MachineStates Status => machinestatus;

        public void SwitchStates(MachineStates machinestatus)
        {
            //重置所有燈號
            this.machinestatus = machinestatus;
        }
        public void VolumeOff()
        {
            isVolume = false;
        }
        public void VolumeOn()
        {
            isVolume = true;
        }
        public void Dispose()
        {
            try
            {
                isReflash = false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task Reflash()
        {

            while (isReflash)
            {
                ledRed.Off();
                ledGreen.Off();
                ledYellow.Off();
                buzzer.Off();
                switch (machinestatus)
                {
                    case MachineStates.IDLE:

                        ledYellow.On();
                        await Task.Delay(2000);
                        ledYellow.Off();
                        await Task.Delay(2000);
                        break;
                    case MachineStates.RUNNING:
                        ledGreen.On();
                        await Task.Delay(2000);
                        ledGreen.Off();
                        await Task.Delay(2000);
                        break;
                    case MachineStates.PAUSED:

                        break;
                    case MachineStates.Alarm:
                        ledRed.On();
                        await Task.Delay(2000);
                        ledRed.Off();
                        await Task.Delay(2000);
                        break;
                    case MachineStates.Emergency:

                        ledRed.On();

                        if (isVolume)
                        {
                            buzzer.On();
                            await Task.Delay(500); //500毫秒後先關閉蜂鳴器
                            buzzer.Off();
                        }
                        else
                        {
                            await Task.Delay(500);
                        }

                        await Task.Delay(1000);

                        ledRed.Off();
                        await Task.Delay(1000);

                        break;
                    default:
                        break;
                }
            }
            try
            {
                ledRed.Off();
                ledGreen.Off();
                ledYellow.Off();
                buzzer.Off();
            }
            catch (Exception)
            {
            }
        }

    }
}
