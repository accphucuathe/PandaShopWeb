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
    
    public partial class tblSanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSanPham()
        {
            this.tblChiTietDonHangs = new HashSet<tblChiTietDonHang>();
            this.tblChiTietKhuyenMais = new HashSet<tblChiTietKhuyenMai>();
            this.tblChiTietPhieuNhaps = new HashSet<tblChiTietPhieuNhap>();
            this.tblDanhGiaSanPhams = new HashSet<tblDanhGiaSanPham>();
            this.tblNguonCungCaps = new HashSet<tblNguonCungCap>();
            this.tblGiaThayDois = new HashSet<tblGiaThayDoi>();
        }
    
        public int PK_MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string KichCo { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public string DonVi { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public int FK_MaDanhMucSanPham { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblChiTietDonHang> tblChiTietDonHangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblChiTietKhuyenMai> tblChiTietKhuyenMais { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblChiTietPhieuNhap> tblChiTietPhieuNhaps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDanhGiaSanPham> tblDanhGiaSanPhams { get; set; }
        public virtual tblDanhMucSanPham tblDanhMucSanPham { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblNguonCungCap> tblNguonCungCaps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblGiaThayDoi> tblGiaThayDois { get; set; }
    }
}
