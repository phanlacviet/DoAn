using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[PrimaryKey("TaiKhoan", "MaTruyen")]
[Table("LuuTruyen")]
public partial class LuuTruyen
{
    [Key]
    [StringLength(50)]
    public string TaiKhoan { get; set; } = null!;

    [Key]
    public int MaTruyen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayLuu { get; set; }

    [ForeignKey("MaTruyen")]
    [InverseProperty("LuuTruyens")]
    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    [ForeignKey("TaiKhoan")]
    [InverseProperty("LuuTruyens")]
    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
