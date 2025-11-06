from flask import Blueprint, jsonify, request
from Backend.database import get_connection

truyen_bp = Blueprint('truyen_bp', __name__)

#Lấy tất cả truyện
@truyen_bp.route('/', methods=['GET'])
def get_all_truyen():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaTruyen, TenTruyen, MoTa, MaTheLoai, SoChuong, TacGia, NguoiDang, LoaiTruyen, NgayDang, NgayCapNhat, IsDeleted FROM Truyen")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {
            "MaTruyen": r[0],
            "TenTruyen": r[1],
            "MoTa": r[2],
            "MaTheLoai": r[3],
            "SoChuong": r[4],
            "TacGia": r[5],
            "NguoiDang": r[6],
            "LoaiTruyen": r[7],
            "NgayDang": r[8],
            "NgayCapNhat": r[9],
            "IsDeleted": bool(r[10])
        } for r in rows
    ])

#Lấy truyện theo mã
@truyen_bp.route('/<int:idtruyen>', methods=['GET'])
def get_truyen(idtruyen):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT * FROM Truyen WHERE MaTruyen=?", (idtruyen,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaTruyen": row[0],
        "TenTruyen": row[1],
        "MoTa": row[2],
        "MaTheLoai": row[3],
        "SoChuong": row[4],
        "TacGia": row[5],
        "NguoiDang": row[6],
        "LoaiTruyen": row[7],
        "NgayDang": row[8],
        "NgayCapNhat": row[9],
        "IsDeleted": bool(row[10])
    })

#Thêm truyện
@truyen_bp.route('/', methods=['POST'])
def add_truyen():
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO Truyen (TenTruyen, MoTa, MaTheLoai, SoChuong, TacGia, NguoiDang, LoaiTruyen, NgayCapNhat, IsDeleted)
        VALUES (?, ?, ?, ?, ?, ?, ?, GETDATE(), ?)
    """, (
        data['TenTruyen'], data.get('MoTa', ''), data.get('MaTheLoai', None),
        data.get('SoChuong', 0), data.get('TacGia', ''), data.get('NguoiDang', ''),
        data.get('LoaiTruyen', ''), data.get('IsDeleted', 0)
    ))
    conn.commit()
    conn.close()
    return jsonify({"message": "Thêm truyện thành công"}), 201

#Cập nhật truyện
@truyen_bp.route('/<int:idtruyen>', methods=['PUT'])
def update_truyen(idtruyen):
    data = request.json
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        UPDATE Truyen SET TenTruyen=?, MoTa=?, MaTheLoai=?, SoChuong=?, TacGia=?, NguoiDang=?, LoaiTruyen=?, NgayCapNhat=GETDATE(), IsDeleted=?
        WHERE MaTruyen=?
    """, (
        data['TenTruyen'], data['MoTa'], data['MaTheLoai'], data['SoChuong'],
        data['TacGia'], data['NguoiDang'], data['LoaiTruyen'], data['IsDeleted'], idtruyen
    ))
    conn.commit()
    conn.close()
    return jsonify({"message": "Cập nhật thành công"})

#Xóa truyện
@truyen_bp.route('/<int:idtruyen>', methods=['DELETE'])
def delete_truyen(idtruyen):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("UPDATE Truyen SET IsDeleted=1 WHERE MaTruyen=?", (idtruyen,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã ẩn truyện"})
