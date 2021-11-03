using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HardwareWizard.Core.Helpers
{
    public class ThreadTimer
    {
        #region Variables
        private int interval;
        private CancellationTokenSource tokenSource;
        private Task task;
        #endregion

        #region Main
        /// <summary>
        /// Ciclo di lavoro del timer.
        /// </summary>
        private void DoWork(Action action, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                Thread.Sleep(interval);
                action();
            }
        }
        #endregion

        #region Controls
        /// <summary>
        /// Avvia il timer.
        /// </summary>
        public void Start(Action action, int seconds = 1)
        {
            this.interval = (seconds * 1000);
            this.tokenSource = new CancellationTokenSource();
            this.task = Task.Factory.StartNew(()
                => DoWork(action, tokenSource.Token), tokenSource.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        /// <summary>
        /// Cambia l'intervallo del timer.
        /// </summary>
        public void SetInterval(int seconds)
        {
            this.interval = (seconds * 1000);
        }
        /// <summary>
        /// Arresta il timer.
        /// </summary>
        public void Stop()
        {
            tokenSource.Cancel();
            task.Wait();
        }
        #endregion
    }
}
