using System;

namespace HardwareWizard.Core.Helpers
{
    public enum TemperatureUnit
    {
        Celsius,
        Farenight,
        Kelvin
    }

    public static class TemperatureHelper
    {
        public static TemperatureUnit unit = TemperatureUnit.Kelvin;

        /// <summary>
        /// Converte la temperatura fornita in gradi centigradi (°C) nell'unità
        /// di temperatura selezionata dall'utente.
        /// </summary>
        public static string Get(float? celsius)
        {
            //double temperature = Math.Round(celsius.Value, 1);
            if (unit == TemperatureUnit.Celsius) {
                return $"{Math.Round(celsius.Value, 0)} °C";
            } else if (unit == TemperatureUnit.Farenight) {
                return $"{Math.Round(((celsius.Value * 18) / 10) + 32, 0)} °F";
            } else if (unit == TemperatureUnit.Kelvin) {
                return $"{Math.Round(celsius.Value + 273.15, 0)} K";
            } else {
                return "-";
            }
        }
    }
}
