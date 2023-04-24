using BMT_DATN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMT_DATN.Controllers
{
    public class OrderController : Controller
    {
        BMT_DATNEntities db = new BMT_DATNEntities();
        // GET: Order
        public ActionResult Index()
        {
            // check permission
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Admin");
        }

        // quan ly don hang
        public ActionResult QuanLyDonHang(int? pageNumber, String searchKeyword)
        {
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang &&
                HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.NhanVien)
            {
                return RedirectToAction("Index", "Home");
            }
            int pageSize = 10;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;

            return View();
        }

        // QL don hang - admin xem don hang
        public ActionResult XemDonHang(int maDh)
        {
            ViewBag.maDonHang = maDh;

            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.NhanVien)
            {
                return View("XemDonHang", "layout_staff");
            }
            return View();
        }

        // QL don hang - trang cap nhat trang thai don hang
        public ActionResult CapNhatTrangThaiDonHang(int maDh)
        {
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang &&
                HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.NhanVien)
            {
                return RedirectToAction("Index", "Home");
            }
            var infoDonHang = (from dh in db.tblDonHangs
                               where dh.PK_MaDonHang == maDh
                               select dh).FirstOrDefault();
            if (infoDonHang.FK_MaTrangThaiDonHang == (int)EnumTrangThaiDonHang.DaNhan ||
                infoDonHang.FK_MaTrangThaiDonHang == (int)EnumTrangThaiDonHang.DaHoanTra ||
                infoDonHang.FK_MaTrangThaiDonHang == (int)EnumTrangThaiDonHang.DaHuy ||
                infoDonHang.FK_MaTrangThaiDonHang == (int)EnumTrangThaiDonHang.MuaTaiCuaHang)
            {
                return RedirectToAction("QuanLyDonHang", "Order");
            }
            ViewBag.maDonHang = maDh;

            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.NhanVien)
            {
                return View("XemDonHang", "layout_staff");
            }
            return View();
        }

        // QL don hang - admin cap nhat trang thai don hang
        public JsonResult AdminCapNhatTrangThaiDonHang(int orderId, int newOrderStatusId)
        {
            string result = "";
            string redirect = "";
            string refresh = "";
            Guid userId = HomeController.nguoidung.maNguoiDung;
            string userName = HomeController.nguoidung.tenNguoiDung;
            //var checkTrangThaiDonHang = (from ctttdh in db.tblChiTietTrangThaiDonHangs
            //                             where ctttdh.FK_MaDonHang == orderId &&
            //                                    ctttdh.FK_MaTrangThaiDonHang == newOrderStatusId
            //                             select ctttdh).FirstOrDefault();
            var getTenTrangThaiDonHang = (from ttdh in db.tblTrangThaiDonHangs
                                          where ttdh.PK_MaTrangThaiDonHang == newOrderStatusId
                                          select ttdh.TenTrangThaiDonHang).FirstOrDefault();
            var checkTrangThaiDonHang = (from lsttdh in db.tblLichSuTrangThaiDonHangs
                                         where lsttdh.FK_MaDonHang == orderId &&
                                                lsttdh.TenTrangThaiDonHang.Equals(getTenTrangThaiDonHang)
                                         select lsttdh).FirstOrDefault();
            if (checkTrangThaiDonHang != null)
            {
                result = "Đơn hàng đã có trạng thái này";
                redirect = "0";
                refresh = "1";
            }
            else
            {
                // lay trang thai hien tai cua don hang
                //var currentOrderStatus = (from ctttdh in db.tblChiTietTrangThaiDonHangs
                //                          where ctttdh.FK_MaDonHang == orderId &&
                //                                ctttdh.FK_MaTrangThaiDonHang != (int)EnumTrangThaiDonHang.GioHang
                //                          orderby ctttdh.FK_MaTrangThaiDonHang descending
                //                          select ctttdh).FirstOrDefault();
                // lay don hang hien tai
                var currentOrder = (from dh in db.tblDonHangs
                                    where dh.PK_MaDonHang == orderId
                                    select dh).FirstOrDefault();
                currentOrder.FK_MaTrangThaiDonHang = newOrderStatusId;
                currentOrder.ThoiGianCapNhatTrangThai = DateTime.Now;

                result = "Đã cập nhật trạng thái cho đơn hàng này";
                redirect = "1";
                // neu trang thai moi la "Da xac nhan" thi se luu lai nhan vien xu ly
                if (newOrderStatusId == (int)EnumTrangThaiDonHang.DaXacNhan)
                {
                    currentOrder.FK_MaNhanVienXuLy = userId;
                }
                // neu trang thai moi la "Dang Giao" thi se giam tru so luong sp trong kho
                if (newOrderStatusId == (int)EnumTrangThaiDonHang.DangGiao)
                {
                    var productsOfOrder = (from  ctdh in db.tblChiTietDonHangs
                                           where ctdh.FK_MaDonHang == orderId
                                           select ctdh).ToList();
                    foreach (var product in productsOfOrder)
                    {
                        var quantityProductOfOrder = product.SoLuongMua;
                        var productOfStore = (from sp in db.tblSanPhams
                                              where sp.PK_MaSanPham == product.FK_MaSanPham
                                              select sp).FirstOrDefault();
                        // check so luong dat mua va so luong con lai trong kho
                        if (quantityProductOfOrder > productOfStore.SoLuong)
                        {
                            return Json(new { msg = "Số lượng trong kho không đủ để vận đơn", rdt = "0" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            productOfStore.SoLuong -= quantityProductOfOrder;
                        }
                    }
                }
                // neu trang thai moi la "Da Huy"
                if (newOrderStatusId == (int)EnumTrangThaiDonHang.DaHuy)
                {
                    currentOrder.FK_MaNhanVienXuLy = userId;    // luu nhan vien huy don
                    result = "Đã hủy đơn hàng này";
                    redirect = "0";
                    refresh = "1";
                }
                // neu trang thai moi la "Dang Hoan Tra"
                if (newOrderStatusId == (int)EnumTrangThaiDonHang.DangHoanTra)
                {
                    result = "Bắt đầu hoàn đơn hàng này";
                    redirect = "0";
                    refresh = "1";
                }
                // neu trang thai moi la "Da Hoan Tra" thi se cong lai so luong sp trong kho
                if (newOrderStatusId == (int)EnumTrangThaiDonHang.DaHoanTra)
                {
                    var productsOfOrder = (from ctdh in db.tblChiTietDonHangs
                                           where ctdh.FK_MaDonHang == orderId
                                           select ctdh).ToList();
                    foreach (var product in productsOfOrder)
                    {
                        var quantityProductOfOrder = product.SoLuongMua;
                        var productOfStore = (from sp in db.tblSanPhams
                                              where sp.PK_MaSanPham == product.FK_MaSanPham
                                              select sp).FirstOrDefault();
                        // cong lai so luong sp trong kho
                        productOfStore.SoLuong += quantityProductOfOrder;
                    }
                    result = "Hoàn tất hoàn trả đơn hàng này \n Số lượng sản phẩm trong kho đã cập nhật lại";
                    redirect = "1";
                    refresh = "1";
                }

                // luu lai lich su trang thai don hang moi
                var lichSuTrangThaiDonHangMoi = new tblLichSuTrangThaiDonHang();
                lichSuTrangThaiDonHangMoi.FK_MaDonHang = orderId;
                lichSuTrangThaiDonHangMoi.TenTrangThaiDonHang = getTenTrangThaiDonHang;
                lichSuTrangThaiDonHangMoi.ThoiGianCapNhat = DateTime.Now;
                lichSuTrangThaiDonHangMoi.TenNguoiCapNhat = userName;
                db.tblLichSuTrangThaiDonHangs.Add(lichSuTrangThaiDonHangMoi);
                // save
                db.SaveChanges();
            }

            return Json(new
            {
                msg = result,
                rdt = redirect,
                rfr = refresh
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL don hang - them don hang
        public ActionResult ThemDonHang()
        {
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang &&
                HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.NhanVien)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.userId = HomeController.nguoidung.maNguoiDung;

            return View();
        }

        // QL don hang - admin them don hang moi
        [HttpPost]
        public JsonResult AdminThemDonHang(String receiverShipping, String addressShipping, String phoneShipping, String noteShipping, int[] productIds, int[] productQuantitys)
        {
            string result = "";
            string redirect = "";
            Guid userId = HomeController.nguoidung.maNguoiDung;
            string userName = HomeController.nguoidung.tenNguoiDung;
            int[] idProd = productIds;
            int[] quantityProd = productQuantitys;
            int[] priceProd = new int[idProd.Length];
            int checkValidQuantity = 0;
            for (int i = 0; i < idProd.Length; i++)
            {
                var productIdTemp = idProd[i];
                var productQuantityTemp = quantityProd[i];
                var prd = db.tblSanPhams.Where(p => p.PK_MaSanPham == productIdTemp && p.SoLuong >= productQuantityTemp).FirstOrDefault();
                if (prd == null)
                {
                    checkValidQuantity++;
                }
                else
                {
                    priceProd[i] = prd.DonGia;
                }
            }
            if (checkValidQuantity != 0)
            {
                result = "Số lượng sản phẩm trong kho không đủ!";
                redirect = "0";
            }
            else
            {
                // insert tblDonHang
                var donHangMoi = new tblDonHang();
                donHangMoi.TenNguoiNhan = receiverShipping;
                donHangMoi.DiaChiGiaoHang = addressShipping;
                donHangMoi.SoDienThoai = phoneShipping;
                donHangMoi.GhiChu = noteShipping;
                donHangMoi.FK_MaNguoiDung = userId;
                donHangMoi.ThoiGianTaoDonHang = DateTime.Now;
                donHangMoi.FK_MaNhanVienXuLy = userId;
                donHangMoi.FK_MaTrangThaiDonHang = (int)EnumTrangThaiDonHang.MuaTaiCuaHang;
                donHangMoi.ThoiGianCapNhatTrangThai = DateTime.Now;
                db.tblDonHangs.Add(donHangMoi);

                // insert tblChiTietTrangThaiDonHang
                var lichSuTrangThaiDonHangMoi = new tblLichSuTrangThaiDonHang();
                lichSuTrangThaiDonHangMoi.FK_MaDonHang = donHangMoi.PK_MaDonHang;
                lichSuTrangThaiDonHangMoi.TenTrangThaiDonHang = "Mua tại cửa hàng";
                lichSuTrangThaiDonHangMoi.ThoiGianCapNhat = DateTime.Now;
                lichSuTrangThaiDonHangMoi.TenNguoiCapNhat = userName;
                db.tblLichSuTrangThaiDonHangs.Add(lichSuTrangThaiDonHangMoi);

                // insert tblChiTietDonHang
                List<tblChiTietDonHang> listCtdh = new List<tblChiTietDonHang>();
                for (int i = 0; i < idProd.Length; i++)
                {
                    var idProduct = idProd[i];
                    var chiTietDonHangMoi = new tblChiTietDonHang();
                    chiTietDonHangMoi.FK_MaDonHang = donHangMoi.PK_MaDonHang;
                    chiTietDonHangMoi.FK_MaSanPham = idProduct;
                    chiTietDonHangMoi.SoLuongMua = quantityProd[i];
                    chiTietDonHangMoi.DonGia = priceProd[i];
                    listCtdh.Add(chiTietDonHangMoi);
                    // giam tru so luong sp trong kho
                    var productOfStore = (from sp in db.tblSanPhams
                                          where sp.PK_MaSanPham == idProduct
                                          select sp).FirstOrDefault();
                    productOfStore.SoLuong -= quantityProd[i];
                }
                db.tblChiTietDonHangs.AddRange(listCtdh);

                // save
                db.SaveChanges();

                result = "Thêm mới đơn hàng thành công!";
                redirect = "1";
            }

            return Json(new
            {
                msg = result,
                rdt = redirect
            },
            JsonRequestBehavior.AllowGet);
        }

        // QL don hang - tim kiem don hang
        [HttpPost]
        public JsonResult AdminTimKiemDonHang(int? pageNumber, String searchKeyword)
        {
            string searchK = searchKeyword;
            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.NhanVien)
            {
                return Json(new { redirectToUrl = Url.Action("QuanLyDonHang_NhanVien", "Order", new { searchKeyword = searchK }) });
            }
            return Json(new { redirectToUrl = Url.Action("QuanLyDonHang", "Order", new { searchKeyword = searchK }) });
        }

        // QL don hang cua NHAN VIEN
        public ActionResult QuanLyDonHang_NhanVien(int? pageNumber, String searchKeyword)
        {
            if (HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.ChuCuaHang &&
                HomeController.nguoidung.quyenNguoiDung != (int)EnumQuyen.NhanVien)
            {
                return RedirectToAction("Index", "Home");
            }
            int pageSize = 10;   // set page size
            ViewBag.searchKeyword = searchKeyword;
            ViewBag.pageNumber = pageNumber == null ? 1 : pageNumber;
            ViewBag.pageSize = pageSize;

            return View();
        }
    }
}