using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class ThanhVien
{
    public string TaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? Avatar { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<LichSuDoc> LichSuDocs { get; set; } = new List<LichSuDoc>();

    public virtual ICollection<LuuTruyen> LuuTruyens { get; set; } = new List<LuuTruyen>();

    public virtual ICollection<RepBinhLuan> RepBinhLuans { get; set; } = new List<RepBinhLuan>();

    public virtual ICollection<TheoDoi> TheoDois { get; set; } = new List<TheoDoi>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    public virtual ICollection<Truyen> Truyens { get; set; } = new List<Truyen>();
}
