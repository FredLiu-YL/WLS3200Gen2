using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WLS3200Gen2
{
    /// <summary>
    /// InfomationWidow.xaml 的互動邏輯
    /// </summary>
    public partial class InfomationWidow : Window
    {

        private string text = "元利儀器股份有限公司創立於西元1970年，總代理日本OLYMPUS株式会社所生產之工業、醫療與生技檢測相關製造產品，" +
            "如：工業顯微鏡、工業內視鏡、高速攝影機、顯微量測儀器、生物顯微鏡及LCD自動檢查設備等，為OLYMPUS在台灣半世紀以來最信賴、關係最密切之合作夥伴。作為國內歷史最悠久的儀器代理商之一，" +
            "元利儀器參與並見證了台灣產業發展的各個時期，無論是鋼鐵、石化、機械等重工業領域起飛的年代，抑或是電子、資訊、面板產業蓬勃發展的時期，OLYMPUS顯微鏡及內視鏡都是各大產業研發、製造、檢查所不可或缺的重要工具。" +
            "進入21世紀後，元利儀器持續提供OLYMPUS最專業的光學產品於半導體製程、印刷電路板製造、航太產業、汽車、能源業、精密機械與自動化、高級材料、特用化學品與製藥、醫療保健、運動科學、汙染防治⋯⋯等各大工業領域。" +
            "在生命科學與研究領域方面，OLYMPUS的生物顯微鏡自1970年以來即廣泛用於國內各大醫院的病理檢驗與教學，對於國內醫療品質的提升甚有助益，數十年來頗獲業界好評。" +
            "近年隨著國內生醫學界、產業界的研究題材多元發展、質量大幅躍進，元利儀器亦導入OLYMPUS最先進之雷射掃描共軛焦顯微鏡等高階系統，" +
            "協助中央研究院及各研究型大學、醫院、生技公司等機構在細胞、癌症、神經科學、生殖醫學、製藥及各類跨領域研究建立最專業的實驗環境。" +
            "展望未來，元利儀器期許能奠基於半世紀累積而來的知識、經驗、技術與服務品質，從既有產業逐步拓展至自動化光學檢測、顯微影像、非破壞檢查、數據科學、人工智慧運用及其他客製化設備等新興專業領域，為不同產業客戶的各式應用繼續提供最佳的解決方案。";

        public InfomationWidow()
        {
            InitializeComponent();

            TextBlock1.Text = text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                // 處理錯誤
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
