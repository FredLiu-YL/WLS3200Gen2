using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.ImageProcess;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Recipe
{
    public class MainRecipe : AbstractRecipe
    {

        public DetectionRecipe DetectRecipe { get; set; } = new DetectionRecipe();



        /// <summary>
        /// 因某些元件無法被正常序列化 所以另外做存檔功能
        /// </summary>
        /// <param name="Path"></param>
        public void RecipeSave(string path, string recipeName)
        {

            string dirpath = $"{path}\\{recipeName}";
            if (!Directory.Exists(dirpath))
                Directory.CreateDirectory(dirpath);


            //刪除所有Vistiontool 的檔案避免 id重複 寫錯，或是 原先檔案數量5個  後來變更成3個  讀檔會錯誤
            string[] files = Directory.GetFiles(dirpath, "*VsTool_*");
            foreach (string file in files)
            {
                if (file.Contains("VsTool")) // 如果文件名包含 "VSP"
                {

                    File.Delete(file); // 删除该文件
                }
            }

            //Patternmatch  id 在100-199  用於檔案區隔  從100開始是因為檔案名稱搜尋1開頭比較簡單
            if (DetectRecipe.AlignRecipe.FiducialDatas != null)
            {
                foreach (var param in DetectRecipe.AlignRecipe.FiducialDatas)
                {
                    param.MatchParam.Save(dirpath);
                }
            }

            /* //RunParams  id 在301-399  用於檔案區隔
             if (AOIParams2 != null)
             {
                 foreach (CogParameter param in AOIParams2)
                 {
                     param.Save(path);
                 }
             }*/

            base.Save(dirpath + "\\Recipe.json");
        }


        /// <summary>
        /// 因某些元件無法被正常序列化 所以另外做讀檔功能
        /// </summary>
        /// <param name="Path"></param>
        public void Load(string path, string recipeName)
        {

            /*
             //清除先前new過的 cognex元件
             foreach (var item in DetectRecipe.AlignRecipe.FiducialDatas)
             {
                 item.MatchParam.Dispose();
             }
             */
            string dirpath = $"{path}\\{recipeName}";
            //想不到好方法做序列化 ， 如果需要修改 就要用JsonConvert 把不能序列化的屬性都改掉  這樣就能正常做load
            var mRecipe = AbstractRecipe.Load<MainRecipe>($"{dirpath}\\Recipe.json");



            //因序列化少了COGNEX ，要手動重新塞 ，所以屬性讀回要分成兩段來做
            //先讀取可以序列化的部分
            DetectRecipe = mRecipe.DetectRecipe;



            // 讀取pattern 資料
            string[] algorithmfiles = Directory.GetFiles(dirpath, "*VsTool_1*"); //資料夾內有
            for (int i = 0; i < algorithmfiles.Length; i++)
            {

                string fileName = Path.GetFileName(algorithmfiles[i]);

                //檔名後面就是ID 
                string[] id = fileName.Split(new string[] { "VsTool_", ".tool" }, StringSplitOptions.RemoveEmptyEntries);

                CogParameter param = CogParameter.Load(dirpath, Convert.ToInt32(id[0]));

                DetectRecipe.AlignRecipe.FiducialDatas[i].MatchParam = param as PatmaxParams;

             
            }
            // AOI 八角形
            /*  string[] algorithmfiles2 = Directory.GetFiles(path, "*VsTool_3*");
              foreach (var file in algorithmfiles2)
              {

                  string fileName = Path.GetFileName(file);

                  string[] id = fileName.Split(new string[] { "VsTool_", ".tool" }, StringSplitOptions.RemoveEmptyEntries);
                  if (id[0] == "0") continue; // 0 是定位用的樣本 所以排除
                  CogParameter param = CogParameter.Load(path, Convert.ToInt32(id[0]));
                  AOIParams2.Add(param);


              }*/

        }
    }
}
