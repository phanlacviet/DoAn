namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class ChuongTruyenDto
    {
        public int MaChuongTruyen { get; set; }

        public int MaTruyen { get; set; }

        public int ThuTuChuong { get; set; }
        public string? TieuDe { get; set; }

        public string? NoiDung { get; set; }
        public DateTime? NgayDang { get; set; }
    }
}
