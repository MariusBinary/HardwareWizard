using System.Collections.Generic;

namespace HardwareWizard.Core
{
    public class GraphCollection
    {
        public List<double> Points { get; set; }
        public double Lenght { get; set; }
        public bool IsFillBasedOnValue { get; set; }

        /// <summary>
        /// Crea la classe.
        /// </summary>
        public GraphCollection()
        {
            Points = new List<double>();
            Lenght = 130;
        }
        /// <summary>
        /// Aggiunge un punto al grafico.
        /// </summary>
        public void AddPoint(double value)
        {
            Points.Insert(0, value);
            if (Points.Count > Lenght) {
                Points.RemoveAt(Points.Count - 1);
            }
        }
        /// <summary>
        /// Cancella tutti i punti del grafico
        /// </summary>
        public void Clear()
        {
            Points.Clear();
        }
    }
}
