using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;
using HardwareWizard.Controls;
using HardwareWizard.Models;
using System.Reflection;

namespace HardwareWizard.Core
{
    public class ReportHelper
    {
        #region Variables
        private StringBuilder reportData;
        private string reportId;
        private string appVersion;
        #endregion

        #region Main
        public ReportHelper(string id)
        {
            reportId = id;
            appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 3);
            reportData = new StringBuilder();
            reportData.Append($"\nHardware Wizard v{appVersion}\n\n");
            reportData.Append($"--------------------------------------------------------------------------------");
            reportData.Append($"\n\n{id.ToUpper()} Report\n\n");
            reportData.Append($"--------------------------------------------------------------------------------");
        }

        public void AddToReport(string title, DetailsTableControl table)
        {
            reportData.Append($"\n\n{title.ToUpper()}:\n\n");

            foreach (DetailsTableItemModel item in table.GetValues().Values)
            {
                reportData.Append(String.Format("{0,-40} :{1}\n", item.Name, item.Value));
            }
        }

        public void MakeReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            DateTime currentDate = DateTime.Now;
            saveFileDialog.FileName = $"HardwareWizard.Report.{reportId.ToUpper()}-{currentDate:dd.MM.yyyy}.txt";
            saveFileDialog.Filter = "Text File (.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true) {
                File.WriteAllText(saveFileDialog.FileName, reportData.ToString());
                Process.Start(saveFileDialog.FileName);
            }
        }
        #endregion
    }
}
