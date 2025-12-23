using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("LuotXemTruyen")]
[Index("MaTruyen", "Ngay", Name = "UQ_LuotXem", IsUnique = true)]
public partial class LuotXemTruyen
{
    [Key]
    public int MaLuotXem { get; set; }

    public int? MaTruyen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ngay { get; set; }

    public int? SoLuotXem { get; set; }

    [ForeignKey("MaTruyen")]
    [InverseProperty("LuotXemTruyens")]
    public virtual Truyen? MaTruyenNavigation { get; set; }
}
