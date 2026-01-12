using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class TheoDoi
{
    public string TaiKhoan { get; set; } = null!;

    public int MaTruyen { get; set; }

    public DateTime? NgayTheoDoi { get; set; }

    public virtual Truyen MaTruyenNavigation { get; set; } = null!;

    public virtual ThanhVien TaiKhoanNavigation { get; set; } = null!;
}
