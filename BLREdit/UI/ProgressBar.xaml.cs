using BLREdit.API.Utils;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
			for(int i = 0; i < items.Count; i++)
			{
				ImgCache.CreateImageChacheForItem(items[i]);
				(sender as BackgroundWorker).ReportProgress(i);
			}
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
