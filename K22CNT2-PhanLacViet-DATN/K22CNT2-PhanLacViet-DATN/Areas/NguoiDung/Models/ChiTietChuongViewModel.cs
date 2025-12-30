using K22CNT2_PhanLacViet_DATN.Models.Dtos;

namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class ChiTietChuongViewModel
    {
        public ChuongTruyenDto ChuongHienTai { get; set; } = default!;
        public int? MaChuongTruoc { get; set; } 
        public int? MaChuongSau { get; set; }   
        public int MaTruyen { get; set; }
        public List<BinhLuanDto> DanhSachBinhLuan { get; set; } = new List<BinhLuanDto>();
        public int ThuTuChuongDaDoc { get; set; } = 0;
    }
}