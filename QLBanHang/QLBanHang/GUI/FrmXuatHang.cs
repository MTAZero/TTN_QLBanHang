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
    public partial class FrmXuatHang : Form
    {
        private QLBanHangDbContext db = Service.DBService.db;
        private int indexHOADONBAN = 0, indexHOADONBAN1 = 0;
        private int indexCHITIETXUAT = 0, indexCHITIETXUAT1 = 0;
        private NHANVIEN nv = new NHANVIEN();

        #region constructor
        public FrmXuatHang(NHANVIEN _nv)
        {
            InitializeComponent();
            Service.DBService.Reload();
            nv = _nv;
        }
        #endregion

        #region Form chính
        private void FrmNhapHang_Load(object sender, EventArgs e)
        {
            LoadHOADONBAN();
            LoadCHITIETXUAT();
        }
        #endregion

        #region Phiếu nhập

        #region Load
        private void LoadInitControl()
        {
            // cbx Nhân viên
            cbxNhanVien.DataSource = db.NHANVIENs.ToList();
            cbxNhanVien.ValueMember = "ID";
            cbxNhanVien.DisplayMember = "TEN";

            dateNgayNhap.Value = DateTime.Now;

            groupThongTinHoaDonBan.Enabled = false;

        }

        private void LoadDgvHOADONBAN()
        {
            int i = 0;
            var dataHOADONBAN = db.HOADONBANs.ToList()
                                .Select(p => new
                                {
                                    ID = p.ID,
                                    STT = ++i,
                                    Ngay = ((DateTime)p.NGAY).ToString("dd/MM/yyyy"),
                                    NhanVien = db.NHANVIENs.Where(z => z.ID == p.NHANVIENID).FirstOrDefault().TEN,
                                    TongTien = p.TONGTIEN
                                })
                                .ToList();
            dgvHoaDonBan.DataSource = dataHOADONBAN;

            // thêm index 
            indexHOADONBAN = indexHOADONBAN1;
            try
            {
                dgvHoaDonBan.Rows[indexHOADONBAN].Cells["STTPhieuNhap"].Selected = true;
                dgvHoaDonBan.Select();
            }
            catch { }

            LoadDgvCHITIETXUAT();
        }

        private void LoadHOADONBAN()
        {
            LoadInitControl();
            LoadDgvHOADONBAN();
        }
        #endregion

        #region Hàm chức năng

        private void UpdateDetailHOADONBAN()
        {
            ClearControlHOADONBAN();
            HOADONBAN tg = getHOADONBANByID();
            if (tg.ID == 0) return;


            int TongTienCu = (int)tg.TONGTIEN;

            try
            {
                try
                {

                    int cnt = 0;
                    cnt = db.CHITIETXUATs.Where(p => p.HOADONBANID == tg.ID).ToList().Count;
                    if (cnt == 0) tg.TONGTIEN = 0;


                    tg.TONGTIEN = db.CHITIETXUATs.Where(p => p.HOADONBANID == tg.ID).Sum(p => p.THANHTIEN).Value;

                }
                catch { tg.TONGTIEN = 0; }

                if (TongTienCu != tg.TONGTIEN) LoadDgvHOADONBAN();
                db.SaveChanges();

                cbxNhanVien.SelectedValue = tg.NHANVIENID;
                dateNgayNhap.Value = (DateTime)tg.NGAY;
                txtTongTien.Text = tg.TONGTIEN.ToString();

                indexHOADONBAN1 = indexHOADONBAN;
                indexHOADONBAN = dgvHoaDonBan.SelectedRows[0].Index;


                LoadDgvCHITIETXUAT();
            }
            catch { }
        }

        private void ClearControlHOADONBAN()
        {
            try
            {
                cbxNhanVien.SelectedIndex = 0;
                dateNgayNhap.Value = DateTime.Now;
                txtTongTien.Text = "";
            }
            catch { }
        }

        private bool CheckHOADONBAN()
        {

            return true;
        }

        private HOADONBAN getHOADONBANByID()
        {
            HOADONBAN ans = new HOADONBAN();

            try
            {
                int id = (int)dgvHoaDonBan.SelectedRows[0].Cells["IDPhieuNhap"].Value;
                HOADONBAN z = db.HOADONBANs.Where(p => p.ID == id).FirstOrDefault();

                if (z != null) ans = z;
            }
            catch { }

            return ans;
        }

        private HOADONBAN getHOADONBANByForm()
        {
            HOADONBAN ans = new HOADONBAN();

            ans.NHANVIENID = (int)cbxNhanVien.SelectedValue;
            ans.NGAY = dateNgayNhap.Value;
            ans.TONGTIEN = 0;
            //ans.TONGTIEN = Int32.Parse(txtTongTien.Text);

            return ans;
        }

        #endregion

        #region Sự kiện ngầm

        private void dgvHOADONBAN_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDetailHOADONBAN();
        }

        #endregion

        #region Sự kiện
        private void btnThemHOADONBAN_Click(object sender, EventArgs e)
        {
            if (btnThemHoaDonBan.Text == "Thêm")
            {

                btnThemHoaDonBan.Text = "Lưu";
                btnSuaHoaDonBan.Enabled = false;
                btnXoaHoaDonBan.Text = "Hủy";

                groupThongTinHoaDonBan.Enabled = true;
                dgvHoaDonBan.Enabled = false;

                panelChiTietXuat.Enabled = false;

                ClearControlHOADONBAN();

                return;
            }

            if (btnThemHoaDonBan.Text == "Lưu")
            {
                if (CheckHOADONBAN())
                {

                    btnThemHoaDonBan.Text = "Thêm";
                    btnSuaHoaDonBan.Enabled = true;
                    btnXoaHoaDonBan.Text = "Xóa";

                    groupThongTinHoaDonBan.Enabled = false;
                    dgvHoaDonBan.Enabled = true;

                    panelChiTietXuat.Enabled = true;


                    try
                    {
                        HOADONBAN tg = getHOADONBANByForm();
                        db.HOADONBANs.Add(tg);
                        db.SaveChanges();
                        MessageBox.Show("Thêm thông tin phiếu nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm thông tin phiếu nhập thất bại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    LoadDgvHOADONBAN();
                }

                return;
            }
        }

        private void btnSuaHOADONBAN_Click(object sender, EventArgs e)
        {
            HOADONBAN tg = getHOADONBANByID();
            if (tg.ID == 0)
            {
                MessageBox.Show("Chưa có phiếu nhập nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (btnSuaHoaDonBan.Text == "Sửa")
            {
                btnSuaHoaDonBan.Text = "Lưu";
                btnThemHoaDonBan.Enabled = false;
                btnXoaHoaDonBan.Text = "Hủy";

                groupThongTinHoaDonBan.Enabled = true;
                dgvHoaDonBan.Enabled = false;

                panelChiTietXuat.Enabled = false;

                return;
            }

            if (btnSuaHoaDonBan.Text == "Lưu")
            {
                if (CheckHOADONBAN())
                {
                    btnSuaHoaDonBan.Text = "Sửa";
                    btnThemHoaDonBan.Enabled = true;
                    btnXoaHoaDonBan.Text = "Xóa";

                    groupThongTinHoaDonBan.Enabled = false;
                    dgvHoaDonBan.Enabled = true;

                    panelChiTietXuat.Enabled = true;

                    HOADONBAN tgs = getHOADONBANByForm();
                    tg.NHANVIENID = tgs.NHANVIENID;
                    tg.NGAY = tgs.NGAY;
                    tg.TONGTIEN = tgs.TONGTIEN;

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Sửa thông tin phiếu nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sửa thông tin phiếu nhập thất bại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoadDgvHOADONBAN();
                }

                return;
            }
        }

        private void btnXoaHOADONBAN_Click(object sender, EventArgs e)
        {
            if (btnXoaHoaDonBan.Text == "Xóa")
            {
                HOADONBAN tg = getHOADONBANByID();
                if (tg.ID == 0)
                {
                    MessageBox.Show("Chưa có phiếu nhập nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult rs = MessageBox.Show("Bạn có chắc chắn xóa thông tin phiếu nhập này?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (rs == DialogResult.Cancel) return;

                try
                {
                    db.HOADONBANs.Remove(tg);
                    db.SaveChanges();
                    MessageBox.Show("Xóa phiếu nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Xóa phiếu nhập thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadDgvHOADONBAN();

                return;
            }

            if (btnXoaHoaDonBan.Text == "Hủy")
            {
                btnXoaHoaDonBan.Text = "Xóa";
                btnThemHoaDonBan.Text = "Thêm";
                btnSuaHoaDonBan.Text = "Sửa";

                btnThemHoaDonBan.Enabled = true;
                btnSuaHoaDonBan.Enabled = true;

                groupThongTinHoaDonBan.Enabled = false;
                dgvHoaDonBan.Enabled = true;

                panelChiTietXuat.Enabled = true;

                UpdateDetailHOADONBAN();

                return;
            }
        }

        #endregion

        #endregion

        #region Chi tiết bán

        #region Load
        private void LoadInitControlCHITIETXUAT()
        {
            // cbx Nhân viên
            cbxMatHang.DataSource = db.MATHANGs.ToList();
            cbxMatHang.ValueMember = "ID";
            cbxMatHang.DisplayMember = "TEN";

            groupThongTinChiTietBan.Enabled = false;

        }

        private void LoadDgvCHITIETXUAT()
        {

            int idHOADONBAN = 0;
            try
            {
                idHOADONBAN = (int)dgvHoaDonBan.SelectedRows[0].Cells["IDPhieuNhap"].Value;
            }
            catch
            {
            }

            int i = 0;
            var dataCHITIETXUAT = db.CHITIETXUATs.ToList()
                                  .Where(p => p.HOADONBANID == idHOADONBAN)
                                  .Select(p => new
                                  {
                                      ID = p.ID,
                                      STT = ++i,
                                      MatHang = db.MATHANGs.Where(z => z.ID == p.MATHANGID).FirstOrDefault().TEN,
                                      SoLuong = p.SOLUONG,
                                      DonGia = p.GIABAN,
                                      ThanhTien = p.THANHTIEN
                                  })
                                  .ToList();
            dgvChiTietBan.DataSource = dataCHITIETXUAT;

            // thêm index 
            indexCHITIETXUAT = indexCHITIETXUAT1;
            try
            {
                dgvChiTietBan.Rows[indexCHITIETXUAT].Cells["STTChiTietNhap"].Selected = true;
                dgvChiTietBan.Select();
            }
            catch { }
        }

        private void LoadCHITIETXUAT()
        {
            LoadInitControlCHITIETXUAT();
            LoadDgvCHITIETXUAT();
        }
        #endregion

        #region Hàm chức năng

        private void UpdateDetailChiTietBan()
        {
            ClearControlCHITIETBAN();
            CHITIETXUAT tg = getChiTietBanByID();
            if (tg.ID == 0) return;

            try
            {
                cbxMatHang.SelectedValue = (int)tg.MATHANGID;
                txtSoLuong.Text = tg.SOLUONG.ToString();
                txtDonGia.Text = tg.GIABAN.ToString();
                txtThanhTien.Text = tg.THANHTIEN.ToString();

                indexCHITIETXUAT1 = indexCHITIETXUAT;
                indexCHITIETXUAT = dgvChiTietBan.SelectedRows[0].Index;
            }
            catch { }
        }

        private void ClearControlCHITIETBAN()
        {
            try
            {
                cbxMatHang.SelectedIndex = 0;
                txtSoLuong.Text = "";
                txtDonGia.Text = "";
                txtThanhTien.Text = "";
            }
            catch { }
        }

        private bool CheckChiTietBan()
        {
            // số lượng
            try
            {
                int k = Int32.Parse(txtSoLuong.Text);
                if (k == 0) k = 3 / k;
            }
            catch
            {
                MessageBox.Show("Số lượng phải là số nguyên dương", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // check hàng trong kho
            try
            {
                CHITIETXUAT tg = getChiTietBanByForm();
                int k = Int32.Parse(txtSoLuong.Text);
                int sl = (int)db.KHOes.Where(p => p.MATHANGID == tg.MATHANGID).FirstOrDefault().SOLUONG;
                if (k > sl)
                {
                    MessageBox.Show("Không đủ hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch { }

            // đơn giá
            try
            {
                int k = Int32.Parse(txtDonGia.Text);
            }
            catch
            {
                MessageBox.Show("Đơn giá phải là số nguyên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private CHITIETXUAT getChiTietBanByID()
        {
            CHITIETXUAT ans = new CHITIETXUAT();

            try
            {
                int id = (int)dgvChiTietBan.SelectedRows[0].Cells["IDChiTietNhap"].Value;
                CHITIETXUAT z = db.CHITIETXUATs.Where(p => p.ID == id).FirstOrDefault();

                if (z != null) ans = z;
            }
            catch { }

            return ans;
        }

        private CHITIETXUAT getChiTietBanByForm()
        {
            CHITIETXUAT ans = new CHITIETXUAT();

            try
            {
                int idHOADONBAN;
                ans.MATHANGID = (int)cbxMatHang.SelectedValue;
                ans.SOLUONG = Int32.Parse(txtSoLuong.Text);
                ans.GIABAN = Int32.Parse(txtDonGia.Text);
                ans.THANHTIEN = ans.SOLUONG * ans.GIABAN;

                idHOADONBAN = (int)dgvHoaDonBan.SelectedRows[0].Cells["IDPhieuNhap"].Value;
                ans.HOADONBANID = idHOADONBAN;
            }
            catch { }

            return ans;
        }

        #endregion

        #region Sự kiện ngầm

        private void dgvChiTietNhap_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDetailChiTietBan();
        }

        private void cbxMatHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int id = (int)cbxMatHang.SelectedValue;
                txtDonViTinh.Text = db.MATHANGs.Where(p => p.ID == id).FirstOrDefault().DONVITINH;
            }
            catch { }
        }

        #endregion

        #region Sự kiện
        private void btnThemChiTietBan_Click(object sender, EventArgs e)
        {
            if (btnThemChiTietBan.Text == "Thêm")
            {

                btnThemChiTietBan.Text = "Lưu";
                btnSuaChiTietBan.Enabled = false;
                btnXoaChiTietBan.Text = "Hủy";

                groupThongTinChiTietBan.Enabled = true;
                dgvChiTietBan.Enabled = false;

                panelHoaDonBan.Enabled = false;

                ClearControlCHITIETBAN();

                return;
            }

            if (btnThemChiTietBan.Text == "Lưu")
            {
                if (CheckChiTietBan())
                {

                    btnThemChiTietBan.Text = "Thêm";
                    btnSuaChiTietBan.Enabled = true;
                    btnXoaChiTietBan.Text = "Xóa";

                    groupThongTinChiTietBan.Enabled = false;
                    dgvChiTietBan.Enabled = true;

                    panelHoaDonBan.Enabled = true;


                    try
                    {
                        CHITIETXUAT tg = getChiTietBanByForm();
                        db.CHITIETXUATs.Add(tg);
                        db.SaveChanges();
                        MessageBox.Show("Thêm thông tin Chi tiết nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm thông tin Chi tiết nhập thất bại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    LoadDgvCHITIETXUAT();
                    UpdateDetailHOADONBAN();
                }

                return;
            }
        }

        private void btnSuaChiTietBan_Click(object sender, EventArgs e)
        {
            CHITIETXUAT tg = getChiTietBanByID();
            if (tg.ID == 0)
            {
                MessageBox.Show("Chưa có Chi tiết nhập nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (btnSuaChiTietBan.Text == "Sửa")
            {
                btnSuaChiTietBan.Text = "Lưu";
                btnThemChiTietBan.Enabled = false;
                btnXoaChiTietBan.Text = "Hủy";

                groupThongTinChiTietBan.Enabled = true;
                dgvChiTietBan.Enabled = false;

                panelHoaDonBan.Enabled = false;

                return;
            }

            if (btnSuaChiTietBan.Text == "Lưu")
            {
                if (CheckChiTietBan())
                {
                    btnSuaChiTietBan.Text = "Sửa";
                    btnThemChiTietBan.Enabled = true;
                    btnXoaChiTietBan.Text = "Xóa";

                    groupThongTinChiTietBan.Enabled = false;
                    dgvChiTietBan.Enabled = true;

                    panelHoaDonBan.Enabled = true;

                    CHITIETXUAT tgs = getChiTietBanByForm();
                    tg.MATHANGID = tgs.MATHANGID;
                    tg.SOLUONG = tgs.SOLUONG;
                    tg.GIABAN = tgs.GIABAN;
                    tg.THANHTIEN = tgs.THANHTIEN;

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Sửa thông tin Chi tiết nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sửa thông tin Chi tiết nhập thất bại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    UpdateDetailHOADONBAN();
                    LoadDgvCHITIETXUAT();
                }

                return;
            }
        }

        private void btnXoaChiTietBan_Click(object sender, EventArgs e)
        {
            if (btnXoaChiTietBan.Text == "Xóa")
            {
                CHITIETXUAT tg = getChiTietBanByID();
                if (tg.ID == 0)
                {
                    MessageBox.Show("Chưa có Chi tiết nhập nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult rs = MessageBox.Show("Bạn có chắc chắn xóa thông tin Chi tiết nhập này?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (rs == DialogResult.Cancel) return;

                try
                {
                    db.CHITIETXUATs.Remove(tg);
                    db.SaveChanges();
                    MessageBox.Show("Xóa Chi tiết nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Xóa Chi tiết nhập thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadDgvCHITIETXUAT();
                UpdateDetailHOADONBAN();

                return;
            }

            if (btnXoaChiTietBan.Text == "Hủy")
            {
                btnXoaChiTietBan.Text = "Xóa";
                btnThemChiTietBan.Text = "Thêm";
                btnSuaChiTietBan.Text = "Sửa";

                btnThemChiTietBan.Enabled = true;
                btnSuaChiTietBan.Enabled = true;

                groupThongTinChiTietBan.Enabled = false;
                dgvChiTietBan.Enabled = true;

                panelHoaDonBan.Enabled = true;

                UpdateDetailChiTietBan();


                return;
            }
        }

        #endregion

        #endregion

    }
}
