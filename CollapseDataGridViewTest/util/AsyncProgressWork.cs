using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollapseDataGridViewTest.util
{
    public class AsyncProgressWork
    {
        public BackgroundWorker BgWorker { get; set; }

        public DataGridViewProgressCell DGVProgressCell { get; set; }

        public string DestFileName { get; set; }

        private bool IsSuccess;

        private CancellationTokenSource tokenSource;

        public AsyncProgressWork(string destFileName, DataGridViewProgressCell dgvProgressCell)
        {
            tokenSource = new CancellationTokenSource();
            IsSuccess = true;
            this.DestFileName = destFileName;
            this.DGVProgressCell = dgvProgressCell;
            this.BgWorker = new BackgroundWorker();
            this.BgWorker.WorkerReportsProgress = true;
            this.BgWorker.WorkerSupportsCancellation = true;

            this.BgWorker.DoWork += BgWorker_DoWork;
            this.BgWorker.ProgressChanged += BgWorker_ProgressChanged;
            this.BgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
        }

        public void CancelDo()
        {
            tokenSource.Cancel();
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DGVProgressCell.Value = IsSuccess ? 100 : 0;
            if (e.Error != null)
            {
                MessageBox.Show(this.DestFileName + " upload error！", "alter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //if (e.Cancelled)
            //{
            //    //if (IsSuccess)
            //    //{
            //    //    MessageBox.Show(this.DestFileName + "upload success！", "alter", MessageBoxButtons.OK);
            //    //}
            //}
            this.BgWorker.Dispose();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Do()
        {
            BgWorker.RunWorkerAsync();
        }

        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DGVProgressCell.Value = e.ProgressPercentage;
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int showProgressEnableCounter = 0;
            //sync do
            var task = Task.Factory.StartNew(() => {
                var updateResult = ServiceClient.UploadFile(this.DestFileName, "", tokenSource.Token);
                if (!updateResult)
                {
                    IsSuccess = false;
                    MessageBox.Show($"upload { this.DestFileName} fail");
                }
                //delete client file
                File.Delete(this.DestFileName);
            }).ContinueWith(t =>
            BgWorker.CancelAsync());
            int k = 0;
            while (true)
            {
                if (BgWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                Interlocked.Increment(ref showProgressEnableCounter);
                if (k == showProgressEnableCounter / 1000000)
                {
                    continue;
                }
                k = showProgressEnableCounter / 1000000;
                if (k > 100)
                {
                    showProgressEnableCounter = 0;
                }

                BgWorker.ReportProgress(k);
            }
        }
    }
}
