using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HardwareWizard.Core.Helpers;

namespace HardwareWizard.Controls
{
    public enum ChassisType
    {
        Desktop,
        Laptop
    }

    [TemplatePart(Name = "PART_Case", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Wallpaper", Type = typeof(Image))]
    public class ChassisControl : Control
    {
        #region Properties
        // Property: ChassisType
        public static readonly DependencyProperty ChassisTypeProperty =
            DependencyProperty.Register("ChassisType", typeof(ChassisType), typeof(ChassisControl),
                new FrameworkPropertyMetadata(ChassisType.Desktop));

        public ChassisType ChassisType
        {
            get { return (ChassisType)GetValue(ChassisTypeProperty); }
            set { SetValue(ChassisTypeProperty, value); }
        }
        #endregion

        #region Template
        private Image m_case;
        private Image m_wallpaper;
        #endregion

        #region Main
        static ChassisControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChassisControl), 
                new FrameworkPropertyMetadata(typeof(ChassisControl)));
        }

        public override void OnApplyTemplate()
        {
            m_case = Template.FindName("PART_Case", this) as Image;
            m_wallpaper = Template.FindName("PART_Wallpaper", this) as Image;

            base.OnApplyTemplate();
        }

        public ChassisControl()
        {
            this.Loaded += (s, e) => {
                m_case.Source = new BitmapImage(new Uri(GetCaseImage()));
                m_wallpaper.Source = WallpaperHelper.GetWallaperImage();
            };
        }
        #endregion

        #region Helpers
        private string GetCaseImage()
        {
            switch(this.ChassisType)
            {
                case ChassisType.Desktop:
                    m_wallpaper.Height = 98;
                    m_wallpaper.Width = 155;
                    m_wallpaper.Margin = new Thickness(30, 50, 30, 70);
                    return "pack://application:,,,/HardwareWizard;component/Images/Shared/ic_desktop.png";
                case ChassisType.Laptop:
                    m_wallpaper.Height = 76;
                    m_wallpaper.Width = 120;
                    m_wallpaper.Margin = new Thickness(50, 61, 50, 83);
                    return "pack://application:,,,/HardwareWizard;component/Images/Shared/ic_laptop.png";
                default:
                    m_wallpaper.Height = 98;
                    m_wallpaper.Width = 155;
                    m_wallpaper.Margin = new Thickness(30, 50, 30, 70);
                    return "pack://application:,,,/HardwareWizard;component/Images/Shared/ic_desktop.png";
            }
        }
        #endregion
    }
}
