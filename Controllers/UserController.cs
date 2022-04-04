using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWatchStore.Models;

namespace MyWatchStore.Controllers
{
    public class UserController : Controller
    {
        WatchStoreDataContext data = new WatchStoreDataContext();
        // GET: User
        public ActionResult LoginPartial() {
            KHACHHANG kh = Session["KhachHang"] as KHACHHANG;
            if (kh == null)
            {
                ViewBag.TenKhachHang = "";
            }
            else {
                ViewBag.TenKhachHang = kh.TenKH;
            }
            return PartialView();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DangKy() {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collector) {
            var tenDN = collector["TaiKhoan"];
            var matKhau = collector["MatKhau"];
            if (String.IsNullOrEmpty(tenDN))
            {
                ViewData["Error1"] = "Thiếu tên đăng nhập!";
            }
            else if (String.IsNullOrEmpty(matKhau))
            {
                ViewData["Error2"] = "Thiếu mật khẩu!";
            }
            else {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.TenDN == tenDN && n.MatKhau == matKhau);
                if (kh != null) {
                    ViewBag.Thongbao = "Đăng nhập thành công!";
                    Session["KhachHang"] = kh;
                    return RedirectToAction("Index", "WatchStore");
                }
                else
                    ViewBag.Thongbao = "Sai tên đăng nhập hoặc mật khẩu!";
            }
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collector, KHACHHANG kh) {
            var hoTen = collector["HoTen"];
            var tenDN = collector["TaiKhoan"];
            var matKhau = collector["MatKhau"];
            var email = collector["Email"];
            var sdt = collector["Sdt"];
            var diaChi = collector["DiaChi"];
            if (String.IsNullOrEmpty(hoTen))
            {
                ViewData["Error1"] = "Thiếu họ tên!";
            }
            else if (String.IsNullOrEmpty(tenDN))
            {
                ViewData["Error2"] = "Thiếu tên đang nhập!";
            }
            else if (String.IsNullOrEmpty(matKhau))
            {
                ViewData["Error3"] = "Thiếu mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Error4"] = "Thiếu email";
            }
            else if (String.IsNullOrEmpty(sdt.ToString()))
            {
                ViewData["Error5"] = "Thiếu số điện thoại";
            }
            else if (String.IsNullOrEmpty(diaChi)) {
                ViewData["Error6"] = "Thiếu địa chỉ";
            }
            else
            {
                kh.TenKH = hoTen;
                kh.TenDN = tenDN;
                kh.MatKhau = matKhau;
                kh.Email = email;
                kh.SoDT = sdt;
                kh.DiaChi = diaChi;
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("Index", "WatchStore");
            }
            return this.DangKy();
        }
    }
}