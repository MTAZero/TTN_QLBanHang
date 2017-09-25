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
    public partial class FrmNhapHang : Form
    {
        private QLBanHangDbContext db = Service.DBService.db;
        private int indexPhieuNhap = 0, indexPhieuNhap1 = 0;
        private int indexChiTietNhap = 0, indexChiTietNhap1 = 0;
        private NHANVIEN nv = new NHANVIEN();

        #region constructor
        public FrmNhapHang(NHANVIEN _nv)
        {
            InitializeComponent();
            Service.DBService.Reload();
            nv = _nv;
        }
        #endregion

        #region Form chính
        private void FrmNhapHang_Load(object sender, EventArgs e)
        {
            LoadPhieuNhap();
            LoadChiTietNhap();
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

            groupThongTinPhieuNhap.Enabled = false;

        }

        private void LoadDgvPhieuNhap()
        {
            int i = 0;
            var dataPhieuNhap = db.PHIEUNHAPs.ToList()
                                .Select(p => new
                                {
                                    ID = p.ID,
                                    STT = ++i,
                                    Ngay = ((DateTime)p.NGAY).ToString("dd/MM/yyyy"),
                                    NhanVien = db.NHANVIENs.Where(z => z.ID == p.NHANVIENID).FirstOrDefault().TEN,
                                    DiaDiem = p.DIADIEM,
                                    TongTien = p.TONGTIEN
                                })
                                .ToList();
            dgvPhieuNhap.DataSource = dataPhieuNhap;

            // thêm index 
            indexPhieuNhap = indexPhieuNhap1;
            try
            {
                dgvPhieuNhap.Rows[indexPhieuNhap].Cells["STTPhieuNhap"].Selected = true;
                dgvPhieuNhap.Select();
            }
            catch { }

            LoadDgvChiTietNhap();
        }

        private void LoadPhieuNhap()
        {
            LoadInitControl();
            LoadDgvPhieuNhap();
        }
        #endregion

        #region Hàm chức năng

        private void UpdateDetailPhieuNhap()
        {
            ClearControlPhieuNhap();
            PHIEUNHAP tg = getPhieuNhapByID();
            if (tg.ID == 0) return;

            int TongTienCu = (int)tg.TONGTIEN;

            try
            {
                try
                {

                    int cnt = 0;
                    cnt = db.CHITIETNHAPs.Where(p => p.PHIEUNHAPID == tg.ID).ToList().Count;
                    if (cnt == 0) tg.TONGTIEN = 0;


                    tg.TONGTIEN = db.CHITIETNHAPs.Where(p => p.PHIEUNHAPID == tg.ID).Sum(p => p.THANHTIEN).Value;

                }
                catch { tg.TONGTIEN = 0; }

                if (TongTienCu != tg.TONGTIEN) LoadDgvPhieuNhap();
                db.SaveChanges();

                cbxNhanVien.SelectedValue = tg.NHANVIENID;
                dateNgayNhap.Value = (DateTime)tg.NGAY;
                txtDiaDiem.Text = tg.DIADIEM;
                txtTongTien.Text = tg.TONGTIEN.ToString();

                indexPhieuNhap1 = indexPhieuNhap;
                indexPhieuNhap = dgvPhieuNhap.SelectedRows[0].Index;


                LoadDgvChiTietNhap();
            }
            catch { }
        }

        private void ClearControlPhieuNhap()
        {
            try
            {
                cbxNhanVien.SelectedIndex = 0;
                dateNgayNhap.Value = DateTime.Now;
                txtDiaDiem.Text = "";
                txtTongTien.Text = "";
            }
            catch { }
        }

        private bool CheckPhieuNhap()
        {
            if (txtDiaDiem.Text == "")
            {
                MessageBox.Show("Địa điểm không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private PHIEUNHAP getPhieuNhapByID()
        {
            PHIEUNHAP ans = new PHIEUNHAP();

            try
            {
                int id = (int)dgvPhieuNhap.SelectedRows[0].Cells["IDPhieuNhap"].Value;
                PHIEUNHAP z = db.PHIEUNHAPs.Where(p => p.ID == id).FirstOrDefault();

                if (z != null) ans = z;
            }
            catch { }

            return ans;
        }

        private PHIEUNHAP getPhieuNhapByForm()
        {
            PHIEUNHAP ans = new PHIEUNHAP();

            ans.NHANVIENID = (int)cbxNhanVien.SelectedValue;
            ans.NGAY = dateNgayNhap.Value;
            ans.DIADIEM = txtDiaDiem.Text;
            ans.TONGTIEN = 0;
            //ans.TONGTIEN = Int32.Parse(txtTongTien.Text);

            return ans;
        }

        #endregion

    }
}
