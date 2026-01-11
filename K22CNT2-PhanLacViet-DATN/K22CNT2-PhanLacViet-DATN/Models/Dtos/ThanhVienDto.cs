namespace K22CNT2_PhanLacViet_DATN.Models.Dtos
{
    public class ThanhVienDto
    {
        public string TaiKhoan { get; set; } = null!;
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string NgayThamGia { get; set; } = "";

        public int SoTruyenDaDoc { get; set; }
        public int SoChuongDaDoc { get; set; }
        public int SoDanhGia { get; set; }
        public int SoBinhLuan { get; set; }
    }
    public class TruyenProfileItem
    {
        public int MaTruyen { get; set; }
        public string TenTruyen { get; set; } = "";
        public string AnhBia { get; set; } = "";
        public string? TacGia { get; set; }
        public long LuotXem { get; set; }
        public string ThoiGian { get; set; } = "";
        public string? TienDo { get; set; }
        public bool CoChuongMoi { get; set; } = false;
        public string LoaiDanhSach { get; set; } = "";
    }

}
