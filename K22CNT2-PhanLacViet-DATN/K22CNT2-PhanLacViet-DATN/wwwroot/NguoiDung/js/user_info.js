// Biến toàn cục chứa dữ liệu sau khi tải từ API
let userProfileData = null;
// 1. Hàm gọi API lấy toàn bộ dữ liệu (Giống cách search lấy data)
async function loadUserProfile() {
    const currentUser = window.USER_ACCOUNT;
    if (!currentUser) {
        alert("Vui lòng đăng nhập!");
        return;
    }

    try {
        const res = await fetch(`/api/nguoidung/get-profile/${currentUser}`);
        if (res.ok) {
            userProfileData = await res.json();
            renderUI(); // Gọi hàm vẽ giao diện
        } else {
            console.error("Lỗi tải dữ liệu");
        }
    } catch (err) {
        console.error(err);
    }
}

function renderUI() {
    if (!userProfileData) return;

    const info = userProfileData.thongTinNguoiDung;
    document.getElementById('m2Name').innerText = info.hoTen || info.taiKhoan;
    document.getElementById('m2Join').innerText = `Tham gia: ${info.ngayThamGia}`;
    document.getElementById('m2Books').innerText = info.soTruyenDaDoc;
    document.getElementById('m2Chaps').innerText = info.soChuongDaDoc;
    document.getElementById('m2Rate').innerText = info.soDanhGia;
    document.getElementById('m2Comm').innerText = info.soBinhLuan;

    if (info.avatar) {
        document.getElementById('m2Avatar').src = info.avatar;
    } else {
        const char = info.taiKhoan.charAt(0).toUpperCase();
        document.getElementById('m2Avatar').src = `https://via.placeholder.com/400x300?text=${char}`;
    }

    renderList(userProfileData.dsTheoDoi, 'm2FollowList', 'follow');
    renderList(userProfileData.dsLichSu, 'm2HistoryList', 'history');
    renderList(userProfileData.dsDaLuu, 'm2BookmarkList', 'bookmark');
}

function renderList(listData, elementId, type) {
    const container = document.getElementById(elementId);
    container.innerHTML = '';

    if (!listData || listData.length === 0) {
        container.innerHTML = '<p style="color:#999; font-style:italic">Chưa có dữ liệu.</p>';
        return;
    }

    listData.forEach(item => {
        const urlTruyen = `/NguoiDung/Truyen/ChiTiet/${item.maTruyen}`;
        let metaHtml = '';
        if (type === 'history') {
            metaHtml = `<div class="text-meta">Đang đọc: ${item.tienDo || 'Chưa rõ'}</div>
                            <div class="text-meta">Lúc: ${item.thoiGian}</div>`;
        } else if (type === 'bookmark') {
            metaHtml = `<div class="text-meta">Đã lưu: ${item.thoiGian}</div>`;
        } else {
            metaHtml = `<div class="text-meta">${item.luotXem.toLocaleString()} lượt xem</div>
                            ${item.coChuongMoi ? '<span class="badge-new">Mới</span>' : ''}`;
        }

        const html = `
                <div class="list-row">
                    <div class="thumb">
                        <a href="${urlTruyen}"><img src="${item.anhBia}" alt=""></a>
                    </div>
                    <div class="info">
                        <div style="display:flex;justify-content:space-between;align-items:center">
                            <a href="${urlTruyen}" style="font-weight:bold; color:#333; text-decoration:none">${item.tenTruyen}</a>
                        </div>
                        ${metaHtml}
                    </div>
                </div>`;
        container.insertAdjacentHTML('beforeend', html);
    });
}

function m2Tab(t) {
    document.querySelectorAll('.tabs button').forEach(b => b.classList.toggle('active', b.dataset.tab === t));
    document.getElementById('m2Follow').style.display = t === 'follow' ? 'block' : 'none';
    document.getElementById('m2History').style.display = t === 'history' ? 'block' : 'none';
    document.getElementById('m2Bookmark').style.display = t === 'bookmark' ? 'block' : 'none';
}
document.addEventListener('DOMContentLoaded', () => {
    loadUserProfile();
});
function openEditAvatar() {
    document.getElementById('modalEditAvatar').style.display = 'flex';
    document.getElementById('imgPreview').src = document.getElementById('m2Avatar').src;
}
function closeModals() {
    document.getElementById('modalEditAvatar').style.display = 'none';
    document.getElementById('inputAvatar').value = "";
}
window.onclick = function (event) {
    const modal = document.getElementById('modalEditAvatar');
    if (event.target == modal) {
        closeModals();
    }
}
function prviewFile() {
    const file = document.getElementById('inputAvatar').files[0];
    const preview = document.getElementById('imgPreview');
    const btnSave = document.getElementById('btnSaveAvatar');

    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result; 
            btnSave.disabled = false;
        }
        reader.readAsDataURL(file);
    }
}

// Gọi API để upload ảnh
async function uploadAvatar() {
    const fileInput = document.getElementById('inputAvatar');
    const file = fileInput.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);
    formData.append('userName', window.USER_ACCOUNT); 

    const btnSave = document.getElementById('btnSaveAvatar');
    btnSave.innerText = 'Đang lưu...';
    btnSave.disabled = true;

    try {
        const res = await fetch('/api/nguoidung/update-avatar', {
            method: 'POST',
            body: formData
        });

        const result = await res.json();
        if (result.success) {
            alert('Cập nhật ảnh đại diện thành công!');
            // Cập nhật ảnh ngoài trang chính
            document.getElementById('m2Avatar').src = result.newUrl;
            closeModals();
        } else {
            alert('Lỗi: ' + result.message);
        }
    } catch (err) {
        console.error(err);
        alert('Có lỗi xảy ra khi upload.');
    } finally {
        btnSave.innerText = 'Lưu thay đổi';
    }
}