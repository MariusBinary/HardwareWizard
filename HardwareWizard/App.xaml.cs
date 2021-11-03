using System;
using System.Windows;
using Microsoft.Win32;

namespace HardwareWizard
{
	public enum WindowsTheme
	{
		Light,
		Dark
	}

	public partial class App : Application
    {
		#region Variables
		private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
		private const string RegistryValueName = "AppsUseLightTheme";
		#endregion

		#region Main
		protected override void OnStartup(StartupEventArgs e)
        {
			// Recupera il tema selezionato nelle impostazioni.
			WindowsTheme theme = WindowsTheme.Dark;
			if (HardwareWizard.Properties.Settings.Default.IsFirstOpen) {
				theme = GetWindowsTheme();
				HardwareWizard.Properties.Settings.Default.IsFirstOpen = false;
				HardwareWizard.Properties.Settings.Default.Theme = (int)theme;
				HardwareWizard.Properties.Settings.Default.Save();
			}  else {
				theme = (WindowsTheme)HardwareWizard.Properties.Settings.Default.Theme;
			}

			// Imposta il tema.
			if (theme != WindowsTheme.Dark) {
				ChangeAppTheme(theme);
			}

			base.OnStartup(e);
        }
		#endregion

		#region Helpers
		/// <summary>
		/// Ritorna il tema corrente di Windows.
		/// </summary>
		public WindowsTheme GetWindowsTheme()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
			{
				object registryValueObject = key?.GetValue(RegistryValueName);
				if (registryValueObject == null) return WindowsTheme.Light;
				int registryValue = (int)registryValueObject;
				return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
			}
		}
		/// <summary>
		/// Cambia il tema dell'applicazione.
		/// </summary>
		public void ChangeAppTheme(WindowsTheme theme)
        {
			this.Resources.MergedDictionaries[0].Source = new Uri($"/Themes/{theme}Theme.xaml", UriKind.Relative);
		}
		#endregion
	}
}
