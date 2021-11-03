using System;
using System.Management;
using System.Xml;
using System.Collections.Generic;
using HardwareWizard.Interfaces;
using HardwareWizard.Core.Helpers;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class GraphicsData : IDataUpdate, ITemperature
    {
        #region Properties
        public List<GraphicsCardData> Cards { get; set; }
        public List<GraphicsMonitorData> Monitors { get; set; }
        #endregion

        #region Variables
        private string[] videoArchitectures = new string[] {
            "Other", "Unknown", "CGA", "EGA", "VGA", "SVGA", "MDA", 
            "HGC", "MCGA", "8514A", "XGA", "Linear Frame Buffer", "PC-98"};

        private string[] videoMemoryTypes = new string[] {
            "Other", "Unknown", "VRAM", "DRAM", "SRAM", "WRAM",
            "EDO RAM", "Burst Synchronous DRAM", "Pipelined Burst SRAM",
            "CDRAM", "3DRAM", "SDRAM", "SGRAM"};

        private string[] scanModes = new string[] {
            "Other", "Unknown", "Interlaced", "Non Interlaced"};

        private int storagePairIndex = 0;
        #endregion

        public GraphicsData()
        {
            this.Cards = new List<GraphicsCardData>();
            this.Monitors = new List<GraphicsMonitorData>();

            // Aggiunge tutte le schede grafiche.
            ManagementClass mc = new ManagementClass("Win32_VideoController");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject obj in moc)
            {
                // Recupera le informazioni principali sulla scheda grafica.
                string model = WMIHelper.Retreive(obj, "Name", WMIValue.WMI_String);
                string videoProcessor = WMIHelper.Retreive(obj, "VideoProcessor", WMIValue.WMI_String);

                UInt16 videoArchitectureIndex = WMIHelper.GetUInt16(obj, "VideoArchitecture");
                string videoArchitecture = videoArchitectures[videoArchitectureIndex > 0 ? videoArchitectureIndex - 1 : 0];

                UInt16 videoMemoryTypeIndex = WMIHelper.GetUInt16(obj, "VideoMemoryType");
                string videoMemoryType = videoMemoryTypes[videoMemoryTypeIndex > 0 ? videoMemoryTypeIndex - 1 : 0];

                string bitsPerPixel = $"{WMIHelper.GetUInt32(obj, "CurrentBitsPerPixel")} bits";
                string horizontalResolution = $"{WMIHelper.GetUInt32(obj, "CurrentHorizontalResolution")} pixels";
                string verticalResolution = $"{WMIHelper.GetUInt32(obj, "CurrentVerticalResolution")} pixels";
                string refreshRate = $"{WMIHelper.GetUInt32(obj, "CurrentRefreshRate")} Hz";

                UInt16 scanModeIndex = WMIHelper.GetUInt16(obj, "CurrentScanMode");
                string scanMode = scanModes[scanModeIndex > 0 ? scanModeIndex - 1 : 0];

                // Identifica il produttore della scheda grafica.
                string manufacturer = "Unknown";
                string manufacturerId = "unknown";
                if (videoProcessor.ToLower().Contains("amd") || 
                    videoProcessor.ToLower().Contains("radeon") || 
                    videoProcessor.ToLower().Contains("ati")) {
                    manufacturer = "Advanced Micro Devices, Inc.";
                    manufacturerId = "amd";
                }
                else if (videoProcessor.ToLower().Contains("nvidia") || 
                    videoProcessor.ToLower().Contains("geforce") ||
                    videoProcessor.ToLower().Contains("quadro")) {
                    manufacturer = "NVIDIA Corporation";
                    manufacturerId = "nvidia";
                }
                else if (videoProcessor.ToLower().Contains("intel") || 
                    videoProcessor.ToLower().Contains("hd") ||
                    videoProcessor.ToLower().Contains("uhd") ||
                    videoProcessor.ToLower().Contains("iris")) {
                    manufacturer = "Intel";
                    manufacturerId = "intel";
                } 
                else {
                    manufacturer = "Unknown";
                    manufacturerId = "unknown";
                }

                // Crea il modello della scheda grafica.
                var card = new GraphicsCardData()
                {
                    Model = model,
                    Manufacturer = manufacturer,
                    ManufacturerId = manufacturerId,
                    VideoArchitecture = videoArchitecture,
                    VideoMemoryType = videoMemoryType,
                    BitsPerPixel = bitsPerPixel,
                    HorizontalResolution = horizontalResolution,
                    VerticalResolution = verticalResolution,
                    RefreshRate = refreshRate,
                    ScanMode = scanMode,
                    VideoProcessor = videoProcessor,
                };

                // Aggiunge al modello tutte le proprietà WMI disponibili.
                card.WMI.Add(WMIHelper.GetItem(obj, "AcceleratorCapabilities", WMIValue.WMI_UInt16_));
                card.WMI.Add(WMIHelper.GetItem(obj, "AdapterCompatibility", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "AdapterDACType", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "AdapterRAM", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "Availability", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "CapabilityDescriptions", WMIValue.WMI_String_));
                card.WMI.Add(WMIHelper.GetItem(obj, "Caption", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "ColorTableEntries", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "ConfigManagerErrorCode", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "ConfigManagerUserConfig", WMIValue.WMI_Boolean));
                card.WMI.Add(WMIHelper.GetItem(obj, "CreationClassName", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentBitsPerPixel", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentHorizontalResolution", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentNumberOfColors", WMIValue.WMI_UInt64));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentNumberOfColumns", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentNumberOfRows", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentRefreshRate", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentScanMode", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "CurrentVerticalResolution", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "Description", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "DeviceID", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "DeviceSpecificPens", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "DitherType", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "DriverDate", WMIValue.WMI_DateTime));
                card.WMI.Add(WMIHelper.GetItem(obj, "DriverVersion", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "ErrorCleared", WMIValue.WMI_Boolean));
                card.WMI.Add(WMIHelper.GetItem(obj, "ErrorDescription", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "ICMIntent", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "ICMMethod", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "InfFilename", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "InfSection", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "InstallDate", WMIValue.WMI_DateTime));
                card.WMI.Add(WMIHelper.GetItem(obj, "InstalledDisplayDrivers", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "LastErrorCode", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "MaxMemorySupported", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "MaxNumberControlled", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "MaxRefreshRate", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "MinRefreshRate", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "Monochrome", WMIValue.WMI_Boolean));
                card.WMI.Add(WMIHelper.GetItem(obj, "Name", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "NumberOfColorPlanes", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "NumberOfVideoPages", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "PNPDeviceID", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "PowerManagementCapabilities", WMIValue.WMI_UInt16_));
                card.WMI.Add(WMIHelper.GetItem(obj, "PowerManagementSupported", WMIValue.WMI_Boolean));
                card.WMI.Add(WMIHelper.GetItem(obj, "ProtocolSupported", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "ReservedSystemPaletteEntries", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "SpecificationVersion", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "Status", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "StatusInfo", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "SystemCreationClassName", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "SystemName", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "SystemPaletteEntries", WMIValue.WMI_UInt32));
                card.WMI.Add(WMIHelper.GetItem(obj, "TimeOfLastReset", WMIValue.WMI_DateTime));
                card.WMI.Add(WMIHelper.GetItem(obj, "VideoArchitecture", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "VideoMemoryType", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "VideoMode", WMIValue.WMI_UInt16));
                card.WMI.Add(WMIHelper.GetItem(obj, "VideoModeDescription", WMIValue.WMI_String));
                card.WMI.Add(WMIHelper.GetItem(obj, "VideoProcessor", WMIValue.WMI_String));

                // Aggiunge il modello alla lista.
                Cards.Add(card);
            }

            // Aggiunge tutti i monitor.
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\\\.\\ROOT\\WMI", "SELECT * FROM WMIMonitorID");
            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    // Recupera le informazioni principali sul monitor.
                    string model = WMIHelper.Retreive(obj, "UserFriendlyName", WMIValue.WMI_UInt16_);
                    string serialNumber = WMIHelper.Retreive(obj, "SerialNumberID", WMIValue.WMI_UInt16_);
                    string manufacturer = WMIHelper.Retreive(obj, "ManufacturerName", WMIValue.WMI_UInt16_);

                    manufacturer = GetMonitorManufacturer(manufacturer);
                    model = model.Equals("N/A") ? "Monitor" : model;

                    // Crea il modello del monitor.
                    var monitor = new GraphicsMonitorData()
                    {
                        Manufacturer = manufacturer,
                        Model = model,
                        SerialNumber = serialNumber,
                    };

                    // Aggiunge al modello tutte le proprietà WMI disponibili.
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "Active", WMIValue.WMI_Boolean));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "InstanceName", WMIValue.WMI_String));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "ManufacturerName", WMIValue.WMI_UInt16_));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "ProductCodeID", WMIValue.WMI_UInt16_));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "SerialNumberID", WMIValue.WMI_UInt16_));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "UserFriendlyName", WMIValue.WMI_UInt16));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "UserFriendlyNameLength	", WMIValue.WMI_UInt16));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "WeekOfManufacture", WMIValue.WMI_UInt8));
                    monitor.WMI.Add(WMIHelper.GetItem(obj, "YearOfManufacture", WMIValue.WMI_UInt16));

                    // Aggiunge il modello alla lista.
                    Monitors.Add(monitor);
                }
            } catch {         
            }
        }

        #region IDataUpdate
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiunto.
        /// </summary>
        public void OnHardwareAdded(IHardware hardware)
        {
            GraphicsCardData graphicCard = GetGraphicsCardPair(hardware.Name);
            if (graphicCard == null) return;
            graphicCard.Hardware = hardware;

            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Fan)
                {
                    graphicCard.Fans.Add(new CoolingFanData()
                    {
                        Name = sensor.Name,
                        Sensor = sensor
                    });
                }
                else if (sensor.SensorType == SensorType.Temperature)
                {
                    graphicCard.Thermals.Add(new CoolingThermalData()
                    {
                        Name = sensor.Name,
                        Sensor = sensor
                    });
                }
                else if (sensor.SensorType == SensorType.Clock)
                {
                    if (sensor.Name.ToLower().Equals("gpu memory")) {
                        graphicCard.MemorySensor = sensor;
                    }
                }
                else if (sensor.SensorType == SensorType.Load)
                {
                    if (sensor.Name.ToLower().Equals("gpu core")) {
                        graphicCard.UsageSensor = sensor;
                    }
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
            foreach (GraphicsCardData graphicsCard in Cards)
            {
                if (graphicsCard.Hardware == null) continue;

                // Aggiorna l'hardware.
                graphicsCard.Hardware.Update();

                // Aggiorna i valori di utilizzo.
                graphicsCard.UsageCollection.AddPoint((double)graphicsCard.Usage / 100.0);

                // Aggiorna i valori delle ventole.
                graphicsCard.Fans.ForEach(delegate (CoolingFanData fan) {
                    fan.SpeedData.AddPoint((double)fan.Speed / (double)fan.Maximum);
                });

                // Aggiorna i valori di sensori termici.
                graphicsCard.Thermals.ForEach(delegate (CoolingThermalData thermal) {
                    thermal.TemperatureData.AddPoint((double)thermal.Temperature / (double)thermal.Maximum);
                });
            }
        }
        #endregion

        #region ITemperature
        /// <summary>
        /// Ritorna le ventole appartenenti a questo dispositivo.
        /// </summary>
        public CoolingFanData[] GetHardwareFans()
        {
            var fans = new List<CoolingFanData>();
            foreach (GraphicsCardData graphicsCard in Cards) {
                if (graphicsCard.Hardware != null) {
                    fans.AddRange(graphicsCard.Fans);
                }
            }

            return fans.ToArray();
        }
        /// <summary>
        /// Ritorna i sensori termici appartenenti a questo dispositivo.
        /// </summary>
        public CoolingThermalGroup[] GetHardwareThermal()
        {
            int cardIndex = 0;
            bool isSingleCard = Cards.Count == 1;
            var thermalGroups = new List<CoolingThermalGroup>();

            foreach (GraphicsCardData graphicsCard in Cards) {
                if (graphicsCard.Hardware != null && graphicsCard.Thermals.Count > 0) {
                    var item = new CoolingThermalGroup() {
                        IsGroup = true,
                        Name = isSingleCard ? "GPU" : $"GPU #{cardIndex}",
                        HeaderIndex = -1,
                        HasHeader = true,
                        IsHeaderVisible = false,
                        ThermalDatas = graphicsCard.Thermals.ToArray()
                    };
                    thermalGroups.Add(item);
                }
                cardIndex++;
            }

            return thermalGroups.ToArray();
        }
        #endregion

        #region Helepers
        /// <summary>
        /// Ritorna la scheda grafica con il nome più simile a quello fornito.
        /// </summary>
        private GraphicsCardData GetGraphicsCardPair(string name)
        {
            GraphicsCardData matchDrive = null;
            double lastDriveMatch = -1;

            foreach (GraphicsCardData drive in Cards)
            {
                double match = Utils.CalculateSimilarity(drive.Model.ToLower(), name.ToLower());
                if (drive.Hardware == null && match >= 0.6 && match > lastDriveMatch)
                {
                    matchDrive = drive;
                    lastDriveMatch = match;
                }
            }

            if (lastDriveMatch != -1)
            {
                storagePairIndex += 1;
                return matchDrive;
            }

            if (storagePairIndex + 1 <= Cards.Count - 1) {
                return Cards[storagePairIndex++];
            } else {
                return Cards[storagePairIndex];
            }
        }
        /// <summary>
        /// Ritorna il nome completo del produttore del monitor in base
        /// ai tre caratteri di riconoscimento.
        /// </summary>
        private string GetMonitorManufacturer(string id)
        {
            XmlDocument document = new XmlDocument();
            document.Load("Resources\\monitors.xml");

            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                if (node.Attributes["id"]?.InnerText == id)
                {
                    return node.Attributes["manufacturer"]?.InnerText;
                }
            }

            return "Unknown";
        }
        #endregion
    }
}
