using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class DonateListDialog : CommonDialog
    {
        public string DanateData { get; set; }

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(DonateListForm frm = new DonateListForm())
            {
                frm.ShowDonateList(DanateData);
                frm.ShowDialog();
            }
            return true;
        }

        sealed class DonateListForm : Form
        {
            public DonateListForm()
            {
                this.Text = AppString.Other.DonationList;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimizeBox = this.MaximizeBox = this.ShowInTaskbar = false;
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Font = new Font(SystemFonts.DialogFont.FontFamily, 9F);
                this.ClientSize = new Size(520, 378).DpiZoom();
                dgvDonate.ColumnHeadersDefaultCellStyle.Alignment
                    = dgvDonate.RowsDefaultCellStyle.Alignment
                    = DataGridViewContentAlignment.BottomCenter;
                this.Controls.AddRange(new Control[] { lblDonate, dgvDonate });
                lblDonate.Resize += (sender, e) => this.OnResize(null);
            }

            readonly DataGridView dgvDonate = new DataGridView
            {
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
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

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                int a = 20.DpiZoom();
                lblDonate.Location = new Point(a, a);
                dgvDonate.Location = new Point(a, lblDonate.Bottom + a);
                dgvDonate.Width = this.ClientSize.Width - 2 * a;
                dgvDonate.Height = this.ClientSize.Height - 3 * a - lblDonate.Height;
            }

            public void ShowDonateList(string contents)
            {
                string[] lines = contents.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                int index = Array.FindIndex(lines, line => line == "|:--:|:--:|:--:|:--:|:--:");
                string[] heads = lines[index - 1].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                dgvDonate.ColumnCount = heads.Length;
                for(int m = 0; m < heads.Length; m++)
                {
                    dgvDonate.Columns[m].HeaderText = heads[m];
                }
                for(int n = index + 1; n < lines.Length; n++)
                {
                    string[] values = lines[n].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    dgvDonate.Rows.Add(values);
                }

                DateTime date = new DateTime();
                float money = 0;
                foreach(DataGridViewRow row in dgvDonate.Rows)
                {
                    DateTime temp = Convert.ToDateTime(row.Cells[0].Value);
                    if(temp > date) date = temp;
                    money += Convert.ToSingle(row.Cells[3].Value);
                }
                dgvDonate.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                lblDonate.Text = AppString.Dialog.DonateInfo.Replace("%date", date.ToLongDateString())
                    .Replace("%money", money.ToString()).Replace("%count", dgvDonate.RowCount.ToString());
            }
        }
    }
}