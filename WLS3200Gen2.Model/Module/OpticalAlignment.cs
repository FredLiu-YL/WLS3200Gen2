using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.AffineTransform;
using YuanliCore.Data;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;
using YuanliCore.Model;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class OpticalAlignment
    {
        //private AlignmentRecipe alignmentRecipe;
        private Axis axisX, axisY;
        private ICamera camera;
        private ITransform affineTransform;

        private CogMatcher matcher = new CogMatcher();

        public OpticalAlignment(Axis axisX, Axis axisY, ICamera camera)
        {
            this.axisX = axisX;
            this.axisY = axisY;
            this.camera = camera;
        }


        public PauseTokenSource PauseToken { get; set; }
        public CancellationTokenSource CancelToken { get; set; }
        public Point PixelTable { get; set; }
        public Action<YuanliCore.Logger.LogType, string> WriteLog { get; set; }
        public Action<BitmapSource, Point?, int> FiducialRecord { get; set; }
        /// <summary>
        /// 對位失敗 手動對位
        /// </summary>
        public event Func<PauseTokenSource, CancellationTokenSource, double, double, Task<Point>> AlignmentManual;

        public async Task<ITransform> Alignment(LocateParam[] fiducialDatas, MicroscopeLens lensSetting)
        {
            BitmapSource image = null;
            try
            {
                List<Point> targetPos = new List<Point>();
                //移動到每一個樣本的 "拍照座標"做取像 ，計算出實際座標
                foreach (LocateParam fiducial in fiducialDatas)
                {
                    //目前鏡頭lens 1pixel行走距離
                    PixelTable = new Point(lensSetting.RatioX, lensSetting.RatioY);
                    int number = fiducialDatas.ToList().IndexOf(fiducial);
                    WriteLog(YuanliCore.Logger.LogType.PROCESS, $"Move To Fiducial : { fiducial.IndexY}-{ fiducial.IndexY}  Position:{fiducial.GrabPositionX},{fiducial.GrabPositionY}  ");

                    int retryCount = 3;
                    Point movePos = new Point(fiducial.GrabPositionX, fiducial.GrabPositionY);
                    Point actualPos = new Point(fiducial.GrabPositionX, fiducial.GrabPositionY);

                    //Pattern match參數傳入 蒐尋器內
                    matcher.RunParams = fiducial.MatchParam;
                    if (retryCount <= 1)
                    {
                        retryCount = 1;
                    }
                    await TableMoveToAsync(movePos.X, movePos.Y);
                    await Task.Delay(500);
                    //取像有機會取到模糊圖像或者平台尚未定位，所以多次Retry
                    for (int count = 1; count <= retryCount; count++)
                    {
                        int errorReTryCount = 0;
                        while (true)
                        {
                            try
                            {
                                //取像
                                image = camera.GrabAsync();
                                actualPos = await FindFiducial(image, movePos.X, movePos.Y, number);
                                //移動到取像位子
                                if (count == 1)
                                {
                                    movePos = actualPos;
                                    await TableMoveToAsync(movePos.X, movePos.Y);
                                }
                                await Task.Delay(100);
                                break;
                            }
                            catch (Exception ex)
                            {
                                if (errorReTryCount >= 2)
                                {
                                    if (AlignmentManual != null && CancelToken != null && PauseToken != null)// 如果有接實作 就手動對位
                                    {
                                        CancelToken.Token.ThrowIfCancellationRequested();
                                        await PauseToken.Token.WaitWhilePausedAsync(CancelToken.Token);
                                        Task<Point> alignmentManual = AlignmentManual?.Invoke(PauseToken, CancelToken, movePos.X, movePos.Y);
                                        actualPos = await alignmentManual;
                                        count = retryCount;
                                        break;
                                    }
                                    else
                                        throw ex;
                                }
                                errorReTryCount += 1;
                                await Task.Delay(50);
                            }
                        }
                    }
                    //加上鏡頭偏差
                    //actualPos = new Point(actualPos.X - lensSetting.RatioX * lensSetting.OffsetPixelX, actualPos.Y - lensSetting.RatioY * lensSetting.OffsetPixelY);
                    targetPos.Add(actualPos);

                    try
                    {
                        if (CancelToken != null && CancelToken.IsCancellationRequested && CancelToken.Token != null && PauseToken != null)
                        {
                            CancelToken.Token.ThrowIfCancellationRequested();
                            await PauseToken.Token.WaitWhilePausedAsync(CancelToken.Token);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //獲得設計座標
                Point[] designPos = ConvertDesignPos(fiducialDatas);


                //設計座標與實際座標做對應 ，計算出轉換矩陣       
                affineTransform = new CogAffineTransform(designPos, targetPos);

                return affineTransform;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 找出定位點實際座標
        /// </summary>
        /// <param name="image"></param>
        /// <param name="currentPosX"></param>
        /// <param name="currentPosY"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Point> FindFiducial(BitmapSource image, double currentPosX, double currentPosY, int number)
        {
            MatchResult[] result = matcher.Find(image.ToByteFrame()).ToArray();
            Point actualPos = new Point(currentPosX, currentPosY);
            if (result.Length == 0)
            {
                FiducialRecord?.Invoke(image, null, number);
                WriteLog(YuanliCore.Logger.LogType.ALARM, $"Search failed ");
                //沒搜尋到  要做些處置(目前沒做事)  改到外層去做事
                throw new FlowException("搜尋失敗");
            }
            else
            {
                FiducialRecord?.Invoke(image, result[0].Center, number);
                actualPos = GetTargetPos(image, currentPosX, currentPosY, result[0].Center);
            }
            return actualPos;
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
        private Point GetTargetPos(BitmapSource image, double currentPosX, double currentPosY, Point objPixel)
        {
            try
            {
                //目標-影像中心  * PixelSize   = 要移動的距離
                var deltaX = (objPixel.X - image.PixelWidth / 2) * PixelTable.X;
                var deltaY = (objPixel.Y - image.PixelHeight / 2) * PixelTable.Y;

                //當前位置+要移動的距離  = 目標實際機台座標
                return new Point(currentPosX + deltaX, currentPosY + deltaY);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //因 Index 或 設計圖座標 與Table的 XY軸方向可能不一致 ，所以需要多一個轉換方法將設計座標轉換成與實際機台座標同方向
        //如果原先設計座標就是以機台本身取得的座標 則直接取出就好 (目前設計邏輯都是在UI端就定實際機台座標)
        private Point[] ConvertDesignPos(LocateParam[] fiducialDatas)
        {

            Point[] designPos = fiducialDatas.Select(f => new Point(f.DesignPositionX, f.DesignPositionY)).ToArray();
            return designPos;
        }

    }
}
