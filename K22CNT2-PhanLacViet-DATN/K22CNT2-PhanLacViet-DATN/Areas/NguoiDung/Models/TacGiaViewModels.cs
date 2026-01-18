namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class TacGiaViewModels
    {
        public TacGiaStatsDto ThongKeChung { get; set; } = new TacGiaStatsDto();
        public List<TruyenDashboardDto> DanhSachTruyen { get; set; } = new List<TruyenDashboardDto>();
    }
    public class TacGiaStatsDto
    {
        public long TongLuotXem { get; set; }
        public int TongNguoiTheoDoi { get; set; }
        public int TongLuotLuu { get; set; }
        public double DiemDanhGiaTrungBinh { get; set; }
    }

    // DTO chi tiết từng truyện trong danh sách (Kế thừa hoặc mở rộng TruyenDto)
    public class TruyenDashboardDto
    {
        public int MaTruyen { get; set; }
        public string TenTruyen { get; set; } = string.Empty;
        public string? AnhBia { get; set; }
        public int SoChuong { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        // Chỉ số tương tác riêng của truyện này
        public int LuotTheoDoi { get; set; }
        public int LuotLuu { get; set; }
        public int LuotBinhLuan { get; set; }
        public double DiemDanhGia { get; set; }
        public int LuotDanhGia { get; set; } // Số lượng người đánh giá
        public long TongLuotXem { get; internal set; }
    }

    // DTO dùng cho biểu đồ (API trả về)
    public class ChartDataDto
    {
        public string? Label { get; set; } // T2, T3...
        public long Value { get; set; }   // Giá trị view
    }
}
