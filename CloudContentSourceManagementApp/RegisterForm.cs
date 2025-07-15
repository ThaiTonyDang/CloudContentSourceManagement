using System;
using System.Drawing;
using System.Windows.Forms;

namespace CloudContentSourceManagementApp
{
    public partial class RegisterForm : Form
    {
        private TextBox txtUser, txtPass, txtRePass;
        private Button btnRegister, btnLogin;
        private Label lblError;
        private CheckBox chkShowPass;

        public RegisterForm()
        {
            InitializeComponent();
            this.Text = "Đăng ký tài khoản mới";
            this.Size = new Size(430, 470);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            // Title
            var lblTitle = new Label
            {
                Text = "Đăng ký tài khoản",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 119, 242),
                Location = new Point(90, 35),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Username
            var lblUser = new Label { Text = "Tên đăng nhập", Location = new Point(45, 100), AutoSize = true };
            txtUser = new TextBox { Location = new Point(45, 125), Width = 320, Font = new Font("Segoe UI", 11) };
            txtUser.GotFocus += (s, e) => txtUser.BackColor = Color.FromArgb(225, 240, 255);
            txtUser.LostFocus += (s, e) => txtUser.BackColor = Color.White;
            this.Controls.Add(lblUser); this.Controls.Add(txtUser);

            // Password
            var lblPass = new Label { Text = "Mật khẩu", Location = new Point(45, 170), AutoSize = true };
            txtPass = new TextBox { Location = new Point(45, 195), Width = 320, Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            txtPass.GotFocus += (s, e) => txtPass.BackColor = Color.FromArgb(225, 240, 255);
            txtPass.LostFocus += (s, e) => txtPass.BackColor = Color.White;
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);

            // Confirm Password
            var lblRePass = new Label { Text = "Nhập lại mật khẩu", Location = new Point(45, 240), AutoSize = true };
            txtRePass = new TextBox { Location = new Point(45, 265), Width = 320, Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            txtRePass.GotFocus += (s, e) => txtRePass.BackColor = Color.FromArgb(225, 240, 255);
            txtRePass.LostFocus += (s, e) => txtRePass.BackColor = Color.White;
            this.Controls.Add(lblRePass); this.Controls.Add(txtRePass);

            // Show Password
            chkShowPass = new CheckBox
            {
                Text = "Hiện mật khẩu",
                Location = new Point(45, 300),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            chkShowPass.CheckedChanged += (s, e) =>
            {
                txtPass.PasswordChar = txtRePass.PasswordChar = chkShowPass.Checked ? '\0' : '●';
            };
            this.Controls.Add(chkShowPass);

            // Error label
            lblError = new Label
            {
                ForeColor = Color.Red,
                Location = new Point(45, 330),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };
            this.Controls.Add(lblError);

            // Register Button
            btnRegister = new Button
            {
                Text = "Đăng ký",
                Location = new Point(45, 370),
                Width = 320,
                Height = 40,
                BackColor = Color.FromArgb(24, 119, 242),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(10, 90, 200);
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(24, 119, 242);
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // Login Button
            btnLogin = new Button
            {
                Text = "Quay lại đăng nhập",
                Location = new Point(45, 420),
                Width = 320,
                Height = 30,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(24, 119, 242),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderColor = Color.FromArgb(24, 119, 242);
            btnLogin.MouseEnter += (s, e) =>
            {
                btnLogin.BackColor = Color.FromArgb(220, 235, 255);
                btnLogin.ForeColor = Color.FromArgb(10, 90, 200);
            };
            btnLogin.MouseLeave += (s, e) =>
            {
                btnLogin.BackColor = Color.White;
                btnLogin.ForeColor = Color.FromArgb(24, 119, 242);
            };
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            // Validate
            if (string.IsNullOrWhiteSpace(txtUser.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Text) ||
                string.IsNullOrWhiteSpace(txtRePass.Text))
            {
                lblError.Text = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }
            if (txtPass.Text.Length < 6)
            {
                lblError.Text = "Mật khẩu tối thiểu 6 ký tự!";
                return;
            }
            if (txtPass.Text != txtRePass.Text)
            {
                lblError.Text = "Mật khẩu nhập lại không khớp!";
                return;
            }
            if (UserService.UsernameExists(txtUser.Text.Trim()))
            {
                lblError.Text = "Tên đăng nhập đã tồn tại!";
                return;
            }
            // Mã hóa pass, lưu vào DB
            var account = new UserAccount
            {
                Username = txtUser.Text.Trim(),
                PasswordHash = PasswordHelper.HashPassword(txtPass.Text.Trim()),
                TenantId = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.Now
            };
            UserService.AddUser(account);

            MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            new LoginForm().Show();
            this.Hide();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            new LoginForm().Show();
            this.Hide();
        }
    }
}
