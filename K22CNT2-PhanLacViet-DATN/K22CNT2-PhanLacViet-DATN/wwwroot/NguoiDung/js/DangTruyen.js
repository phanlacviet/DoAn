// 1. Xử lý Rich Text Editor
function cmd(command) {
    document.execCommand(command, false, null);
    updateToolbar();
}

function updateToolbar() {
    const commands = [
        { id: 'bold', btn: 'btn-bold' },
        { id: 'italic', btn: 'btn-italic' },
        { id: 'underline', btn: 'btn-underline' },
        { id: 'strikeThrough', btn: 'btn-strike' },
        { id: 'insertUnorderedList', btn: 'btn-list' }
    ];

    commands.forEach(c => {
        const btn = document.getElementById(c.btn);
        if (document.queryCommandState(c.id)) {
            btn.classList.add('active');
        } else {
            btn.classList.remove('active');
        }
    });
}

// 2. Preview Ảnh
function preview(input) {
    const box = document.getElementById('previewBox');
    if (input.files && input.files[0]) {
        box.innerHTML = '';
        const img = document.createElement('img');
        img.src = URL.createObjectURL(input.files[0]);
        box.appendChild(img);
    }
}
const editor = document.getElementById('moTa');
if (editor) { // Kiểm tra tồn tại để tránh lỗi ở trang Chương
    editor.addEventListener('mouseup', updateToolbar);
    editor.addEventListener('keyup', updateToolbar);
}
// 1. Hàm Submit Truyện (Dùng cho cả Đăng mới và Sửa)
async function submitStory(isEdit = false, maTruyen = 0) {
    const tenTruyen = document.getElementById('txtTenTruyen').value;
    const moTa = document.getElementById('moTa').innerHTML;
    const tacGia = document.getElementById('txtTacGia').value;
    const loaiTruyen = document.querySelector('input[name="type"]:checked')?.value;
    const fileInput = document.getElementById('fileAnhBia');

    if (!tenTruyen || !tacGia) { alert("Vui lòng nhập Tên truyện và Tác giả!"); return; }

    const formData = new FormData();
    formData.append('TenTruyen', tenTruyen);
    formData.append('MoTa', moTa);
    formData.append('TacGia', tacGia);
    formData.append('LoaiTruyen', loaiTruyen);

    // Nếu có file ảnh mới thì gửi, không thì thôi
    if (fileInput && fileInput.files[0]) {
        formData.append('FileAnhBia', fileInput.files[0]);
    }

    const selectedGenres = document.querySelectorAll('.genre-cb:checked');
    if (selectedGenres.length === 0) { alert("Chọn ít nhất 1 thể loại!"); return; }
    selectedGenres.forEach(cb => formData.append('SelectedTheLoais', cb.value));

    // Cấu hình URL và Method
    let url = '/api/nguoidung/dang-truyen';
    let method = 'POST';

    if (isEdit) {
        url = '/api/nguoidung/sua-truyen';
        method = 'PUT'; // Hoặc POST tùy config server
        formData.append('MaTruyen', maTruyen);
    }

    await sendRequest(url, method, formData, "/NguoiDung/NguoiDung/ThongTinTacGia");
}

// 2. Hàm Submit Chương (Dùng cho cả Đăng chương và Sửa chương)
async function submitChapter(isEdit = false, id = 0, maTruyen = 0) {
    const tieuDe = document.getElementById('txtTenTruyen').value; // Tận dụng id css cũ
    const noiDung = document.getElementById('moTa').innerHTML;   // Tận dụng id css cũ (Editor)
    const thuTu = document.getElementById('txtThuTu')?.value || 0;

    if (!tieuDe || !noiDung) { alert("Tiêu đề và Nội dung không được trống!"); return; }

    const formData = new FormData();
    formData.append('TieuDe', tieuDe);
    formData.append('NoiDung', noiDung);
    formData.append('MaTruyen', maTruyen);
    formData.append('ThuTuChuong', thuTu);

    let url = '/api/nguoidung/dang-chuong';
    let method = 'POST';

    if (isEdit) {
        url = '/api/nguoidung/sua-chuong';
        method = 'PUT';
        formData.append('MaChuongTruyen', id);
    }

    await sendRequest(url, method, formData, "/NguoiDung/NguoiDung/ThongTinTacGia");
}

// 3. Hàm gửi request chung
async function sendRequest(url, method, bodyData, redirectUrl) {
    const btn = document.querySelector('.submit');
    try {
        btn.disabled = true;
        btn.innerText = "Đang xử lý...";

        const res = await fetch(url, { method: method, body: bodyData });
        const data = await res.json();

        if (res.ok && (data.success || data.Success)) { // check cả hoa thường tùy API trả về
            alert("Thành công!");
            window.location.href = redirectUrl;
        } else {
            alert("Lỗi: " + (data.message || "Có lỗi xảy ra"));
        }
    } catch (e) {
        console.error(e);
        alert("Lỗi kết nối server");
    } finally {
        btn.disabled = false;
        btn.innerText = "Xác nhận";
    }
}