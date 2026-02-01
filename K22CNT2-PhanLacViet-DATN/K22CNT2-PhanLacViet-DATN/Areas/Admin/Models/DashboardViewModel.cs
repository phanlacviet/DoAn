using System.Collections.Generic;
namespace K22CNT2_PhanLacViet_DATN.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TongThanhVien { get; set; }
        public int TongTruyen { get; set; }
        public int TongChuong { get; set; }
        public int LuotXemHomNay { get; set; } 
        public int BinhLuan24h { get; set; }
        public int ThanhVienMoi7Ngay { get; set; }

        public string[] ChartLabels { get; set; } = Array.Empty<string>();
        public int[] ChartData { get; set; } = Array.Empty<int>();

        public List<K22CNT2_PhanLacViet_DATN.Models.Truyen> TopTruyenXemNhieu { get; set; } = new();
        public List<K22CNT2_PhanLacViet_DATN.Models.Truyen> TruyenMoiCapNhat { get; set; } = new();
    }
}
