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
            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.ChuCuaHang)
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
                //return RedirectToAction("Index", "Home");
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
                                where sp.TenSanPham.Equals(productName) && sp.KichCo.Equals(productSize)
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

            string fileName = Path.GetFileName(productImage.FileName);
            string path = Path.Combine(Server.MapPath("~/Images_Data"), fileName);
            productImage.SaveAs(path);
            sanPhamMoi.HinhAnh = fileName;

            var ch = new CurrencyHelper();
            var yy = ch.FormatToCurrency(1234567890);
            sanPhamMoi.DonGia = ch.FormatToNumber(productPrice);

            db.tblSanPhams.Add(sanPhamMoi);

            // insert tblNguonCungCap
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

            db.SaveChanges();

            return RedirectToAction("QuanLySanPham", "Product");
        }
    }
}