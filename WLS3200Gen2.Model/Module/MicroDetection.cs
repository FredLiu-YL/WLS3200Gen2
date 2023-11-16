using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Interface;
using YuanliCore.Model.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class MicroDetection
    {
        private ICamera camera;
        private PauseTokenSource pauseToken;
        private CancellationTokenSource cancelToken;



        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes, DigitalOutput[] outputs, DigitalInput[] inputs)
        {
            this.camera = camera;
            this.Microscope = microscope;
            AxisX = axes[0];
            AxisY = axes[1];
            TableVacuum = outputs[1];
            LiftPin = outputs[2];

        }

        public Axis AxisX { get; }
        public Axis AxisY { get; }
        public DigitalOutput TableVacuum { get; }
        public DigitalOutput LiftPin { get; }
        public IMicroscope Microscope { get; }


        public async Task Home()
        {

            await Task.Run(() => { });

        }




        public async Task Run(DetectionRecipe recipe, PauseTokenSource pauseToken, CancellationTokenSource cancellationToken)
        {
            this.pauseToken = pauseToken;
            this.cancelToken = cancellationToken;


        }
    }
}
