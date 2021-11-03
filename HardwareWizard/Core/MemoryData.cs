using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;
using Microsoft.VisualBasic.Devices;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class MemoryData : IDataUpdate, IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out ulong memoryInKilobytes);

        #region Properties
        public ulong osTotalMemory { get; private set; }
        public ulong ModifiedBytes { get; private set; }
        public ulong InUseBytes { get; private set; }
        public ulong StandbyBytes { get; private set; }
        public ulong FreeBytes { get; private set; }
        public ulong HardwareReserved { get; }
        public List<MemoryBankData> Banks { get; set; }
        public GraphCollection UsageCollection { get; set; }
        public List<ProcessesWatcherData> Processes { get; set; }
        #endregion

        #region Variables
        private readonly PerformanceCounter availableCounter;
        private readonly PerformanceCounter modifiedCounter;
        private readonly PerformanceCounter freeCounter;
        private readonly PerformanceCounter standbyCoreCounter;
        private readonly PerformanceCounter standbyNormalCounter;
        private readonly PerformanceCounter standbyReserveCounter;

        private string[] memoryTypes = new string[] {
            "Unknown", "Other", "DRAM", "Synchronous DRAM", "Cache DRAM", "EDO", "EDRAM",
            "VRAM", "SRAM", "RAM", "ROM", "Flash", "EEPROM", "FEPROM", "EPROM", "CDRAM",
            "3DRAM", "SDRAM", "SGRAM", "RDRAM", "DDR", "DDR2", "FB-DIMM", "DDR3", "FBD2"
        };

        private string[] formFactors = new string[] {
           "Unknown", "Other", "SIP", "DIP", "ZIP", "SOJ", "Proprietary", "SIMM", "DIMM",
           "TSOP", "PGA", "RIMM", "SODIMM", "SRIMM", "SMD", "SSMP", "QFP", "TQFP", "SOIC",
           "LCC", "PLCC", "BGA", "FPBGA", "LGA", "FB-DIMM"
        };

        private IHardware hardware;
        private ProcessesWatcher processesWatcher;
        #endregion

        #region Main
        public MemoryData()
        {
            this.Processes = new List<ProcessesWatcherData>();
            this.Banks = new List<MemoryBankData>();
            this.UsageCollection = new GraphCollection();
            this.osTotalMemory = new ComputerInfo().TotalPhysicalMemory;

            ulong installedPhysicalMemInKb;
            GetPhysicallyInstalledSystemMemory(out installedPhysicalMemInKb);

            this.HardwareReserved = installedPhysicalMemInKb * 1024 - osTotalMemory;

            this.modifiedCounter = new PerformanceCounter("Memory", "Modified Page List Bytes", true);
            this.standbyCoreCounter = new PerformanceCounter("Memory", "Standby Cache Core Bytes", true);
            this.standbyNormalCounter = new PerformanceCounter("Memory", "Standby Cache Normal Priority Bytes", true);
            this.standbyReserveCounter = new PerformanceCounter("Memory", "Standby Cache Reserve Bytes", true);
            this.freeCounter = new PerformanceCounter("Memory", "Free & Zero Page List Bytes", true);
            this.availableCounter = new PerformanceCounter("Memory", "Available Bytes", true);

            // Aggiunge tutti i banchi di ram.
            ManagementClass mc = new ManagementClass("Win32_PhysicalMemory");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject obj in moc)
            {
                // Recupera le informazioni principali sul banco di ram.
                string model = WMIHelper.Retreive(obj, "PartNumber", WMIValue.WMI_String);

                UInt16 memoryTypeIndex = WMIHelper.GetUInt16(obj, "MemoryType");
                string memoryType = memoryTypes[memoryTypeIndex > 0 ? memoryTypeIndex - 1 : 0];

                UInt16 formFactorIndex = WMIHelper.GetUInt16(obj, "FormFactor");
                string formFactor = formFactors[formFactorIndex > 0 ? formFactorIndex - 1 : 0];

                // Identifica il produttore della scheda grafica.
                string manufacturer = WMIHelper.Retreive(obj, "Manufacturer", WMIValue.WMI_String);
                if (manufacturer.Equals("N/A") || manufacturer.ToLower().Equals("unknown"))
                {
                    if (model.ToLower().StartsWith("ksm") || 
                        model.ToLower().StartsWith("ksr")) {
                        manufacturer = "Kingston® ValueRAM®";
                    }
                    else if (model.ToLower().StartsWith("hx")) {
                        manufacturer = "Kingston® HyperX®";
                    }
                    else if (model.ToLower().StartsWith("cm")) {
                        manufacturer = "Corsair";
                    }
                    else if (model.ToLower().StartsWith("ct")) {
                        manufacturer = "Crucial";
                    }
                    else if (model.ToLower().StartsWith("blt")) {
                        manufacturer = "Ballistix";
                    }
                    else if (model.ToLower().StartsWith("f1") ||
                        model.ToLower().StartsWith("f2") ||
                        model.ToLower().StartsWith("f3") ||
                        model.ToLower().StartsWith("f4")) {
                        manufacturer = "G.Skill";
                    }
                    else if (model.ToLower().StartsWith("k")) {
                        manufacturer = "Samsung";
                    }
                    else {
                        manufacturer = "Unknown";
                    }
                }

                // Crea il modello del banco di ram.
                var bank = new MemoryBankData()
                {
                    Model = model,
                    Manufacturer = manufacturer,
                    MemoryType = memoryType,
                    FormFactor = formFactor,
                    Speed = $"{WMIHelper.GetUInt32(obj, "Speed")} MHz",
                    Capacity = Utils.SizeSuffix(WMIHelper.GetUInt64(obj, "Capacity")),
                    Position = WMIHelper.Retreive(obj, "DeviceLocator", WMIValue.WMI_String),
                    Channel = WMIHelper.Retreive(obj, "BankLabel", WMIValue.WMI_String),
                };

                // Aggiunge al modello tutte le proprietà WMI disponibili.
                bank.WMI.Add(WMIHelper.GetItem(obj, "Attributes", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "BankLabel", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Capacity", WMIValue.WMI_UInt64));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Caption", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "ConfiguredClockSpeed", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "ConfiguredVoltage", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "CreationClassName", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "DataWidth", WMIValue.WMI_UInt16));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Description", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "DeviceLocator", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "FormFactor", WMIValue.WMI_UInt16));
                bank.WMI.Add(WMIHelper.GetItem(obj, "HotSwappable", WMIValue.WMI_Boolean));
                bank.WMI.Add(WMIHelper.GetItem(obj, "InstallDate", WMIValue.WMI_DateTime));
                bank.WMI.Add(WMIHelper.GetItem(obj, "InterleaveDataDepth", WMIValue.WMI_UInt16));
                bank.WMI.Add(WMIHelper.GetItem(obj, "InterleavePosition", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Manufacturer", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "MaxVoltage", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "MemoryType", WMIValue.WMI_UInt16));
                bank.WMI.Add(WMIHelper.GetItem(obj, "MinVoltage", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Model", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Name", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "OtherIdentifyingInfo", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "PartNumber", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "PositionInRow", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "PoweredOn", WMIValue.WMI_Boolean));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Removable", WMIValue.WMI_Boolean));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Replaceable", WMIValue.WMI_Boolean));
                bank.WMI.Add(WMIHelper.GetItem(obj, "SerialNumber", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "SKU", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "SMBIOSMemoryType", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Speed", WMIValue.WMI_UInt32));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Status", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Tag", WMIValue.WMI_String));
                bank.WMI.Add(WMIHelper.GetItem(obj, "TotalWidth", WMIValue.WMI_UInt16));
                bank.WMI.Add(WMIHelper.GetItem(obj, "TypeDetail", WMIValue.WMI_UInt16));
                bank.WMI.Add(WMIHelper.GetItem(obj, "Version", WMIValue.WMI_String));

                // Aggiunge il modello alla lista.
                Banks.Add(bank);
            }
        }

        public void Dispose()
        {
            modifiedCounter.Dispose();
            standbyCoreCounter.Dispose();
            standbyNormalCounter.Dispose();
            standbyReserveCounter.Dispose();
            freeCounter.Dispose();
            availableCounter.Dispose();
        }
        #endregion

        #region IDataUpdate
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiunto.
        /// </summary>
        public void OnHardwareAdded(IHardware hardware)
        {
            this.hardware = hardware;
        }
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato rimosso.
        /// </summary>
        public void OnHardwareRemoved(IHardware hardware)
        {
            // Nessuna azione.
        }
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiornato.
        /// </summary>
        public void OnHardwareUpdate(HardwareUpdateType updateType)
        {
            if (updateType == HardwareUpdateType.Active || hardware == null)
            {
                // Aggiorna lo stato della memoria in maniera dettagliata.
                ModifiedBytes = (ulong)modifiedCounter.NextSample().RawValue;
                StandbyBytes = (ulong)standbyCoreCounter.NextSample().RawValue +
                               (ulong)standbyNormalCounter.NextSample().RawValue +
                               (ulong)standbyReserveCounter.NextSample().RawValue;
                FreeBytes = (ulong)freeCounter.NextSample().RawValue;
                InUseBytes = osTotalMemory - (ulong)availableCounter.NextSample().RawValue;

                double usageValue = (InUseBytes / (double)osTotalMemory) + (ModifiedBytes / (double)osTotalMemory);
                UsageCollection.AddPoint(usageValue);

                if (processesWatcher.Processes.Count >= 5) {
                    processesWatcher.Processes.Sort((x, y) => y.RAM.CompareTo(x.RAM));
                    Processes = processesWatcher.Processes.GetRange(0, 5);
                }
            } 
            else 
            {
                // Aggiorna lo stato della memoria in maniera superficiale.
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load && sensor.Name.ToLower().Equals("memory"))
                    {
                        UsageCollection.AddPoint(sensor.Value.HasValue ? ((double)sensor.Value / 100.0) : 0.0);
                    }
                }
            }
        }
        #endregion

        #region IProcessData
        /// <summary>
        /// Il gestore dei processi è stato inizializzato.
        /// </summary>
        public void OnProcessWatcherAdded(ProcessesWatcher processesWatcher)
        {
            this.processesWatcher = processesWatcher;
        }
        #endregion
    }
}
