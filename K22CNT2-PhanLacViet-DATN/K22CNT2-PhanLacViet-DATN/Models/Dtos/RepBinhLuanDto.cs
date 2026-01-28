namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class RepBinhLuanDto
    {
        public int MaRep { get; set; }
        public string? TaiKhoan { get; set; }
        public string? Avatar { get; set; }
        public string? NoiDung { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public DateTime? NgayGui { get; set; }
    }
}
