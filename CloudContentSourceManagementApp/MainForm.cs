using CloudContentSourceManagementApp.CommonHelper;
using CloudContentSourceManagementApp.Models;
using CloudContentSourceManagementApp.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Label = System.Windows.Forms.Label;

namespace CloudContentSourceManagementApp
{
    public partial class MainForm : Form
    {
        private TabControl tabControlMain;
        private TabPage tabAppProfile;
        private TabPage tabContentSource;
        private TreeView tvwContentSources;
        private ContextMenuStrip ctxMenuTree;
        private Panel pnlScanConfig;
        private ComboBox cmbScanMode, cmbFilterType;
        private TextBox txtFilterValue;
        private Button btnScan;
        public MainForm()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            // TabControl (nếu không dùng Designer thì code tay như dưới)
            InitializeComponent();

            tabControlMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11)
            };

            // Tab Quản lý App Profile (Gọn hơn)
            tabAppProfile = new TabPage("App Profile")
            {
                Name = "tabAppProfile"
            };
            InitAppProfileTabUI();

            // Tab Google Content Source
            tabContentSource = new TabPage("Google Content Source")
            {
                Name = "tabContentSource"
            };
            InitContentSourceTabUI();

            // Thêm tab vào control
            tabControlMain.TabPages.Add(tabAppProfile);
            tabControlMain.TabPages.Add(tabContentSource);

            this.Controls.Add(tabControlMain);
            this.Load += MainForm_Load;
        }

        private void InitAppProfileTabUI()
        {
            // Title gọn
            var lblTitle = new Label
            {
                Text = "App Profile Google Drive",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true,
                ForeColor = Color.FromArgb(24, 119, 242)
            };
            tabAppProfile.Controls.Add(lblTitle);

            // DataGridView
            var dgvProfiles = new DataGridView
            {
                Name = "dgvProfiles",
                Location = new Point(20, 45),
                Size = new Size(670, 180),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabAppProfile.Controls.Add(dgvProfiles);

            // Button tạo mới
            var btnCreateProfile = new Button
            {
                Text = "Tạo App Profile",
                Location = new Point(20, 240),
                Size = new Size(180, 34),
                BackColor = Color.FromArgb(24, 119, 242),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnCreateProfile.FlatAppearance.BorderSize = 0;
            btnCreateProfile.Click += BtnCreateProfile_Click;
            tabAppProfile.Controls.Add(btnCreateProfile);
        }

        private void BtnCreateProfile_Click(object sender, EventArgs e)
        {
            var dlg = new CreateAppProfileForm();
            dlg.ShowDialog();
            // Nên reload lại grid sau khi tạo
            ReloadAppProfiles(GetProfiles());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Check profile tồn tại chưa
            var appProfiles = GoogleAppProfileService.GetGoogleAppProfilesByTenantId(LogonSystem.TenantId);
            if (appProfiles.Count() == 0)
            {
                // Chưa có: mở form tạo profile
                MessageBox.Show("Chưa có App Profile. Vui lòng tạo mới để sử dụng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var createForm = new CreateAppProfileForm();
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    // Sau khi tạo xong, reload lại tab profile
                    ReloadAppProfiles(appProfiles);
                }
            }
            else
            {
                // Đã có, load vào grid
                ReloadAppProfiles(appProfiles);
            }
        }

        #region Tab Content Source
        private void InitContentSourceTabUI()
        {
            var lblTitle = new Label
            {
                Text = "Google Content Source (Shared Drive)",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true,
                ForeColor = Color.FromArgb(24, 119, 242)
            };
            tabContentSource.Controls.Add(lblTitle);

            // TreeView
            tvwContentSources = new TreeView
            {
                Name = "tvwContentSources",
                Location = new Point(20, 50),
                Size = new Size(350, 370),
                Font = new Font("Segoe UI", 11),
                HideSelection = false
            };
            tabContentSource.Controls.Add(tvwContentSources);

            // Context menu chuột phải cho TreeView
            ctxMenuTree = new ContextMenuStrip();
            ctxMenuTree.Items.Add("Tạo Container mới", null, (s, e) => ShowCreateContainerDialog());
            tvwContentSources.ContextMenuStrip = ctxMenuTree;

            // Xử lý click vào node container (chỉ node cấp 1 mới có quyền scan)
            tvwContentSources.NodeMouseClick += TvwContentSources_NodeMouseClick;
        }

        private void ShowCreateContainerDialog()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập tên Container mới:",
                "Tạo Container",
                "New Container");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var containerNode = new TreeNode(name)
                {
                    Tag = new ContentContainer { Name = name, Id = Guid.NewGuid() }
                };
                tvwContentSources.Nodes.Add(containerNode);
                // nên xử lý lưu Container vào DB hoặc service nếu cần
            }
        }

        private void TvwContentSources_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvwContentSources.SelectedNode = e.Node;

            // Nếu là node Container (cấp 1)
            if (e.Node.Level == 0)
            {
                ShowScanConfigPanel(e.Node);
            }
            // Nếu là node Shared Drive (cấp 2)
            else if (e.Node.Level == 1)
            {
                MessageBox.Show("Bạn chọn Shared Drive: " + e.Node.Text);
                // Có thể hiện chi tiết shared drive ở đây
            }
        }

        private void ShowScanConfigPanel(TreeNode containerNode)
        {
            // Xoá panel cũ nếu có
            if (pnlScanConfig != null)
                tabContentSource.Controls.Remove(pnlScanConfig);

            pnlScanConfig = new Panel
            {
                Name = "pnlScanConfig",
                Location = new Point(400, 50),
                Size = new Size(340, 200),
                BorderStyle = BorderStyle.FixedSingle
            };
            tabContentSource.Controls.Add(pnlScanConfig);

            // Label Mode
            pnlScanConfig.Controls.Add(new Label
            {
                Text = "Chế độ scan:",
                Location = new Point(10, 15),
                AutoSize = true
            });

            cmbScanMode = new ComboBox
            {
                Location = new Point(110, 12),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbScanMode.Items.AddRange(new[] { "All", "Tuỳ chọn" });
            cmbScanMode.SelectedIndex = 0;
            cmbScanMode.SelectedIndexChanged += (s, e) => SwitchScanModeUI();
            pnlScanConfig.Controls.Add(cmbScanMode);

            // Filter theo tên shared drive
            var lblType = new Label { Text = "Tên Shared Drive:", Location = new Point(10, 55), AutoSize = true };
            pnlScanConfig.Controls.Add(lblType);

            cmbFilterType = new ComboBox
            {
                Location = new Point(130, 52),
                Width = 70,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterType.Items.AddRange(new[] { "Contains", "Equal" });
            cmbFilterType.SelectedIndex = 0;
            pnlScanConfig.Controls.Add(cmbFilterType);

            txtFilterValue = new TextBox { Location = new Point(210, 52), Width = 100 };
            pnlScanConfig.Controls.Add(txtFilterValue);

            // Ban đầu nếu là All thì ẩn filter
            SwitchScanModeUI();

            // Scan button
            btnScan = new Button
            {
                Text = "Scan",
                Location = new Point(110, 120),
                Width = 100,
                Height = 36,
                BackColor = Color.FromArgb(24, 119, 242),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnScan.FlatAppearance.BorderSize = 0;
            btnScan.Click += (s, e) => ScanSharedDrives(containerNode);
            pnlScanConfig.Controls.Add(btnScan);
        }

        private void SwitchScanModeUI()
        {
            bool filterMode = cmbScanMode.SelectedIndex == 1; // Tuỳ chọn
            cmbFilterType.Visible = filterMode;
            txtFilterValue.Visible = filterMode;
        }

        private async void ScanSharedDrives(TreeNode containerNode)
        {
            // Tạo form Job monitor và Tab Job Monitor , trong Tb này gồm Tab Process và Tab Quue
            // Tạo job worker để xử lý viêc Scan
            var profile = GoogleAppProfileService.GetGoogleAppProfilesByTenantId(LogonSystem.TenantId)[0];
            var service = new GoogleDriveService(profile);
        }

        public async Task<List<Drive>> GetSharedDrivesByNameAsync(DriveService service, string keyword)
        {
            var result = new List<Drive>();
            string pageToken = null;

            do
            {
                var request = service.Drives.List();
                request.PageSize = 100;
                request.Fields = "*";
                request.Q = null; // Không cần filter gì ở đây, API không hỗ trợ Q cho shared drive name
                request.PageToken = pageToken;

                var response = await request.ExecuteAsync();

                // Lọc bằng LINQ vì Google API v3 chưa support Q cho Drives.List
                //result.AddRange(response.Drives.Where(d => d.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
                var list = response.Drives.Where(d => d.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
                result.AddRange(list);
                pageToken = response.NextPageToken;
            }
            while (!string.IsNullOrEmpty(pageToken));

            return result;
        }
        #endregion

        private void ReloadAppProfiles(List<GoogleAppProfile> appProfiles)
        {
            // Tải profile lên DataGridView...
            // var profiles = AppProfileService.GetProfilesByTenant(LogonSystem.TenantId);
            // dgvProfiles.DataSource = profiles;
            // Nếu dùng Unix timestamp (seconds)
            var bindingList = appProfiles.Select(x => new
            {
                x.Id,
                x.ProfileName,
                x.UserEmail,
                x.ClientId,
                x.ClientEmail,
                // Giả sử CreatedTime là Unix time (seconds), còn ticks thì phải đổi khác nhé!
                CreatedTime = new DateTime(x.CreatedTime).ToString("dd/MM/yyyy HH:mm:ss"),
                x.TenantId
            }).ToList();

            var dgv = tabAppProfile.Controls.Find("dgvProfiles", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                dgv.DataSource = null; // clear cũ
                dgv.DataSource = bindingList;

                dgv.Columns["Id"].Visible = false; // Ẩn Id nếu muốn
                dgv.Columns["TenantId"].Visible = false; // Ẩn TenantId nếu không cần
                dgv.Columns["CreatedTime"].HeaderText = "Ngày tạo";
                dgv.Columns["ProfileName"].HeaderText = "Tên Profile";
                dgv.Columns["UserEmail"].HeaderText = "User Email";
                dgv.Columns["ClientId"].HeaderText = "Client Id";
                dgv.Columns["ClientEmail"].HeaderText = "Client Email";
                dgv.AutoResizeColumns();
            }
        }

        private List<GoogleAppProfile> GetProfiles()
        {
            // Có thể sửa lại service lấy danh sách, đây là demo
            return GoogleAppProfileService.GetGoogleAppProfilesByTenantId(LogonSystem.TenantId).ToList();
        }
    }
}
