using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
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
        private Subject<BitmapSource> subject = new Subject<BitmapSource>();
        private IDisposable camlive;
        public IObservable<BitmapSource> Observable => subject;
        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes, DigitalOutput[] outputs, DigitalInput[] inputs)
        {
            this.camera = camera;
            this.Microscope = microscope;
            AxisX = axes[0];
            AxisY = axes[1];
            TableVacuum = outputs[1];
            LiftPin = outputs[2];

            ObservableDetection();
          
        }

        public Axis AxisX { get; }
        public Axis AxisY { get; }
        public Axis AxisR { get; }
        public DigitalOutput TableVacuum { get; }
        public DigitalOutput LiftPin { get; }
        public IMicroscope Microscope { get; }
        public DetectionRecipe detectionRecipe;


        public async Task Home()
        {

            await Task.Run(() => { });

        }




        public async Task Run(DetectionRecipe recipe, bool isAutoSave, PauseTokenSource pst, CancellationTokenSource ctk)
        {
            this.pauseToken = pst;
            this.cancelToken = ctk;



            cancelToken.Token.ThrowIfCancellationRequested();
            await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);
            
            //每一個座標需要檢查的座標
            foreach (var point in recipe.DetectionPoints)
            {
                //對位




                await TableMoveToAsync(point.Position);
                await Task.Delay(200);
                BitmapSource bmp = camera.GrabAsync();

                subject.OnNext(bmp);//AOI另外丟到其他執行續處理
              
                if (isAutoSave)
                {

                }

                pauseToken.IsPaused = true;

                cancelToken.Token.ThrowIfCancellationRequested();
                await pauseToken.Token.WaitWhilePausedAsync(cancelToken.Token);

            }



        }

        public async Task TableMoveToAsync(Point pos)
        {

        }
        public async Task TableMoveAsync(Vector distance)
        {

        }

        public async Task DefectDetection(BitmapSource bmp)
        {

          

        }



        private void ObservableDetection()
        {
            camlive = Observable//.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
                    .ObserveOn(TaskPoolScheduler.Default)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
                    .Subscribe(async frame =>
                    {
                        await DefectDetection(frame);
                    });
        }

    }
}
