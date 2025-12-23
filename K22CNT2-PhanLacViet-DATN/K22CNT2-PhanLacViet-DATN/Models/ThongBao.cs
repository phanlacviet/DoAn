using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("ThongBao")]
public partial class ThongBao
{
    [Key]
    public int MaThongBao { get; set; }

    [StringLength(50)]
    public string? TaiKhoan { get; set; }

    public string? NoiDung { get; set; }

    public bool? DaDoc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    [ForeignKey("TaiKhoan")]
    [InverseProperty("ThongBaos")]
    public virtual ThanhVien? TaiKhoanNavigation { get; set; }
}
