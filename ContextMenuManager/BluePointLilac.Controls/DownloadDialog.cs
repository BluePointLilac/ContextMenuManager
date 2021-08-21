using BluePointLilac.Methods;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    sealed class DownloadDialog : CommonDialog
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string FilePath { get; set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(Process process = Process.GetCurrentProcess())
            using(DownloadForm frm = new DownloadForm())
            {
                frm.Url = this.Url;
                frm.Text = this.Text;
                frm.FilePath = this.FilePath;
                return frm.ShowDialog() == DialogResult.OK;
            }
        }

        sealed class DownloadForm : Form
        {
            public DownloadForm()
            {
                this.SuspendLayout();
                this.Font = SystemFonts.MessageBoxFont;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.MinimizeBox = this.MaximizeBox = this.ShowInTaskbar = false;
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Controls.AddRange(new Control[] { pgbDownload, btnCancel });
                this.Load += (sender, e) => DownloadFile(Url, FilePath);
                this.InitializeComponents();
                this.ResumeLayout();
            }

            readonly ProgressBar pgbDownload = new ProgressBar
            {
                Width = 200.DpiZoom(),
                Maximum = 100
            };
            readonly Button btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Text = ResourceString.Cancel,
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
                    MessageBox.Show(e.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.Cancel;
                }
            }

            protected override void OnLoad(EventArgs e)
            {
                if(this.Owner == null && Form.ActiveForm != this) this.Owner = Form.ActiveForm;
                if(this.Owner == null) this.StartPosition = FormStartPosition.CenterScreen;
                else
                {
                    this.TopMost = this.Owner.TopMost;
                    this.StartPosition = FormStartPosition.CenterParent;
                }
                base.OnLoad(e);
            }
        }
    }
}