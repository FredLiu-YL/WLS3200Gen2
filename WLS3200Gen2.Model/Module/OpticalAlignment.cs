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
using YuanliCore.ImageProcess.Match;
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

        private CogMatcher matcher = new CogMatcher();

        public OpticalAlignment(Axis axisX, Axis axisY, ICamera camera)
        {

            this.axisX = axisX;
            this.axisY = axisY;
        }


        public PauseTokenSource PauseToken { get; set; }
        public CancellationTokenSource CancelToken { get; set; }
 
        public async Task<ITransform> Alignment(LocateParam[] fiducialDatas)
        {
            List<Point> targetPos = new List<Point>();
            //移動到每一個樣本的 "拍照座標"做取像 ，計算出實際座標
            foreach (LocateParam fiducial in fiducialDatas)
            {


                await TableMoveToAsync(fiducial.GrabPositionX, fiducial.GrabPositionY);
                BitmapSource image = camera.GrabAsync();

                //Pattern match參數傳入 蒐尋器內
                matcher.RunParams = fiducial.MatchParam;
                MatchResult[] result = matcher.Find(image.ToByteFrame()).ToArray();

                if(result.Length==0)
                {
                    //沒搜尋到  要做些處置

                    CancelToken.Token.ThrowIfCancellationRequested();
                    await PauseToken.Token.WaitWhilePausedAsync(CancelToken.Token);
                    throw new Exception("搜尋失敗");
                }
                Point actualPos = await GetTargetPos(image, fiducial.GrabPositionX, fiducial.GrabPositionY, result[0].Center);

                targetPos.Add(actualPos);

                CancelToken.Token.ThrowIfCancellationRequested();
                await PauseToken.Token.WaitWhilePausedAsync(CancelToken.Token);

            }
            //獲得設計座標
            Point[] designPos = ConvertDesignPos();

            //設計座標與實際座標做對應 ，計算出轉換矩陣       
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
        //Pixel轉換成實際座標
        private async Task<Point> GetTargetPos(BitmapSource image, double currentPosX, double currentPosY, Point objPixel)
        {

            return new Point();
        }

        //因 Index 或 設計圖座標 與Table的 XY軸方向可能不一致 ，所以需要多一個轉換方法將設計座標轉換成與實際機台座標同方向
        //如果原先設計座標就是以機台本身取得的座標 則直接取出就好 (目前設計邏輯都是在UI端就定實際機台座標)
        private Point[] ConvertDesignPos()
        {

            Point[] designPos = alignmentRecipe.FiducialDatas.Select(f =>new Point( f.DesignPositionX, f.DesignPositionY)).ToArray();
            return designPos;
        }

    }
}
