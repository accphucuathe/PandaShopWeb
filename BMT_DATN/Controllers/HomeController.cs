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
            // gio hang hien tai
            public static int? maGioHang;
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

        public ActionResult ViewProfile()
        {
            ViewBag.userId = nguoidung.maNguoiDung;
            return View();
        }

        public ActionResult EditProfile(String editNotification)
        {
            ViewBag.checkEdit = editNotification;
            ViewBag.userId = nguoidung.maNguoiDung;
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
                // lay gio hang hien tai
                LayGioHangHienTai();

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
        public ActionResult DanhSachSanPham(int? maDmsp, int? pageNumber, String searchKeyword, int? sortBy)
        {
            ViewBag.maDmsp = maDmsp == null ? 0 : maDmsp;
            int pageSize = 9;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.sortBy = sortBy;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;
            return View();
        }

        // danh sach san pham - tim kiem san pham
        [HttpPost]
        public JsonResult UserTimKiemSanPham(int? maDmsp, String searchKeyword, int sortBy)
        {
            int maD = (int)(maDmsp == null ? 0 : maDmsp);
            string searchK = searchKeyword;
            int sortB = sortBy;
            return Json(new { redirectToUrl = Url.Action("DanhSachSanPham", "Home", new { maDmsp = maD, searchKeyword = searchK, sortBy = sortB }) });
        }

        // chi tiet san pham
        public ActionResult ChiTietSanPham(int maSp)
        {
            ViewBag.maSanPham = maSp;
            return View();
        }

        // gio hang
        public ActionResult GioHang()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // lay gio hang hien tai
        public void LayGioHangHienTai()
        {
            var listOrder = (from o in db.tblDonHangs
                             join c in db.tblChiTietTrangThaiDonHangs on o.PK_MaDonHang equals c.FK_MaDonHang
                             where o.FK_MaNguoiDung.Equals(nguoidung.maNguoiDung)
                             orderby c.FK_MaTrangThaiDonHang
                             group c by c.FK_MaDonHang
                                 into gr
                             select new
                             {
                                 maDonHang = gr.Key,
                                 soTrangThai = gr.Select(g => g.FK_MaTrangThaiDonHang).Count()
                             }
                                 ).ToList();
            var currentCart = listOrder.Where(o => o.soTrangThai == 1).LastOrDefault();
            HomeController.nguoidung.maGioHang = currentCart?.maDonHang;
        }

        // user them san pham vao gio hang
        [HttpPost]
        public JsonResult UserThemSanPhamVaoGioHang(int productId)
        {
            string result = "";
            string redirect = "";
            Guid userId = nguoidung.maNguoiDung;
            int maDonHang = !nguoidung.maGioHang.HasValue ? 0 : nguoidung.maGioHang.Value;
            var sanPhamDuocThem = (from sp in db.tblSanPhams
                                   where sp.PK_MaSanPham == productId
                                   select sp).FirstOrDefault();
            if (Session["userID"] == null)         // chua dang nhap
            {
                result = "Vui lòng đăng nhập!";
                redirect = "1";         // chuyen huong trang dang nhap
            }
            else
            {
                if (maDonHang == 0)   //chua co gio hang thi tao gio hang moi
                {
                    // insert tblDonHang
                    var donHangMoi = new tblDonHang();
                    donHangMoi.DiaChiGiaoHang = "";
                    donHangMoi.SoDienThoai = "";
                    donHangMoi.GhiChu = "";
                    donHangMoi.FK_MaNguoiDung = userId;
                    db.tblDonHangs.Add(donHangMoi);

                    // insert tblChiTietTrangThaiDonHang
                    var trangThaiDonHangMoi = new tblChiTietTrangThaiDonHang();
                    trangThaiDonHangMoi.FK_MaDonHang = donHangMoi.PK_MaDonHang;
                    trangThaiDonHangMoi.FK_MaTrangThaiDonHang = (int)EnumTrangThaiDonHang.GioHang;
                    trangThaiDonHangMoi.ThoiGianCapNhat = DateTime.Now;
                    trangThaiDonHangMoi.FK_MaNhanVienCapNhat = userId;
                    db.tblChiTietTrangThaiDonHangs.Add(trangThaiDonHangMoi);

                    // insert tblChiTietDonHang
                    var chiTietDonHangMoi = new tblChiTietDonHang();
                    chiTietDonHangMoi.FK_MaDonHang = donHangMoi.PK_MaDonHang;
                    chiTietDonHangMoi.FK_MaSanPham = sanPhamDuocThem.PK_MaSanPham;
                    chiTietDonHangMoi.SoLuongMua = 1;
                    chiTietDonHangMoi.DonGia = sanPhamDuocThem.DonGia;
                    db.tblChiTietDonHangs.Add(chiTietDonHangMoi);

                    // save
                    db.SaveChanges();
                    HomeController.nguoidung.maGioHang = donHangMoi.PK_MaDonHang;

                    result = "Thêm sản phẩm vào giỏ hàng thành công!";
                    redirect = "0";
                }
                else
                {
                    // check san pham da co trong gio hang chua
                    var productInCurrentCart = (from sp in db.tblChiTietDonHangs
                                                 where sp.FK_MaDonHang == maDonHang &&
                                                        sp.FK_MaSanPham == sanPhamDuocThem.PK_MaSanPham
                                                 select sp).FirstOrDefault();
                    if (productInCurrentCart == null)      // chua co sp nay trong gio hang
                    {
                        // insert tblChiTietDonHang
                        var chiTietDonHangMoi = new tblChiTietDonHang();
                        chiTietDonHangMoi.FK_MaDonHang = maDonHang;
                        chiTietDonHangMoi.FK_MaSanPham = sanPhamDuocThem.PK_MaSanPham;
                        chiTietDonHangMoi.SoLuongMua = 1;
                        chiTietDonHangMoi.DonGia = sanPhamDuocThem.DonGia;
                        db.tblChiTietDonHangs.Add(chiTietDonHangMoi);
                        // save
                        db.SaveChanges();

                        result = "Thêm sản phẩm vào giỏ hàng thành công!";
                        redirect = "0";
                    }
                    else
                    {
                        productInCurrentCart.SoLuongMua++;
                        // save
                        db.SaveChanges();

                        result = "Đã tăng số lượng sản phẩm trong giỏ hàng!";
                        redirect = "0";
                    }                  
                }            
            }
            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // user xoa san pham khoi gio hang
        [HttpPost]
        public JsonResult UserXoaSanPhamKhoiGioHang(int productId)
        {
            string result = "";
            string success = "";
            int maDonHang = !nguoidung.maGioHang.HasValue ? 0 : nguoidung.maGioHang.Value;
            var checkProductInCart = (from sp in db.tblChiTietDonHangs
                                      where sp.FK_MaDonHang == maDonHang &&
                                             sp.FK_MaSanPham == productId
                                      select sp).FirstOrDefault();
            if (checkProductInCart != null) {
                db.tblChiTietDonHangs.Remove(checkProductInCart);
                // save
                db.SaveChanges();

                result = "Đã xóa sản phẩm khỏi giỏ hàng";
                success = "1";
            }
            return Json(new
            {
                msg = result,
                scs = success
            },
            JsonRequestBehavior.AllowGet);
        }

        // trang thanh toan
        public ActionResult ThanhToan(int[] idProduct, int[] quantityProduct)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login");
            }
            // luu thay doi so luong san pham
            int maDonHang = !nguoidung.maGioHang.HasValue ? 0 : nguoidung.maGioHang.Value;
            int[] idProd = idProduct;
            int[] quantityProd = quantityProduct;
            var productsInCurrentCart = (from ctdh in db.tblChiTietDonHangs
                                         where ctdh.FK_MaDonHang == maDonHang
                                         select ctdh).ToList();
            for (int i = 0; i < productsInCurrentCart.Count; i++)
            {
                var prd = productsInCurrentCart.Where(p => p.FK_MaSanPham == idProd[i]).FirstOrDefault();
                if (prd != null)
                {
                    prd.SoLuongMua = quantityProd[i];
                }
            }
            // save
            db.SaveChanges();

            ViewBag.userId = nguoidung.maNguoiDung;
            return View();
        }

        // Action khi user thanh toan
        public ActionResult UserThanhToan(String addressShipping, String phoneShipping) 
        {
            Guid userId = nguoidung.maNguoiDung;
            int maDonHang = !nguoidung.maGioHang.HasValue ? 0 : nguoidung.maGioHang.Value;
            // ghi nhan trang thai moi cua don hang
            var trangThaiDonHangMoi = new tblChiTietTrangThaiDonHang();
            trangThaiDonHangMoi.FK_MaDonHang = maDonHang;
            trangThaiDonHangMoi.FK_MaTrangThaiDonHang = (int)EnumTrangThaiDonHang.DatHangChoXacNhan;
            trangThaiDonHangMoi.ThoiGianCapNhat = DateTime.Now;
            trangThaiDonHangMoi.FK_MaNhanVienCapNhat = userId;
            db.tblChiTietTrangThaiDonHangs.Add(trangThaiDonHangMoi);
            // luu sdt va dia chi giao hang
            var donHangHienTai = (from dh in db.tblDonHangs
                                  where dh.PK_MaDonHang == maDonHang
                                  select dh).FirstOrDefault();
            donHangHienTai.DiaChiGiaoHang = addressShipping;
            donHangHienTai.SoDienThoai = phoneShipping;
            // save
            db.SaveChanges();
            LayGioHangHienTai();

            return RedirectToAction("DanhSachDonHang");
        }

        // trang danh sach don hang cua user
        public ActionResult DanhSachDonHang()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // trang chi tiet don hang cua user
        public ActionResult ChiTietDonHang(int maDh)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.maDonHang = maDh;

            return View();
        }
    }
}