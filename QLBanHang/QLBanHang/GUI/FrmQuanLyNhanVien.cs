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
    }
}
