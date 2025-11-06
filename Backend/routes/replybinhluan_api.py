from flask import Blueprint, jsonify, request
from Backend.database import get_connection

reply_binhluan_bp = Blueprint('reply_binhluan_bp', __name__)

#Lấy tất cả bình luận rep
@reply_binhluan_bp.route('/', methods=['GET'])
def get_all_reply():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaRepBinhLuan, TaiKhoan, NoiDung, NgayGui FROM RepBinhLuan")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {"MaRepBinhLuan": r[0], "TaiKhoan": r[1], "NoiDung": r[2], "NgayGui": r[3]} for r in rows
    ])

#Lấy bình luận rep theo mã
@reply_binhluan_bp.route('/<int:idrbl>', methods=['GET'])
def get_reply(idrbl):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT * FROM RepBinhLuan WHERE MaRepBinhLuan=?", (idrbl,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaRepBinhLuan": row[0], "TaiKhoan": row[1], "NoiDung": row[2], "NgayGui": row[3]
    })

#Thêm bình luận rep
@reply_binhluan_bp.route('/', methods=['POST'])
def add_reply():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("INSERT INTO RepBinhLuan (TaiKhoan, NoiDung) VALUES (?, ?)", (data['TaiKhoan'], data['NoiDung']))
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm phản hồi bình luận thành công"}), 201

#cập nhật bình luận
@reply_binhluan_bp.route('/<int:idrbl>', methods=['PUT'])
def update_reply(idrbl):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("UPDATE RepBinhLuan SET TaiKhoan=?, NoiDung=? WHERE MaRepBinhLuan=?", (data['TaiKhoan'], data['NoiDung'], idrbl))
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa bình luận rep
@reply_binhluan_bp.route('/<int:id>', methods=['DELETE'])
def delete_reply(idrbl):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM RepBinhLuan WHERE MaRepBinhLuan=?", (idrbl,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa phản hồi"})
