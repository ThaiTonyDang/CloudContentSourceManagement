using CloudContentSourceManagementApp.Services;
using CloudContentSourceManagementApp.CommonHelper;
using CloudContentSourceManagementApp.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace CloudContentSourceManagementApp
{
    public partial class LoginForm : Form
    {
        private TextBox txtUser, txtPass;
        private Button btnLogin, btnRegister;
        private Label lblError;
        private CheckBox chkShowPass, chkRemember;

        private static readonly string RememberFile = Path.Combine(
            Program.AppFolder, "remember_login.txt");

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Đăng nhập";
            this.Size = new Size(430, 470);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            // Title
            var lblTitle = new Label
            {
                Text = "Đăng nhập tài khoản",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 119, 242),
                Location = new Point(80, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Username
            var lblUser = new Label { Text = "Tên đăng nhập", Location = new Point(45, 65), AutoSize = true };
            txtUser = new TextBox { Location = new Point(45, 90), Width = 320, Font = new Font("Segoe UI", 11) };
            txtUser.GotFocus += (s, e) => txtUser.BackColor = Color.FromArgb(225, 240, 255);
            txtUser.LostFocus += (s, e) => txtUser.BackColor = Color.White;
            this.Controls.Add(lblUser); this.Controls.Add(txtUser);

            // Password
            var lblPass = new Label { Text = "Mật khẩu", Location = new Point(45, 130), AutoSize = true };
            txtPass = new TextBox { Location = new Point(45, 155), Width = 320, Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            txtPass.GotFocus += (s, e) => txtPass.BackColor = Color.FromArgb(225, 240, 255);
            txtPass.LostFocus += (s, e) => txtPass.BackColor = Color.White;
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);

            // Show Password
            chkShowPass = new CheckBox
            {
                Text = "Hiện mật khẩu",
                Location = new Point(45, 190),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            chkShowPass.CheckedChanged += (s, e) =>
            {
                txtPass.PasswordChar = chkShowPass.Checked ? '\0' : '●';
            };
            this.Controls.Add(chkShowPass);

            // Remember
            chkRemember = new CheckBox
            {
                Text = "Nhớ tài khoản",
                Location = new Point(170, 190),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            this.Controls.Add(chkRemember);

            // Error label
            lblError = new Label
            {
                ForeColor = Color.Red,
                Location = new Point(45, 215),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };
            this.Controls.Add(lblError);

            // Login Button
            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Location = new Point(45, 250),
                Width = 320,
                Height = 38,
                BackColor = Color.FromArgb(24, 119, 242),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(10, 90, 200);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(24, 119, 242);
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // Register Button
            btnRegister = new Button
            {
                Text = "Đăng ký tài khoản mới",
                Location = new Point(45, 305),
                Width = 320,
                Height = 30,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(24, 119, 242),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderColor = Color.FromArgb(24, 119, 242);
            btnRegister.MouseEnter += (s, e) =>
            {
                btnRegister.BackColor = Color.FromArgb(220, 235, 255);
                btnRegister.ForeColor = Color.FromArgb(10, 90, 200);
            };
            btnRegister.MouseLeave += (s, e) =>
            {
                btnRegister.BackColor = Color.White;
                btnRegister.ForeColor = Color.FromArgb(24, 119, 242);
            };
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // Load remembered
            LoadRememberedUser();
        }

        private void LoadRememberedUser()
        {
            try
            {
                if (File.Exists(RememberFile))
                {
                    var lines = File.ReadAllLines(RememberFile);
                    if (lines.Length >= 2)
                    {
                        txtUser.Text = lines[0];
                        txtPass.Text = lines[1];
                        chkRemember.Checked = true;
                    }
                }
            }
            catch { }
        }

        private void SaveRememberedUser()
        {
            try
            {
                if (chkRemember.Checked)
                {
                    File.WriteAllLines(RememberFile, new[] { txtUser.Text, txtPass.Text });
                }
                else
                {
                    if (File.Exists(RememberFile))
                        File.Delete(RememberFile);
                }
            }
            catch { }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            var user = txtUser.Text.Trim();
            var pass = txtPass.Text.Trim();
            var account = UserAccountService.GetUserByUsername(user);

            if (account == null || !PasswordHelper.VerifyPassword(pass, account.PasswordHash))
            {
                lblError.Text = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return;
            }

            // Gán LogonId (TenantId) toàn app
            LogonSystem.TenantId = account.TenantId;
            LogonSystem.Username = account.Username;
            SaveRememberedUser();

            // Đăng nhập thành công – mở MainForm
            // new MainForm().Show();
            // this.Hide();
            var result = MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                var mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => Application.Exit(); // Khi mainform tắt thì app tắt
                mainForm.Show();
                this.Hide();
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            new RegisterForm().Show();
            this.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit(); // Đảm bảo app tắt hẳn, không bị ẩn dưới khay
        }
    }
}
