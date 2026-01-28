namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class DanhGiaDto
    {
        public string TaiKhoan { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public double Diem { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? NgayDanhGia { get; set; }
    }
}
