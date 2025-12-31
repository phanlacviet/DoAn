using K22CNT2_PhanLacViet_DATN.Models.Dtos;

namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class TimKiemViewModel
    {
        public string? Keyword { get; set; }
        public string? TacGia { get; set; }
        public string? MoTa { get; set; }
        public string Sort { get; set; } = "NgayDang_Desc";
        public List<int> SelectedGenreIds { get; set; } = new List<int>();
        public List<TruyenDto> KetQua { get; set; } = new List<TruyenDto>();
        public List<TheLoaiItem> AllTheLoais { get; set; } = new List<TheLoaiItem>();
    }

    public class TheLoaiItem
    {
        public int Id { get; set; }
        public string? Ten { get; set; }
    }
}