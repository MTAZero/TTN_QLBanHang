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

    }
}
