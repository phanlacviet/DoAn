using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class LuuTruyen
{
    public string TaiKhoan { get; set; } = null!;

    public int MaTruyen { get; set; }

    public DateTime? NgayLuu { get; set; }

    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
