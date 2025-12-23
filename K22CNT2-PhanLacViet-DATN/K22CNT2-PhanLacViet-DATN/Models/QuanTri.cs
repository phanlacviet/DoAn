using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("QuanTri")]
public partial class QuanTri
{
    [Key]
    [Column("TaiKhoanQT")]
    [StringLength(50)]
    public string TaiKhoanQt { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;
}
