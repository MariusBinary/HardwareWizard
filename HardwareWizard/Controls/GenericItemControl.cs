using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_Clickable", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Image", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Description", Type = typeof(TextBlock))]
    public class GenericItemControl : Control
    {
        #region Properties
        // Property: Image
        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register("Image", typeof(string), typeof(GenericItemControl),
                new FrameworkPropertyMetadata(null));

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Property: Title
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(GenericItemControl),
                new FrameworkPropertyMetadata("-"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Property: Description
        public static readonly DependencyProperty DescriptionProperty
            = DependencyProperty.Register("Description", typeof(string), typeof(GenericItemControl),
                new FrameworkPropertyMetadata("-"));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }


        // Property: Content
        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register("Content", typeof(object), typeof(GenericItemControl),
                new FrameworkPropertyMetadata(null));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Property: TargetView
        public static readonly DependencyProperty TargetViewProperty
            = DependencyProperty.Register("TargetView", typeof(int), typeof(GenericItemControl),
                new FrameworkPropertyMetadata(-1));

        public int TargetView
        {
            get { return (int)GetValue(TargetViewProperty); }
            set { SetValue(TargetViewProperty, value); }
        }
        #endregion

        #region Template
        private Button m_clickable;
        private Image m_image;
        private TextBlock m_title;
        private TextBlock m_description;
        #endregion

        #region Main
        static GenericItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GenericItemControl), 
                new FrameworkPropertyMetadata(typeof(GenericItemControl)));
        }

		public override void OnApplyTemplate()
        {
            m_clickable = Template.FindName("PART_Clickable", this) as Button;
            m_image = Template.FindName("PART_Image", this) as Image;
            m_title = Template.FindName("PART_Title", this) as TextBlock;
            m_description = Template.FindName("PART_Description", this) as TextBlock;

            base.OnApplyTemplate();
        }

        public GenericItemControl()
        {
            this.Loaded += (s, e) => {
                if (Image != null) {
                    m_image.Source = new BitmapImage(new Uri(Image, UriKind.RelativeOrAbsolute));
                }
                m_title.Text = Title;
                m_description.Text = Description;
                m_clickable.CommandParameter = new object[] { TargetView, Content };
            };  
        }
        #endregion
    }
}
