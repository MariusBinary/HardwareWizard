using System.Management;
using System.Collections.Generic;
using HardwareWizard.Interfaces;
using OpenHardwareMonitor.Hardware;
using HardwareWizard.Core.Helpers;

namespace HardwareWizard.Core
{
    public class MotherboardData : IDataUpdate, ITemperature
    {
        #region Properties
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public string SerialNumber { get; set; }
        public string BIOSBrand { get; set; }
        public string BIOSVersion { get; set; }
        public string BIOSDate { get; set; }
        public List<CoolingThermalData> Thermals { get; set; }
        public List<CoolingFanData> Fans { get; set; }
        public List<WMICollection> WMI { get; set; }
        #endregion

        #region Variables
        private IHardware hardware;
        #endregion

        #region Main
        public MotherboardData()
        {
            this.Thermals = new List<CoolingThermalData>();
            this.Fans = new List<CoolingFanData>();
            this.WMI = new List<WMICollection>();

            // Aggiunge la scheda madre.
            ManagementObjectSearcher board = new ManagementObjectSearcher(
                "Select * from Win32_BaseBoard");
            foreach (ManagementObject obj in board.Get())
            {
                Manufacturer = WMIHelper.Retreive(obj, "Manufacturer", WMIValue.WMI_String);
                Model = WMIHelper.Retreive(obj, "Product", WMIValue.WMI_String);
                Version = WMIHelper.Retreive(obj, "Version", WMIValue.WMI_String);
                SerialNumber = WMIHelper.Retreive(obj, "SerialNumber", WMIValue.WMI_String);

                // Aggiunge tutte le proprietà WMI disponibili.
                WMI.Add(WMIHelper.GetItem(obj, "Caption", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "ConfigOptions", WMIValue.WMI_String_));
                WMI.Add(WMIHelper.GetItem(obj, "CreationClassName", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Depth", WMIValue.WMI_Real32));
                WMI.Add(WMIHelper.GetItem(obj, "Description", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Height", WMIValue.WMI_Real32));
                WMI.Add(WMIHelper.GetItem(obj, "HostingBoard", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "HotSwappable", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "InstallDate", WMIValue.WMI_DateTime));
                WMI.Add(WMIHelper.GetItem(obj, "Manufacturer", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Model", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Name", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "OtherIdentifyingInfo", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "PartNumber", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "PoweredOn", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "Product", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Removable", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "Replaceable", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "RequirementsDescription", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "RequiresDaughterBoard", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "SerialNumber", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SKU", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SlotLayout", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "SpecialRequirements", WMIValue.WMI_Boolean));
                WMI.Add(WMIHelper.GetItem(obj, "Status", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Tag", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Version", WMIValue.WMI_String));
                WMI.Add(WMIHelper.GetItem(obj, "Weight", WMIValue.WMI_Real32));
                WMI.Add(WMIHelper.GetItem(obj, "Width", WMIValue.WMI_Real32));
            }

            // Aggiungi le informazioni sul BIOS.
            ManagementObjectSearcher bios = new ManagementObjectSearcher(
                "Select * from Win32_BIOS");
            foreach (ManagementObject obj in bios.Get())
            {
                BIOSBrand = WMIHelper.Retreive(obj, "Manufacturer", WMIValue.WMI_String);
                BIOSDate = WMIHelper.Retreive(obj, "ReleaseDate", WMIValue.WMI_DateTime); 
                BIOSVersion = WMIHelper.Retreive(obj, "SMBIOSBIOSVersion", WMIValue.WMI_String);
            }
        }
        #endregion

        #region IDataUpdate
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiunto.
        /// </summary>
        public void OnHardwareAdded(IHardware hardware)
        {
            this.hardware = hardware;

            foreach (IHardware subHardware in hardware.SubHardware)
            {
                subHardware.Update();
                foreach (var sensor in subHardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Fan)
                    {
                        Fans.Add(new CoolingFanData() {
                            Name = sensor.Name,
                            Sensor = sensor
                        });
                    }
                    else if (sensor.SensorType == SensorType.Temperature)
                    {
                        Thermals.Add(new CoolingThermalData() {
                            Name = sensor.Name,
                            Sensor = sensor
                        });
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
            if (hardware == null) return;

            // Aggiorna l'hardware.
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) {
                subHardware.Update();
            }

            // Aggiorna i valori delle ventole.
            Fans.ForEach(delegate (CoolingFanData fan) {
                fan.SpeedData.AddPoint((double)fan.Speed / (double)fan.Maximum);
            });

            // Aggiorna i valori delle ventole.
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
                    Name = "Motherboard",
                    HeaderIndex = -1,
                    HasHeader = false,
                    IsHeaderVisible = false,
                    ThermalDatas = Thermals.ToArray()
                }
            };
        }
        #endregion
    }
}
