using QLBanHang.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLBanHang.GUI
{
    public partial class FrmMain : Form
    {
        private QLBanHangDbContext db = Service.DBService.db;
        private NHANVIEN nv = new NHANVIEN();

        #region constructor
        public FrmMain(NHANVIEN _nv)
        {
            InitializeComponent();
            Service.DBService.Reload();
            nv = _nv;
        }
        #endregion

        #region LoadForm

        private void LoadPhanQuyen()
        {
            if (nv.QUYEN == 1)
            {
                // phan quyen admin
                btnQLNhanVien.Enabled = true;
                btnQLMatHang.Enabled = true;
                btnQLKho.Enabled = true;
                btnXuatHang.Enabled = true;
                btnNhapHang.Enabled = true;
                return;
            }

            if (nv.QUYEN == 0)
            {
                // phan quyen nhan vien
                btnQLNhanVien.Enabled = false;
                btnQLMatHang.Enabled = false;
                btnQLKho.Enabled = true;
                btnXuatHang.Enabled = true;
                btnNhapHang.Enabled = true;
            }
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadPhanQuyen();
            txtTTNhanVien.Text = nv.TEN + " - " + ((nv.QUYEN == 0) ? "Nhân viên" : "Quản trị");
        }
        #endregion

        #region sự kiện
        private void btnQLNhanVien_Click(object sender, EventArgs e)
        {
            FrmQuanLyNhanVien form = new FrmQuanLyNhanVien();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(form);
            form.Show();
        }

        private void btnQLMatHang_Click(object sender, EventArgs e)
        {
            FrmQuanLyMatHang form = new FrmQuanLyMatHang();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(form);
            form.Show();
        }
        private void btnQLKho_Click(object sender, EventArgs e)
        {
            FrmKho form = new FrmKho();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(form);
            form.Show();
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn có chắc chắn đăng xuất không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (rs == DialogResult.Cancel) return;

            this.Close();
        }

        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            FrmNhapHang form = new FrmNhapHang(nv);
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(form);
            form.Show();
        }

        private void btnXuatHang_Click(object sender, EventArgs e)
        {
            FrmXuatHang form = new FrmXuatHang(nv);
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(form);
            form.Show();
        }



        private void txtDoiMatKhau_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmDoiMatKhau form = new FrmDoiMatKhau(nv);
            form.ShowDialog();
        }
        #endregion

    }
}
