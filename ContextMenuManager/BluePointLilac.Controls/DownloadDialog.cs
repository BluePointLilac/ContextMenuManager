using BluePointLilac.Methods;
using ContextMenuManager;
using System;
using System.Drawing;
using System.IO;
using System.Net;
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
            using(DownloadForm frm = new DownloadForm())
            {
                frm.Url = this.Url;
                frm.FilePath = this.FilePath;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(!flag) File.Delete(FilePath);
                return flag;
            }
        }

        sealed class DownloadForm : Form
        {
            public DownloadForm()
            {
                this.MinimizeBox = this.MaximizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
                this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 9F);
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Controls.AddRange(new Control[] { pgbDownload, btnCancel });
                pgbDownload.Left = pgbDownload.Top = btnCancel.Top = 20.DpiZoom();
                pgbDownload.Height = btnCancel.Height;
                pgbDownload.Width = 200.DpiZoom();
                btnCancel.Left = pgbDownload.Right + 20.DpiZoom();
                this.ClientSize = new Size(btnCancel.Right + 20.DpiZoom(), btnCancel.Bottom + 20.DpiZoom());
                this.Load += (sender, e) => DownloadFile(Url, FilePath);
            }

            readonly ProgressBar pgbDownload = new ProgressBar();
            readonly Button btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Text = AppString.Dialog.Cancel,
                AutoSize = true
            };

            public string Url { get; set; }
            public string FilePath { get; set; }

            private void DownloadFile(string url, string filePath)
            {
                try
                {
                    this.Activate();
                    using(WebResponse response = WebRequest.Create(url).GetResponse())
                    using(Stream webStream = response.GetResponseStream())
                    using(FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        double fullSize = response.ContentLength;
                        pgbDownload.Maximum = (int)fullSize;
                        double downloadedSize = 0;
                        int incrementSize;
                        do
                        {
                            if(this.DialogResult == DialogResult.Cancel) return;
                            byte[] by = new byte[1024];
                            incrementSize = webStream.Read(by, 0, by.Length);
                            downloadedSize += incrementSize;
                            fileStream.Write(by, 0, incrementSize);
                            pgbDownload.Value = (int)downloadedSize;

                            double downloaded = Math.Round(downloadedSize / fullSize * 100, 2);
                            this.Text = $"Downloading: {downloaded}%";
                            Application.DoEvents();
                        } while(incrementSize > 0);
                        this.DialogResult = DialogResult.OK;
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