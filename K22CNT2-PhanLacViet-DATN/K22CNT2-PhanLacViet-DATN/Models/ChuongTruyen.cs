using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("ChuongTruyen")]
[Index("MaTruyen", "ThuTuChuong", Name = "UQ_Truyen_Chuong", IsUnique = true)]
public partial class ChuongTruyen
{
    [Key]
    public int MaChuongTruyen { get; set; }

    public int MaTruyen { get; set; }

    public int ThuTuChuong { get; set; }

    [StringLength(255)]
    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDang { get; set; }

    [InverseProperty("MaChuongTruyenNavigation")]
    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    [InverseProperty("MaChuongTruyenNavigation")]
    public virtual ICollection<LichSuDoc> LichSuDocs { get; set; } = new List<LichSuDoc>();

    [ForeignKey("MaTruyen")]
    [InverseProperty("ChuongTruyens")]
    public virtual Truyen MaTruyenNavigation { get; set; } = null!;
}
