using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TechMartManager
{
    // ==========================================
    // 1. DATA MODELS (MÔ HÌNH DỮ LIỆU)
    // ==========================================
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        
        // Property phụ để hiển thị Tên Danh mục trên Grid
        public string CategoryName { get; set; } 
    }

    // ==========================================
    // 2. MAIN FORM (GIAO DIỆN & LOGIC)
    // ==========================================
    public class Form1 : Form
    {
        // Khai báo UI Controls
        private TableLayoutPanel mainLayout;
        private Panel leftPanel, rightPanel;
        
        private TextBox txtProductId, txtProductName, txtUnitPrice, txtQuantity, txtSearch;
        private ComboBox cboCategory;
        private PictureBox picAvatar;
        private Button btnChooseImage, btnAdd, btnUpdate, btnDelete;
        
        private DataGridView dgvProducts;
        private ErrorProvider errorProvider;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        // Data Management (Binding)
        private List<Product> _masterProductList; // Danh sách gốc
        private BindingList<Product> _bindingList; // Danh sách binding lên Grid
        private BindingSource _bindingSource;
        private List<Category> _categories;

        public Form1()
        {
            SetupUI(); // Tạo giao diện bằng code đảm bảo 100% tỷ lệ 35/65 (TC01)
            InitializeData();
            WireEvents();
        }

        #region Setup UI & Layout (Đáp ứng TC01)
        private void SetupUI()
        {
            this.Text = "TechMart Product Manager";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 1. Menu & Status Strip
            menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("File");
            var exportItem = new ToolStripMenuItem("Export CSV", null, ExportCSV_Click) { ShortcutKeys = Keys.Control | Keys.E };
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) => Application.Exit()) { ShortcutKeys = Keys.Control | Keys.X };
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { exportItem, exitItem });
            menuStrip.Items.Add(fileMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Tổng số sản phẩm: 0");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);

            // 2. TableLayoutPanel chia 35% - 65%
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            this.Controls.Add(mainLayout);
            mainLayout.BringToFront();

            // 3. Left Panel (Khung nhập liệu)
            leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainLayout.Controls.Add(leftPanel, 0, 0);

            int y = 20;
            leftPanel.Controls.Add(new Label { Text = "Mã SP:", Location = new Point(10, y), AutoSize = true });
            txtProductId = new TextBox { Location = new Point(100, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtProductId);

            y += 40;
            leftPanel.Controls.Add(new Label { Text = "Tên SP:", Location = new Point(10, y), AutoSize = true });
            txtProductName = new TextBox { Location = new Point(100, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtProductName);

            y += 40;
            leftPanel.Controls.Add(new Label { Text = "Danh mục:", Location = new Point(10, y), AutoSize = true });
            cboCategory = new ComboBox { Location = new Point(100, y), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(cboCategory);

            y += 40;
            leftPanel.Controls.Add(new Label { Text = "Đơn giá:", Location = new Point(10, y), AutoSize = true });
            txtUnitPrice = new TextBox { Location = new Point(100, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtUnitPrice);

            y += 40;
            leftPanel.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(10, y), AutoSize = true });
            txtQuantity = new TextBox { Location = new Point(100, y), Width = 200, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            leftPanel.Controls.Add(txtQuantity);

            y += 40;
            leftPanel.Controls.Add(new Label { Text = "Ảnh SP:", Location = new Point(10, y), AutoSize = true });
            picAvatar = new PictureBox { Location = new Point(100, y), Size = new Size(120, 120), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            btnChooseImage = new Button { Text = "Chọn Ảnh", Location = new Point(230, y), Width = 80 };
            leftPanel.Controls.Add(picAvatar);
            leftPanel.Controls.Add(btnChooseImage);

            y += 140;
            btnAdd = new Button { Text = "Thêm mới", Location = new Point(10, y), Width = 90 };
            btnUpdate = new Button { Text = "Cập nhật", Location = new Point(110, y), Width = 90 };
            btnDelete = new Button { Text = "Xóa", Location = new Point(210, y), Width = 90 };
            leftPanel.Controls.Add(btnAdd); leftPanel.Controls.Add(btnUpdate); leftPanel.Controls.Add(btnDelete);

            // 4. Right Panel (Bảng dữ liệu & Tìm kiếm)
            rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainLayout.Controls.Add(rightPanel, 1, 0);

            rightPanel.Controls.Add(new Label { Text = "Tìm kiếm SP:", Location = new Point(10, 15), AutoSize = true });
            txtSearch = new TextBox { Location = new Point(100, 12), Width = 300, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            rightPanel.Controls.Add(txtSearch);

            dgvProducts = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(rightPanel.Width - 20, rightPanel.Height - 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            
            // Cấu hình Cột Grid (TC03)
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Mã SP" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Tên SP" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Danh Mục" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Đơn Giá", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Số Lượng" });
            
            rightPanel.Controls.Add(dgvProducts);

            // Error Provider
            errorProvider = new ErrorProvider(this);
        }
        #endregion

        #region Initialization & Binding
        private void InitializeData()
        {
            // Init Data
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Điện thoại" },
                new Category { Id = 2, Name = "Laptop" },
                new Category { Id = 3, Name = "Phụ kiện" }
            };

            // Bind ComboBox
            cboCategory.DataSource = _categories;
            cboCategory.DisplayMember = "Name";
            cboCategory.ValueMember = "Id";

            _masterProductList = new List<Product>();
            _bindingList = new BindingList<Product>(_masterProductList);
            _bindingSource = new BindingSource { DataSource = _bindingList };
            dgvProducts.DataSource = _bindingSource;

            UpdateStatus();
        }

        private void WireEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
            btnChooseImage.Click += BtnChooseImage_Click;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }
        #endregion

        #region Event Handlers & Core Logic
        // TC02: Validation
        private bool ValidateInput()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                errorProvider.SetError(txtProductName, "Tên sản phẩm không được để trống!");
                isValid = false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal price) || price <= 0)
            {
                errorProvider.SetError(txtUnitPrice, "Đơn giá phải là số và lớn hơn 0!");
                isValid = false;
            }

            if (!int.TryParse(txtQuantity.Text, out int qty) || qty < 0)
            {
                errorProvider.SetError(txtQuantity, "Số lượng phải là số và >= 0!");
                isValid = false;
            }

            return isValid;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return; // TC02 Triggered

            var p = new Product
            {
                Id = string.IsNullOrWhiteSpace(txtProductId.Text) ? $"SP{DateTime.Now.Ticks.ToString().Substring(10)}" : txtProductId.Text,
                Name = txtProductName.Text,
                CategoryId = (int)cboCategory.SelectedValue,
                CategoryName = cboCategory.Text,
                UnitPrice = decimal.Parse(txtUnitPrice.Text),
                Quantity = int.Parse(txtQuantity.Text),
                ImagePath = picAvatar.ImageLocation
            };

            _masterProductList.Add(p);
            RefreshGrid();
            UpdateStatus();
            MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Product p && ValidateInput())
            {
                p.Id = txtProductId.Text;
                p.Name = txtProductName.Text;
                p.CategoryId = (int)cboCategory.SelectedValue;
                p.CategoryName = cboCategory.Text;
                p.UnitPrice = decimal.Parse(txtUnitPrice.Text);
                p.Quantity = int.Parse(txtQuantity.Text);
                p.ImagePath = picAvatar.ImageLocation;

                RefreshGrid();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // TC05: Xóa có Dialog xác nhận
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Product p)
            {
                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa '{p.Name}'?", "Xác nhận xóa", 
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    _masterProductList.Remove(p);
                    RefreshGrid();
                    UpdateStatus();
                }
            }
        }

        // Đổ dữ liệu ngược lên form khi Click Grid
        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Product p)
            {
                txtProductId.Text = p.Id;
                txtProductName.Text = p.Name;
                cboCategory.SelectedValue = p.CategoryId;
                txtUnitPrice.Text = p.UnitPrice.ToString("0");
                txtQuantity.Text = p.Quantity.ToString();
                
                if (File.Exists(p.ImagePath))
                    picAvatar.ImageLocation = p.ImagePath;
                else
                    picAvatar.Image = null; // Hoặc ảnh mặc định
            }
        }

        // TC04: OpenFileDialog load Ảnh
        private void BtnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh sản phẩm";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picAvatar.ImageLocation = ofd.FileName; // Nạp ảnh mượt mà, SizeMode = Zoom đã set ở trên
                }
            }
        }

        // Live Search TextChanged
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            string keyword = txtSearch.Text.ToLower();
            var filtered = _masterProductList.Where(p => p.Name.ToLower().Contains(keyword)).ToList();
            
            _bindingList.Clear();
            foreach (var item in filtered) _bindingList.Add(item);
        }

        private void UpdateStatus()
        {
            statusLabel.Text = $"Tổng số sản phẩm: {_masterProductList.Count}";
        }

        // Export CSV
        private void ExportCSV_Click(object sender, EventArgs e)
        {
            if (_masterProductList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV files (*.csv)|*.csv", FileName = "Products.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Mã SP,Tên SP,Danh Mục,Đơn Giá,Số Lượng"); // Header
                    
                    foreach (var p in _masterProductList)
                    {
                        sb.AppendLine($"{p.Id},{p.Name},{p.CategoryName},{p.UnitPrice},{p.Quantity}");
                    }

                    // Lưu file với UTF8 (Hỗ trợ tiếng Việt cho Excel)
                    File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion
    }
}