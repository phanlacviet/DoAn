namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class BinhLuanDto
    {
        public int MaBinhLuan { get; set; }
        public string? NoiDung { get; set; }
        public string? TaiKhoan { get; set; }
        public DateTime? NgayGui { get; set; }
        public List<RepBinhLuanDto> RepBinhLuans { get; set; } = new List<RepBinhLuanDto>();
    }
}
