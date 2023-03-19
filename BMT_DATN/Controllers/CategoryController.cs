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
        public ActionResult QuanLyDanhMucSanPham(int? pageNumber, String searchKeyword)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            int pageSize = 10;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;
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
        public JsonResult AdminThemDanhMucSanPham(String categoryName, String categoryDescription)
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
                danhMucSanPhamMoi.MoTa = categoryDescription;
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
        public JsonResult AdminSuaDanhMucSanPham(int categoryId, String categoryName, String categoryDescription)
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
                    dmspDuocSua.TenDanhMucSanPham = cateName;
                    dmspDuocSua.MoTa = categoryDescription;
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
                if (dmspBiXoa.tblSanPhams.Count > 0)        // tồn tại sp đang liên kết dmsp
                {
                    result = "Danh mục đang liên kết với sản phẩm. Không thể xóa!";
                }
                else
                {
                    db.tblDanhMucSanPhams.Remove(dmspBiXoa);
                    db.SaveChanges();
                    result = "Đã xóa thành công danh mục sản phẩm!";
                }
            }
            else
            {
                result = "Xóa danh mục sản phẩm thất bại!";
            }

            return Json(new
            {
                msg = result
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL danh muc san pham - tim kiem danh muc sp
        [HttpPost]
        public JsonResult AdminTimKiemDanhMucSanPham(int? pageNumber, String searchKeyword)
        {
            string searchK = searchKeyword;
            return Json(new { redirectToUrl = Url.Action("QuanLyDanhMucSanPham", "Category", new { searchKeyword = searchK }) });
        }
    }
}