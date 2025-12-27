namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class TuongTacDto
    {
        public string TaiKhoan { get; set; } = null!;
        public int MaTruyen { get; set; }
    }

    public class DanhGiaInputDto : TuongTacDto
    {
        public int Diem { get; set; }
        public string? NoiDung { get; set; }
    }
}
