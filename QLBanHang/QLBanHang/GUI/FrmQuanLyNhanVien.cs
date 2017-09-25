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
    public partial class FrmQuanLyNhanVien : Form
    {
        private QLBanHangDbContext db = Service.DBService.db;
        private int index = 0, index1 = 0;

        #region constructor
        public FrmQuanLyNhanVien()
        {
            InitializeComponent();
        }
        #endregion

        #region LoadForm

        private void LoadControl()
        {
            cbxGioiTinh.SelectedIndex = 0;
            groupThongTin.Enabled = false;
        }

        private void LoadDgvNhanVien()
        {
            int i = 0;
            string keyword = txtTimKiem.Text;
            var dbNV = db.NHANVIENs.ToList()
                       .Select(p => new
                       {
                           ID = p.ID,
                           STT = ++i,
                           HoTen = p.TEN,
                           NgaySinh = ((DateTime)p.NGAYSINH).ToString("dd/MM/yyyy"),
                           GioiTinh = (p.GIOITINH == 0) ? "Nữ" : "Nam",
                           Quyen = (p.QUYEN == 1) ? "Quản trị" : "Nhân viên"
                       })
                       .OrderByDescending(p => p.Quyen)
                       .ToList();

            dgvNhanVien.DataSource = dbNV.Where(p => p.HoTen.Contains(keyword) || p.GioiTinh.Contains(keyword) || p.Quyen.Contains(keyword) || p.NgaySinh.Contains(keyword))
                                    .ToList();

            // cập nhật index 
            index = index1;
            try
            {
                dgvNhanVien.Rows[index].Cells["STT"].Selected = true;
                dgvNhanVien.Select();
            }
            catch { }
        }


        private void FrmQuanLyNhanVien_Load(object sender, EventArgs e)
        {
            LoadControl();
            LoadDgvNhanVien();
        }
        #region Hàm chức năng

        private void ClearControl()
        {
            txtMaNhanVien.Text = "";
            txtHoVaTen.Text = "";
            cbxGioiTinh.SelectedIndex = 0;
            txtSDT.Text = "";
            txtQueQuan.Text = "";
            dateNgaySinh.Value = DateTime.Now;
            txtTaiKhoan.Text = "";
            cbxQuyen.SelectedIndex = 0;
        }

        private void UpdateDetail()
        {
            ClearControl();
            try
            {
                NHANVIEN nhanvien = getNhanVienByID();

                if (nhanvien == null || nhanvien.ID == 0) return;

                // cập nhật trên giao diện
                txtMaNhanVien.Text = nhanvien.MANV;
                txtHoVaTen.Text = nhanvien.TEN;
                cbxGioiTinh.SelectedIndex = (int)nhanvien.GIOITINH;
                txtSDT.Text = nhanvien.SDT;
                txtQueQuan.Text = nhanvien.QUEQUAN;
                dateNgaySinh.Value = (DateTime)nhanvien.NGAYSINH;
                txtTaiKhoan.Text = nhanvien.TAIKHOAN;
                cbxQuyen.SelectedIndex = (int)nhanvien.QUYEN;

                index1 = index;
                index = dgvNhanVien.SelectedRows[0].Index;
            }
            catch { }

        }

        private NHANVIEN getNhanVienByID()
        {
            try
            {
                int id = (int)dgvNhanVien.SelectedRows[0].Cells["ID"].Value;
                NHANVIEN nhanvien = db.NHANVIENs.Where(p => p.ID == id).FirstOrDefault();
                return (nhanvien != null) ? nhanvien : new NHANVIEN();
            }
            catch
            {
                return new NHANVIEN();
            }
        }

        private NHANVIEN getNhanVienByForm()
        {
            NHANVIEN ans = new NHANVIEN();
            ans.MANV = txtMaNhanVien.Text;
            ans.TEN = txtHoVaTen.Text;
            ans.GIOITINH = cbxGioiTinh.SelectedIndex;
            ans.SDT = txtSDT.Text;
            ans.QUEQUAN = txtQueQuan.Text;
            ans.TAIKHOAN = txtTaiKhoan.Text;
            ans.NGAYSINH = dateNgaySinh.Value;
            ans.QUYEN = cbxQuyen.SelectedIndex;
            ans.MATKHAU = "1";

            return ans;
        }

        private bool Check()
        {
            if (txtMaNhanVien.Text == "")
            {
                MessageBox.Show("Mã nhân viên không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int cnt = db.NHANVIENs.Where(p => p.MANV == txtMaNhanVien.Text).ToList().Count;
            if (cnt > 0)
            {
                bool ok = false;
                if (btnSua.Text == "Lưu")
                {
                    // Nếu là sửa
                    NHANVIEN nv = getNhanVienByID();
                    if (nv.MANV == txtMaNhanVien.Text) ok = true;
                }

                if (!ok)
                {
                    MessageBox.Show("Mã nhân viên đã được sử dụng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


            if (txtHoVaTen.Text == "")
            {
                MessageBox.Show("Họ và tên nhân viên không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            if (txtTaiKhoan.Text == "")
            {
                MessageBox.Show("Tài khoản của nhân viên không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (txtQueQuan.Text == "")
            {
                MessageBox.Show("Quê quán của nhân viên không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
