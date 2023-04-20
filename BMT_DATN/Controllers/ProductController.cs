using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // QL san pham
        public ActionResult QuanLySanPham(int? pageNumber, String searchKeyword)
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

        // QL san pham - them san pham
        public ActionResult ThemSanPham(String duplicateProductCheck)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            if (duplicateProductCheck != "")
            {
                ViewBag.checkDuplicateProduct = duplicateProductCheck;
            }
            return View();
        }

        public ActionResult AdminThemSanPham(String productName, String productSize, String productUnit, String productPrice, String productDescription, HttpPostedFileBase productImage, int[] providerIds, int productCategory)
        {
            if (productPrice == "NaN")
            {
                return RedirectToAction("ThemSanPham", new { duplicateProductCheck = "Thông tin đơn giá không hợp lệ" });
            }
            var checkProduct = (from sp in db.tblSanPhams
                                where sp.TenSanPham.Equals(productName.Trim()) && sp.KichCo.Equals(productSize.Trim())
                                select sp).FirstOrDefault();
            if (checkProduct != null)
            {
                return RedirectToAction("ThemSanPham", new { duplicateProductCheck = "Đã có sản phẩm và kích cỡ này trong dữ liệu. Hãy kiểm tra lại!" });
            }
            // insert tblSanPham
            var sanPhamMoi = new tblSanPham();
            sanPhamMoi.TenSanPham = productName.Trim();
            sanPhamMoi.KichCo = productSize.Trim();
            sanPhamMoi.MoTa = productDescription;
            sanPhamMoi.DonVi = productUnit.Trim();
            sanPhamMoi.SoLuong = 0;
            sanPhamMoi.FK_MaDanhMucSanPham = productCategory;

            long unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(productImage.FileName);
            string fileName = fileNameWithoutExtension + "_" + unixTimestamp + ".png";
            string path = Path.Combine(Server.MapPath("~/Images_Data"), fileName);
            productImage.SaveAs(path);
            sanPhamMoi.HinhAnh = fileName;

            var ch = new CurrencyHelper();
            sanPhamMoi.DonGia = ch.FormatToNumber(productPrice);

            db.tblSanPhams.Add(sanPhamMoi);

            // insert tblNguonCungCap
            if (providerIds != null)
            {
                var nguonCungCap = new List<tblNguonCungCap>();
                foreach (int maNCC in providerIds)
                {
                    var nguon = new tblNguonCungCap();
                    nguon.FK_MaSanPham = sanPhamMoi.PK_MaSanPham;
                    nguon.FK_MaNhaCungCap = maNCC;
                    nguon.Note = "";
                    nguonCungCap.Add(nguon);
                }
                db.tblNguonCungCaps.AddRange(nguonCungCap);
            }

            db.SaveChanges();

            return RedirectToAction("QuanLySanPham", "Product");
        }

        // QL san pham - xem san pham
        public ActionResult XemSanPham(int maSanPham)
        {
            ViewBag.maSanPham = maSanPham;
            return View();
        }

        // QL san pham - sua san pham
        public ActionResult SuaSanPham(int maSanPham)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                //return RedirectToAction("Index", "Home");
            }
            ViewBag.maSanPham = maSanPham;
            return View();
        }

        [HttpPost]
        public JsonResult KiemTraTonTaiSanPhamVaNhaCungCapTrongPhieuNhap(int productId, int providerId)
        {
            string result = "";
            // kiem tra co phieu nhap cua ncc + sp nay hay ko
            var checkImport = (from pn in db.tblPhieuNhaps
                               join ctnh in db.tblChiTietPhieuNhaps on pn.PK_MaPhieuNhap equals ctnh.FK_MaPhieuNhap
                               where pn.FK_MaNhaCungCap == providerId && ctnh.FK_MaSanPham == productId
                               select ctnh).FirstOrDefault();
            if (checkImport != null)    // tim dc phieu nhap cua ncc + sp
            {
                result = "1";
            }
            else
            {
                result = "0";
            }

            return Json(new
            {
                res = result
            },
            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AdminSuaSanPham(int productId, int productCategory, String productName, String productSize, String productUnit, String productPrice, String productDescription, HttpPostedFileBase productImage, int[] providerIds)
        {
            string result = "";
            string redirect = "";
            var sanPhamDuocSua = (from sp in db.tblSanPhams
                                where sp.PK_MaSanPham == productId
                                select sp).FirstOrDefault();
            if (sanPhamDuocSua == null)
            {
                result = "Sản phẩm không còn tồn tại!";
                redirect = "1";     // chuyển về trang QL SP
            }
            if (productPrice == "NaN")
            {
                result = "Đơn giá không hợp lệ!";
                redirect = "0";     // ở lại trang Sửa SP
            }
            var checkProduct = (from spCheck in db.tblSanPhams
                                where spCheck.TenSanPham.Equals(productName) && 
                                        spCheck.KichCo.Equals(productSize) &&
                                        spCheck.PK_MaSanPham != sanPhamDuocSua.PK_MaSanPham
                                select spCheck).FirstOrDefault();
            if (checkProduct != null)
            {
                result = "Đã tồn tại sản phẩm có tên và kích cỡ này!";
                redirect = "0";     // ở lại trang Sửa SP
            }
            else
            {
                // update sp
                sanPhamDuocSua.TenSanPham = productName.Trim();
                sanPhamDuocSua.KichCo = productSize.Trim();
                sanPhamDuocSua.MoTa = productDescription;
                sanPhamDuocSua.DonVi = productUnit.Trim();
                sanPhamDuocSua.FK_MaDanhMucSanPham = productCategory;

                if (productImage != null)
                {
                    string fileName = Path.GetFileName(productImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images_Data"), fileName);
                    productImage.SaveAs(path);
                    sanPhamDuocSua.HinhAnh = fileName;
                }

                var ch = new CurrencyHelper();
                sanPhamDuocSua.DonGia = ch.FormatToNumber(productPrice);

                // update tblNguonCungCap
                if (providerIds != null)
                {
                    var nguonCungCapCu = (from nccc in db.tblNguonCungCaps
                                          where nccc.FK_MaSanPham == sanPhamDuocSua.PK_MaSanPham
                                          select nccc).ToList();
                    db.tblNguonCungCaps.RemoveRange(nguonCungCapCu);

                    var nguonCungCapMoi = new List<tblNguonCungCap>();
                    foreach (int maNCC in providerIds)
                    {
                        var nguon = new tblNguonCungCap();
                        nguon.FK_MaSanPham = sanPhamDuocSua.PK_MaSanPham;
                        nguon.FK_MaNhaCungCap = maNCC;
                        nguon.Note = "";
                        nguonCungCapMoi.Add(nguon);
                    }
                    db.tblNguonCungCaps.AddRange(nguonCungCapMoi);
                }
                // save all
                db.SaveChanges();
                result = "Sửa thông tin sản phẩm thành công!";
                redirect = "1";     // chuyển về trang QL SP
            }

            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL san pham - xoa san pham
        [HttpPost]
        public JsonResult AdminXoaSanPham(int productId)
        {
            string result = "";
            string refresh = "";
            var sanPhamBiXoa = (from sp in db.tblSanPhams
                             where sp.PK_MaSanPham == productId
                             select sp).FirstOrDefault();
            if (sanPhamBiXoa != null)
            {
                if (sanPhamBiXoa.tblChiTietDonHangs.Count > 0 ||
                    sanPhamBiXoa.tblChiTietPhieuNhaps.Count > 0)        // tồn tại sp đang liên kết đơn hàng, phiếu nhập
                {
                    result = "Sản phẩm đã có đơn hàng hoặc phiếu nhập. Không thể xóa!";
                    refresh = "0";
                }
                else
                {
                    db.tblSanPhams.Remove(sanPhamBiXoa);
                    var nguonCungCapCu = (from nccc in db.tblNguonCungCaps
                                          where nccc.FK_MaSanPham == sanPhamBiXoa.PK_MaSanPham
                                          select nccc).ToList();
                    db.tblNguonCungCaps.RemoveRange(nguonCungCapCu);

                    db.SaveChanges();
                    result = "Đã xóa thành công sản phẩm!";
                    refresh = "1";
                }
            }
            else
            {
                result = "Xóa sản phẩm thất bại!";
                refresh = "0";
            }

            return Json(new
            {
                msg = result,
                rf = refresh
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL san pham - tim kiem san pham
        [HttpPost]
        public JsonResult AdminTimKiemSanPham(int? pageNumber, String searchKeyword)
        {
            string searchK = searchKeyword;
            return Json(new { redirectToUrl = Url.Action("QuanLySanPham", "Product", new { searchKeyword = searchK }) });
        }
    }
}