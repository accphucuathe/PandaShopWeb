using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class OrderController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Order
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // quan ly don hang
        public ActionResult QuanLyDonHang(int? pageNumber, String searchKeyword)
        {
            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            int pageSize = 10;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;

            return View();
        }
    }
}