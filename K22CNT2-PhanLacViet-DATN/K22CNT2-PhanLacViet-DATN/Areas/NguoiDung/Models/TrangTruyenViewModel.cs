using K22CNT2_PhanLacViet_DATN.Models;
using K22CNT2_PhanLacViet_DATN.Models.Dtos;
namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class TrangTruyenViewModel
    {
        public TruyenDto ThongTinTruyen { get; set; } = new TruyenDto();
        public List<ChuongTruyenDto> DanhSachChuong { get; set; } = new List<ChuongTruyenDto>();

        // Danh sách bình luận (lồng rep bên trong)
        public List<BinhLuanDto> DanhSachBinhLuan { get; set; } = new List<BinhLuanDto>();

        // Các thông tin tính toán thêm
        public int MaChuongDau { get; set; }
        public int MaChuongDocTiep { get; set; }
        public string? TrangThaiDoc { get; set; }
        public double DiemDanhGiaTrungBinh { get; set; } = 0;
        public bool DaTheoDoi { get; set; } = false; // Check nếu user đã login
        public bool DaDanhGia { get; set; } = false;
        public int? ThuTuChuongDaDoc { get; set; }
    }
}
