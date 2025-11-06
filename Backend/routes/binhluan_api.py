from flask import Blueprint, jsonify, request
from Backend.database import get_connection

binhluan_bp = Blueprint('binhluan_bp', __name__)

#lấy tất cả bình luận
@binhluan_bp.route('/', methods=['GET'])
def get_all_binhluan():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaBinhLuan, MaChuongTruyen, TaiKhoan, NoiDung, NgayGui FROM BinhLuan")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {"MaBinhLuan": r[0], "MaChuongTruyen": r[1], "TaiKhoan": r[2], "NoiDung": r[3], "NgayGui": r[4]} for r in rows
    ])

#Lấy bình luận theo mã
@binhluan_bp.route('/<int:id>', methods=['GET'])
def get_binhluan(idbl):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT * FROM BinhLuan WHERE MaBinhLuan=?", (idbl,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaBinhLuan": row[0], "MaChuongTruyen": row[1], "TaiKhoan": row[2], "NoiDung": row[3], "NgayGui": row[4]
    })

#Thêm bình luận
@binhluan_bp.route('/', methods=['POST'])
def add_binhluan():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO BinhLuan (MaChuongTruyen, TaiKhoan, NoiDung)
        VALUES (?, ?, ?)
    """, (data['MaChuongTruyen'], data['TaiKhoan'], data['NoiDung']))
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm bình luận thành công"}), 201

#Cập nhật bình luận
@binhluan_bp.route('/<int:id>', methods=['PUT'])
def update_binhluan(idbl):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        UPDATE BinhLuan SET MaChuongTruyen=?, TaiKhoan=?, NoiDung=? WHERE MaBinhLuan=?
    """, (data['MaChuongTruyen'], data['TaiKhoan'], data['NoiDung'], idbl))
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa bình luận
@binhluan_bp.route('/api/binhluan/<int:id>', methods=['DELETE'])
def delete_binhluan(idbl):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM BinhLuan WHERE MaBinhLuan=?", (idbl,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa bình luận"})
