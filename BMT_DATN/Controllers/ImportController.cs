using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class ImportController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Import
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // quan ly phieu nhap
        public ActionResult QuanLyPhieuNhap(int? pageNumber, String searchKeyword)
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

        // QL phieu nhap - them phieu nhap
        public ActionResult ThemPhieuNhap()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.userId = HomeController.nguoidung.maNguoiDung;

            return View();
        }

        [HttpPost]
        public JsonResult LayDanhSachSanPhamTheoNhaCungCap(int providerId)
        {
            string result = "";
            var listProduct = (from sp in db.tblSanPhams
                               join nguon in db.tblNguonCungCaps on sp.PK_MaSanPham equals nguon.FK_MaSanPham
                               where nguon.FK_MaNhaCungCap == providerId
                               select sp).ToList();
            var arrayProductId = listProduct.Select(p => p.PK_MaSanPham).ToArray();
            var arrayProductName = listProduct.Select(p => p.TenSanPham).ToArray();
            var arrayProductPrice = listProduct.Select(p => p.DonGia).ToArray();

            return Json(new
            {
                arrId = arrayProductId,
                arrName = arrayProductName,
                arrPrice = arrayProductPrice
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL phieu nhap - admin them phieu nhap moi
        [HttpPost]
        public JsonResult AdminThemPhieuNhap(int productProvider, int[] selectImportProduct, int[] importProductQuantity, String[] importProductPrice, String[] productPrice, String noteImport)
        {
            string result = "";
            string redirect = "";
            Guid userId = HomeController.nguoidung.maNguoiDung;
            int idProvider = productProvider;
            int[] idProd = selectImportProduct;
            int[] importQuantityProd = importProductQuantity;
            String[] importPriceProd = importProductPrice;
            String[] priceProd = productPrice;

            // insert tblPhieuNhap
            var phieuNhapMoi = new tblPhieuNhap();
            phieuNhapMoi.NgayNhap = DateTime.Now;
            phieuNhapMoi.GhiChu = noteImport;
            phieuNhapMoi.FK_MaNguoiDung = userId;
            phieuNhapMoi.FK_MaNhaCungCap = idProvider;
            db.tblPhieuNhaps.Add(phieuNhapMoi);

            var ch = new CurrencyHelper();
            // insert tblChiTietNhapHang
            List<tblChiTietNhapHang> listCtnh = new List<tblChiTietNhapHang>();
            for (int i = 0; i < idProd.Length; i++)
            {
                var idProduct = idProd[i];
                var chiTietNhapHangMoi = new tblChiTietNhapHang ();
                chiTietNhapHangMoi.FK_MaPhieuNhap = phieuNhapMoi.PK_MaPhieuNhap;
                chiTietNhapHangMoi.FK_MaSanPham = idProduct;
                chiTietNhapHangMoi.SoLuongNhap = importQuantityProd[i];

                chiTietNhapHangMoi.GiaNhap = ch.FormatToNumber(importPriceProd[i]);

                chiTietNhapHangMoi.GiaBan = ch.FormatToNumber(priceProd[i]);
                listCtnh.Add(chiTietNhapHangMoi);
                // tang so luong sp trong kho
                var productOfStore = (from sp in db.tblSanPhams
                                      where sp.PK_MaSanPham == idProduct
                                      select sp).FirstOrDefault();
                productOfStore.SoLuong += importQuantityProd[i];
                productOfStore.DonGia = ch.FormatToNumber(priceProd[i]);
            }
            db.tblChiTietNhapHangs.AddRange(listCtnh);

            // save
            db.SaveChanges();

            result = "Thêm mới phiếu nhập thành công!";
            redirect = "1";

            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL phieu nhap - admin xem phieu nhap
        public ActionResult XemPhieuNhap(int maPn)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maPhieuNhap = maPn;

            return View();
        }
    }
}