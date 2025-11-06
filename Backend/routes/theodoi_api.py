from flask import Blueprint, jsonify, request
from Backend.database import get_connection

theodoi_bp = Blueprint('theodoi_bp', __name__)

#Lấy tất cả theo dõi
@theodoi_bp.route('/', methods=['GET'])
def get_all_theodoi():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT IDTheoDoi, TaiKhoan, MaTruyen FROM TheoDoi")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {"IDTheoDoi": r[0], "TaiKhoan": r[1], "MaTruyen": r[2]} for r in rows
    ])

#Thêm theo dõi
@theodoi_bp.route('/', methods=['POST'])
def add_theodoi():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("INSERT INTO TheoDoi (TaiKhoan, MaTruyen) VALUES (?, ?)", (data['TaiKhoan'], data['MaTruyen']))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã theo dõi truyện"}), 201

#xóa theo dõi
@theodoi_bp.route('/<int:idtd>', methods=['DELETE'])
def delete_theodoi(idtd):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM TheoDoi WHERE IDTheoDoi=?", (idtd,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Bỏ theo dõi thành công"})
