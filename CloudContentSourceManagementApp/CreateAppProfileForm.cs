using CloudContentSourceManagementApp.CommonHelper;
using CloudContentSourceManagementApp.Models;
using CloudContentSourceManagementApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudContentSourceManagementApp
{
    public partial class CreateAppProfileForm : Form
    {
        private TextBox txtProfileName, txtUserEmail, txtClientId, txtClientEmail, txtPrivateKey;
        private Button btnSave, btnCancel;
        public GoogleAppProfile CreatedProfile { get; private set; }

        public CreateAppProfileForm()
        {
            this.Text = "Tạo Google App Profile";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            int y = 30;

            var lblTitle = new Label
            {
               Text = "Tạo Google App Profile",
               Font = new Font("Segoe UI", 15, FontStyle.Bold),
               ForeColor = Color.FromArgb(24, 119, 242),
               Location = new Point(120, y),
               AutoSize = true
            };
            this.Controls.Add(lblTitle);

            y += 50;
            // Profile Name
            this.Controls.Add(new Label { Text = "Tên Profile", Location = new Point(50, y), AutoSize = true });
            txtProfileName = new TextBox { Location = new Point(170, y), Width = 250 };
            this.Controls.Add(txtProfileName);

            y += 40;
            // User Email
            this.Controls.Add(new Label { Text = "User Email", Location = new Point(50, y), AutoSize = true });
            txtUserEmail = new TextBox { Location = new Point(170, y), Width = 250 };
            this.Controls.Add(txtUserEmail);

            y += 40;
            // ClientId
            this.Controls.Add(new Label { Text = "Client Id", Location = new Point(50, y), AutoSize = true });
            txtClientId = new TextBox { Location = new Point(170, y), Width = 250 };
            this.Controls.Add(txtClientId);

            y += 40;
            // Client Email
            this.Controls.Add(new Label { Text = "Client Email", Location = new Point(50, y), AutoSize = true });
            txtClientEmail = new TextBox { Location = new Point(170, y), Width = 250 };
            this.Controls.Add(txtClientEmail);

            y += 40;
            // Private Key (multi-line)
            this.Controls.Add(new Label { Text = "Private Key", Location = new Point(50, y), AutoSize = true });
            txtPrivateKey = new TextBox { Location = new Point(170, y), Width = 250, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtPrivateKey);

            y += 80;
            // Save Button
            btnSave = new Button
        {
            Text = "Lưu",
            Location = new Point(170, y),
            Size = new Size(110, 35),
            BackColor = Color.FromArgb(24, 119, 242),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat
        };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click; ;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button
        {
            Text = "Hủy",
            Location = new Point(310, y),
            Size = new Size(110, 35),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(24, 119, 242),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat
        };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(24, 119, 242);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtProfileName.Text) ||
                string.IsNullOrWhiteSpace(txtUserEmail.Text) ||
                string.IsNullOrWhiteSpace(txtClientId.Text) ||
                string.IsNullOrWhiteSpace(txtClientEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPrivateKey.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Map dữ liệu
            CreatedProfile = new GoogleAppProfile
            {
                ProfileName = txtProfileName.Text.Trim(),
                UserEmail = txtUserEmail.Text.Trim(),
                ClientId = txtClientId.Text.Trim(),
                ClientEmail = txtClientEmail.Text.Trim(),
                PrivateKey = txtPrivateKey.Text.Trim(),
                TenantId = LogonSystem.TenantId,
                CreatedTime = DateTime.Now.Ticks
            };

            // Lưu vào DB
            try
            {
                GoogleAppProfileService.CreateGoogleAppProfile(CreatedProfile);
                MessageBox.Show("Tạo App Profile thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
