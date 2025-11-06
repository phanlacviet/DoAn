from flask import Blueprint, jsonify, request
from Backend.database import get_connection

thanhvien_bp = Blueprint('thanhvien_bp', __name__)

#  Lấy toàn bộ thành viên
@thanhvien_bp.route('/', methods=['GET'])
def get_all_thanhvien():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT TaiKhoan, MatKhau, Avatar, IsDeleted FROM ThanhVien")
    rows = cursor.fetchall()
    conn.close()
    result = []
    for r in rows:
        result.append({
            "TaiKhoan": r[0],
            "MatKhau": r[1],
            "Avatar": r[2],
            "IsDeleted": bool(r[3])
        })
    return jsonify(result)

#  Lấy 1 thành viên theo tài khoản
@thanhvien_bp.route('/<taikhoan>', methods=['GET'])
def get_thanhvien(taikhoan):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT TaiKhoan, MatKhau, Avatar, IsDeleted FROM ThanhVien WHERE TaiKhoan=?", (taikhoan,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "TaiKhoan": row[0],
        "MatKhau": row[1],
        "Avatar": row[2],
        "IsDeleted": bool(row[3])
    })

# Thêm mới thành viên
@thanhvien_bp.route('/', methods=['POST'])
def add_thanhvien():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute(
        "INSERT INTO ThanhVien (TaiKhoan, MatKhau, Avatar, IsDeleted) VALUES (?, ?, ?, ?)",
        (data['TaiKhoan'], data['MatKhau'], data.get('Avatar', None), data.get('IsDeleted', 0))
    )
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm thành viên thành công"}), 201

# Cập nhật thông tin
@thanhvien_bp.route('/<taikhoan>', methods=['PUT'])
def update_thanhvien(taikhoan):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute(
        "UPDATE ThanhVien SET MatKhau=?, Avatar=?, IsDeleted=? WHERE TaiKhoan=?",
        (data['MatKhau'], data['Avatar'], data['IsDeleted'], taikhoan)
    )
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa (cập nhật IsDeleted)
@thanhvien_bp.route('/<taikhoan>', methods=['DELETE'])
def delete_thanhvien(taikhoan):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("UPDATE ThanhVien SET IsDeleted=1 WHERE TaiKhoan=?", (taikhoan,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa (ẩn) thành viên"})
