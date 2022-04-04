using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWatchStore.Models
{
    public class SanPhamMua
    {
        WatchStoreDataContext data = new WatchStoreDataContext();
        public int iMaDH { get; set; }
        public string sImage { get; set; }
        public string sTenDH { get; set; }
        public int iDonGia { get; set; }
        public int iSoLuong { get; set; }
        public int iThanhTien {
            get { return iSoLuong * iDonGia; }
        }

        public SanPhamMua(int maDH) {
            iMaDH = maDH;
            DONGHO dh = data.DONGHOs.Single(n => n.MaDH == iMaDH);
            sImage = dh.Image;
            sTenDH = dh.TenDH;
            iDonGia = int.Parse(dh.Gia.ToString());
            iSoLuong = 1;
        }
    }
}