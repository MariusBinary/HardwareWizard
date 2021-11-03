using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using HardwareWizard.Core;
using HardwareWizard.Interfaces;
using HardwareWizard.Views;
using HardwareWizard.Core.Helpers;
namespace HardwareWizard
{
    public partial class MainWindow : Window
    {
        #region Variables
        public HardwareWatcher hardwareWatcher;
        private bool canClose = false;
        private int updateInterval = 1;
        private int backgroundInterval = 1;
        private bool canBackgroundUpdate = true;
        #endregion

        #region Main
        public MainWindow()
        {
            InitializeComponent();

            // Carica le dimensione della finestra dalle impostazioni.
            if (Properties.Settings.Default.LastHeight > 0 && Properties.Settings.Default.LastWidth > 0)
            {
                this.Height = Properties.Settings.Default.LastHeight;
                this.Width = Properties.Settings.Default.LastWidth;
            }

            // Carica le impostazioni.
            switch (Properties.Settings.Default.UpdatePeriod)
            {
                case 0:
                    updateInterval = 1;
                    backgroundInterval = 5;
                    break;
                case 1:
                    updateInterval = 2;
                    backgroundInterval = 8;
                    break;
                case 2:
                    updateInterval = 5;
                    backgroundInterval = 12;
                    break;
            }
            canBackgroundUpdate = Properties.Settings.Default.CanBakgroundUpdate;
            TemperatureHelper.unit = (TemperatureUnit)Properties.Settings.Default.TemperatureUnit;
            Tab_Container.SetAnimation(Properties.Settings.Default.AllowTransitionAnimation);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lastNavigationTime = DateTime.Now;
            navigationHistory = new List<Tuple<NavigationViews, object>>();
            currentNavigationView = NavigationViews.None;
            hardwareViewType = HardwareViewType.None;

            // Crea il gestore dell'hardware.
            hardwareWatcher = new HardwareWatcher(() => {
                Dispatcher.Invoke(new Action(() => {
                    string chassisType = hardwareWatcher.Computer.Chassis.ToString();
                    Img_Dashboard.SetResourceReference(Image.SourceProperty, $"Image.NavigationBar.Dashboard.{chassisType}");
                    this.Navigate(Properties.Settings.Default.WelcomePage);
                    Tab_Main.Children.Remove(Tab_Loading);
                }));
            });

            hardwareWatcher.SetInterval(updateInterval, backgroundInterval);
            hardwareWatcher.SetBackgroundUpdate(canBackgroundUpdate);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!canClose)
            {
                // Annulla la chiusura.
                e.Cancel = true;

                // Chiude il database dell'hardware
                hardwareWatcher.Close(new Action(() => {
                    canClose = true;
                    this.Close();
                }));
            } 
            else
            {
                if (this.WindowState == WindowState.Normal)
                {
                    Properties.Settings.Default.LastHeight = this.ActualHeight;
                    Properties.Settings.Default.LastWidth = this.ActualWidth;
                    Properties.Settings.Default.Save();
                }
            }
        }
        #endregion

        #region Navigation
        private int navigationIndex = 0;
        private List<Tuple<NavigationViews, object>> navigationHistory;
        private DateTime lastNavigationTime;
        private NavigationViews currentNavigationView;
        private HardwareViewType hardwareViewType;
        private UserControl currentView;

        public enum NavigationViews
        {
            None,
            Dashboard,
            Motherboard,
            Processor,
            Memory,
            MemoryBank,
            Graphics,
            GraphicsCard,
            GraphicsMonitor,
            Storage,
            StorageDrive,
            Cooling,
            Settings
        }

        private void Navigate(int index)
        {
            NavigationViews[] views = new NavigationViews[] {
                NavigationViews.Dashboard, 
                NavigationViews.Motherboard,
                NavigationViews.Processor, 
                NavigationViews.Memory,
                NavigationViews.Graphics, 
                NavigationViews.Storage,
                NavigationViews.Cooling
            };

            Navigate(views[index]);
        }

        private void Navigate(object sender, RoutedEventArgs e)
        {
            string name = (sender as RadioButton).Name;

            if (name.Equals("Btn_Dashboard")) {
                Navigate(NavigationViews.Dashboard);
            } else if(name.Equals("Btn_Motherboard")) {
                Navigate(NavigationViews.Motherboard);
            } else if (name.Equals("Btn_Processor")) {
                Navigate(NavigationViews.Processor);
            } else if (name.Equals("Btn_Memory")) {
                Navigate(NavigationViews.Memory);
            } else if (name.Equals("Btn_Graphics")) {
                Navigate(NavigationViews.Graphics);
            } else if (name.Equals("Btn_Storage")) {
                Navigate(NavigationViews.Storage);
            } else if (name.Equals("Btn_Cooling")) {
                Navigate(NavigationViews.Cooling);
            } else if (name.Equals("Btn_Settings")) {
                Navigate(NavigationViews.Settings);
            }
        }

        private void Navigate(object sender, ExecutedRoutedEventArgs e)
        {
            object[] parameters = e.Parameter as object[];
            this.Navigate((NavigationViews)parameters[0], parameters[1]);
        }
        
        private bool Navigate(NavigationViews view, object parameter = null, bool isHistoryNavigation = false)
        {
            // Se la pagina richiesta è già aperta annullare l'azione.
            if (view == currentNavigationView) return false;

            // Controlla se sono passati almeno 500ms tra un cambio e l'altro.
            if (new TimeSpan(DateTime.Now.Ticks - lastNavigationTime.Ticks)
                .TotalMilliseconds < 250) return false;

            // Se la pagina è stata richiesta dopo che si è tornati indietro,
            // cancellare tutte le pagine successive.
            if (navigationIndex < navigationHistory.Count - 1 && !isHistoryNavigation) {
                if (navigationIndex + 1 <= navigationHistory.Count - 1) {
                    navigationHistory.RemoveRange(navigationIndex + 1, (navigationHistory.Count - 1) - navigationIndex);
                }
            }

            // Aggiungere la pagina richiesta alla cronologia.
            if (!isHistoryNavigation) {
                navigationHistory.Add(new Tuple<NavigationViews, object>(view, parameter));
                navigationIndex = navigationHistory.Count - 1;
            }
            currentNavigationView = view;

            // Inizializzare la pagina richiesta con eventuali parametri.
            if (view == NavigationViews.Dashboard) {
                hardwareViewType = HardwareViewType.Dashboard;
                currentView = new DashboardView(hardwareWatcher);
                Btn_Dashboard.IsChecked = true;
            } else if (view == NavigationViews.Motherboard) {
                hardwareViewType = HardwareViewType.Motherboard;
                currentView = new MotherboardView(hardwareWatcher.Motherboard);
                Btn_Motherboard.IsChecked = true;
            } else if (view == NavigationViews.Processor) {
                hardwareViewType = HardwareViewType.Processor;
                currentView = new ProcessorView(hardwareWatcher.Processor);
                Btn_Processor.IsChecked = true;
            } else if (view == NavigationViews.Memory) {
                hardwareViewType = HardwareViewType.Memory;
                currentView = new MemoryView(hardwareWatcher.Memory);
                Btn_Memory.IsChecked = true;
            } else if (view == NavigationViews.MemoryBank) {
                if (parameter.GetType() != typeof(MemoryBankData)) return false;
                hardwareViewType = HardwareViewType.Memory;
                currentView = new MemoryBankView(parameter as MemoryBankData);
                Btn_Memory.IsChecked = true;
            } else if (view == NavigationViews.Graphics) {
                hardwareViewType = HardwareViewType.Graphics;
                currentView = new GraphicsView(hardwareWatcher.Graphics);
                Btn_Graphics.IsChecked = true;
            } else if (view == NavigationViews.GraphicsCard) {
                if (parameter.GetType() != typeof(GraphicsCardData)) return false;
                hardwareViewType = HardwareViewType.Graphics;
                currentView = new GraphicsCardView(parameter as GraphicsCardData);
                Btn_Graphics.IsChecked = true;
            } else if (view == NavigationViews.GraphicsMonitor) {
                if (parameter.GetType() != typeof(GraphicsMonitorData)) return false;
                hardwareViewType = HardwareViewType.Graphics;
                currentView = new GraphicsMonitorView(parameter as GraphicsMonitorData);
                Btn_Graphics.IsChecked = true;
            } else if (view == NavigationViews.Storage) {
                hardwareViewType = HardwareViewType.Storage;
                currentView = new StorageView(hardwareWatcher.Storage);
                Btn_Storage.IsChecked = true;
            } else if (view == NavigationViews.StorageDrive) {
                if (parameter.GetType() != typeof(StorageDriveData)) return false;
                hardwareViewType = HardwareViewType.Storage;
                currentView = new StorageDriveView(parameter as StorageDriveData);
                Btn_Storage.IsChecked = true;
            } else if (view == NavigationViews.Cooling) {
                hardwareViewType = HardwareViewType.Cooling;
                currentView = new CoolingView(hardwareWatcher.Cooling);
                Btn_Cooling.IsChecked = true;
            } else if (view == NavigationViews.Settings) {
                hardwareViewType = HardwareViewType.None;
                currentView = new SettingsView();
                Btn_Settings.IsChecked = true;
            }

            // Aggiorna il gestore dell'hardware.
            hardwareWatcher.ChangeView(currentView as IViewUpdate, hardwareViewType);

            // Mostra la pagina.
            Tab_Container.Content = currentView;
            Scroll_Container.ScrollToVerticalOffset(0);
            lastNavigationTime = DateTime.Now;
            return true;
        }

        private void NavigateBack(object sender, ExecutedRoutedEventArgs e)
        {
            // Se siamo in prima posizione annullare l'azione.
            if (navigationIndex <= 0) return;

            // Naviga fino alla pagina precedente.
            if (Navigate(navigationHistory[navigationIndex - 1].Item1,
                navigationHistory[navigationIndex - 1].Item2, true)) {
                navigationIndex -= 1;
            }
        }

        private void NavigateForward(object sender, ExecutedRoutedEventArgs e)
        {
            // Se siamo in ultima posizione annullare l'azione.
            if (navigationIndex >= navigationHistory.Count - 1) return;

            // Naviga fino alla pagina successiva.
            if (Navigate(navigationHistory[navigationIndex + 1].Item1,
                navigationHistory[navigationIndex + 1].Item2, true)) {
                navigationIndex += 1;
            }
        }
        #endregion
    }
}
