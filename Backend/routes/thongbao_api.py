from flask import Blueprint, jsonify, request
from Backend.database import get_connection

thongbao_bp = Blueprint('thongbao_bp', __name__)

#Lấy tất cả thông báo
@thongbao_bp.route('/', methods=['GET'])
def get_all_thongbao():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaThongBao, NoiDung, NgayGui, TaiKhoan, DaDoc FROM ThongBao")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {"MaThongBao": r[0], "NoiDung": r[1], "NgayGui": r[2], "TaiKhoan": r[3], "DaDoc": bool(r[4])} for r in rows
    ])

#Lấy thông báo theo mã
@thongbao_bp.route('/<int:idtb>', methods=['GET'])
def get_thongbao(idtb):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT * FROM ThongBao WHERE MaThongBao=?", (idtb,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaThongBao": row[0], "NoiDung": row[1], "NgayGui": row[2], "TaiKhoan": row[3], "DaDoc": bool(row[4])
    })

#Thêm thông báo
@thongbao_bp.route('/', methods=['POST'])
def add_thongbao():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO ThongBao (NoiDung, TaiKhoan, DaDoc)
        VALUES (?, ?, ?)
    """, (data['NoiDung'], data['TaiKhoan'], data.get('DaDoc', 0)))
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm thông báo thành công"}), 201

#cập nhật thông báo
@thongbao_bp.route('/<int:idtb>', methods=['PUT'])
def update_thongbao(idtb):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        UPDATE ThongBao SET NoiDung=?, TaiKhoan=?, DaDoc=? WHERE MaThongBao=?
    """, (data['NoiDung'], data['TaiKhoan'], data['DaDoc'], idtb))
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa thông báo
@thongbao_bp.route('/<int:idtb>', methods=['DELETE'])
def delete_thongbao(idtb):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM ThongBao WHERE MaThongBao=?", (idtb,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa thông báo"})
