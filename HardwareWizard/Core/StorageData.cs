using System;
using System.Collections.Generic;
using System.Management;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class StorageData : IDataUpdate, ITemperature
    {
        #region Properties
        public List<StorageDriveData> Drives { get; set; }
        #endregion

        #region Variables
        private int storagePairIndex = 0;
        private bool canUpdateSpace = true;
        #endregion

        #region Main
        public StorageData()
        {
            this.Drives = new List<StorageDriveData>();

            // Recupera tutte le unità di archiviazione fisiche.
            ManagementObjectSearcher hdd = new ManagementObjectSearcher(
                "Select * from Win32_DiskDrive");
            foreach (ManagementObject obj in hdd.Get())
            {
                // Recupera le informazioni principali sull'unità di archiviazione.
                string model = WMIHelper.Retreive(obj, "Model", WMIValue.WMI_String).Replace(" ATA Device", "");
                string serialNumber = WMIHelper.Retreive(obj, "SerialNumber", WMIValue.WMI_String);
                ulong capacity = WMIHelper.GetUInt64(obj, "Size");
                uint index = WMIHelper.GetUInt32(obj, "Index");
                string location = WMIHelper.Retreive(obj, "MediaType", WMIValue.WMI_String);
                location = location.ToLower().Contains("fixed") ? "Internal" : "External";
                string interfaceType = WMIHelper.Retreive(obj, "InterfaceType", WMIValue.WMI_String);
                string mediaType = WMIHelper.Retreive(obj, "MediaType", WMIValue.WMI_String);

                // Identifica il produttore dell'unità di archiviazione.
                string manufacturer = "Unknown";
                if (model.ToLower().Contains("samsung") ){
                    manufacturer = "Samsung";
                }
                else if (model.ToLower().Contains("sandisk")){
                    manufacturer = "SanDisk";
                }
                else if (model.ToLower().Contains("wdc")) {
                    manufacturer = "Western Digital";
                }
                else if (model.ToLower().Contains("toshiba")) {
                    manufacturer = "Toshiba";
                }
                else if (model.ToLower().Contains("kingston")) {
                    manufacturer = "Kingston";
                }
                else if (model.ToLower().Contains("intel")) {
                    manufacturer = "Intel";
                }
                else if (model.ToLower().Contains("maxtor")) {
                    manufacturer = "Maxtor";
                }
                else if (model.ToLower().StartsWith("st") ||
                    model.ToLower().Contains("seagate")) {
                    manufacturer = "Seagate";
                }
                else if (model.ToLower().StartsWith("hts") ||
                    model.ToLower().Contains("hgst")) {
                    manufacturer = "Hitachi";
                }
                else if (model.ToLower().StartsWith("crucial") ||
                    model.ToLower().Contains("crucial")) {
                    manufacturer = "Crucial";
                }
                else if (model.ToLower().Contains("spcc")) {
                    manufacturer = "Silicon Power";
                }
                else {
                    manufacturer = "Unknown";
                }

                // Crea il modello dell'unità di archiviazione.
                var drive = new StorageDriveData()
                {
                    Model = model,
                    Manufacturer = manufacturer,
                    Serial = serialNumber,
                    Capacity = (long)capacity,
                    Index = (int)index,
                    Location = location,
                    Interface = interfaceType,
                    MediaType = mediaType,
                    Partitions = new List<StoragePartitionData>()
                };

                // Recupera tutte le partitizioni relative all'unità di archiviazione.
                ManagementObjectSearcher partitions = new ManagementObjectSearcher(
                    $"Select * From Win32_DiskPartition Where DiskIndex='{obj["Index"]}'");
                foreach (ManagementObject part in partitions.Get())
                {
                    string deviceID = WMIHelper.Retreive(part, "DeviceID", WMIValue.WMI_String);
                    bool isBootable = WMIHelper.GetBoolean(part, "Bootable");
                    bool IsBootParition = WMIHelper.GetBoolean(part, "BootPartition");
                    string partitionName = "";

                    if (isBootable && IsBootParition) {
                        partitionName = "Recovery";
                    } else {
                        partitionName = GetParitionName(deviceID);
                    }

                    string volumeName = GetPartitionVolumeName(partitionName);
                    if (String.IsNullOrEmpty(volumeName)) {
                        bool isParitionNameEmpty = String.IsNullOrEmpty(partitionName);
                        string diskName = isParitionNameEmpty ? "Unnamed" : partitionName;
                        volumeName = $"Local Disk ({diskName})";
                    } else {
                        volumeName = $"{volumeName} ({partitionName})";
                    }

                    Tuple<long, long> paritionData = GetPartitionUsedSpace(partitionName);
                    if (paritionData.Item2 == 0) {
                        paritionData = new Tuple<long, long>(paritionData.Item1, (long)WMIHelper.GetUInt64(part, "Size"));
                    }

                    drive.UsedSpace += paritionData.Item1;
                    drive.Partitions.Add(new StoragePartitionData()
                    {
                        DeviceID = deviceID,
                        Name = volumeName,
                        PartitionName = partitionName,
                        UsedSpace = paritionData.Item1,
                        Capacity = paritionData.Item2
                    });
                }

                // Aggiunge al modello tutte le proprietà WMI disponibili.
                drive.WMI.Add(WMIHelper.GetItem(obj, "Availability", WMIValue.WMI_UInt16));
                drive.WMI.Add(WMIHelper.GetItem(obj, "BytesPerSector", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Capabilities", WMIValue.WMI_UInt16_));
                drive.WMI.Add(WMIHelper.GetItem(obj, "CapabilityDescriptions", WMIValue.WMI_String_));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Caption", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "CompressionMethod", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "ConfigManagerErrorCode", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "ConfigManagerUserConfig", WMIValue.WMI_Boolean));
                drive.WMI.Add(WMIHelper.GetItem(obj, "CreationClassName", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "DefaultBlockSize", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Description", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "DeviceID", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "ErrorCleared", WMIValue.WMI_Boolean));
                drive.WMI.Add(WMIHelper.GetItem(obj, "ErrorDescription", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "ErrorMethodology", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "FirmwareRevision", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Index", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "InstallDate", WMIValue.WMI_DateTime));
                drive.WMI.Add(WMIHelper.GetItem(obj, "InterfaceType", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "LastErrorCode", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Manufacturer", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "MaxBlockSize", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "MaxMediaSize", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "MediaLoaded", WMIValue.WMI_Boolean));
                drive.WMI.Add(WMIHelper.GetItem(obj, "MediaType", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "MinBlockSize", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Model", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Name", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "NeedsCleaning", WMIValue.WMI_Boolean));
                drive.WMI.Add(WMIHelper.GetItem(obj, "NumberOfMediaSupported", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Partitions", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "PNPDeviceID", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "PowerManagementCapabilities", WMIValue.WMI_UInt16_));
                drive.WMI.Add(WMIHelper.GetItem(obj, "PowerManagementSupported", WMIValue.WMI_Boolean));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SCSIBus", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SCSILogicalUnit", WMIValue.WMI_UInt16));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SCSIPort", WMIValue.WMI_UInt16));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SCSITargetId", WMIValue.WMI_UInt16));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SectorsPerTrack", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SerialNumber", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Signature", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Size", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "Status", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "StatusInfo", WMIValue.WMI_UInt16));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SystemCreationClassName", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "SystemName", WMIValue.WMI_String));
                drive.WMI.Add(WMIHelper.GetItem(obj, "TotalCylinders", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "TotalHeads", WMIValue.WMI_UInt32));
                drive.WMI.Add(WMIHelper.GetItem(obj, "TotalSectors", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "TotalTracks", WMIValue.WMI_UInt64));
                drive.WMI.Add(WMIHelper.GetItem(obj, "TracksPerCylinder", WMIValue.WMI_UInt32));

                // Aggiunge il modello alla lista.
                Drives.Add(drive);
            }
        }
        #endregion

        #region IDataUpdate
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiunto.
        /// </summary>
        public void OnHardwareAdded(IHardware hardware)
        {
            StorageDriveData storageDrive = GetStorageDrivePair(hardware.Name);
            if (storageDrive == null) return;
            storageDrive.Hardware = hardware;

            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Fan)
                {
                    storageDrive.Fans.Add(new CoolingFanData()
                    {
                        Name = sensor.Name,
                        Sensor = sensor
                    });
                }
                else if (sensor.SensorType == SensorType.Temperature)
                {
                    storageDrive.Thermals.Add(new CoolingThermalData()
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
            foreach (StorageDriveData storageDrive in Drives)
            {
                if (canUpdateSpace)
                {
                    storageDrive.UsedSpace = 0;
                    foreach (StoragePartitionData partition in storageDrive.Partitions)
                    {
                        long partitionUsed = GetPartitionUsedSpace(partition.PartitionName).Item1;
                        storageDrive.UsedSpace += partitionUsed;
                        partition.UsedSpace = partitionUsed;
                    }
                }

                if (storageDrive.Hardware == null) continue;

                // Aggiorna l'hardware.
                storageDrive.Hardware.Update();

                // Aggiorna i valori delle ventole.
                storageDrive.Fans.ForEach(delegate (CoolingFanData fan) {
                    fan.SpeedData.AddPoint((double)fan.Speed / (double)fan.Maximum);
                });

                // Aggiorna i valori di sensori termici.
                storageDrive.Thermals.ForEach(delegate (CoolingThermalData thermal) {
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
            foreach (StorageDriveData storageDrive in Drives)
            {
                if (storageDrive.Fans.Count > 0)
                {
                    fans.AddRange(storageDrive.Fans);
                }
            }

            return fans.ToArray();
        }
        /// <summary>
        /// Ritorna i sensori termici appartenenti a questo dispositivo.
        /// </summary>
        public CoolingThermalGroup[] GetHardwareThermal()
        {
            var thermalGroups = new List<CoolingThermalGroup>();
            foreach (StorageDriveData storageDrive in Drives) {
                if (storageDrive.Hardware != null && storageDrive.Thermals.Count > 0) {
                    thermalGroups.Add(new CoolingThermalGroup() {
                        IsGroup = true,
                        Name = storageDrive.Model,
                        HeaderIndex = -1,
                        HasHeader = true,
                        IsHeaderVisible = false,
                        ThermalDatas = storageDrive.Thermals.ToArray()
                    });
                }
            }

            return thermalGroups.ToArray();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Ritorna la l'unità di archiviazione con il nome più simile a quello fornito.
        /// </summary>
        private StorageDriveData GetStorageDrivePair(string name)
        {
            StorageDriveData matchDrive = null;
            double lastDriveMatch = -1;

            foreach (StorageDriveData drive in Drives)
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

            if (storagePairIndex + 1 <= Drives.Count - 1) {
                return Drives[storagePairIndex++];
            } else {
                return Drives[storagePairIndex];
            }
        }
        private Tuple<long, long> GetPartitionUsedSpace(string inp)
        {
            ManagementObjectSearcher getspace = new ManagementObjectSearcher(
                $"Select * from Win32_LogicalDisk Where DeviceID='{inp}'");
            foreach (ManagementObject drive in getspace.Get())
            {
                if (drive["DeviceID"].ToString() == inp)
                {
                    long sFree = Convert.ToInt64(drive["FreeSpace"]);
                    long sTotal = Convert.ToInt64(drive["Size"]);
                    return new Tuple<long, long>(sTotal - sFree, sTotal);
                }
            }
            return new Tuple<long, long>(0, 0);
        }
        private string GetPartitionVolumeName(string inp)
        {
            ManagementObjectSearcher getspace = new ManagementObjectSearcher(
                $"Select * from Win32_LogicalDisk Where DeviceID='{inp}'");
            foreach (ManagementObject drive in getspace.Get())
            {
                return drive["VolumeName"].ToString();
            }
            return null;
        }
        private string GetParitionName(string inp)
        {
            string ret = "";
            ManagementObjectSearcher LogicalDisk = new ManagementObjectSearcher(
                "Select * from Win32_LogicalDiskToPartition");
            foreach (ManagementObject drive in LogicalDisk.Get())
            {
                if (drive["Antecedent"].ToString().Contains(inp))
                {
                    string Dependent = drive["Dependent"].ToString();
                    ret = Dependent.Substring(Dependent.Length - 3, 2);
                    break;
                }

            }
            return ret;
        }
        #endregion

        #region Controls
        public void SetUpdateSpace(bool isAllowed)
        {
            canUpdateSpace = isAllowed;
        }
        #endregion
    }
}
