using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class ChuongTruyen
{
    public int MaChuongTruyen { get; set; }

    public int MaTruyen { get; set; }

    public int ThuTuChuong { get; set; }

    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayDang { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<LichSuDoc> LichSuDocs { get; set; } = new List<LichSuDoc>();

    public virtual Truyen MaTruyenNavigation { get; set; } = null!;
}
