from flask import Blueprint, jsonify, request
from Backend.database import get_connection

chuongtruyen_bp = Blueprint('chuongtruyen_bp', __name__)

# Lấy tất cả chương truyện
@chuongtruyen_bp.route('/', methods=['GET'])
def get_all_chuongtruyen():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaChuongTruyen, MaTruyen, ThuTuChuong, TieuDe, NgayDang, NoiDung FROM ChuongTruyen")
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {
            "MaChuongTruyen": r[0],
            "MaTruyen": r[1],
            "ThuTuChuong": r[2],
            "TieuDe": r[3],
            "NgayDang": r[4],
            "NoiDung": r[5]
        } for r in rows
    ])

# Lấy chương truyện theo mã (MaChuongTruyen)
@chuongtruyen_bp.route('/<int:idct>', methods=['GET'])
def get_chuongtruyen(idct):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaChuongTruyen, MaTruyen, ThuTuChuong, TieuDe, NgayDang, NoiDung FROM ChuongTruyen WHERE MaChuongTruyen=?", (idct,))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaChuongTruyen": row[0],
        "MaTruyen": row[1],
        "ThuTuChuong": row[2],
        "TieuDe": row[3],
        "NgayDang": row[4],
        "NoiDung": row[5]
    })

# Lấy tất cả chương của 1 truyện theo MaTruyen
@chuongtruyen_bp.route('/truyen/<int:matruyen>', methods=['GET'])
def get_chuong_by_truyen(matruyen):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaChuongTruyen, MaTruyen, ThuTuChuong, TieuDe, NgayDang, NoiDung FROM ChuongTruyen WHERE MaTruyen=? ORDER BY ThuTuChuong", (matruyen,))
    rows = cursor.fetchall()
    conn.close()
    return jsonify([
        {
            "MaChuongTruyen": r[0],
            "MaTruyen": r[1],
            "ThuTuChuong": r[2],
            "TieuDe": r[3],
            "NgayDang": r[4],
            "NoiDung": r[5]
        } for r in rows
    ])

# Lấy chương theo mã truyện và thứ tự chương (nếu cần)
@chuongtruyen_bp.route('/truyen/<int:matruyen>/thuTu/<int:thutu>', methods=['GET'])
def get_chuong_by_truyen_and_thutu(matruyen, thutu):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT MaChuongTruyen, MaTruyen, ThuTuChuong, TieuDe, NgayDang, NoiDung FROM ChuongTruyen WHERE MaTruyen=? AND ThuTuChuong=?", (matruyen, thutu))
    row = cursor.fetchone()
    conn.close()
    if not row:
        return jsonify({"message": "Không tìm thấy"}), 404
    return jsonify({
        "MaChuongTruyen": row[0],
        "MaTruyen": row[1],
        "ThuTuChuong": row[2],
        "TieuDe": row[3],
        "NgayDang": row[4],
        "NoiDung": row[5]
    })

# Thêm chương truyện
@chuongtruyen_bp.route('/', methods=['POST'])
def add_chuongtruyen():
    data = request.json or {}
    ma_truyen = data.get('MaTruyen') or data.get('maTruyen') or data.get('MaTruyen')
    thu_tu = data.get('ThuTuChuong') or data.get('thuTu') or data.get('ThuTuChuong')
    tieu_de = data.get('TieuDe') or data.get('TieuDe') or ''
    noi_dung = data.get('NoiDung') or data.get('NoiDung') or ''

    if ma_truyen is None or thu_tu is None or not str(tieu_de).strip():
        return jsonify({"message": "Thiếu trường bắt buộc: MaTruyen, ThuTuChuong, TieuDe"}), 400

    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO ChuongTruyen (MaTruyen, ThuTuChuong, TieuDe, NoiDung)
        VALUES (?, ?, ?, ?)
    """, (ma_truyen, thu_tu, tieu_de, noi_dung))
    conn.commit()
    last_id = cursor.lastrowid if hasattr(cursor, 'lastrowid') else None
    conn.close()
    return jsonify({"message": "Thêm chương truyện thành công", "MaChuongTruyen": last_id}), 201

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

# Xóa chương truyện
@chuongtruyen_bp.route('/<int:idct>', methods=['DELETE'])
def delete_chuongtruyen(idct):
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM ChuongTruyen WHERE MaChuongTruyen=?", (idct,))
    conn.commit()
    conn.close()
    return jsonify({"message": "Đã xóa chương truyện"})