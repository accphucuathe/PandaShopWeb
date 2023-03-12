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

            return RedirectToAction("Index", "Admin");
        }

        // QL Danh muc san pham
        public ActionResult QuanLyDanhMucSanPham()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // QL danh muc san pham - them danh muc sp
        public ActionResult ThemDanhMucSanPham()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public JsonResult AdminThemDanhMucSanPham(String categoryName)
        {
            string result = "";
            string redirect = "";
            string cateName = categoryName.Trim();

            var checkCategoryName = (from dm in db.tblDanhMucSanPhams
                                     where dm.TenDanhMucSanPham.Equals(cateName)
                                     select dm).FirstOrDefault();
            if (checkCategoryName != null)
            {
                result = "Đã tồn tại danh mục sản phẩm này!";
                redirect = "0";     // ở lại trang Thêm DMSP
            }
            else
            {
                // insert danh muc 
                var danhMucSanPhamMoi = new tblDanhMucSanPham();
                danhMucSanPhamMoi.TenDanhMucSanPham = categoryName;
                db.tblDanhMucSanPhams.Add(danhMucSanPhamMoi);
                db.SaveChanges();
                result = "Thêm danh mục sản phẩm thành công!";
                redirect = "1";     // về trang Quản lý DMSP
            }

            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL danh muc san pham - xem danh muc sp
        public ActionResult XemDanhMucSanPham(int maDmsp)
        {
            ViewBag.maDmsp = maDmsp;
            return View();
        }

        // QL danh muc san pham - sua danh muc sp
        public ActionResult SuaDanhMucSanPham(int maDmsp)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maDmsp = maDmsp;

            return View();
        }

        [HttpPost]
        public JsonResult AdminSuaDanhMucSanPham(int categoryId, String categoryName)
        {
            string result = "";
            string redirect = "";
            string cateName = categoryName.Trim();

            var dmspDuocSua = db.tblDanhMucSanPhams.FirstOrDefault(dm => dm.PK_MaDanhMucSanPham == categoryId);
            if (dmspDuocSua != null)
            {
                var checkCategoryName = (from dm in db.tblDanhMucSanPhams
                                         where dm.TenDanhMucSanPham.Equals(cateName) && 
                                                dm.PK_MaDanhMucSanPham != dmspDuocSua.PK_MaDanhMucSanPham
                                         select dm).FirstOrDefault();
                if (checkCategoryName != null)
                {
                    result = "Đã tồn tại danh mục sản phẩm này!";
                    redirect = "0";     // ở lại trang Sửa DMSP
                }
                else
                {
                    // update danh muc
                    dmspDuocSua.TenDanhMucSanPham = categoryName;
                    db.SaveChanges();
                    result = "Sửa danh mục sản phẩm thành công!";
                    redirect = "1";     // về trang Quản lý DMSP
                }
            }
            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL danh muc san pham - xoa danh muc sp
        [HttpPost]
        public JsonResult AdminXoaDanhMucSanPham(int categoryId)
        {
            string result = "";
            var dmspBiXoa = (from dm in db.tblDanhMucSanPhams
                             where dm.PK_MaDanhMucSanPham == categoryId
                             select dm).FirstOrDefault();
            if (dmspBiXoa != null)
            {
                db.tblDanhMucSanPhams.Remove(dmspBiXoa);
                db.SaveChanges();
                result = "Đã xóa thành công danh mục sản phẩm!";
            }
            else
            {
                result = "Không tồn tại danh mục sản phẩm!";
            }

            return Json(new
            {
                msg = result
            },
            JsonRequestBehavior.AllowGet);
        }
    }
}