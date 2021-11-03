using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace HardwareWizard.Core.Helpers
{
    public static class WallpaperHelper
    {
        /// <summary>
        /// Ritorna l'immagine dello sfondo corrente.
        /// </summary>
        public static BitmapImage GetWallaperImage()
        {
            string fileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft\\Windows\\Themes\\TranscodedWallpaper");

            if (!File.Exists(fileName)) {
                int currentTheme = Properties.Settings.Default.Theme;
                string iconFolder = currentTheme == 0 ? "Dark" : "Light";
                fileName = $"pack://application:,,,/HardwareWizard;component/Images/{iconFolder}/ic_default_background.png";
                return new BitmapImage(new Uri(fileName));
            } else {
                return LoadImage(File.ReadAllBytes(fileName));
            }
        }
        /// <summary>
        /// Restituisce l'array di byte che compone l'immagine fornita.
        /// </summary>
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
