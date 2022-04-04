using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWatchStore.Models;
using PagedList;
using System.IO;

namespace MyWatchStore.Controllers
{
    public class AdminController : Controller
    {
        WatchStoreDataContext data = new WatchStoreDataContext();
        // GET: Admin
        [HttpGet]
        public ActionResult Boss()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Boss(FormCollection colletor) {
            var taiKhoan = colletor["TaiKhoan"];
            var password = colletor["Password"];
            ADMIN admin = data.ADMINs.SingleOrDefault(a => a.TaiKhoan == taiKhoan && a.MatKhau == password);
            if (admin != null)
            {
                Session["Admin"] = admin;
                return RedirectToAction("AdminDashboard");
            }
            else
                ViewBag.ThongBao = "Sai tên đăng nhập hoặc mật khẩu";
            return View();
        }

        public ActionResult AdminDashboard() {
            return View();
        }

        public ActionResult AdminProduct(int? page) {
            if (page == null) {
                page = 1;
            }

            int pageSize = 8;
            int pageNumber = (page ?? 1);

            var product = (from sp in data.DONGHOs select sp).OrderBy(s => s.MaDH);
            return View(product.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult XoaProduct(int maDH) {
            DONGHO dongHo = data.DONGHOs.SingleOrDefault(dh => dh.MaDH == maDH);
            if (dongHo == null) {
                Response.StatusCode = 404;
                return null;
            }
            data.DONGHOs.DeleteOnSubmit(dongHo);
            data.SubmitChanges();
            return RedirectToAction("AdminProduct");
        }

        [HttpGet]
        public ActionResult AddProduct() {
            ViewBag.MaHang = new SelectList(data.HANGs.ToList().OrderBy(h => h.MaHang), "MaHang", "TenHang");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProduct(DONGHO dongHo, HttpPostedFileBase anhUpload) {
            ViewBag.MaDH = new SelectList(data.DONGHOs.ToList().OrderBy(dh => dh.MaDH), "MaDH", "TenDH");
            if (anhUpload == null) {
                ViewBag.ThongBao = "Vui lòng chọn ảnh";
                return View();
            }
            else
            {
                if (ModelState.IsValid) {
                    var tenAnh = Path.GetFileName(anhUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/DataImages"), tenAnh);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else {
                        anhUpload.SaveAs(path);
                    }
                    dongHo.Image = tenAnh;
                    data.DONGHOs.InsertOnSubmit(dongHo);
                    data.SubmitChanges();
                }
            }
            return RedirectToAction("AdminProduct");
        }

        [HttpGet]
        public ActionResult AdminEdit(int maDH) {
            DONGHO dongHo = data.DONGHOs.SingleOrDefault(dh => dh.MaDH == maDH);
            ViewBag.MaHang = new SelectList(data.HANGs.ToList().OrderBy(h => h.MaHang), "MaHang", "TenHang", dongHo.MaHang);
            if (dongHo == null) {
                Response.StatusCode = 404;
                return null;
            }
            return View(dongHo);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminEdit(DONGHO dh, HttpPostedFileBase anhUpload) {
            ViewBag.MaHang = new SelectList(data.HANGs.ToList().OrderBy(m => m.MaHang), "MaHang", "TenHang");
            if (anhUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else {
                if (ModelState.IsValid) {
                    var tenAnh = Path.GetFileName(anhUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/DataImages"), tenAnh);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else {
                        anhUpload.SaveAs(path);
                    }
                    dh = data.DONGHOs.Single(x => x.MaDH == dh.MaDH);
                    dh.Image = tenAnh;
                    UpdateModel(dh);
                    data.SubmitChanges();
                }
                return RedirectToAction("AdminProduct");
            }
        }
    }
}