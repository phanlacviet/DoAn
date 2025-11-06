from flask import Flask
from flask_cors import CORS

from Backend.routes.thanhvien_api import thanhvien_bp
from Backend.routes.theloai_api import theloai_bp
from Backend.routes.truyen_api import truyen_bp
from Backend.routes.chuongtruyen_api import chuongtruyen_bp
from Backend.routes.thongbao_api import thongbao_bp
from Backend.routes.binhluan_api import binhluan_bp
from Backend.routes.replybinhluan_api import reply_binhluan_bp
from Backend.routes.theodoi_api import theodoi_bp
from Backend.routes.danhgia_api import danhgia_bp
from Backend.routes.luutruyen_api import luutruyen_bp

app = Flask(__name__)
CORS(app)

# Đăng ký tất cả blueprint
app.register_blueprint(thanhvien_bp, url_prefix='/api/thanhvien')
app.register_blueprint(theloai_bp, url_prefix='/api/theloai')
app.register_blueprint(truyen_bp, url_prefix='/api/truyen')
app.register_blueprint(chuongtruyen_bp, url_prefix='/api/chuongtruyen')
app.register_blueprint(thongbao_bp, url_prefix='/api/thongbao')
app.register_blueprint(binhluan_bp, url_prefix='/api/binhluan')
app.register_blueprint(reply_binhluan_bp, url_prefix='/api/replybinhluan')
app.register_blueprint(theodoi_bp, url_prefix='/api/theodoi')
app.register_blueprint(danhgia_bp, url_prefix='/api/danhgia')
app.register_blueprint(luutruyen_bp, url_prefix='/api/luutruyen')

@app.route('/')
def home():
    return {"message": "Flask backend is running!"}

if __name__ == '__main__':
    app.run(debug=True)
