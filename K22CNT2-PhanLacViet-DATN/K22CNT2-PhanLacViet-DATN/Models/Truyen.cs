using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class Truyen
{
    public int MaTruyen { get; set; }

    public string TenTruyen { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? TacGia { get; set; }

    public string? NguoiDang { get; set; }

    public string? LoaiTruyen { get; set; }

    public int? SoChuong { get; set; }

    public long? TongLuotXem { get; set; }

    public DateTime? NgayDang { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public bool? IsDeleted { get; set; }

    public string? AnhBia { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<ChuongTruyen> ChuongTruyens { get; set; } = new List<ChuongTruyen>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<LichSuDoc> LichSuDocs { get; set; } = new List<LichSuDoc>();

    public virtual ICollection<LuotXemTruyen> LuotXemTruyens { get; set; } = new List<LuotXemTruyen>();

    public virtual ICollection<LuuTruyen> LuuTruyens { get; set; } = new List<LuuTruyen>();

    public virtual ThanhVien? NguoiDangNavigation { get; set; }

    public virtual ICollection<TheoDoi> TheoDois { get; set; } = new List<TheoDoi>();

    public virtual ICollection<TheLoai> MaTheLoais { get; set; } = new List<TheLoai>();
}
