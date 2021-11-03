using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using HardwareWizard.Interfaces;
using HardwareWizard.Core.Helpers;

namespace HardwareWizard.Core
{
    public class ProcessesWatcher: IProcessData
    {
        #region Properties
        public List<ProcessesWatcherData> Processes { get; set; }
        #endregion

        #region Variables
        private ProcessComparer processComparer;
        private List<ProcessesWatcherData> processesTemp;
        #endregion

        #region Main
        public ProcessesWatcher()
        {
            Processes = new List<ProcessesWatcherData>();
            processesTemp = new List<ProcessesWatcherData>();
            processComparer = new ProcessComparer();
        }

        public void Update()
        {
            // Ottiene una lista di tutti processi attualmente in esecuzione.
            Process[] processes = Process.GetProcesses();
            processesTemp.Clear();

            // Aggiunge alla lista temporanea i processi attualmente in esecuzione.
            foreach (Process process in processes) {
                processesTemp.Add(new ProcessesWatcherData() { 
                    Process = process 
                });
            }

            // Confronta i processi della lista temporanea e con quelli della lista
            // principale e lascia solo quelli in comune ad entrabe.
            Processes = Processes.Union(processesTemp, processComparer).ToList();

            // Aggiorna la percentuale di utilizzo della CPU per i processi in esecuzione.
            foreach (ProcessesWatcherData process in Processes)
            {
                try
                {
                    if (process.lastTime == null || process.lastTime == new DateTime())
                    {
                        process.lastTime = DateTime.Now;
                        process.lastTotalProcessorTime = process.Process.TotalProcessorTime;
                    }
                    else
                    {
                        process.curTime = DateTime.Now;
                        process.curTotalProcessorTime = process.Process.TotalProcessorTime;

                        process.CPU = ((process.curTotalProcessorTime.TotalMilliseconds -
                            process.lastTotalProcessorTime.TotalMilliseconds) / 
                            process.curTime.Subtract(process.lastTime).TotalMilliseconds / 
                            Convert.ToDouble(Environment.ProcessorCount)) * 100;

                        process.RAM = process.Process.WorkingSet64;

                        process.lastTime = process.curTime;
                        process.lastTotalProcessorTime = process.curTotalProcessorTime;
                    }
                } 
                catch 
                {
                }
            }
        }   
        #endregion
    }

    internal static class Extensions
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                null;
        }
    }
}
