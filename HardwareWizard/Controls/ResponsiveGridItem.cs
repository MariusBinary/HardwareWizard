using System.Windows.Controls;

namespace HardwareWizard.Controls
{
    public class ResponsiveGridItem
    {
        public Control Control { get; set; }
        public int ActualColumn { get; set; }
        public int DesiredColumn { get; set; }

        public ResponsiveGridItem(Control control, int actualColumn, int desiredColumn)
        {
            Control = control;
            ActualColumn = actualColumn;
            DesiredColumn = desiredColumn;
        }
    }
}
