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
    public partial class FrmQuanLyMatHang : Form
    {
        private QLBanHangDbContext db = Service.DBService.db;
        private int index = 0, index1 = 0;

        #region constructor
        public FrmQuanLyMatHang()
        {
            InitializeComponent();
        }
        #endregion

        #region LoadForm

        private void LoadControl()
        {
            groupThongTin.Enabled = false;
        }

        private void LoadDgvNhanVien()
        {
            int i = 0;
            string keyword = txtTimKiem.Text;
            var dbNV = db.MATHANGs.ToList()
                       .Select(p => new
                       {
                           ID = p.ID,
                           STT = ++i,
                           TenMH = p.TEN,
                           DonViTinh = p.DONVITINH,
                           GhiChu = p.GHICHU
                       })
                       .ToList();

            dgvMatHang.DataSource = dbNV
                                    .Where(p => p.TenMH.Contains(keyword) || p.DonViTinh.Contains(keyword))
                                    .ToList();

            // cập nhật index 
            index = index1;
            try
            {
                dgvMatHang.Rows[index].Cells["STT"].Selected = true;
                dgvMatHang.Select();
            }
            catch { }
        }


        private void FrmQuanLyNhanVien_Load(object sender, EventArgs e)
        {
            LoadControl();
            LoadDgvNhanVien();
        }
        #endregion

        #region Hàm chức năng

        private void ClearControl()
        {
            txtMaMH.Text = "";
            txtTenMH.Text = "";
            txtDVT.Text = "";
            txtGhiChu.Text = "";
        }

        private void UpdateDetail()
        {
            ClearControl();
            try
            {
                MATHANG tg = getMatHangByID();

                if (tg == null || tg.ID == 0) return;

                // cập nhật trên giao diện
                txtMaMH.Text = tg.MAMH;
                txtTenMH.Text = tg.TEN;
                txtDVT.Text = tg.DONVITINH;
                txtGhiChu.Text = tg.GHICHU;

                index1 = index;
                index = dgvMatHang.SelectedRows[0].Index;
            }
            catch { }

        }

        private MATHANG getMatHangByID()
        {
            try
            {
                int id = (int)dgvMatHang.SelectedRows[0].Cells["ID"].Value;
                MATHANG nhanvien = db.MATHANGs.Where(p => p.ID == id).FirstOrDefault();
                return (nhanvien != null) ? nhanvien : new MATHANG();
            }
            catch
            {
                return new MATHANG();
            }
        }

        private MATHANG getMatHangByForm()
        {
            MATHANG ans = new MATHANG();
            ans.MAMH = txtMaMH.Text;
            ans.TEN = txtTenMH.Text;
            ans.DONVITINH = txtDVT.Text;
            ans.GHICHU = txtGhiChu.Text;

            return ans;
        }

        private bool Check()
        {
            if (txtMaMH.Text == "")
            {
                MessageBox.Show("Mã mặt hàng không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int cnt = db.MATHANGs.Where(p => p.MAMH == txtMaMH.Text).ToList().Count;
            if (cnt > 0)
            {
                bool ok = false;
                if (btnSua.Text == "Lưu")
                {
                    // Nếu là sửa
                    MATHANG tg = getMatHangByID();
                    if (tg.MAMH == txtMaMH.Text) ok = true;
                }

                if (!ok)
                {
                    MessageBox.Show("Mã mặt hàng đã được sử dụng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


            if (txtTenMH.Text == "")
            {
                MessageBox.Show("Tên mặt hàng không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            if (txtDVT.Text == "")
            {
                MessageBox.Show("Đơn vị tính không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        #endregion

        #region sự kiện ngầm
        private void dgvNhanVien_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDetail();
        }
        #endregion

        #region sự kiện
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadDgvNhanVien();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (btnThem.Text == "Thêm")
            {

                btnThem.Text = "Lưu";
                btnSua.Enabled = false;
                btnXoa.Text = "Hủy";

                groupThongTin.Enabled = true;
                dgvMatHang.Enabled = false;

                btnTimKiem.Enabled = false;
                txtTimKiem.Enabled = false;

                ClearControl();

                return;
            }

            if (btnThem.Text == "Lưu")
            {
                if (Check())
                {

                    btnThem.Text = "Thêm";
                    btnSua.Enabled = true;
                    btnXoa.Text = "Xóa";

                    groupThongTin.Enabled = false;
                    dgvMatHang.Enabled = true;

                    btnTimKiem.Enabled = true;
                    txtTimKiem.Enabled = true;

                    try
                    {
                        MATHANG tg = getMatHangByForm();
                        db.MATHANGs.Add(tg);
                        db.SaveChanges();
                        MessageBox.Show("Thêm thông tin mặt hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm thông tin mặt hàng thất bại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    LoadDgvNhanVien();
                }

                return;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            MATHANG tg = getMatHangByID();
            if (tg.ID == 0)
            {
                MessageBox.Show("Chưa có mặt hàng nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (btnSua.Text == "Sửa")
            {
                btnSua.Text = "Lưu";
                btnThem.Enabled = false;
                btnXoa.Text = "Hủy";

                groupThongTin.Enabled = true;
                dgvMatHang.Enabled = false;

                btnTimKiem.Enabled = false;
                txtTimKiem.Enabled = false;
                return;
            }

            if (btnSua.Text == "Lưu")
            {
                if (Check())
                {
                    btnSua.Text = "Sửa";
                    btnThem.Enabled = true;
                    btnXoa.Text = "Xóa";

                    groupThongTin.Enabled = false;
                    dgvMatHang.Enabled = true;

                    btnTimKiem.Enabled = true;
                    txtTimKiem.Enabled = true;

                    MATHANG tgs = getMatHangByForm();
                    tg.MAMH = tgs.MAMH;
                    tg.TEN = tgs.TEN;
                    tg.DONVITINH = tgs.DONVITINH;
                    tg.GHICHU = tgs.GHICHU;

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Sửa thông tin mặt hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sửa thông tin mặt hàng thất bại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    LoadDgvNhanVien();
                }

                return;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (btnXoa.Text == "Xóa")
            {
                MATHANG tg = getMatHangByID();
                if (tg.ID == 0)
                {
                    MessageBox.Show("Chưa có mặt hàng nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult rs = MessageBox.Show("Bạn có chắc chắn xóa thông tin mặt hàng này?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (rs == DialogResult.Cancel) return;

                try
                {
                    db.MATHANGs.Remove(tg);
                    db.SaveChanges();
                    MessageBox.Show("Xóa mặt hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Xóa mặt hàng thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadDgvNhanVien();

                return;
            }

            if (btnXoa.Text == "Hủy")
            {
                btnXoa.Text = "Xóa";
                btnThem.Text = "Thêm";
                btnSua.Text = "Sửa";

                btnThem.Enabled = true;
                btnSua.Enabled = true;

                groupThongTin.Enabled = false;
                dgvMatHang.Enabled = true;

                btnTimKiem.Enabled = true;
                txtTimKiem.Enabled = true;

                UpdateDetail();

                return;
            }
        }
        #endregion
    }
}
