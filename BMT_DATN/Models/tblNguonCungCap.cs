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
    
    public partial class tblNguonCungCap
    {
        public int FK_MaSanPham { get; set; }
        public int FK_MaNhaCungCap { get; set; }
        public string Note { get; set; }
    
        public virtual tblNhaCungCap tblNhaCungCap { get; set; }
        public virtual tblSanPham tblSanPham { get; set; }
    }
}