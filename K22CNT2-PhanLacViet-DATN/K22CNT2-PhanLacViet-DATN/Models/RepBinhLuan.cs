using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("RepBinhLuan")]
public partial class RepBinhLuan
{
    [Key]
    public int MaRep { get; set; }

    public int? MaBinhLuan { get; set; }

    [StringLength(50)]
    public string? TaiKhoan { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    [ForeignKey("MaBinhLuan")]
    [InverseProperty("RepBinhLuans")]
    public virtual BinhLuan? MaBinhLuanNavigation { get; set; }

    [ForeignKey("TaiKhoan")]
    [InverseProperty("RepBinhLuans")]
    public virtual ThanhVien? TaiKhoanNavigation { get; set; }
}
