using System;
using System.Linq;
using System.Management;

namespace HardwareWizard.Core.Helpers
{
    public enum WMIValue
    {
        WMI_String,
        WMI_String_,
        WMI_UInt8,
        WMI_UInt16,
        WMI_UInt16_,
        WMI_UInt32,
        WMI_UInt64,
        WMI_Real32,
        WMI_Real64,
        WMI_Boolean,
        WMI_DateTime
    }

    public static class WMIHelper
    {
        /// <summary>
        /// Ritorna un elemento WMI che comprende nome e valore della proprietà.
        /// </summary>
        public static WMICollection GetItem(ManagementObject wmi, string name, WMIValue valueType)
        {
            return new WMICollection(name, Retreive(wmi, name, valueType));
        }
        /// <summary>
        /// Ritorna il valore della proprietà WMI richiesta.
        /// </summary>
        public static string Retreive(ManagementObject wmi, string name, WMIValue valueType)
        {
            try {
                object property = wmi.GetPropertyValue(name);
                if (property == null) return "N/A";

                if (valueType == WMIValue.WMI_String) {
                    string result = property.ToString().TrimEnd();
                    return string.IsNullOrEmpty(result) ? "N/A" : result;
                }
                else if (valueType == WMIValue.WMI_String_) {
                    string result = "";
                    string[] list = (string[])property;
                    foreach (string item in list) {
                        result += String.IsNullOrEmpty(result) ? item : $", {item}";
                    }
                    return String.IsNullOrEmpty(result) ? "N/A" : result;
                }
                else if (valueType == WMIValue.WMI_UInt8) {
                    return Convert.ToInt32((byte)property).ToString();
                }
                else if (valueType == WMIValue.WMI_UInt16) {
                    return Convert.ToInt32((UInt16)property).ToString();
                } 
                else if (valueType == WMIValue.WMI_UInt16_) {
                    char[] chars = Array.ConvertAll((UInt16[])property, Convert.ToChar);
                    string result = new string(chars.Where(b => b != 0x00).ToArray());
                    return string.IsNullOrEmpty(result) ? "N/A" : result;
                }
                else if (valueType == WMIValue.WMI_UInt32) {
                    return Convert.ToInt32((UInt32)property).ToString();
                }
                else if (valueType == WMIValue.WMI_UInt64) {
                    return Convert.ToInt64((UInt64)property).ToString();
                }
                else if (valueType == WMIValue.WMI_Real32) {
                    return Convert.ToInt32((Decimal)property).ToString();
                }
                else if (valueType == WMIValue.WMI_Real64) {
                    return Convert.ToInt64((Decimal)property).ToString();
                }
                else if (valueType == WMIValue.WMI_Boolean) {
                    return ((Boolean)property).ToString();
                }
                else if (valueType == WMIValue.WMI_DateTime) {
                    string result = property.ToString();
                    return string.IsNullOrEmpty(result) ? "N/A" :
                        ManagementDateTimeConverter.ToDateTime(result).ToString("dd/MM/yyyy");
                } 
                else {
                    return "N/A";
                }
            } catch {
                return "N/A";
            }
        }
        /// <summary>
        /// Ritorna il valore booleano della proprietà WMI richiesta.
        /// </summary>
        public static bool GetBoolean(ManagementObject wmi, string name)
        {
            try {
                object property = wmi.GetPropertyValue(name);
                return (Boolean)property;
            } catch {
                return false;
            }
        }
        /// <summary>
        /// Ritorna il valore testuale della proprietà WMI richiesta.
        /// </summary>
        public static string GetString(ManagementObject wmi, string name)
        {
            try {
                object property = wmi.GetPropertyValue(name);
                string result = property.ToString().TrimEnd();
                return string.IsNullOrEmpty(result) ? null : result;
            } catch {
                return null;
            }
        }
        /// <summary>
        /// Ritorna il valore numerico (UInt16) della proprietà WMI richiesta.
        /// </summary>
        public static UInt16 GetUInt16(ManagementObject wmi, string name)
        {
            try {
                object property = wmi.GetPropertyValue(name);
                return (UInt16)property;
            } catch {
                return 0;
            }
        }
        /// <summary>
        /// Ritorna il valore numerico (UInt32) della proprietà WMI richiesta.
        /// </summary>
        public static UInt32 GetUInt32(ManagementObject wmi, string name)
        {
            try {
                object property = wmi.GetPropertyValue(name);
                return (UInt32)property;
            } catch {
                return 0;
            }
        }
        /// <summary>
        /// Ritorna il valore numerico (UInt64) della proprietà WMI richiesta.
        /// </summary>
        public static UInt64 GetUInt64(ManagementObject wmi, string name)
        {
            try {
                object property = wmi.GetPropertyValue(name);
                return (UInt64)property;
            } catch {
                return 0;
            }
        }
    }
}
