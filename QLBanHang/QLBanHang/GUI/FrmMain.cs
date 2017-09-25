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


    }
}
