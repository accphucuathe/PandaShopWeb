using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class ProviderController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Provider
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // QL nha cung cap
        public ActionResult QuanLyNhaCungCap(int? pageNumber, String searchKeyword)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                //return RedirectToAction("Index", "Home");
            }
            int pageSize = 10;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;
            return View();
        }

        // QL nha cung cap - them nha cung cap
        public ActionResult ThemNhaCungCap(String duplicateProviderNameCheck)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            if (duplicateProviderNameCheck != "")
            {
                ViewBag.checkDuplicateProviderName = duplicateProviderNameCheck;
            }
            return View();
        }
    }
}