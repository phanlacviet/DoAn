using K22CNT2_PhanLacViet_DATN.Models;
using K22CNT2_PhanLacViet_DATN.Models.Dtos;
namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class TrangChuViewModel
    {
        public List<TruyenDto> DanhSachTruyen { get; set; } = new();
        public List<LichSuDocDto> LichSu { get; set; } = new();
        public List<TruyenDto> DaLuu { get; set; } = new();
    }

    public class LichSuDoc
    {
        public int MaTruyen { get; set; }
        public string? TenTruyen { get; set; }
        public int ThuTuChuong { get; set; }
        public string? AnhBia { get; set; }
    }
}
