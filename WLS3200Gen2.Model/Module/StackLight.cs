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
        private DigitalOutput ledRed, ledGreen, ledYellow, buzzer;
        private bool isVolume;//蜂鳴器功能開
        private bool isReflash;
        private Task reflashTask;
        public StackLight(DigitalOutput[]  outputs)
        {


            isVolume = true;
            this.ledRed = outputs[6];
            this.ledGreen = outputs[7];
            this.ledYellow = outputs[8];
            this.buzzer = outputs[8];
            SwitchStates(MachineStates.IDLE);
            isReflash = true;
            reflashTask = Task.Run(Reflash);
        }

        public MachineStates Status => machinestatus;

        public void SwitchStates(MachineStates machinestatus)
        {
            //重置所有燈號
            ledRed.Off();
            ledGreen.Off();
            ledYellow.Off();
            buzzer.Off();
            isVolume=true;

            this.machinestatus = machinestatus;

        }
        public void VolumeOff()
        {

            isVolume = false;


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
                            await Task.Delay(500);


                        await Task.Delay(1000);

                        ledRed.Off();
                        await Task.Delay(1000);

                        break;
                    default:
                        break;
                }




            }
        }



    }
}
