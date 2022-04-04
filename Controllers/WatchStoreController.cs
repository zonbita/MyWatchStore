using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWatchStore.Models;
using PagedList;
using PagedList.Mvc;

namespace MyWatchStore.Controllers
{
    public class WatchStoreController : Controller
    {
        WatchStoreDataContext data = new WatchStoreDataContext();

        // GET: WatchStore
        public ActionResult Index(int? page)
        {
            if (page == null) {
                page = 1;
            }

            int pageSize = 16;
            int pageNumber = (page ?? 1);

            var dongHo = (from dh in data.DONGHOs select dh).OrderBy(x => x.MaDH);
            return View(dongHo.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Category() {
            var hangDH = from hangdh in data.HANGs select hangdh;
            return PartialView(hangDH);
        }

        public ActionResult ViewCategory(string maHang, int ? page) {
            if (page == null)
            {
                page = 1;
            }

            int pageSize = 16;
            int pageNumber = (page ?? 1);

            var dongHoHang = (from dhh in data.DONGHOs where dhh.MaHang == maHang select dhh).OrderBy(m => m.MaDH);
            return View(dongHoHang.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detail(int maDH) {
            var dh = from dongHo in data.DONGHOs where dongHo.MaDH == maDH select dongHo;
            return View(dh.Single());
        }

        public ActionResult DetailInfo(string maHang)
        {
            var detail = from dt 
                         in data.HANGs
                         where dt.MaHang == maHang
                         select dt;
            return PartialView(detail.Single());
        }
    }
}