using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class ProductController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Product
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}