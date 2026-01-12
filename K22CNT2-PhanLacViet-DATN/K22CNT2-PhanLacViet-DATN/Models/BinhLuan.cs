using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class BinhLuan
{
    public int MaBinhLuan { get; set; }

    public int? MaChuongTruyen { get; set; }

    public string? TaiKhoan { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayGui { get; set; }

    public virtual ChuongTruyen? MaChuongTruyenNavigation { get; set; }

    public virtual ICollection<RepBinhLuan> RepBinhLuans { get; set; } = new List<RepBinhLuan>();

    public virtual ThanhVien? TaiKhoanNavigation { get; set; }
}
