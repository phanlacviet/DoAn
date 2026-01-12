using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class LuotXemTruyen
{
    public int MaLuotXem { get; set; }

    public int? MaTruyen { get; set; }

    public DateTime? Ngay { get; set; }

    public int? SoLuotXem { get; set; }

    public virtual Truyen? MaTruyenNavigation { get; set; }
}
