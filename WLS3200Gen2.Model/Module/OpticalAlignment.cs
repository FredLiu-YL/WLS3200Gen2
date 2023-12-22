using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using YuanliCore.AffineTransform;
using YuanliCore.Interface;
using YuanliCore.Model;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class OpticalAlignment
    {

        private AlignmentRecipe alignmentRecipe;
        private Axis axisX, axisY;
        private ICamera camera;
        private ITransform affineTransform;

        public OpticalAlignment(Axis axisX, Axis axisY, ICamera camera)
        {

            this.axisX = axisX;
            this.axisY = axisY;
        }


        public PauseTokenSource PauseToken { get; set; }
        public CancellationTokenSource CancelToken { get; set; }

        public async Task<ITransform> Alignment(AlignmentRecipe alignmentRecipe)
        {
            List<Point> targetPos = new List<Point>();
            //移動到每一個樣本的 "拍照座標"做取像 ，計算出實際座標
            foreach (FiducialData fiducial in alignmentRecipe.fiducialDatas)
            {

                await TableMoveToAsync(fiducial.GrabPosition.X, fiducial.GrabPosition.Y);
                BitmapSource image = camera.GrabAsync();
                Point actualPos = await GetTargetPos(image, fiducial.GrabPosition.X, fiducial.GrabPosition.Y);

                targetPos.Add(actualPos);

                CancelToken.Token.ThrowIfCancellationRequested();
                await PauseToken.Token.WaitWhilePausedAsync(CancelToken.Token);

            }

            //設計座標與實際座標做對應 ，計算出轉換矩陣
            Point[] designPos = alignmentRecipe.fiducialDatas.Select(f => f.DesignPosition).ToArray();
            affineTransform = new CogAffineTransform(designPos, targetPos);

            return affineTransform;
        }
        private async Task TableMoveToAsync(double posX, double posY)
        {
            try
            {
                await Task.WhenAll(axisX.MoveToAsync(posX),
                axisY.MoveToAsync(posY));
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        private async Task<Point> GetTargetPos(BitmapSource image, double currentPosX, double currentPosY)
        {

            return new Point();
        }

    }
}
