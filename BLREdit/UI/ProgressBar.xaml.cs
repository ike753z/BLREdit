using BLREdit.API.Utils;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BLREdit.UI
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {

		List<ImportItem> items;
        public ProgressBar(List<ImportItem> items)
        {
            InitializeComponent();
			this.items = items;
			this.pbStatus.Maximum = items.Count-1;
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			worker.ProgressChanged += worker_ProgressChanged;

			worker.RunWorkerAsync();
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			int progress = 0;

			ThreadPool.GetMinThreads(out int minT, out int minIO);

			ThreadPool.SetMinThreads(64, 16);

			Parallel.For(0, items.Count, (i) =>
			{
				ImgCache.CreateImageChacheForItem(items[i]);
				Interlocked.Increment(ref progress);
				(sender as BackgroundWorker).ReportProgress(progress);
			});

			ThreadPool.SetMinThreads(minT, minIO);

			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
				DialogResult = true;
			}));
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pbStatus.Value = e.ProgressPercentage;

		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			if (!DialogResult ?? true)
			{
				e.Cancel = true;
			}
		}
	}
}
