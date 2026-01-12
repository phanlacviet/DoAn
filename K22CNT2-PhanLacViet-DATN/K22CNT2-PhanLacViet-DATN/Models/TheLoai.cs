using System;
using System.Collections.Generic;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class TheLoai
{
    public int MaTheLoai { get; set; }

    public string TenTheLoai { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Truyen> MaTruyens { get; set; } = new List<Truyen>();
}
