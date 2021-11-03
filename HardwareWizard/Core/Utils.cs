using System;

namespace HardwareWizard.Core
{
    public static class Utils
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Converte un valore definito in byte nell'unità più appropriata.
        /// </summary>
        public static string SizeSuffix(ulong value, int decimalPlaces = 1)
        {
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }
            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        /// <summary>
        /// Determina se la stringa contiene la parola indicata.
        /// </summary>
        public static bool Contains(string text, string word)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.ToUpper().IndexOf(word.ToUpper()) >= 0;
        }
        /// <summary>
        /// Arrotonda un valore in base alla soglia indicata.
        /// </summary>
        public static int RoundWithBreak(double value, double breakValue = .5)
        {
            return (int)Math.Round(value - (breakValue - 0.5), 0);
        }
        /// <summary>
        /// Determina la similarità tra due stringhe.
        /// </summary>
        private static int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++);
            for (int j = 0; j <= m; d[0, j] = j++);

            for (int i = 1; i <= n; i++) {
                for (int j = 1; j <= m; j++) {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), 
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
        /// <summary>
        /// Determina la percentuale di somiglianza tra due stringhe.
        /// </summary>
        public static double CalculateSimilarity(string s, string t)
        {
            string biggestString = s.Length > t.Length ? s : t;
            return (biggestString.Length - ComputeLevenshteinDistance(s, t)) / (double)biggestString.Length;
        }
    }
}
