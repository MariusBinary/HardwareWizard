using System.Windows;
using System.Windows.Controls;
using HardwareWizard.Core.Helpers;

namespace HardwareWizard.Views
{
    public partial class SettingsView : UserControl
    {
        #region Variables
        private bool canUpdateSettings = false;
        #endregion

        #region Main
        public SettingsView()
        {
            InitializeComponent();

            // Carica le impostazioni.
            CBox_UpdatePeriod.SelectedIndex = Properties.Settings.Default.UpdatePeriod;
            Sw_CanBackgroundUpdate.IsChecked = Properties.Settings.Default.CanBakgroundUpdate;
            CBox_TemperatureUnit.SelectedIndex = Properties.Settings.Default.TemperatureUnit;
            CBox_WelcomePage.SelectedIndex = Properties.Settings.Default.WelcomePage;
            CBox_Theme.SelectedIndex = Properties.Settings.Default.Theme;
            Sw_AllowTransitionAnimation.IsChecked = Properties.Settings.Default.AllowTransitionAnimation;
            Sw_UpdateDiskSpace.IsChecked = Properties.Settings.Default.UpdateDiskSpace;

            canUpdateSettings = true;
        }
        #endregion
       
        private void CBox_UpdatePeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!canUpdateSettings) return;
            int updateInterval = 1;
            int backgroundInterval = 1;
            switch (CBox_UpdatePeriod.SelectedIndex)
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
            ((MainWindow)Application.Current.MainWindow).hardwareWatcher.SetInterval(updateInterval, backgroundInterval);
            Properties.Settings.Default.UpdatePeriod = CBox_UpdatePeriod.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void Sw_CanBackgroundUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!canUpdateSettings) return;
            ((MainWindow)Application.Current.MainWindow).hardwareWatcher.SetBackgroundUpdate(Sw_CanBackgroundUpdate.IsChecked.Value);
            Properties.Settings.Default.CanBakgroundUpdate = Sw_CanBackgroundUpdate.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void CBox_TemperatureUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!canUpdateSettings) return;
            TemperatureHelper.unit = (TemperatureUnit)CBox_TemperatureUnit.SelectedIndex;
            Properties.Settings.Default.TemperatureUnit = CBox_TemperatureUnit.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void CBox_WelcomePage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!canUpdateSettings) return;
            Properties.Settings.Default.WelcomePage = CBox_WelcomePage.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void CBox_Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!canUpdateSettings) return;
            ((App)Application.Current).ChangeAppTheme((WindowsTheme)CBox_Theme.SelectedIndex);
            Properties.Settings.Default.Theme = CBox_Theme.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void Sw_AllowTransitionAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (!canUpdateSettings) return;
            ((MainWindow)Application.Current.MainWindow).Tab_Container.SetAnimation(Sw_AllowTransitionAnimation.IsChecked.Value);
            Properties.Settings.Default.AllowTransitionAnimation = Sw_AllowTransitionAnimation.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void Sw_UpdateDiskSpace_Click(object sender, RoutedEventArgs e)
        {
            if (!canUpdateSettings) return;
            ((MainWindow)Application.Current.MainWindow).hardwareWatcher.Storage.SetUpdateSpace(Sw_UpdateDiskSpace.IsChecked.Value);
            Properties.Settings.Default.UpdateDiskSpace = Sw_UpdateDiskSpace.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }
}
