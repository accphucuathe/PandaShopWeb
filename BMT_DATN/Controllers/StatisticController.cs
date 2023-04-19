using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class StatisticController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Statistic
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang && HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.NhanVien)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Thong ke san pham
        public ActionResult ThongKeSanPham(int? categoryId)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang && HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.NhanVien)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maDmsp = categoryId;
            return View();
        }

        [HttpPost]
        public JsonResult AdminLocThongKeSanPham(int categoryId)
        {
            if (categoryId == 0)
            {
                return Json(new { redirectToUrl = Url.Action("ThongKeSanPham", "Statistic") });
            }
            return Json(new { redirectToUrl = Url.Action("ThongKeSanPham", "Statistic", new { categoryId = categoryId }) });
        }
    }
}