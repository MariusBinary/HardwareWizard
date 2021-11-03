using System.Linq;
using System.Collections.Generic;
using HardwareWizard.Interfaces;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class CoolingData : IDataUpdate
    {
        #region Properties
        public IEnumerable<CoolingFanData> Fans { 
            get {
                List<CoolingFanData> fans = new List<CoolingFanData>();
                foreach(ITemperature hardware in tempHardwares) {
                    foreach (CoolingFanData fan in hardware.GetHardwareFans() ?? 
                        Enumerable.Empty<CoolingFanData>()) {
                        fans.Add(fan);
                    }
                }
                return fans;
            }
        }
        public IEnumerable<CoolingThermalGroup> Thermals {
            get {
                List<CoolingThermalGroup> thermals = new List<CoolingThermalGroup>();
                foreach (ITemperature hardware in tempHardwares) {
                    foreach (CoolingThermalGroup thermal in hardware.GetHardwareThermal() ?? 
                        Enumerable.Empty<CoolingThermalGroup>()) {
                        thermals.Add(thermal);
                    }
                }
                return thermals;
            }
        }
        #endregion

        #region Variables
        private ITemperature[] tempHardwares;
        #endregion

        #region Main
        public CoolingData(ITemperature[] tempHardwares)
        {
            this.tempHardwares = tempHardwares;
        }
        #endregion

        #region Interface
        /// <summary>
        /// Un dispositivo appartente a questa classe è stato aggiunto.
        /// </summary>
        public void OnHardwareAdded(IHardware hardware)
        {
            // Nessuna azione.
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
            // Nessuna azione.
        }
        #endregion
    }
}
