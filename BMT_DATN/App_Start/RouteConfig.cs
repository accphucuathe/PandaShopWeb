using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BMT_DATN
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Home View - link than thien
            routes.MapRoute(
                name: "TrangChu",
                url: "trang-chu",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DangNhap",
                url: "dang-nhap",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DangKy",
                url: "dang-ky",
                defaults: new { controller = "Home", action = "Register", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "XemThongTinCaNhan",
                url: "thong-tin-ca-nhan",
                defaults: new { controller = "Home", action = "ViewProfile", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ChinhSuaThongTinCaNhan",
                url: "chinh-sua-thong-tin-ca-nhan",
                defaults: new { controller = "Home", action = "EditProfile", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DanhSachSanPham",
                url: "danh-sach-san-pham",
                defaults: new { controller = "Home", action = "DanhSachSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ChiTietSanPham",
                url: "chi-tiet-san-pham",
                defaults: new { controller = "Home", action = "ChiTietSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "GioHang",
                url: "gio-hang",
                defaults: new { controller = "Home", action = "GioHang", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ThanhToan",
                url: "thanh-toan",
                defaults: new { controller = "Home", action = "ThanhToan", id = UrlParameter.Optional }
            );

            // Admin + QuanLyNguoiDung View
            routes.MapRoute(
                name: "TrangChuAdmin",
                url: "trang-chu-admin",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "QuanLyNguoiDung",
                url: "quan-ly-nguoi-dung",
                defaults: new { controller = "Admin", action = "QuanLyNguoiDung", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ThemNguoiDung",
                url: "them-nguoi-dung",
                defaults: new { controller = "Admin", action = "ThemNguoiDung", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "XemNguoiDung",
                url: "xem-nguoi-dung",
                defaults: new { controller = "Admin", action = "XemNguoiDung", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SuaNguoiDung",
                url: "sua-nguoi-dung",
                defaults: new { controller = "Admin", action = "SuaNguoiDung", id = UrlParameter.Optional }
            );

            // QuanLyDanhMucSanPham View
            routes.MapRoute(
                name: "QuanLyDanhMucSanPham",
                url: "quan-ly-danh-muc-san-pham",
                defaults: new { controller = "Category", action = "QuanLyDanhMucSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ThemDanhMucSanPham",
                url: "them-danh-muc-san-pham",
                defaults: new { controller = "Category", action = "ThemDanhMucSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "XemDanhMucSanPham",
                url: "xem-danh-muc-san-pham",
                defaults: new { controller = "Category", action = "XemDanhMucSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SuaDanhMucSanPham",
                url: "sua-danh-muc-san-pham",
                defaults: new { controller = "Category", action = "SuaDanhMucSanPham", id = UrlParameter.Optional }
            );

            // QuanLyNhaCungCap View
            routes.MapRoute(
                name: "QuanLyNhaCungCap",
                url: "quan-ly-nha-cung-cap",
                defaults: new { controller = "Provider", action = "QuanLyNhaCungCap", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ThemNhaCungCap",
                url: "them-nha-cung-cap",
                defaults: new { controller = "Provider", action = "ThemNhaCungCap", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "XemNhaCungCap",
                url: "xem-nha-cung-cap",
                defaults: new { controller = "Provider", action = "XemNhaCungCap", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SuaNhaCungCap",
                url: "sua-nha-cung-cap",
                defaults: new { controller = "Provider", action = "SuaNhaCungCap", id = UrlParameter.Optional }
            );

            // QuanLySanPham View
            routes.MapRoute(
                name: "QuanLySanPham",
                url: "quan-ly-san-pham",
                defaults: new { controller = "Product", action = "QuanLySanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ThemSanPham",
                url: "them-san-pham",
                defaults: new { controller = "Product", action = "ThemSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "XemSanPham",
                url: "xem-san-pham",
                defaults: new { controller = "Product", action = "XemSanPham", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SuaSanPham",
                url: "sua-san-pham",
                defaults: new { controller = "Product", action = "SuaSanPham", id = UrlParameter.Optional }
            );

            // mac dinh
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
