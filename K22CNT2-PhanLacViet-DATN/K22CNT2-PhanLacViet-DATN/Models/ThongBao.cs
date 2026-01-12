using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class ThongBao
{
    public int MaThongBao { get; set; }

    public string? TaiKhoan { get; set; }

    public string? NoiDung { get; set; }

    public bool? DaDoc { get; set; }

    public DateTime? NgayGui { get; set; }

    public virtual ThanhVien? TaiKhoanNavigation { get; set; }
}
