using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public enum HardwareViewType
    {
        None,
        Dashboard,
        Motherboard,
        Processor,
        Graphics,
        Memory,
        Storage,
        Cooling
    }

    public enum HardwareUpdateType
    {
        Active,
        Passive
    }

    public class HardwareWatcher
    {
        #region Properties
        public ComputerData Computer { get; set; }
        public StorageData Storage { get; set; }
        public ProcessorData Processor { get; set; }
        public GraphicsData Graphics { get; set; }
        public MotherboardData Motherboard { get; set; }
        public MemoryData Memory { get; set; }
        public CoolingData Cooling { get; set; }
        #endregion

        #region Variables
        private Computer computer;
        private ThreadTimer updateTimer;
        private IViewUpdate viewUpdate;
        private HardwareViewType viewType;
        private int updateInterval = 1;
        private int backgroundInterval = 5;
        private int backgroundIntervalTick = 0;
        private bool isSleepMode = false;
        private List<IHardware> hardwares;
        private ProcessesWatcher processesWatcher;
        private bool canBackgroundUpdate = true;
        #endregion

        #region Main
        public HardwareWatcher(Action callback)
        {
            hardwares = new List<IHardware>();
            processesWatcher = new ProcessesWatcher();
            updateTimer = new ThreadTimer();

            new Thread(() => {
                Thread.Sleep(250);
                computer = new Computer();
                computer.CPUEnabled = true;
                computer.GPUEnabled = true;
                computer.MainboardEnabled = true;
                computer.HDDEnabled = true;
                computer.RAMEnabled = true;
                computer.Open();
                Thread.Sleep(250);
                Create();
                callback.Invoke();
            }).Start();
        }

        private void UpdateTimer()
        {
            // Controlla se è attivo uno stato di sospensione.
            if (isSleepMode) return;

            // Calcola se l'hardware passivo può essere aggiornato.
            bool canUpdatePassiveHardware = false;
            if (backgroundIntervalTick >= backgroundInterval) {
                backgroundIntervalTick = 0;
                canUpdatePassiveHardware = canBackgroundUpdate ? true : false;
            } else {
                backgroundIntervalTick += 1;
            }

            if (viewType == HardwareViewType.Dashboard || 
                viewType == HardwareViewType.Processor || 
                viewType == HardwareViewType.Memory) {
                processesWatcher.Update();
            }

            // Aggiorna le informazioni relative alle unità di archiviazione.
            if (viewType == HardwareViewType.Dashboard ||
                viewType == HardwareViewType.Cooling ||
                viewType == HardwareViewType.Storage) {
                Storage.OnHardwareUpdate(HardwareUpdateType.Active);
            } else if (canUpdatePassiveHardware) {
                Storage.OnHardwareUpdate(HardwareUpdateType.Passive);
            }

            // Aggiorna le informazioni relative al processore.
            if (viewType == HardwareViewType.Dashboard ||
                viewType == HardwareViewType.Cooling ||
                viewType == HardwareViewType.Processor) {
                Processor.OnHardwareUpdate(HardwareUpdateType.Active);
            } else if (canUpdatePassiveHardware) {
                Processor.OnHardwareUpdate(HardwareUpdateType.Passive);
            }

            // Aggiorna le informazioni relative alla grafica.
            if (viewType == HardwareViewType.Dashboard ||
                viewType == HardwareViewType.Cooling ||
                viewType == HardwareViewType.Graphics) {
                Graphics.OnHardwareUpdate(HardwareUpdateType.Active);
            } else if (canUpdatePassiveHardware) {
                Graphics.OnHardwareUpdate(HardwareUpdateType.Passive);
            }

            // Aggiorna le informazioni relative alla scheda madre.
            if (viewType == HardwareViewType.Cooling || 
                viewType == HardwareViewType.Motherboard) {
                Motherboard.OnHardwareUpdate(HardwareUpdateType.Active);
            } else if (canUpdatePassiveHardware) {
                Motherboard.OnHardwareUpdate(HardwareUpdateType.Passive);
            }

            // Aggiorna le informazioni relative alla memoria.
            if (viewType == HardwareViewType.Dashboard ||
                viewType == HardwareViewType.Memory) {
                Memory.OnHardwareUpdate(HardwareUpdateType.Active);
            } else if (canUpdatePassiveHardware) {
                Memory.OnHardwareUpdate(HardwareUpdateType.Passive);
            }

            // Aggiorna i controlli grafici.
            if (viewUpdate != null && viewType != HardwareViewType.None) {
                Application.Current.Dispatcher.Invoke(new Action(() => { 
                    viewUpdate.OnViewUpdate(); 
                }));
            }
        }

        #endregion

        #region Controls
        /// <summary>
        /// Inizializza e avvia il gestore dell'hardware.
        /// </summary>
        public void Create()
        {
            // Inzializza tutti i tipi di dispositivi.
            Storage = new StorageData();
            Processor = new ProcessorData();
            Graphics = new GraphicsData();
            Motherboard = new MotherboardData();
            Memory = new MemoryData();
            Cooling = new CoolingData(new ITemperature[] {
                Processor, Motherboard, Graphics, Storage, 
            });

            // Aggiunge l'interfaccia di collegamento con OpenHardwareLib.
            foreach (var hardwareItem in computer.Hardware)
            {
                // Aggiorna il dispositivo per la prima volta.
                if (hardwareItem.HardwareType != HardwareType.SuperIO &&
                    hardwareItem.HardwareType != HardwareType.Heatmaster &&
                    hardwareItem.HardwareType != HardwareType.TBalancer) {
                    hardwareItem.Update();
                    hardwares.Add(hardwareItem);
                }

                // Assega il dispositivo alla classe di appartenenza.
                switch (hardwareItem.HardwareType)
                {
                    case HardwareType.Mainboard:
                        Motherboard.OnHardwareAdded(hardwareItem);
                        break;
                    case HardwareType.CPU:
                        Processor.OnHardwareAdded(hardwareItem);
                        Processor.OnProcessWatcherAdded(processesWatcher);
                        break;
                    case HardwareType.GpuAti:
                    case HardwareType.GpuNvidia:
                        Graphics.OnHardwareAdded(hardwareItem);
                        break;
                    case HardwareType.RAM:
                        Memory.OnHardwareAdded(hardwareItem);
                        Memory.OnProcessWatcherAdded(processesWatcher);
                        break;
                    case HardwareType.HDD:
                        Storage.OnHardwareAdded(hardwareItem);
                        break;
                }
            }

            // Importa le impostazioni.
            Storage.SetUpdateSpace(Properties.Settings.Default.UpdateDiskSpace);

            // Inzializza il sommario del computer.
            Computer = new ComputerData(this);

            // Avvia il timer di aggiornamento.
            updateTimer.Start(new Action(() => { 
                UpdateTimer(); 
            }), updateInterval);
        }

        /// <summary>
        /// Imposta la pagina indicata come attiva.
        /// </summary>
        public void ChangeView(IViewUpdate view, HardwareViewType viewType)
        {
            this.viewUpdate = view;
            this.viewType = viewType;
        }
        /// <summary>
        /// Imposta l'intervallo di aggiornamento.
        /// </summary>
        public void SetInterval(int updateInterval, int backgroundInterval)
        {
            this.updateInterval = updateInterval;
            this.backgroundInterval = backgroundInterval;

            updateTimer.SetInterval(updateInterval);
        }
        /// <summary>
        /// Indica se aggiornare i valori in background oppure no.
        /// </summary>
        public void SetBackgroundUpdate(bool isAllowed)
        {
            canBackgroundUpdate = isAllowed;
        } 
        /// <summary>
        /// Chiude il gestore dell'hardware.
        /// </summary>
        public void Close(Action callback)
        {
            Task.Run(() => {
                updateTimer.Stop();
                computer.Close();
                Memory.Dispose();
            }).ContinueWith(result => {
                Application.Current.Dispatcher.Invoke(callback); 
            });
        }
        /// <summary>
        /// Imposta uno stato di sospensione.
        /// </summary>
        public void Sleep()
        {
            isSleepMode = true;
        }
        /// <summary>
        /// Esce dallo stato di sospensione.
        /// </summary>
        public void Awake()
        {
            isSleepMode = false;
        }
        #endregion
    }
}
