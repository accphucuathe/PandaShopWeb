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
            if (HomeController.nguoidung.quyenNguoiDung == (int)EnumQuyen.ChuCuaHang)
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
            ViewBag.maDonHang = maDh;

            return View();
        }

        // QL don hang - admin cap nhat trang thai don hang
        public JsonResult AdminCapNhatTrangThaiDonHang(int orderId, int newOrderStatusId)
        {
            string result = "";
            string redirect = "";
            string refresh = "";
            Guid userId = HomeController.nguoidung.maNguoiDung;
            var checkTrangThaiDonHang = (from ctttdh in db.tblChiTietTrangThaiDonHangs
                                         where ctttdh.FK_MaDonHang == orderId &&
                                                ctttdh.FK_MaTrangThaiDonHang == newOrderStatusId
                                         select ctttdh).FirstOrDefault();
            if (checkTrangThaiDonHang != null)
            {
                result = "Đơn hàng đã có trạng thái này";
                redirect = "0";
                refresh = "1";
            }
            else
            {
                // lay trang thai hien tai cua don hang
                var currentOrderStatus = (from ctttdh in db.tblChiTietTrangThaiDonHangs
                                          where ctttdh.FK_MaDonHang == orderId &&
                                                ctttdh.FK_MaTrangThaiDonHang != (int)EnumTrangThaiDonHang.GioHang
                                          orderby ctttdh.FK_MaTrangThaiDonHang descending
                                          select ctttdh).FirstOrDefault();
                result = "Đã cập nhật trạng thái cho đơn hàng này";
                redirect = "1";
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

                // cap nhat trang thai don hang moi
                var trangThaiDonHangMoi = new tblChiTietTrangThaiDonHang();
                trangThaiDonHangMoi.FK_MaDonHang = orderId;
                trangThaiDonHangMoi.FK_MaTrangThaiDonHang = newOrderStatusId;
                trangThaiDonHangMoi.ThoiGianCapNhat = DateTime.Now;
                trangThaiDonHangMoi.FK_MaNhanVienCapNhat = userId;
                db.tblChiTietTrangThaiDonHangs.Add(trangThaiDonHangMoi);
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

            return View();
        }
    }
}