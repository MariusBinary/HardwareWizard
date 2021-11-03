using System.Threading.Tasks;
using System.Windows.Controls;
using HardwareWizard.Controls;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class GraphicsView : UserControl
    {
        #region Variables
        private GraphicsData graphicsData;
        #endregion

        #region Main
        public GraphicsView(GraphicsData graphicsData)
        {
            InitializeComponent();
            this.graphicsData = graphicsData;
            this.Loaded += (async (s, e) => {
                await Task.Delay(250);
                Panel_Cards.OnChildResized();
                Panel_Monitors.OnChildResized();
            });

            foreach (GraphicsCardData card in graphicsData.Cards)
            {
                Panel_Cards.AddChild(new GenericItemControl() {
                    Content = card,
                    Title = card.Model,
                    Description = card.Manufacturer,
                    TargetView = 7,
                    Image = $"/HardwareWizard;component/Images/Shared/ic_gpu_{card.ManufacturerId}.png",
                });
            }

            foreach (GraphicsMonitorData monitor in graphicsData.Monitors)
            {
                Panel_Monitors.AddChild(new GenericItemControl()
                {
                    Content = monitor,
                    Title = monitor.Model,
                    Description = monitor.Manufacturer,
                    TargetView = 8,
                    Image = $"/HardwareWizard;component/Images/Shared/ic_monitor.png",
                });
            }
        }
        #endregion
    }
}
