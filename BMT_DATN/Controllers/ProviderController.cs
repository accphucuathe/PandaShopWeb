using BMT_DATN.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class ProviderController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Provider
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // QL nha cung cap
        public ActionResult QuanLyNhaCungCap(int? pageNumber, String searchKeyword)
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

        // QL nha cung cap - them nha cung cap
        public ActionResult ThemNhaCungCap(String duplicateProviderNameCheck)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            if (duplicateProviderNameCheck != "")
            {
                ViewBag.checkDuplicateProviderName = duplicateProviderNameCheck;
            }
            return View();
        }

        public ActionResult AdminThemNhaCungCap(String providerName, String address, String phoneNumber)
        {
            // check duplicate providerName
            var checkLoginName = (from ncc in db.tblNhaCungCaps
                                  where ncc.TenNhaCungCap.Equals(providerName)
                                  select ncc).FirstOrDefault();
            if (checkLoginName != null)
            {
                return RedirectToAction("ThemNhaCungCap", new { duplicateProviderNameCheck = "Trùng tên nhà cung cấp" });
            }

            // insert
            var nhaCungCapMoi = new tblNhaCungCap();
            nhaCungCapMoi.TenNhaCungCap = providerName;
            nhaCungCapMoi.DiaChi = address;
            nhaCungCapMoi.SoDienThoai = phoneNumber;

            db.tblNhaCungCaps.Add(nhaCungCapMoi);
            db.SaveChanges();

            return RedirectToAction("QuanLyNhaCungCap");
        }

        // QL nha cung cap - xem nha cung cap
        public ActionResult XemNhaCungCap(int maNhaCungCap)
        {
            ViewBag.maNhaCungCap = maNhaCungCap;

            return View();
        }

        // QL nha cung cap - sua nha cung cap
        public ActionResult SuaNhaCungCap(int maNhaCungCap, String duplicateProviderNameCheck)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maNhaCungCap = maNhaCungCap;
            ViewBag.checkDuplicateProviderName = duplicateProviderNameCheck;
            return View();
        }

        public ActionResult AdminSuaNhaCungCap(int providerId, String providerName, String address, String phoneNumber)
        {
            var nhaCungCapDuocSua = (from ncc in db.tblNhaCungCaps
                                     where ncc.PK_MaNhaCungCap == providerId
                                     select ncc).FirstOrDefault();
            if (nhaCungCapDuocSua != null)
            {
                var checkProviderName = (from ncc in db.tblNhaCungCaps
                                         where ncc.TenNhaCungCap.Equals(providerName) &&
                                                ncc.PK_MaNhaCungCap != nhaCungCapDuocSua.PK_MaNhaCungCap
                                         select ncc).FirstOrDefault();
                if (checkProviderName != null)
                {
                    return RedirectToAction("SuaNhaCungCap", new { maNhaCungCap = providerId, duplicateProviderNameCheck = "Trùng tên nhà cung cấp" });
                }
                // update ncc
                nhaCungCapDuocSua.TenNhaCungCap = providerName;
                nhaCungCapDuocSua.DiaChi = address;
                nhaCungCapDuocSua.SoDienThoai = phoneNumber;
                db.SaveChanges();
            }
            return RedirectToAction("QuanLyNhaCungCap");
        }

        // QL nha cung cap - xoa nha cung cap
        [HttpPost]
        public JsonResult AdminXoaNhaCungCap(int providerId)
        {
            string result = "";
            var nccBiXoa = (from ncc in db.tblNhaCungCaps
                             where ncc.PK_MaNhaCungCap == providerId
                             select ncc).FirstOrDefault();
            if (nccBiXoa != null)
            {
                if (nccBiXoa.tblNguonCungCaps.Count > 0)        // tồn tại sp đang liên kết với ncc
                {
                    result = "Nhà cung cấp đang liên kết với sản phẩm. Không thể xóa!";
                }
                else
                {
                    db.tblNhaCungCaps.Remove(nccBiXoa);
                    db.SaveChanges();
                    result = "Đã xóa thành công nhà cung cấp!";
                }
            }
            else
            {
                result = "Xóa nhà cung cấp thất bại!";
            }

            return Json(new
            {
                msg = result
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL Nha cung cap - tim kiem nha cung cap
        [HttpPost]
        public JsonResult AdminTimKiemNhaCungCap(int? pageNumber, String searchKeyword)
        {
            string searchK = searchKeyword;
            return Json(new { redirectToUrl = Url.Action("QuanLyNhaCungCap", "Provider", new { searchKeyword = searchK }) });
        }
    }
}