using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using HardwareWizard.Controls;
using HardwareWizard.Core;
using HardwareWizard.Interfaces;

namespace HardwareWizard.Views
{
    public partial class DashboardView : UserControl, IViewUpdate
    {
        #region Variables
        private List<IViewUpdate> controls;
        #endregion

        #region Main
        public DashboardView(HardwareWatcher hardware)
        {
            InitializeComponent();
            this.chassisControl.ChassisType = ChassisType.Laptop;
            this.controls = new List<IViewUpdate>();
            this.Loaded += (async (s, e) => {
                await Task.Delay(250);
                Panel_Controls.OnChildResized();
            });

            chassisControl.ChassisType = hardware.Computer.Chassis;

            // Aggiunge le informazioni sul computer.
            Tx_ComputerName.Text = hardware.Computer.Identifier;
            if(!string.IsNullOrEmpty(hardware.Computer.OSVersion)) {
                Tx_ComputerVersion.Text = $"{hardware.Computer.OSEdition} (version {hardware.Computer.OSVersion})";
            } else {
                Tx_ComputerVersion.Text = $"{hardware.Computer.OSEdition} (build {hardware.Computer.OSBuild})";
            }
            Tx_ComputerEdition.Text = hardware.Computer.OSEdition;
            Tx_ComputerBuild.Text = hardware.Computer.OSBuild;
            Tx_ComputerArchitecture.Text = hardware.Computer.Architecture;
            Tx_ComputerInstallDate.Text = hardware.Computer.InstallDate;
            Tx_ComputerIdentifier.Text = hardware.Computer.Identifier;
            Tx_ComputerProcessor.Text = hardware.Computer.Processor;
            Tx_ComputerMemory.Text = hardware.Computer.Memory;
            Tx_ComputerGraphics.Text = hardware.Computer.Graphics;
            Tx_ComputerProductID.Text = hardware.Computer.ProductID;
            Tx_ComputerStorage.Text = hardware.Computer.Storage;

            // Aggiunge il controllo che contiene tutte le unità di archiviazione.
            var dashboardStorage = new DashboardStorageControl(hardware.Storage);
            controls.Add(dashboardStorage);
            Panel_Controls.AddChild(dashboardStorage, 0);

            // Aggiunge il controllo che contiene le informazioni sulla CPU.
            var dashboardCpu = new DashboardCpuControl(hardware.Processor);
            controls.Add(dashboardCpu);
            Panel_Controls.AddChild(dashboardCpu, 0);

            // Aggiunge il controllo che contiene le informazioni sulla RAM.
            var dashboardRam = new DashboardRamControl(hardware.Memory);
            controls.Add(dashboardRam);
            Panel_Controls.AddChild(dashboardRam, 1);

            // Aggiunge i controlli che contengono le informazioni sulle GPU.
            foreach (GraphicsCardData graphicsCard in hardware.Graphics.Cards)
            {
                // Interfaccia da applicare a ogni DashboardControl per aggiornare gli widget.
                var dashboardGpu = new DashboardGpuControl(graphicsCard);
                controls.Add(dashboardGpu);
                Panel_Controls.AddChild(dashboardGpu, 2);
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
