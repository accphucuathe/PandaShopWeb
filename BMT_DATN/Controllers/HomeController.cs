using BMT_DATN.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class HomeController : Controller
    {
        public class nguoidung
        {
            public static Guid maNguoiDung;
            public static string tenNguoiDung = "";
            public static string tenDangNhap;
            public static int quyenNguoiDung;
        }

        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Home
        public ActionResult Index()
        {
            int b = 5;
            //truyen bang ViewBag
            ViewBag.BienB = b;

            return View();
        }

        public ActionResult Login(String loginNotification)
        {
            if (Session["userID"] != null)
            {
                return Redirect("/trang-chu");
            }
            ViewBag.checkAccount = loginNotification;

            return View();
        }

        public ActionResult Register(String sentItem)
        {
            if (sentItem != "")
            {
                ViewBag.checkRegister = sentItem;
            }

            return View();
        }

        public ActionResult ViewProfile(Guid maNguoiDung)
        {
            ViewBag.userId = maNguoiDung;
            return View();
        }

        public ActionResult EditProfile(Guid maNguoiDung, String editNotification)
        {
            ViewBag.checkEdit = editNotification;
            ViewBag.userId = maNguoiDung;
            return View();
        }

        // Action nhan du lieu login
        [HttpPost]
        public ActionResult UserLogin(String loginName, String password)
        {
            var nguoidungdangnhap = (from nd in db.tblNguoiDungs
                                     where nd.TenDangNhap.Equals(loginName)
                                     select nd).FirstOrDefault();

            if (nguoidungdangnhap == null || !nguoidungdangnhap.TinhTrangHoatDong)
            {
                return RedirectToAction("Login", new { loginNotification = "Đăng nhập thất bại!" });
            }

            var eh = new EncodingHelper();
            string hashedPassword = eh.ComputeHash(password, nguoidungdangnhap.MaSalt);

            if (hashedPassword == nguoidungdangnhap.MatKhauDaMaHoa)
            {
                //return RedirectToAction("Index");
                Session["userID"] = nguoidungdangnhap.PK_MaNguoiDung;
                HomeController.nguoidung.maNguoiDung = nguoidungdangnhap.PK_MaNguoiDung;
                HomeController.nguoidung.tenNguoiDung = nguoidungdangnhap.TenNguoiDung;
                HomeController.nguoidung.tenDangNhap = nguoidungdangnhap.TenDangNhap;
                HomeController.nguoidung.quyenNguoiDung = nguoidungdangnhap.FK_MaQuyen;
                return Redirect("/trang-chu");
            }

            return RedirectToAction("Login", new { loginNotification = "Sai mật khẩu!" });
        }

        // Action nhan du lieu register
        [HttpPost]
        public ActionResult UserRegister(String userName, DateTime birthday, String address, String phoneNumber, String loginName, String password1, String password2)
        {
            string result = "";
            // check
            var checkLoginName = (from nd in db.tblNguoiDungs
                                  where nd.TenDangNhap.Equals(loginName)
                                  select nd).FirstOrDefault();
            if (checkLoginName != null)
            {
                result += "Chọn tên đăng nhập khác";
                return RedirectToAction("Register", new { sentItem = result });
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
            nguoiDungMoi.FK_MaQuyen = (int)EnumQuyen.KhachHang;

            db.tblNguoiDungs.Add(nguoiDungMoi);
            db.SaveChanges();

            return Redirect("/dang-nhap");
        }

        // Action nhan logout
        public ActionResult UserLogout()
        {
            Session["userID"] = null;
            nguoidung.maNguoiDung = Guid.Empty;
            nguoidung.tenNguoiDung = null;
            nguoidung.tenDangNhap = null;
            nguoidung.quyenNguoiDung = 0;
            return Redirect("/trang-chu");
        }

        // Action nhan du lieu edit profile
        public ActionResult UserEditProfile(Guid userId, String userName, DateTime birthday, String address, String phoneNumber, String loginName, String changePass, String oldPassword, String password1, String password2)
        {
            var nguoiDungHienTai = (from nd in db.tblNguoiDungs
                                    where nd.PK_MaNguoiDung.Equals(userId)
                                    select nd).FirstOrDefault();
            nguoiDungHienTai.TenNguoiDung = userName;
            nguoiDungHienTai.NgaySinh = birthday;
            nguoiDungHienTai.DiaChi = address;
            nguoiDungHienTai.SoDienThoai = phoneNumber;
            if (changePass == "Yes")
            {
                var eh = new EncodingHelper();
                // check old password
                string hashedOldPassword = eh.ComputeHash(oldPassword, nguoiDungHienTai.MaSalt);
                if (hashedOldPassword != nguoiDungHienTai.MatKhauDaMaHoa)
                {
                    return RedirectToAction("EditProfile", new { maNguoiDung = nguoiDungHienTai.PK_MaNguoiDung, editNotification = "Mật khẩu cũ sai!" });
                }

                // save new pass
                string updateSalt = eh.GenerateSalt();
                nguoiDungHienTai.MaSalt = updateSalt;

                string updateHashedPassword = eh.ComputeHash(password1, updateSalt);
                nguoiDungHienTai.MatKhauDaMaHoa = updateHashedPassword;
            }
            // save
            db.SaveChanges();

            return RedirectToAction("ViewProfile", new { maNguoiDung = nguoiDungHienTai.PK_MaNguoiDung });
        }

        // danh sach san pham
        public ActionResult DanhSachSanPham(int? maDmsp, int? pageNumber, String searchKeyword)
        {
            ViewBag.maDmsp = maDmsp == null ? 0 : maDmsp ;
            int pageSize = 9;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;
            return View();
        }

        // danh sach san pham - tim kiem san pham
        [HttpPost]
        public JsonResult UserTimKiemSanPham(String searchKeyword)
        {
            string searchK = searchKeyword;
            return Json(new { redirectToUrl = Url.Action("DanhSachSanPham", "Home", new { searchKeyword = searchK }) });
        }

        // chi tiet san pham
        public ActionResult ChiTietSanPham(int maSp) 
        {
            ViewBag.maSanPham = maSp;
            return View();
        }
    }
}