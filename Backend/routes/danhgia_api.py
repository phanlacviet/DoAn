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
def add_danhgia():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("INSERT INTO DanhGia (NoiDung, Diem, TaiKhoan, MaTruyen) VALUES (?, ?, ?, ?)",
                   (data['NoiDung'], data['Diem'], data['TaiKhoan'], data['MaTruyen']))
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
