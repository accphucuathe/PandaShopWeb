//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMT_DATN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblDanhGiaSanPham
    {
        public int PK_MaDanhGiaSanPham { get; set; }
        public string NoiDungDanhGiaSanPham { get; set; }
        public System.DateTime ThoiGianDanhGia { get; set; }
        public int FK_MaSanPham { get; set; }
        public System.Guid FK_MaNguoiDung { get; set; }
    
        public virtual tblNguoiDung tblNguoiDung { get; set; }
        public virtual tblSanPham tblSanPham { get; set; }
    }
}
