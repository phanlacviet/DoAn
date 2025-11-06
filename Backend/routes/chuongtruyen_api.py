from flask import Blueprint, jsonify, request
from Backend.database import get_connection

chuongtruyen_bp = Blueprint('chuongtruyen_bp', __name__)

#Lấy tất cả chương truyện
@chuongtruyen_bp.route('/', methods=['GET'])
def get_all_chuongtruyen():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaChuongTruyen, MaTruyen, ThuTuChuong, TieuDe, NgayDang FROM ChuongTruyen")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {"MaChuongTruyen": r[0], "MaTruyen": r[1], "ThuTuChuong": r[2], "TieuDe": r[3], "NgayDang": r[4]} 
        for r in rows
    ])

#Lấy chương truyện theo mã
@chuongtruyen_bp.route('/<int:idct>', methods=['GET'])
def get_chuongtruyen(idct):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT * FROM ChuongTruyen WHERE MaChuongTruyen=?", (idct,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaChuongTruyen": row[0], "MaTruyen": row[1],
        "ThuTuChuong": row[2], "TieuDe": row[3], "NgayDang": row[4]
    })

#Thêm chương truyện
@chuongtruyen_bp.route('/', methods=['POST'])
def add_chuongtruyen():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO ChuongTruyen (MaTruyen, ThuTuChuong, TieuDe)
        VALUES (?, ?, ?)
    """, (data['MaTruyen'], data['ThuTuChuong'], data['TieuDe']))
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm chương truyện thành công"}), 201

#Cập nhật chương truyện
@chuongtruyen_bp.route('/api/chuongtruyen/<int:id>', methods=['PUT'])
def update_chuongtruyen(idct):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        UPDATE ChuongTruyen SET NoiDung=?, TieuDe=? WHERE MaChuongTruyen=?
    """, (data['NoiDung'], data['TieuDe'], idct))
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa chương truyện
@chuongtruyen_bp.route('/<int:id>', methods=['DELETE'])
def delete_chuongtruyen(idct):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM ChuongTruyen WHERE MaChuongTruyen=?", (idct,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa chương truyện"})
