using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class ProcessorData : IDataUpdate, ITemperature
    {
        #region Properties
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerId { get; set; }
        public List<ProcessesWatcherData> Processes { get; set; }
        public double PrivilegedUsage { get; private set; }
        public double UserUsage { get; private set; }
        public GraphCollection UsageCollection { get; set; }
        public double Usage { get; set; }
        public string CoresCount { get; set; }
        public string ThreadsCount { get; set; }
        public string ClockSpeed { get; set; }
        public string MaxClockSpeed { get; set; }
        public string L2Cache { get; set; }
        public string L3Cache { get; set; }
        public bool IsVirtualizationEnabled { get; set; }
        public List<CoolingThermalData> Thermals { get; set; }
        public List<CoolingFanData> Fans { get; set; }
        public List<WMICollection> WMI { get; set; }
        #endregion

        #region Variables
        private IHardware hardware;
        private readonly PerformanceCounter privilegedCounter;
        private readonly PerformanceCounter userCounter;
        private ProcessesWatcher processesWatcher;
        #endregion

        public ProcessorData()
        {
            this.Processes = new List<ProcessesWatcherData>();
            this.UsageCollection = new GraphCollection();
            this.Thermals = new List<CoolingThermalData>();
            this.Fans = new List<CoolingFanData>();
            this.WMI = new List<WMICollection>();

            this.privilegedCounter = new PerformanceCounter("Processor", "% Privileged time", "_Total", true);
            this.userCounter = new PerformanceCounter("Processor", "% User time", "_Total", true);

            // Aggiunge il processore.
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject obj in moc)
            {
                // Recupera le informazioni principali sul processore.
                CoresCount = WMIHelper.GetUInt32(obj, "NumberOfCores").ToString();
                ThreadsCount = WMIHelper.GetUInt32(obj, "NumberOfLogicalProcessors").ToString();
                IsVirtualizationEnabled = WMIHelper.GetBoolean(obj, "VirtualizationFirmwareEnabled");
                L2Cache = Utils.SizeSuffix(WMIHelper.GetUInt32(obj, "L2CacheSize") * 1024);
                L3Cache = Utils.SizeSuffix(WMIHelper.GetUInt32(obj, "L3CacheSize") * 1024);
                ClockSpeed = $"{WMIHelper.GetUInt32(obj, "CurrentClockSpeed")} MHz";
                MaxClockSpeed = $"{WMIHelper.GetUInt32(obj, "MaxClockSpeed")} MHz";

                Model = WMIHelper.Retreive(obj, "Name", WMIValue.WMI_String)
                    .TrimEnd().Replace("(TM)", "™").Replace("(tm)", "™") 
                    .Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©")
                    .Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                // Identifica il produttore della scheda grafica.
                if (Model.ToLower().Contains("amd") ||
                    Model.ToLower().Contains("ryzen")) {
                    Manufacturer = "Advanced Micro Devices, Inc.";
                    ManufacturerId = "amd";
                }
                else if (Model.ToLower().Contains("intel")) {
                    Manufacturer = "Intel Corporation";
                    ManufacturerId = "intel";
                } 
                else {
                    Manufacturer = "Unknown";
                    ManufacturerId = "unknown";
                }

                // Aggiunge tutte le proprietà WMI disponibili.
                WMI.Add(WMIHelper.GetItem(obj, "AddressWidth", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Architecture", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "AssetTag", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Availability", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Caption", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Characteristics", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "ConfigManagerErrorCode", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "ConfigManagerUserConfig", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "CpuStatus", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "CreationClassName", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "CurrentClockSpeed", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "CurrentVoltage", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "DataWidth", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Description", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "DeviceID", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "ErrorCleared", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "ErrorDescription", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "ExtClock", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "Family", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "InstallDate", WMIValue.WMI_DateTime));
                WMI.Add(WMIHelper.GetItem(obj, "L2CacheSize", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "L2CacheSpeed", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "L3CacheSize", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "L3CacheSpeed", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "LastErrorCode", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "Level", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "LoadPercentage", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Manufacturer", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "MaxClockSpeed", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "Name", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "NumberOfCores", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "NumberOfEnabledCore", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "NumberOfLogicalProcessors", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "OtherFamilyDescription", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "PartNumber", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "PNPDeviceID", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "PowerManagementCapabilities", WMIValue.WMI_UInt16_));
                WMI.Add(WMIHelper.GetItem(obj, "PowerManagementSupported", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "ProcessorId", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "ProcessorType", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Revision", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Role", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SecondLevelAddressTranslationExtensions	", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "SerialNumber", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SocketDesignation", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Status", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "StatusInfo", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Stepping", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SystemCreationClassName", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SystemName", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "ThreadCount", WMIValue.WMI_UInt32));
                WMI.Add(WMIHelper.GetItem(obj, "UniqueId", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "UpgradeMethod", WMIValue.WMI_UInt16));
                WMI.Add(WMIHelper.GetItem(obj, "Version", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "VirtualizationFirmwareEnabled", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "VMMonitorModeExtensions", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "VoltageCaps", WMIValue.WMI_UInt32));
            }
        }

        #region IDataUpdate
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiunto.
        /// </summary>
        public void OnHardwareAdded(IHardware hardware)
        {
            this.hardware = hardware;

            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Fan)
                {
                    Fans.Add(new CoolingFanData()
                    {
                        Name = sensor.Name,
                        Sensor = sensor
                    });
                }
                else if (sensor.SensorType == SensorType.Temperature)
                {
                    Thermals.Add(new CoolingThermalData()
                    {
                        Name = sensor.Name,
                        Sensor = sensor
                    });
                }
            }
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
            if (updateType == HardwareUpdateType.Active)
            {
                if (processesWatcher.Processes.Count >= 5) {
                    processesWatcher.Processes.Sort((x, y) => y.CPU.CompareTo(x.CPU));
                    Processes = processesWatcher.Processes.GetRange(0, 5);
                }
            }

            // Aggiorna i valori di utilizzo.
            PrivilegedUsage = (double)privilegedCounter.NextValue();
            UserUsage = (double)userCounter.NextValue();
            Usage = PrivilegedUsage + UserUsage;
            UsageCollection.AddPoint(Usage / 100.0);

            if (hardware == null) return;

            // Aggiorna l'hardware.
            hardware.Update();

            // Aggiorna i valori delle ventole.
            Fans.ForEach(delegate (CoolingFanData fan) {
                fan.SpeedData.AddPoint((double)fan.Speed / (double)fan.Maximum);
            });

            // Aggiorna i valori di sensori termici.
            Thermals.ForEach(delegate (CoolingThermalData thermal) {
                thermal.TemperatureData.AddPoint((double)thermal.Temperature / (double)thermal.Maximum);
            });
        }
        #endregion

        #region ITemperature
        /// <summary>
        /// Ritorna le ventole appartenenti a questo dispositivo.
        /// </summary>
        public CoolingFanData[] GetHardwareFans()
        {
            return Fans.ToArray();
        }
        /// <summary>
        /// Ritorna i sensori termici appartenenti a questo dispositivo.
        /// </summary>
        public CoolingThermalGroup[] GetHardwareThermal()
        {
            if (Thermals.Count == 0) return null;
            return new CoolingThermalGroup[] {
                new CoolingThermalGroup() {
                    IsGroup = true,
                    Name = "CPU",
                    HeaderIndex = -1,
                    HasHeader = true,
                    IsHeaderVisible = false,
                    ThermalDatas = Thermals.ToArray()
                }
            };
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
