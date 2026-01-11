using K22CNT2_PhanLacViet_DATN.Models.Dtos;

namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models
{
    public class UserProfileViewModel
    {
        public ThanhVienDto ThongTinNguoiDung { get; set; } = new ThanhVienDto();
        public List<TruyenProfileItem> DsTheoDoi { get; set; } = new List<TruyenProfileItem>();
        public List<TruyenProfileItem> DsLichSu { get; set; } = new List<TruyenProfileItem>();
        public List<TruyenProfileItem> DsDaLuu { get; set; } = new List<TruyenProfileItem>();
    }
}
