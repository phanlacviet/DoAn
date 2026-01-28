namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class TuongTacDto
    {
        public string TaiKhoan { get; set; } = null!;
        public int MaTruyen { get; set; }
        public string? Avatar { get; set; }
        public string NgayThucHien { get; set; } = string.Empty;
    }

    public class DanhGiaInputDto : TuongTacDto
    {
        public int Diem { get; set; }
        public string? NoiDung { get; set; }
    }
}
