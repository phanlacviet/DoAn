using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Models;

public partial class WebTruyenChuContext : DbContext
{
    public WebTruyenChuContext()
    {
    }

    public WebTruyenChuContext(DbContextOptions<WebTruyenChuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<ChuongTruyen> ChuongTruyens { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<LichSuDoc> LichSuDocs { get; set; }

    public virtual DbSet<LuotXemTruyen> LuotXemTruyens { get; set; }

    public virtual DbSet<LuuTruyen> LuuTruyens { get; set; }

    public virtual DbSet<QuanTri> QuanTris { get; set; }

    public virtual DbSet<RepBinhLuan> RepBinhLuans { get; set; }

    public virtual DbSet<ThanhVien> ThanhViens { get; set; }

    public virtual DbSet<TheLoai> TheLoais { get; set; }

    public virtual DbSet<TheoDoi> TheoDois { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<Truyen> Truyens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-Q6N4J46\\SQLEXPRESS;Database=WebTruyenChu;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BinhLuan__87CB66A0396CF967");

            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaChuongTruyenNavigation).WithMany(p => p.BinhLuans).HasConstraintName("FK_BL_Chuong");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.BinhLuans).HasConstraintName("FK_BL_TV");
        });

        modelBuilder.Entity<ChuongTruyen>(entity =>
        {
            entity.HasKey(e => e.MaChuongTruyen).HasName("PK__ChuongTr__4309739657EC6C2B");

            entity.ToTable("ChuongTruyen", tb => tb.HasTrigger("trg_UpdateTruyen"));

            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTruyenNavigation).WithMany(p => p.ChuongTruyens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chuong_Truyen");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => new { e.TaiKhoan, e.MaTruyen }).HasName("PK__DanhGia__7315E755B77132C8");

            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTruyenNavigation).WithMany(p => p.DanhGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DG_Truyen");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.DanhGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DG_TV");
        });

        modelBuilder.Entity<LichSuDoc>(entity =>
        {
            entity.HasKey(e => new { e.TaiKhoan, e.MaTruyen }).HasName("PK__LichSuDo__7315E75548528530");

            entity.Property(e => e.NgayDoc).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaChuongTruyenNavigation).WithMany(p => p.LichSuDocs).HasConstraintName("FK_LSD_Chuong");

            entity.HasOne(d => d.MaTruyenNavigation).WithMany(p => p.LichSuDocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LSD_Truyen");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.LichSuDocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LSD_TV");
        });

        modelBuilder.Entity<LuotXemTruyen>(entity =>
        {
            entity.HasKey(e => e.MaLuotXem).HasName("PK__LuotXemT__C7725C781E43788B");

            entity.Property(e => e.SoLuotXem).HasDefaultValue(0);

            entity.HasOne(d => d.MaTruyenNavigation).WithMany(p => p.LuotXemTruyens).HasConstraintName("FK_LuotXem_Truyen");
        });

        modelBuilder.Entity<LuuTruyen>(entity =>
        {
            entity.HasKey(e => new { e.TaiKhoan, e.MaTruyen }).HasName("PK__LuuTruye__7315E7552547880E");

            entity.Property(e => e.NgayLuu).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTruyenNavigation).WithMany(p => p.LuuTruyens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Luu_Truyen");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.LuuTruyens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Luu_TV");
        });

        modelBuilder.Entity<QuanTri>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanQt).HasName("PK__QuanTri__9A120A5F1706C6AC");
        });

        modelBuilder.Entity<RepBinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaRep).HasName("PK__RepBinhL__396140FA2A219C9C");

            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaBinhLuanNavigation).WithMany(p => p.RepBinhLuans).HasConstraintName("FK_Rep_BL");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.RepBinhLuans).HasConstraintName("FK_Rep_TV");
        });

        modelBuilder.Entity<ThanhVien>(entity =>
        {
            entity.HasKey(e => e.TaiKhoan).HasName("PK__ThanhVie__D5B8C7F1CA70B5BD");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TheLoai>(entity =>
        {
            entity.HasKey(e => e.MaTheLoai).HasName("PK__TheLoai__D73FF34A59DF4ED2");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<TheoDoi>(entity =>
        {
            entity.HasKey(e => new { e.TaiKhoan, e.MaTruyen }).HasName("PK__TheoDoi__7315E7551536F9E0");

            entity.Property(e => e.NgayTheoDoi).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTruyenNavigation).WithMany(p => p.TheoDois)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TheoDoi_Truyen");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.TheoDois)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TheoDoi_TV");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__ThongBao__04DEB54E577951DE");

            entity.Property(e => e.DaDoc).HasDefaultValue(false);
            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TaiKhoanNavigation).WithMany(p => p.ThongBaos).HasConstraintName("FK_TB_TV");
        });

        modelBuilder.Entity<Truyen>(entity =>
        {
            entity.HasKey(e => e.MaTruyen).HasName("PK__Truyen__6AD20A4B65616B8D");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoChuong).HasDefaultValue(0);
            entity.Property(e => e.TongLuotXem).HasDefaultValue(0L);

            entity.HasOne(d => d.NguoiDangNavigation).WithMany(p => p.Truyens).HasConstraintName("FK_Truyen_ThanhVien");

            entity.HasMany(d => d.MaTheLoais).WithMany(p => p.MaTruyens)
                .UsingEntity<Dictionary<string, object>>(
                    "TruyenTheLoai",
                    r => r.HasOne<TheLoai>().WithMany()
                        .HasForeignKey("MaTheLoai")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TTL_TheLoai"),
                    l => l.HasOne<Truyen>().WithMany()
                        .HasForeignKey("MaTruyen")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TTL_Truyen"),
                    j =>
                    {
                        j.HasKey("MaTruyen", "MaTheLoai").HasName("PK__TruyenTh__D7A1F57F265A2F8A");
                        j.ToTable("TruyenTheLoai");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
