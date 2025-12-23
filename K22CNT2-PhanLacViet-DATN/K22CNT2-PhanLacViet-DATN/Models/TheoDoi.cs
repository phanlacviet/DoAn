using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[PrimaryKey("TaiKhoan", "MaTruyen")]
[Table("TheoDoi")]
public partial class TheoDoi
{
    [Key]
    [StringLength(50)]
    public string TaiKhoan { get; set; } = null!;

    [Key]
    public int MaTruyen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTheoDoi { get; set; }

    [ForeignKey("MaTruyen")]
    [InverseProperty("TheoDois")]
    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    [ForeignKey("TaiKhoan")]
    [InverseProperty("TheoDois")]
    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
