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
    public partial class FrmKho : Form
    {
        private QLBanHangDbContext db = Service.DBService.db;

        #region constructor
        public FrmKho()
        {
            InitializeComponent();
            Service.DBService.Reload();
        }

        #endregion

        #region LoadForm
        private void LoadDgvMatHang()
        {

            string keyword = txtTimKiem.Text;

            int i = 0;
            var dataMatHang = db.KHOes.ToList()
                              .Select(p => new
                              {
                                  STT = ++i,
                                  MatHang = db.MATHANGs.Where(z => z.ID == p.MATHANGID).FirstOrDefault().TEN,
                                  DonViTinh = db.MATHANGs.Where(z => z.ID == p.MATHANGID).FirstOrDefault().DONVITINH,
                                  SoLuong = p.SOLUONG
                              })
                              .Where(p => p.MatHang.Contains(keyword) || p.DonViTinh.Contains(keyword))
                              .ToList();

            dgvMatHang.DataSource = dataMatHang;
        }
        private void FrmKho_Load(object sender, EventArgs e)
        {
            LoadDgvMatHang();
        }
        #endregion

        #region sự kiện
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadDgvMatHang();
        }
        #endregion
    }
}

