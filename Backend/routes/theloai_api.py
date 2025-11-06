from flask import Blueprint, jsonify, request
from Backend.database import get_connection

theloai_bp = Blueprint('theloai_bp', __name__)

#Lấy tất cả thể loại
@theloai_bp.route('/', methods=['GET'])
def get_all_theloai():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaTheLoai, TenTheLoai, IsDeleted FROM TheLoai")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([{"MaTheLoai": r[0], "TenTheLoai": r[1], "IsDeleted": bool(r[2])} for r in rows])

#Lấy thể loại theo mã
@theloai_bp.route('/<int:idtl>', methods=['GET'])
def get_theloai(idtl):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaTheLoai, TenTheLoai, IsDeleted FROM TheLoai WHERE MaTheLoai=?", (idtl,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({"MaTheLoai": row[0], "TenTheLoai": row[1], "IsDeleted": bool(row[2])})

#Thêm thể loại
@theloai_bp.route('/', methods=['POST'])
def add_theloai():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("INSERT INTO TheLoai (TenTheLoai, IsDeleted) VALUES (?, ?)", (data['TenTheLoai'], data.get('IsDeleted', 0)))
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm thể loại thành công"}), 201

#Cập nhật thể loại
@theloai_bp.route('/<int:idtl>', methods=['PUT'])
def update_theloai(idtl):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("UPDATE TheLoai SET TenTheLoai=?, IsDeleted=? WHERE MaTheLoai=?", (data['TenTheLoai'], data['IsDeleted'], idtl))
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa thể loại
@theloai_bp.route('/<int:idtl>', methods=['DELETE'])
def delete_theloai(idtl):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("UPDATE TheLoai SET IsDeleted=1 WHERE MaTheLoai=?", (idtl,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã ẩn thể loại"})
