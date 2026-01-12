using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class DanhGium
{
    public string TaiKhoan { get; set; } = null!;

    public int MaTruyen { get; set; }

    public int? Diem { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
