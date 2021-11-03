using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HardwareWizard.Core
{
    public class ProcessesWatcherData
    {
        #region Properties
        public string Name { get; set; }
        public double CPU { get; set; }
        public double RAM { get; set; }
        public Process Process { get; set; }

        private ImageSource _icon = null;
        public ImageSource Icon { 
            get 
            {
                // Controlla se l'icona del processo è già salvata in memoria.
                if (_icon != null) return _icon;

                // Cerca di caricare l'icon del processo.
                string processFileName = null;
                try {
                    processFileName = Process.GetMainModuleFileName();
                } catch { Console.WriteLine($"***ERROR*** => Failed to load icon from 'Kernel32.dll'"); }

                if (!string.IsNullOrEmpty(processFileName))
                {
                    if (File.Exists(processFileName))
                    {
                        System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(processFileName);
                        System.Drawing.Bitmap bitmap = ico.ToBitmap();
                        IntPtr hBitmap = bitmap.GetHbitmap();

                        _icon = Imaging.CreateBitmapSourceFromHBitmap(
                                  hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                  BitmapSizeOptions.FromEmptyOptions());
                    }
                }

                // Se non è stata caricata alcuna icon per il processo, caricarne una di default.
                if (_icon == null) {
                    _icon = new BitmapImage(new Uri("pack://application:,,,/HardwareWizard;component/Images/Shared/ic_process.png"));
                }

                return _icon;
            } 
        }

        public DateTime lastTime { get; set; }
        public TimeSpan lastTotalProcessorTime { get; set; }
        public DateTime curTime { get; set; }
        public TimeSpan curTotalProcessorTime { get; set; }
        #endregion
    }
}
