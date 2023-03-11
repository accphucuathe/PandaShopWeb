using BMT_DATN.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class CategoryController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Category
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // QL Danh muc san pham
        public ActionResult QuanLyDanhMucSanPham()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                //return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // QL danh muc san pham - them danh muc sp
        public ActionResult ThemDanhMucSanPham()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                //return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public JsonResult AdminThemDanhMucSanPham(String categoryName)
        {
            string result = "";
            string cateName = categoryName.Trim();

            var checkCategoryName = (from dm in db.tblDanhMucSanPhams
                                     where dm.TenDanhMucSanPham.Equals(cateName)
                                     select dm).FirstOrDefault();
            if (checkCategoryName != null)
            {
                result = "Đã tồn tại danh mục sản phẩm này!";
            }

            // insert danh muc 
            var danhMucSanPhamMoi = new tblDanhMucSanPham();
            danhMucSanPhamMoi.TenDanhMucSanPham = categoryName;
            db.SaveChanges();

            return Json(new
            {
                msg = result
            },
            JsonRequestBehavior.AllowGet);
        }

    }
}