from flask import Blueprint, jsonify, request
from Backend.database import get_connection

luutruyen_bp = Blueprint('luutruyen_bp', __name__)

#  Lấy toàn bộ lưu truyện
@luutruyen_bp.route('/', methods=['GET'])
def get_all_luutruyen():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT IDLuuTruyen, TaiKhoan, MaTruyen FROM LuuTruyen")
    rows = cursor.fetchall()
    conn.close()
    result = []
    for r in rows:
        result.append({
            "IDLuuTruyen": r[0],
            "TaiKhoan": r[1],
            "MaTruyen": r[2]
        })
    return jsonify(result)

#  Lấy lưu truyện theo ID
@luutruyen_bp.route('/<int:id_luutruyen>', methods=['GET'])
def get_luutruyen(id_luutruyen):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT IDLuuTruyen, TaiKhoan, MaTruyen FROM LuuTruyen WHERE IDLuuTruyen=?", (id_luutruyen,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy lưu truyện"}), 404
    return jsonify({
        "IDLuuTruyen": row[0],
        "TaiKhoan": row[1],
        "MaTruyen": row[2]
    })

#  Thêm lưu truyện
@luutruyen_bp.route('/', methods=['POST'])
def add_luutruyen():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute(
        "INSERT INTO LuuTruyen (TaiKhoan, MaTruyen) VALUES (?, ?)",
        (data['TaiKhoan'], data['MaTruyen'])
    )
    conn.commit()
    conn.close()
    return jsonify({"message": "Lưu truyện thành công"}), 201

#  Cập nhật lưu truyện
@luutruyen_bp.route('/<int:id_luutruyen>', methods=['PUT'])
def update_luutruyen(id_luutruyen):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute(
        "UPDATE LuuTruyen SET TaiKhoan=?, MaTruyen=? WHERE IDLuuTruyen=?",
        (data['TaiKhoan'], data['MaTruyen'], id_luutruyen)
    )
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật lưu truyện thành công"})

# Xóa lưu truyện
@luutruyen_bp.route('/<int:id_luutruyen>', methods=['DELETE'])
def delete_luutruyen(id_luutruyen):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM LuuTruyen WHERE IDLuuTruyen=?", (id_luutruyen,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Xóa lưu truyện thành công"})
