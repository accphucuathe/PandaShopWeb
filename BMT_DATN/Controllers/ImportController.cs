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
        public JsonResult AdminThemPhieuNhap(DateTime importDate, int productProvider, int[] selectImportProduct, int[] importProductQuantity, String[] importProductPrice, String[] productPrice, String noteImport)
        {
            string result = "";
            string redirect = "";
            Guid userId = HomeController.nguoidung.maNguoiDung;
            int idProvider = productProvider;
            int[] idProd = selectImportProduct;
            int[] importQuantityProd = importProductQuantity;
            String[] importPriceProd = importProductPrice;

            // insert tblPhieuNhap
            var phieuNhapMoi = new tblPhieuNhap();
            phieuNhapMoi.NgayNhap = importDate;
            phieuNhapMoi.GhiChu = noteImport;
            phieuNhapMoi.FK_MaNguoiDung = userId;
            phieuNhapMoi.FK_MaNhaCungCap = idProvider;
            db.tblPhieuNhaps.Add(phieuNhapMoi);

            var ch = new CurrencyHelper();
            // insert tblChiTietNhapHang
            List<tblChiTietPhieuNhap> listCtpn = new List<tblChiTietPhieuNhap>();
            for (int i = 0; i < idProd.Length; i++)
            {
                var idProduct = idProd[i];
                var chiTietNhapHangMoi = new tblChiTietPhieuNhap();
                chiTietNhapHangMoi.FK_MaPhieuNhap = phieuNhapMoi.PK_MaPhieuNhap;
                chiTietNhapHangMoi.FK_MaSanPham = idProduct;
                chiTietNhapHangMoi.SoLuongNhap = importQuantityProd[i];
                chiTietNhapHangMoi.GiaNhap = ch.FormatToNumber(importPriceProd[i]);

                listCtpn.Add(chiTietNhapHangMoi);
                // tang so luong sp trong kho
                var productOfStore = (from sp in db.tblSanPhams
                                      where sp.PK_MaSanPham == idProduct
                                      select sp).FirstOrDefault();
                productOfStore.SoLuong += importQuantityProd[i];
            }
            db.tblChiTietPhieuNhaps.AddRange(listCtpn);

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

        // QL phieu nhap - admin xoa phieu nhap
        [HttpPost]
        public JsonResult AdminXoaPhieuNhap(int importId)
        {
            string result = "";
            string refresh = "";
            var phieuNhapBiXoa = (from pn in db.tblPhieuNhaps
                                where pn.PK_MaPhieuNhap == importId
                                select pn).FirstOrDefault();
            var productsOfImport = (from ctnh in db.tblChiTietPhieuNhaps
                                    where ctnh.FK_MaPhieuNhap == importId
                                    select ctnh).ToList();
            // kiem tra so luong trong kho co hop le khi xoa phieu nhap hay ko 
            foreach (var product in productsOfImport)
            {
                var quantityProductOfImport = product.SoLuongNhap;
                var productOfStore = (from sp in db.tblSanPhams
                                      where sp.PK_MaSanPham == product.FK_MaSanPham
                                      select sp).FirstOrDefault();
                // check so luong da nhap va so luong con lai trong kho
                if (quantityProductOfImport > productOfStore.SoLuong)
                {
                    return Json(new { msg = "Số lượng trong kho thấp hơn số lượng trong phiếu nhập \n Không thể xóa phiếu nhập này", rfr = "0" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    productOfStore.SoLuong -= quantityProductOfImport;
                }
            }
            // delete phieu nhap + chi tiet nhap hang
            db.tblChiTietPhieuNhaps.RemoveRange(productsOfImport);
            db.tblPhieuNhaps.Remove(phieuNhapBiXoa);
            db.SaveChanges();
            result = "Đã xóa thành công phiếu nhập!";
            refresh = "1";

            return Json(new
            {
                msg = result,
                rfr = refresh
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL phieu nhap - sua phieu nhap
        public ActionResult SuaPhieuNhap(int maPn)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maPhieuNhap = maPn;

            return View();
        }

        // QL phieu nhap - admin sua phieu nhap
        public JsonResult AdminSuaPhieuNhap(int importId, DateTime importDate, int productProviderId, int[] selectImportProduct, int[] importProductQuantity, String[] importProductPrice, String[] productPrice, String noteImport)
        {
            string result = "";
            string redirect = "";
            int idProvider = productProviderId;
            int idImport = importId;
            int[] idProd = selectImportProduct.Skip(1).ToArray();
            int[] importQuantityProd = importProductQuantity.Skip(1).ToArray();
            String[] importPriceProd = importProductPrice.Skip(1).ToArray();
            var ch = new CurrencyHelper();
            // phieu nhap duoc sua
            var phieuNhapDuocSua = (from pn in db.tblPhieuNhaps
                                    where pn.PK_MaPhieuNhap == idImport
                                    select pn).FirstOrDefault();
            // get old ChiTietPhieuNhap
            var oldListImportDetail = (from ctnh in db.tblChiTietPhieuNhaps
                                       where ctnh.FK_MaPhieuNhap == idImport
                                       select ctnh).ToList();
            // create new ChiTietPhieuNhap
            List<tblChiTietPhieuNhap> newListImportDetail = new List<tblChiTietPhieuNhap>();
            for (int i = 0; i < idProd.Length; i++)
            {
                var newImportDetail = new tblChiTietPhieuNhap();
                newImportDetail.FK_MaPhieuNhap = idImport;
                newImportDetail.FK_MaSanPham = idProd[i];
                newImportDetail.SoLuongNhap = importQuantityProd[i];
                newImportDetail.GiaNhap = ch.FormatToNumber(importPriceProd[i]);

                newListImportDetail.Add(newImportDetail);
            }

            var allProductsOfProvider = (from sp in db.tblSanPhams
                                         join nguon in db.tblNguonCungCaps on sp.PK_MaSanPham equals nguon.FK_MaSanPham
                                         where nguon.FK_MaNhaCungCap == idProvider
                                         select sp).ToList();
            // check valid quantity 
            var quantityCheck = 0;
            foreach (var productOfStore in allProductsOfProvider.Select((data, i) => new { data, i }))
            {
                var index = productOfStore.i;
                var quantityInStore = productOfStore.data.SoLuong;

                var oldImport = oldListImportDetail.Where(op => op.FK_MaSanPham == productOfStore.data.PK_MaSanPham).FirstOrDefault();
                var quantityInOldImport = oldImport == null ? 0 : oldImport.SoLuongNhap;

                var newImport = newListImportDetail.Where(np => np.FK_MaSanPham == productOfStore.data.PK_MaSanPham).FirstOrDefault();
                var quantityInNewImport = newImport == null ? 0 : newImport.SoLuongNhap;

                var finalQuantityIfEditImport = quantityInStore - quantityInOldImport + quantityInNewImport;
                // rollback quantity
                productOfStore.data.SoLuong = finalQuantityIfEditImport;
                if (finalQuantityIfEditImport < 0)
                {
                    quantityCheck++;
                }
            }
            if (quantityCheck != 0)     // co so luong am
            {
                result = "Có số lượng sản phẩm < 0 khi sửa phiếu nhập :(";
                redirect = "0";
            }
            else        // tat ca so luong hop le khi thay doi
            {
                // sua phieu nhap
                phieuNhapDuocSua.NgayNhap = importDate;
                phieuNhapDuocSua.GhiChu = noteImport;
                // sua chi tiet nhap hang
                db.tblChiTietPhieuNhaps.RemoveRange(oldListImportDetail);
                db.tblChiTietPhieuNhaps.AddRange(newListImportDetail);
                // save
                db.SaveChanges();

                result = "Cập nhật phiếu nhập thành công!";
                redirect = "1";
            }

            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }
    }
}