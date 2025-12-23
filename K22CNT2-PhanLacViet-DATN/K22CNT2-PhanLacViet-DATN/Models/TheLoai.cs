using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

[Table("TheLoai")]
public partial class TheLoai
{
    [Key]
    public int MaTheLoai { get; set; }

    [StringLength(100)]
    public string TenTheLoai { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    [ForeignKey("MaTheLoai")]
    [InverseProperty("MaTheLoais")]
    public virtual ICollection<Truyen> MaTruyens { get; set; } = new List<Truyen>();
}
