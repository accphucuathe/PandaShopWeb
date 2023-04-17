using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class ReviewController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Review
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // quan ly danh gia san pham
        public ActionResult QuanLyDanhGiaSanPham(int? pageNumber, String searchKeyword)
        {
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

        // QL danh gia san pham - xoa danh gia san pham
        [HttpPost]
        public JsonResult AdminXoaDanhGiaSanPham(int reviewId)
        {
            string result = "";
            var dgspBiXoa = (from dg in db.tblDanhGiaSanPhams
                             where dg.PK_MaDanhGiaSanPham == reviewId
                             select dg).FirstOrDefault();
            if (dgspBiXoa != null)
            {
                db.tblDanhGiaSanPhams.Remove(dgspBiXoa);
                db.SaveChanges();
                result = "Đã xóa đánh giá sản phẩm thành công!";
            }
            else
            {
                result = "Xóa đánh giá sản phẩm thất bại!";
            }

            return Json(new
            {
                msg = result
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL danh gia san pham - tim kiem danh gia san pham
        [HttpPost]
        public JsonResult AdminTimKiemDanhGiaSanPham(int? pageNumber, String searchKeyword)
        {
            string searchK = searchKeyword;
            return Json(new { redirectToUrl = Url.Action("QuanLyDanhGiaSanPham", "Review", new { searchKeyword = searchK }) });
        }
    }
}