using System;

namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class TruyenDto
    {
        public int MaTruyen { get; set; }
        public string? TenTruyen { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? TacGia { get; set; }
        public string? LoaiTruyen { get; set; }
        public string? TenTheLoai { get; set; }
        public int SoChuong { get; set; }
        public long TongLuotXem { get; set; }
        public DateTime? NgayDang { get; set; }
    }
}