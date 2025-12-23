using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[PrimaryKey("TaiKhoan", "MaTruyen")]
public partial class DanhGium
{
    [Key]
    [StringLength(50)]
    public string TaiKhoan { get; set; } = null!;

    [Key]
    public int MaTruyen { get; set; }

    public int? Diem { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDanhGia { get; set; }

    [ForeignKey("MaTruyen")]
    [InverseProperty("DanhGia")]
    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    [ForeignKey("TaiKhoan")]
    [InverseProperty("DanhGia")]
    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
