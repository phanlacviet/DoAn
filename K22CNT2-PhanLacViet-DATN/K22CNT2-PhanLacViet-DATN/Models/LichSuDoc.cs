using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[PrimaryKey("TaiKhoan", "MaTruyen")]
[Table("LichSuDoc")]
public partial class LichSuDoc
{
    [Key]
    [StringLength(50)]
    public string TaiKhoan { get; set; } = null!;

    [Key]
    public int MaTruyen { get; set; }

    public int? MaChuongTruyen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDoc { get; set; }

    [ForeignKey("MaChuongTruyen")]
    [InverseProperty("LichSuDocs")]
    public virtual ChuongTruyen? MaChuongTruyenNavigation { get; set; }

    [ForeignKey("MaTruyen")]
    [InverseProperty("LichSuDocs")]
    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    [ForeignKey("TaiKhoan")]
    [InverseProperty("LichSuDocs")]
    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
