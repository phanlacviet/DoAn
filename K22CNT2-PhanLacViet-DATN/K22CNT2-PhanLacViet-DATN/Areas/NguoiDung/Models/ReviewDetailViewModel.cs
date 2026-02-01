using K22CNT2_PhanLacViet_DATN.Models;

namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class ReviewDetailViewModel
    {
        public Truyen Truyen { get; set; } = default!;
        public List<DanhGium> DanhGias { get; set; } = new List<DanhGium>();
        public double DiemTrungBinh { get; set; }
        public int TongSoDanhGia { get; set; }
    }
}
