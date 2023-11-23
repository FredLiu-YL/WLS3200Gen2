using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YuanliCore.ImageProcess.Match;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private CogMatcher matcher = new CogMatcher(); //使用Vision pro 實體
        private PatmaxParams matchParam = new PatmaxParams(0);

     
       


        public ICommand EditSampleCommand => new RelayCommand<string>(async key =>
        {
            try
            {

              /*  matcher.RunParams = matchParam;
                matcher.EditParameter(Image);

                matchParam = (PatmaxParams)matcher.RunParams;
                if (matchParam.PatternImage != null)
                    SampleImage = matchParam.PatternImage.ToBitmapSource();*/

                //  UpdateRecipe();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand LocateSampleCommand => new RelayCommand<string>(async key =>
        {
          /*  ClearShapeAction.Execute(Drawings);
            resultPoint = matcher.Find(Image.ToByteFrame());

            foreach (var item in resultPoint)
            {
                var center = new ROICross
                {
                    X = item.Center.X,
                    Y = item.Center.Y,
                    Size = 5,
                    StrokeThickness = 2,
                    Stroke = Brushes.Red,
                    IsInteractived = false
                };
                AddShapeAction.Execute(center);

            }*/

        });
    }
}
