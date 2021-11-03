using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using HardwareWizard.Controls;
using HardwareWizard.Core;
using HardwareWizard.Interfaces;

namespace HardwareWizard.Views
{
    public partial class CoolingView : UserControl, IViewUpdate
    {
        #region Variables
        private List<IViewUpdate> controls;
        #endregion

        #region Main
        public CoolingView(CoolingData coolingData)
        {
            InitializeComponent();
            this.controls = new List<IViewUpdate>();
            this.Loaded += (async (s, e) => {
                await Task.Delay(250);
                Panel_Fans.OnChildResized();
                Panel_Thermals.OnChildResized();
            });

            // Aggiunge i controlli delle ventole.
            foreach (CoolingFanData fanData in coolingData.Fans)
            {
                var fan = new CoolingFanControl(fanData);
                controls.Add(fan);
                Panel_Fans.AddChild(fan);
            }

            // Aggiunge i controlli dei vari sensori termici.
            foreach (CoolingThermalGroup thermalGroup in coolingData.Thermals)
            {
                var thermal = new CoolingThermalControl(thermalGroup);
                controls.Add(thermal);
                Panel_Thermals.AddChild(thermal);
            }
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            foreach (IViewUpdate control in controls) {
                control.OnViewUpdate();
            }
        }
        #endregion
    }
}
