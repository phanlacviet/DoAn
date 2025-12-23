using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("ThanhVien")]
public partial class ThanhVien
{
    [Key]
    [StringLength(50)]
    public string TaiKhoan { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    [StringLength(255)]
    public string? Avatar { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    public bool? IsDeleted { get; set; }

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<LichSuDoc> LichSuDocs { get; set; } = new List<LichSuDoc>();

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<LuuTruyen> LuuTruyens { get; set; } = new List<LuuTruyen>();

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<RepBinhLuan> RepBinhLuans { get; set; } = new List<RepBinhLuan>();

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<TheoDoi> TheoDois { get; set; } = new List<TheoDoi>();

    [InverseProperty("TaiKhoanNavigation")]
    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    [InverseProperty("NguoiDangNavigation")]
    public virtual ICollection<Truyen> Truyens { get; set; } = new List<Truyen>();
}
