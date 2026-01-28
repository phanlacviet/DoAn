namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class ThemBinhLuanInput
    {
        public string? TaiKhoan { get; set; }
        public int MaChuongId { get; set; }
        public string? NoiDung { get; set; }
        public int? MaBinhLuanGoc { get; set; }
    }
    public class BinhLuanDto
    {
        public int MaBinhLuan { get; set; }
        public string? NoiDung { get; set; }
        public string? TaiKhoan { get; set; }
        public string? Avatar { get; set; }
        public string? TenChuong { get; set; }
        public DateTime? NgayGui { get; set; }
        public List<RepBinhLuanDto> RepBinhLuans { get; set; } = new List<RepBinhLuanDto>();
    }
}
