using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class PromotionController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Promotion
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // QL chuong trinh khuyen mai
        public ActionResult QuanLyChuongTrinhKhuyenMai(int? pageNumber, String searchKeyword)
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

        // QL chuong trinh khuyen mai - them chuong trinh khuyen mai
        public ActionResult ThemChuongTrinhKhuyenMai()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // QL chuong trinh khuyen mai - admin them ctkm moi
        [HttpPost]
        public JsonResult AdminThemChuongTrinhKhuyenMai(String promotionName, DateTime promotionStartDate, DateTime promotionEndDate, int promotionPercent, String promotionTypeChoose, int promotionByCategory, int[] promotionByProducts)
        {
            string result = "";
            string redirect = "";
            int[] idPromotionProducts = promotionByProducts;
            int idCategory = promotionByCategory;

            // insert tblChuongTrinKhuyenMai
            var chuongTrinhKhuyenMaiMoi = new tblChuongTrinhKhuyenMai();
            chuongTrinhKhuyenMaiMoi.TenChuongTrinhKhuyenMai = promotionName.Trim();
            chuongTrinhKhuyenMaiMoi.NgayBatDau = promotionStartDate;
            chuongTrinhKhuyenMaiMoi.NgayKetThuc = promotionEndDate;
            chuongTrinhKhuyenMaiMoi.PhanTramGiamGia = promotionPercent;
            db.tblChuongTrinhKhuyenMais.Add(chuongTrinhKhuyenMaiMoi);

            // insert tblChiTietNhapHang
            List<tblChiTietKhuyenMai> listCtkm = new List<tblChiTietKhuyenMai>();       
            if (promotionTypeChoose == "ByProducts")        //byProducts
            {
                for (int i = 0; i < idPromotionProducts.Length; i++)
                {
                    var chiTietKhuyenMaiMoi = new tblChiTietKhuyenMai();
                    chiTietKhuyenMaiMoi.FK_MaChuongTrinhKhuyenMai = chuongTrinhKhuyenMaiMoi.PK_MaChuongTrinhKhuyenMai;
                    chiTietKhuyenMaiMoi.FK_MaSanPham = idPromotionProducts[i];
                    chiTietKhuyenMaiMoi.Note = "";

                    listCtkm.Add(chiTietKhuyenMaiMoi);
                }
            }
            else       // byCategory
            {
                var allProductsOfCategory = (from sp in db.tblSanPhams
                                             where sp.FK_MaDanhMucSanPham == idCategory
                                             select sp).ToList();
                DateTime today = DateTime.Now.Date;
                var idProductsAreOnPromotionOfCategory = (from ctkm in db.tblChuongTrinhKhuyenMais
                                                        join ctctkm in db.tblChiTietKhuyenMais on ctkm.PK_MaChuongTrinhKhuyenMai equals ctctkm.FK_MaChuongTrinhKhuyenMai
                                                        where ctkm.NgayBatDau >= today
                                                        select ctctkm.FK_MaSanPham).ToList();
                var allIdAvailableProductsOfCategory = allProductsOfCategory.Select(p => p.PK_MaSanPham).Except(idProductsAreOnPromotionOfCategory).ToList();
                if (allIdAvailableProductsOfCategory.Count == 0)  // tat ca sp cua dmsp deu dang khuyen mai
                {
                    return Json(new
                    {
                        msg = "Tất cả sản phẩm của danh mục này đều đang trong khuyến mại khác",
                        rdt = "0"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var idAvailableProduct in allIdAvailableProductsOfCategory)
                    {
                        var chiTietKhuyenMaiMoi = new tblChiTietKhuyenMai();
                        chiTietKhuyenMaiMoi.FK_MaChuongTrinhKhuyenMai = chuongTrinhKhuyenMaiMoi.PK_MaChuongTrinhKhuyenMai;
                        chiTietKhuyenMaiMoi.FK_MaSanPham = idAvailableProduct;
                        chiTietKhuyenMaiMoi.Note = "";

                        listCtkm.Add(chiTietKhuyenMaiMoi);
                    }
                }
            }
            db.tblChiTietKhuyenMais.AddRange(listCtkm);

            // save
            db.SaveChanges();

            result = "Thêm mới chương trình khuyến mại thành công!";
            redirect = "1";

            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL chuong trinh khuyen mai - xem ctkm
        public ActionResult XemChuongTrinhKhuyenMai(int maCtkm)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maChuongTrinhKhuyenMai = maCtkm;

            return View();
        }

        // QL chuong trinh khuyen mai - xoa ctkm
        [HttpPost]
        public JsonResult AdminXoaChuongTrinhKhuyenMai(int promotionId)
        {
            string result = "";
            var ctkmBiXoa = (from ctkm in db.tblChuongTrinhKhuyenMais
                             where ctkm.PK_MaChuongTrinhKhuyenMai == promotionId
                             select ctkm).FirstOrDefault();
            DateTime today = DateTime.Now.Date;
            if (ctkmBiXoa != null && ctkmBiXoa.NgayBatDau >= today)
            {
                var chiTietCtkmBiXoa = (from ctctkm in db.tblChiTietKhuyenMais
                                        where ctctkm.FK_MaChuongTrinhKhuyenMai == ctkmBiXoa.PK_MaChuongTrinhKhuyenMai
                                        select ctctkm).ToList();
                db.tblChiTietKhuyenMais.RemoveRange(chiTietCtkmBiXoa);
                db.tblChuongTrinhKhuyenMais.Remove(ctkmBiXoa);
                db.SaveChanges();
                result = "Đã xóa chương trình khuyến mại thành công!";
            }
            else
            {
                result = "Xóa chương trình khuyến mại thất bại!";
            }

            return Json(new
            {
                msg = result
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL chuong trinh khuyen mai - tim kiem ctkm
        [HttpPost]
        public JsonResult AdminTimKiemChuongTrinhKhuyenMai(int? pageNumber, String searchKeyword)
        {
            string searchK = searchKeyword;
            return Json(new { redirectToUrl = Url.Action("QuanLyChuongTrinhKhuyenMai", "Promotion", new { searchKeyword = searchK }) });
        }

        // QL chuong trinh khuyen mai - sua ctkm
        public ActionResult SuaChuongTrinhKhuyenMai()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}