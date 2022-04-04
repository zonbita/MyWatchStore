using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWatchStore.Models;

namespace MyWatchStore.Controllers
{
    public class CartController : Controller
    {
        WatchStoreDataContext data = new WatchStoreDataContext();
        // GET: Cart
        public List<SanPhamMua> CreateCart()
        {
            List<SanPhamMua> cart = Session["Cart"] as List<SanPhamMua>;
            if (cart == null) {
                cart = new List<SanPhamMua>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public ActionResult AddToCart(int maDH, string strUrl) {
            List<SanPhamMua> cart = CreateCart();
            SanPhamMua sanPham = cart.Find(n => n.iMaDH == maDH);
            if (sanPham == null)
            {
                sanPham = new SanPhamMua(maDH);
                cart.Add(sanPham);
                return Redirect(strUrl);
            }
            else {
                sanPham.iSoLuong++;
                return Redirect(strUrl);
            }
        }

        private int TongSoLuong() {
            int tongSoLuong = 0;
            List<SanPhamMua> cart = Session["Cart"] as List<SanPhamMua>;
            if (cart != null) {
                tongSoLuong = cart.Sum(n => n.iSoLuong);
            }
            return tongSoLuong;
        }

        public ActionResult Refresh(int maDH, FormCollection collector) {
            List<SanPhamMua> cart = CreateCart();
            SanPhamMua sanPham = cart.SingleOrDefault(n => n.iMaDH == maDH);
            if (sanPham != null) {
                sanPham.iSoLuong = int.Parse(collector["SoLuong"].ToString());
            }
            return RedirectToAction("CartView");
        }

        private int TongTien()
        {
            int tongTien = 0;
            List<SanPhamMua> cart = Session["Cart"] as List<SanPhamMua>;
            if (cart != null)
            {
                tongTien = cart.Sum(n => n.iThanhTien);
            }
            return tongTien;
        }

        public ActionResult XoaSanPham(int maDH) {
            List<SanPhamMua> cart = CreateCart();
            SanPhamMua sanPham = cart.SingleOrDefault(n => n.iMaDH == maDH);
            if (sanPham != null) {
                cart.RemoveAll(n => n.iMaDH == maDH);
                return RedirectToAction("CartView");
            }
            if (cart.Count == 0) {
                return RedirectToAction("Index", "WatchStore");
            }
            return RedirectToAction("CartView");
        }

        public int TongSanPham() {
            List<SanPhamMua> cart = CreateCart();
            return cart.Count;
        }

        public ActionResult CartPartial() {
            ViewBag.TongSanPham = TongSanPham();
            return PartialView();
        }

        public ActionResult CartView() {
            List<SanPhamMua> cart = CreateCart();
            if (cart.Count == 0) {
                return RedirectToAction("Index", "WatchStore");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(cart);
        }

        [HttpGet]
        public ActionResult ThanhToan() {
            if (Session["KhachHang"] == null || Session["KhachHang"].ToString() == "") {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["Cart"] == null) {
                return RedirectToAction("Index", "WatchStore");
            }
            List<SanPhamMua> cart = CreateCart();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(cart);
        }

        [HttpPost]
        public ActionResult ThanhToan(FormCollection collector) {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = Session["KhachHang"] as KHACHHANG;
            List<SanPhamMua> cart = CreateCart();

            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangThanhToan = false;
            ddh.TinhTrangGiao = false;

            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();

            foreach (var item in cart) {
                CHITIETDONDATHANG ctdh = new CHITIETDONDATHANG();

                ctdh.MaDatHang = ddh.MaDatHang;
                ctdh.MaSanPham = item.iMaDH;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.ThanhTien = item.iThanhTien;

                data.CHITIETDONDATHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Cart"] = null;
            return RedirectToAction("XacNhanDatHang", "Cart");
        }

        public ActionResult XacNhanDatHang() {
            return View();
        }
    }
}