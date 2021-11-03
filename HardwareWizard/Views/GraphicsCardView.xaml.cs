using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class GraphicsCardView : UserControl
    {
        #region Variables
        private GraphicsCardData cardData;
        #endregion

        #region Main
        public GraphicsCardView(GraphicsCardData cardData)
        {
            InitializeComponent();
            this.cardData = cardData;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Tx_CardModel.Text = cardData.Model;
            Tx_CardManufacturer.Text = cardData.Manufacturer;
            Img_CardIcon.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/HardwareWizard;component/Images/Shared/ic_gpu_{cardData.ManufacturerId}.png"));

            Table_Informations.AddTag("Model", cardData.Model, 0);
            Table_Informations.AddTag("Manufacturer", cardData.Manufacturer, 0);
            Table_Informations.AddTag("Video Processor", cardData.VideoProcessor, 0); 
            Table_Informations.AddTag("Video Architecture", cardData.VideoArchitecture, 0);
            Table_Informations.AddTag("Memory Type", cardData.VideoMemoryType, 0);
            Table_Informations.AddTag("Bits Per Pixel", cardData.BitsPerPixel, 1);
            Table_Informations.AddTag("Horizontal Resolution", cardData.HorizontalResolution, 1);
            Table_Informations.AddTag("Vertical Resolution", cardData.VerticalResolution, 1);
            Table_Informations.AddTag("Refresh Rate", cardData.RefreshRate, 1);
            Table_Informations.AddTag("Scan Mode", cardData.ScanMode, 1);

            // Aggiungere le proprietà WMI.
            for (int i = 0; i < cardData.WMI.Count; i++) {
                Table_WMI.AddTag(cardData.WMI[i].Property, cardData.WMI[i].Value, i % 2 == 0 ? 0 : 1);
            }
        }

        private void Btn_Report_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReportHelper report = new ReportHelper("gpu");
            report.AddToReport("Informations", Table_Informations);
            report.AddToReport("WMI", Table_WMI);
            report.MakeReport();
        }
        #endregion
    }
}
