from flask import Blueprint, jsonify, request
from Backend.database import get_connection

danhgia_bp = Blueprint('danhgia_bp', __name__)

#Lấy tất cả đánh giá
@danhgia_bp.route('/', methods=['GET'])
def get_all_danhgia():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaDanhGia, NoiDung, Diem, NgayDanhGia, TaiKhoan, MaTruyen FROM DanhGia")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {"MaDanhGia": r[0], "NoiDung": r[1], "Diem": r[2], "NgayDanhGia": r[3], "TaiKhoan": r[4], "MaTruyen": r[5]} for r in rows
    ])

#thêm mã đánh giá
@danhgia_bp.route('/', methods=['POST'])
def add_or_update_danhgia():
    data = request.json
    tai_khoan = data.get('TaiKhoan')
    ma_truyen = data.get('MaTruyen')
    diem = data.get('Diem')
    noi_dung = data.get('NoiDung', '')

    conn = get_connection()
    cursor = conn.cursor()
    # Kiểm tra đã tồn tại đánh giá của user cho truyện chưa
    cursor.execute("SELECT MaDanhGia FROM DanhGia WHERE TaiKhoan=? AND MaTruyen=?", (tai_khoan, ma_truyen))
    row = cursor.fetchone()
    if row:
        # Cập nhật đánh giá hiện có
        cursor.execute("UPDATE DanhGia SET NoiDung=?, Diem=?, NgayDanhGia=GETDATE() WHERE MaDanhGia=?", (noi_dung, diem, row[0]))
        conn.commit()
        conn.close()
        return jsonify({"message": "Cập nhật đánh giá thành công"}), 200
    else:
        # Thêm mới
        cursor.execute("INSERT INTO DanhGia (NoiDung, Diem, TaiKhoan, MaTruyen) VALUES (?, ?, ?, ?)",
                       (noi_dung, diem, tai_khoan, ma_truyen))
        conn.commit()
        conn.close()
        return jsonify({"message": "Đánh giá thành công"}), 201

#Xóa mã đánh giá
@danhgia_bp.route('/<int:iddg>', methods=['DELETE'])
def delete_danhgia(iddg):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM DanhGia WHERE MaDanhGia=?", (iddg,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa đánh giá"})
@danhgia_bp.route('/avg/<int:idtruyen>', methods=['GET'])
def get_danhgia_avg(idtruyen):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT AVG(Diem), COUNT(*) FROM DanhGia WHERE MaTruyen=?", (idtruyen,))
    row = cursor.fetchone()
    conn.close()
    avg = row[0] if row and row[0] is not None else 0
    count = int(row[1]) if row and row[1] is not None else 0
    avg = round(float(avg), 1) if count > 0 else 0
    return jsonify({"avg": avg, "count": count})