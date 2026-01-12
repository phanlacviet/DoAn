using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class RepBinhLuan
{
    public int MaRep { get; set; }

    public int? MaBinhLuan { get; set; }

    public string? TaiKhoan { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayGui { get; set; }

    public virtual BinhLuan? MaBinhLuanNavigation { get; set; }

    public virtual ThanhVien? TaiKhoanNavigation { get; set; }
}
