using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class AdminController : Controller
    {
        /*** admin - adm789 ***/
        BMT_DATNEntities db = new BMT_DATNEntities();

        // GET: Admin
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        // QL Nguoi dung
        public ActionResult QuanLyNguoiDung(int? pageNumber)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            int pageSize = 5;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;
            return View();
        }

        // QL Nguoi dung - them tai khoan nguoi dung
        public ActionResult ThemNguoiDung(String sendItem)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            if (sendItem != "")
            {
                ViewBag.checkDuplicateLoginName = sendItem;
            }

            return View();
        }
        public ActionResult AdminThemNguoiDung(String userName, DateTime birthday, String address, String phoneNumber, String loginName, String password1, String password2, string permission )
        {
            // check duplicate loginName
            var checkLoginName = (from nd in db.tblNguoiDungs
                                  where nd.TenDangNhap.Equals(loginName)
                                  select nd).FirstOrDefault();
            if (checkLoginName != null)
            {
                return RedirectToAction("ThemNguoiDung", new { sendItem = "Chọn tên đăng nhập khác" });
            }

            // insert
            var nguoiDungMoi = new tblNguoiDung();
            nguoiDungMoi.PK_MaNguoiDung = Guid.NewGuid();
            nguoiDungMoi.TenNguoiDung = userName;
            nguoiDungMoi.NgaySinh = birthday;
            nguoiDungMoi.DiaChi = address;
            nguoiDungMoi.SoDienThoai = phoneNumber;
            nguoiDungMoi.TenDangNhap = loginName;

            var eh = new EncodingHelper();
            string newSalt = eh.GenerateSalt();
            nguoiDungMoi.MaSalt = newSalt;

            string hashedPassword = eh.ComputeHash(password1, newSalt);
            nguoiDungMoi.MatKhauDaMaHoa = hashedPassword;

            nguoiDungMoi.TinhTrangHoatDong = true;
            nguoiDungMoi.FK_MaQuyen = int.Parse(permission);

            db.tblNguoiDungs.Add(nguoiDungMoi);
            db.SaveChanges();

            //return Redirect("/dang-nhap");

            return RedirectToAction("QuanLyNguoiDung");
        }

        // QL Nguoi dung - xem chi tiet nguoi dung
        public ActionResult XemNguoiDung(Guid maNguoiDung)
        {
            var x = maNguoiDung;
            ViewBag.maNguoiDung = x;

            return View();
        }

        // QL nguoi dung - sua thong tin nguoi dung
        public ActionResult SuaNguoiDung(Guid maNguoiDung)
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.maNguoiDung = maNguoiDung;

            return View();
        }
        public ActionResult AdminSuaNguoiDung(Guid userId, String rsPass, String password1, String password2, String activation, String permission)
        {
            var nguoiDungDuocSua = db.tblNguoiDungs.FirstOrDefault(nd => nd.PK_MaNguoiDung.Equals(userId));
            if (nguoiDungDuocSua != null)
            {
                if (rsPass == "Yes")
                {
                    var eh = new EncodingHelper();
                    string updateSalt = eh.GenerateSalt();
                    nguoiDungDuocSua.MaSalt = updateSalt;

                    string updateHashedPassword = eh.ComputeHash(password1, updateSalt);
                    nguoiDungDuocSua.MatKhauDaMaHoa = updateHashedPassword;
                }
                if (activation == "4")
                {
                    nguoiDungDuocSua.TinhTrangHoatDong = true;
                }
                if (activation == "5")
                {
                    nguoiDungDuocSua.TinhTrangHoatDong = false;
                }    
                nguoiDungDuocSua.FK_MaQuyen = int.Parse(permission);

                // db save
                db.SaveChanges();
            }    
            return RedirectToAction("QuanLyNguoiDung");
        }
    }
}