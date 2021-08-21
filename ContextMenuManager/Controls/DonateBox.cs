using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class DonateBox : Panel
    {
        public DonateBox()
        {
            this.SuspendLayout();
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.Font = SystemFonts.MenuFont;
            this.Font = new Font(this.Font.FontFamily, this.Font.Size + 1F);
            this.Controls.AddRange(new Control[] { lblInfo, picQR, lblList });
            this.VisibleChanged += (sender, e) => this.SetEnabled(this.Visible);
            lblList.Click += (sender, e) => this.ShowDonateDialog();
            picQR.Resize += (sender, e) => this.OnResize(null);
            picQR.MouseDown += SwitchQR;
            this.ResumeLayout();
        }

        readonly Label lblInfo = new Label
        {
            Text = AppString.Other.Donate,
            AutoSize = true
        };

        readonly Label lblList = new Label
        {
            ForeColor = Color.FromArgb(85, 145, 215),
            Text = AppString.Other.DonationList,
            Cursor = Cursors.Hand,
            AutoSize = true
        };

        readonly PictureBox picQR = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Cursor = Cursors.Hand,
            Image = AllQR,
        };

        static readonly Image AllQR = Properties.Resources.Donate;
        static readonly Image WechatQR = GetSingleQR(0);
        static readonly Image AlipayQR = GetSingleQR(1);
        static readonly Image QQQR = GetSingleQR(2);
        private static Image GetSingleQR(int index)
        {
            Bitmap bitmap = new Bitmap(200, 200);
            using(Graphics g = Graphics.FromImage(bitmap))
            {
                Rectangle destRect = new Rectangle(0, 0, 200, 200);
                Rectangle srcRect = new Rectangle(index * 200, 0, 200, 200);
                g.DrawImage(AllQR, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        protected override void OnResize(EventArgs e)
        {
            int a = 60.DpiZoom();
            base.OnResize(e);
            picQR.Left = (this.Width - picQR.Width) / 2;
            lblInfo.Left = (this.Width - lblInfo.Width) / 2;
            lblList.Left = (this.Width - lblList.Width) / 2;
            lblInfo.Top = a;
            picQR.Top = lblInfo.Bottom + a;
            lblList.Top = picQR.Bottom + a;
        }

        private void SwitchQR(object sender, MouseEventArgs e)
        {
            if(picQR.Image == AllQR)
            {
                if(e.X < 200) picQR.Image = WechatQR;
                else if(e.X < 400) picQR.Image = AlipayQR;
                else picQR.Image = QQQR;
            }
            else
            {
                picQR.Image = AllQR;
            }
        }

        private void ShowDonateDialog()
        {
            this.Cursor = Cursors.WaitCursor;
            using(UAWebClient client = new UAWebClient())
            {
                string url = AppConfig.RequestUseGithub ? AppConfig.GithubDonateRaw : AppConfig.GiteeDonateRaw;
                string contents = client.GetWebString(url);
                //contents = System.IO.File.ReadAllText(@"..\..\..\Donate.md");//用于求和更新Donate.md文件
                if(contents == null)
                {
                    if(AppMessageBox.Show(AppString.Message.WebDataReadFailed + "\r\n"
                        + AppString.Message.OpenWebUrl, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        url = AppConfig.RequestUseGithub ? AppConfig.GithubDonate : AppConfig.GiteeDonate;
                        ExternalProgram.OpenWebUrl(url);
                    }
                }
                else
                {
                    using(DonateListDialog dlg = new DonateListDialog())
                    {
                        dlg.DanateData = contents;
                        dlg.ShowDialog();
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }

        sealed class DonateListDialog : CommonDialog
        {
            public string DanateData { get; set; }

            public override void Reset() { }

            protected override bool RunDialog(IntPtr hwndOwner)
            {
                using(DonateListForm frm = new DonateListForm())
                {
                    frm.ShowDonateList(DanateData);
                    MainForm mainForm = (MainForm)FromHandle(hwndOwner);
                    frm.Left = mainForm.Left + (mainForm.Width + mainForm.SideBar.Width - frm.Width) / 2;
                    frm.Top = mainForm.Top + 150.DpiZoom();
                    frm.TopMost = AppConfig.TopMost;
                    frm.ShowDialog();
                }
                return true;
            }

            sealed class DonateListForm : Form
            {
                public DonateListForm()
                {
                    this.Font = SystemFonts.DialogFont;
                    this.Text = AppString.Other.DonationList;
                    this.SizeGripStyle = SizeGripStyle.Hide;
                    this.StartPosition = FormStartPosition.Manual;
                    this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                    this.MinimizeBox = this.MaximizeBox = this.ShowInTaskbar = false;
                    this.ClientSize = new Size(520, 350).DpiZoom();
                    this.MinimumSize = this.Size;
                    dgvDonate.ColumnHeadersDefaultCellStyle.Alignment
                        = dgvDonate.RowsDefaultCellStyle.Alignment
                        = DataGridViewContentAlignment.BottomCenter;
                    this.Controls.AddRange(new Control[] { lblThank, lblDonate, dgvDonate });
                    lblThank.MouseEnter += (sender, e) => lblThank.ForeColor = Color.FromArgb(0, 162, 255);
                    lblThank.MouseLeave += (sender, e) => lblThank.ForeColor = Color.DimGray;
                    lblDonate.Resize += (sender, e) => this.OnResize(null);
                    this.AddEscapeButton();
                }

                readonly DataGridView dgvDonate = new DataGridView
                {
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BackgroundColor = SystemColors.Control,
                    BorderStyle = BorderStyle.None,
                    AllowUserToResizeRows = false,
                    AllowUserToAddRows = false,
                    RowHeadersVisible = false,
                    MultiSelect = false,
                    ReadOnly = true
                };

                readonly Label lblDonate = new Label { AutoSize = true };
                readonly Label lblThank = new Label
                {
                    Font = new Font("Lucida Handwriting", 15F),
                    ForeColor = Color.DimGray,
                    Text = "Thank you!",
                    AutoSize = true,
                };

                protected override void OnResize(EventArgs e)
                {
                    base.OnResize(e);
                    int a = 20.DpiZoom();
                    lblDonate.Location = new Point(a, a);
                    dgvDonate.Location = new Point(a, lblDonate.Bottom + a);
                    dgvDonate.Width = this.ClientSize.Width - 2 * a;
                    dgvDonate.Height = this.ClientSize.Height - 3 * a - lblDonate.Height;
                    lblThank.Location = new Point(dgvDonate.Right - lblThank.Width, lblDonate.Bottom - lblThank.Height);
                }

                public void ShowDonateList(string contents)
                {
                    string[] lines = contents.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int index = Array.FindIndex(lines, line => line == "|:--:|:--:|:--:|:--:|:--:");
                    if(index == -1) return;
                    string[] heads = lines[index - 1].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    dgvDonate.ColumnCount = heads.Length;
                    dgvDonate.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    for(int m = 0; m < heads.Length; m++)
                    {
                        dgvDonate.Columns[m].HeaderText = heads[m];
                    }
                    for(int n = index + 1; n < lines.Length; n++)
                    {
                        string[] strs = lines[n].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        object[] values = new object[strs.Length];
                        for(int k = 0; k < strs.Length; k++)
                        {
                            switch(k)
                            {
                                case 3:
                                    values[k] = Convert.ToSingle(strs[k]);
                                    break;
                                default:
                                    values[k] = strs[k];
                                    break;
                            }
                        }
                        dgvDonate.Rows.Add(values);
                    }
                    dgvDonate.Sort(dgvDonate.Columns[0], ListSortDirection.Descending);
                    DateTime date = Convert.ToDateTime(dgvDonate.Rows[0].Cells[0].Value);
                    float money = dgvDonate.Rows.Cast<DataGridViewRow>().Sum(row => (float)row.Cells[3].Value);
                    lblDonate.Text = AppString.Dialog.DonateInfo.Replace("%date", date.ToLongDateString())
                        .Replace("%money", money.ToString()).Replace("%count", dgvDonate.RowCount.ToString());
                }
            }
        }
    }
}