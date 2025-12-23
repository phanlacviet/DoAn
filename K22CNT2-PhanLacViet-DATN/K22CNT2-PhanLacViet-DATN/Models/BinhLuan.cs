using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("BinhLuan")]
public partial class BinhLuan
{
    [Key]
    public int MaBinhLuan { get; set; }

    public int? MaChuongTruyen { get; set; }

    [StringLength(50)]
    public string? TaiKhoan { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    [ForeignKey("MaChuongTruyen")]
    [InverseProperty("BinhLuans")]
    public virtual ChuongTruyen? MaChuongTruyenNavigation { get; set; }

    [InverseProperty("MaBinhLuanNavigation")]
    public virtual ICollection<RepBinhLuan> RepBinhLuans { get; set; } = new List<RepBinhLuan>();

    [ForeignKey("TaiKhoan")]
    [InverseProperty("BinhLuans")]
    public virtual ThanhVien? TaiKhoanNavigation { get; set; }
}
