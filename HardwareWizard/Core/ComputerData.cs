using System;
using System.Linq;
using System.Collections.Generic;
using System.Management;
using Microsoft.Win32;
using HardwareWizard.Controls;
using HardwareWizard.Core.Helpers;

namespace HardwareWizard.Core
{
    public class ComputerData
    {
        #region Properties
        public string OSEdition { get; set; }
        public string OSVersion { get; set; }
        public string OSBuild { get; set; }
        public string Architecture { get; set; }
        public string ProductID { get; set; }
        public string InstallDate { get; set; }
        public string Identifier { get; set; }
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string Memory { get; set; }
        public string Storage { get; set; }
        public ChassisType Chassis { get; set; }
        #endregion

        #region Main
        public ComputerData(HardwareWatcher hardware)
        {
            this.Identifier = Environment.MachineName;
            this.Architecture = Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit";
            this.Processor = hardware.Processor.Model;
            this.Graphics = hardware.Graphics.Cards.FirstOrDefault().Model ?? "N/A";
            this.Storage = GetStorageTotalSize(hardware.Storage.Drives);
            this.Memory = Utils.SizeSuffix(hardware.Memory.osTotalMemory);

            // Recupera le informazioni sul sistema operativo.
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                this.ProductID = WMIHelper.Retreive(obj, "SerialNumber", WMIValue.WMI_String);
                this.InstallDate = WMIHelper.Retreive(obj, "InstallDate", WMIValue.WMI_DateTime);
            }
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(
            "SELECT * FROM Win32_SystemEnclosure");
            foreach (ManagementObject obj in searcher1.Get())
            {
                UInt16 chassisType = ((UInt16[])obj["ChassisTypes"])[0];
                if (chassisType >= 8 && chassisType <= 11) {
                    Chassis = ChassisType.Laptop;
                } else {
                    Chassis = ChassisType.Desktop;
                }
            }
            RegistryKey baseInfo = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (baseInfo != null)
            {
                this.OSEdition = baseInfo.GetValue("ProductName").ToString();
                this.OSVersion = baseInfo.GetValue("ReleaseId").ToString();
                this.OSBuild = baseInfo.GetValue("CurrentBuild").ToString();
                baseInfo.Close();
            }
        }
        #endregion

        #region Helpers
        private string GetStorageTotalSize(List<StorageDriveData> drives)
        {
            ulong totalSize = 0;
            foreach (StorageDriveData drive in drives) {
                totalSize += (ulong)drive.Capacity;
            }
            return Utils.SizeSuffix(totalSize);
        }
        #endregion
    }
}
