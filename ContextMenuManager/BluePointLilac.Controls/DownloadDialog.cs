using BluePointLilac.Methods;
using ContextMenuManager;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    sealed class DownloadDialog : CommonDialog
    {
        public string Url { get; set; }
        public string FilePath { get; set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(Process process = Process.GetCurrentProcess())
            using(DownloadForm frm = new DownloadForm { Url = this.Url, FilePath = this.FilePath })
            {
                bool isAsyn = hwndOwner == process.MainWindowHandle;
                frm.StartPosition = isAsyn ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
                return frm.ShowDialog() == DialogResult.OK;
            }
        }

        sealed class DownloadForm : Form
        {
            public DownloadForm()
            {
                this.Text = AppString.General.AppName;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimizeBox = this.MaximizeBox = this.ShowInTaskbar = false;
                this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 9F);
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Controls.AddRange(new Control[] { pgbDownload, btnCancel });
                this.Load += (sender, e) => DownloadFile(Url, FilePath);
                this.InitializeComponents();
            }

            readonly ProgressBar pgbDownload = new ProgressBar
            {
                Width = 200.DpiZoom(),
                Maximum = 100
            };
            readonly Button btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Text = AppString.Dialog.Cancel,
                AutoSize = true
            };

            public string Url { get; set; }
            public string FilePath { get; set; }

            private void InitializeComponents()
            {
                int a = 20.DpiZoom();
                pgbDownload.Left = pgbDownload.Top = btnCancel.Top = a;
                pgbDownload.Height = btnCancel.Height;
                btnCancel.Left = pgbDownload.Right + a;
                this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);
            }

            private void DownloadFile(string url, string filePath)
            {
                try
                {
                    using(UAWebClient client = new UAWebClient())
                    {
                        client.DownloadProgressChanged += (sender, e) =>
                        {
                            int value = e.ProgressPercentage;
                            this.Text = $"Downloading: {value}%";
                            pgbDownload.Value = value;
                            if(this.DialogResult == DialogResult.Cancel)
                            {
                                client.CancelAsync();
                                File.Delete(FilePath);
                            }
                        };
                        client.DownloadFileCompleted += (sender, e) =>
                        {
                            this.DialogResult = DialogResult.OK;
                        };
                        client.DownloadFileAsync(new Uri(url), filePath);
                    }
                }
                catch(Exception e)
                {
                    MessageBoxEx.Show(e.Message);
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }
    }
}