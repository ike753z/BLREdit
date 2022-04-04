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
			ThreadPool.GetMaxThreads(out int maxT, out int maxIO);

			ThreadPool.SetMaxThreads(800, 800);
			ThreadPool.SetMinThreads(800, 800);

			Parallel.For(0, items.Count, (i) =>
			{
				ImgCache.CreateImageChacheForItem(items[i]);
				Interlocked.Increment(ref progress);
				(sender as BackgroundWorker).ReportProgress(progress);
			});

			ThreadPool.SetMaxThreads(maxT, maxIO);
			ThreadPool.SetMinThreads(minT, minIO);
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pbStatus.Value = e.ProgressPercentage;
			if (pbStatus.Value >= pbStatus.Maximum)
			{
				DialogResult = true;
			}
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			if (pbStatus.Value < pbStatus.Maximum)
			{
				e.Cancel = true;
			}
		}
	}
}
