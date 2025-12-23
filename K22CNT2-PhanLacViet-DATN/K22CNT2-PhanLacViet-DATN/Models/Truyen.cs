using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("Truyen")]
public partial class Truyen
{
    [Key]
    public int MaTruyen { get; set; }

    [StringLength(255)]
    public string TenTruyen { get; set; } = null!;

    public string? MoTa { get; set; }

    [StringLength(255)]
    public string? TacGia { get; set; }

    [StringLength(50)]
    public string? NguoiDang { get; set; }

    [StringLength(50)]
    public string? LoaiTruyen { get; set; }

    public int? SoChuong { get; set; }

    public long? TongLuotXem { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDang { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    public bool? IsDeleted { get; set; }

    [InverseProperty("MaTruyenNavigation")]
    public virtual ICollection<ChuongTruyen> ChuongTruyens { get; set; } = new List<ChuongTruyen>();

    [InverseProperty("MaTruyenNavigation")]
    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    [InverseProperty("MaTruyenNavigation")]
    public virtual ICollection<LichSuDoc> LichSuDocs { get; set; } = new List<LichSuDoc>();

    [InverseProperty("MaTruyenNavigation")]
    public virtual ICollection<LuotXemTruyen> LuotXemTruyens { get; set; } = new List<LuotXemTruyen>();

    [InverseProperty("MaTruyenNavigation")]
    public virtual ICollection<LuuTruyen> LuuTruyens { get; set; } = new List<LuuTruyen>();

    [ForeignKey("NguoiDang")]
    [InverseProperty("Truyens")]
    public virtual ThanhVien? NguoiDangNavigation { get; set; }

    [InverseProperty("MaTruyenNavigation")]
    public virtual ICollection<TheoDoi> TheoDois { get; set; } = new List<TheoDoi>();

    [ForeignKey("MaTruyen")]
    [InverseProperty("MaTruyens")]
    public virtual ICollection<TheLoai> MaTheLoais { get; set; } = new List<TheLoai>();
}
