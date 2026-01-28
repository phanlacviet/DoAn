const BASE_API = '/api/nguoidung';

// 1. Đóng Modal chung
function closeModals() {
    document.querySelectorAll('.modal').forEach(m => m.style.display = 'none');
}

window.onclick = function (event) {
    if (event.target.classList.contains('modal')) closeModals();
}
async function updateNotificationBadge() {
    const btnNoti = document.getElementById('btnNoti');
    if (!btnNoti) return;

    const badge = btnNoti.querySelector('.bell-badge');
    const currentUser = btnNoti.getAttribute('data-user');
    if (!currentUser || !badge) return;

    try {
        const res = await fetch(`${BASE_API}/GetNotifications/${currentUser}`);
        const data = await res.json();

        // Tính toán số lượng chưa đọc
        const unreadCount = data.filter(n => n.daDoc === false || n.daDoc === 0).length;

        // Cập nhật giao diện số lượng
        badge.innerText = unreadCount;

        // Hiển thị hoặc ẩn badge đỏ
        if (unreadCount > 0) {
            badge.style.display = 'block';
            badge.style.background = 'red';
        } else {
            badge.style.display = 'none';
        }
    } catch (e) {
        console.error("Lỗi cập nhật badge:", e);
    }
}
// 2. Xử lý Thông báo
document.addEventListener('DOMContentLoaded', function () {
    const btnNoti = document.getElementById('btnNoti');
    const modalNoti = document.getElementById('modalNotifications');
    updateNotificationBadge();
    if (btnNoti) {
        btnNoti.onclick = async function () {
            modalNoti.style.display = 'flex';
            const currentUser = btnNoti.getAttribute('data-user');
            const container = modalNoti.querySelector('.modal-content');

            container.innerHTML = `
                <button class="close-modal" onclick="closeModals()">&times;</button>
                <h3 style="margin-bottom:15px; color: var(--text-main);">🔔 Thông báo của bạn</h3>
                <div id="notiList" style="max-height:400px; overflow-y:auto;">
                    <div style="text-align:center; padding:20px; color: var(--text-muted);">Đang tải...</div>
                </div>`;

            try {
                const res = await fetch(`${BASE_API}/GetNotifications/${currentUser}`);
                const data = await res.json();
                const list = document.getElementById('notiList');
                const badge = btnNoti.querySelector('.bell-badge');
                const unreadCount = data.filter(n => n.daDoc === false || n.daDoc === 0).length;
                if (badge) {
                    badge.innerText = unreadCount;
                    if (unreadCount > 0) {
                        badge.style.display = 'block';
                        badge.style.background = 'red';
                    } else {
                        badge.style.display = 'none';
                    }
                }
                if (!data || data.length === 0) {
                    list.innerHTML = '<p style="text-align:center; padding:20px;">Không có thông báo.</p>';
                    return;
                }

                list.innerHTML = data.map(n => {
                    const isUnread = n.daDoc === false || n.daDoc === 0;
                    return `
                        <div class="noti-item"
                             onclick="markAsRead(${n.maThongBao}, this)"
                             style="padding:12px; border-bottom:1px solid var(--header-border); background:${isUnread ? 'var(--hover-bg)' : 'var(--card)'}; display:flex; gap:10px; cursor:pointer; transition: 0.3s;">
        
                            <div class="unread-dot" style="width:8px; height:8px; background:${isUnread ? 'var(--accent)' : 'transparent'}; border-radius:50%; margin-top:6px;"></div>
        
                            <div style="flex:1">
                                <div class="noti-content" style="font-size:14px; color: var(--text-main); ${isUnread ? 'font-weight:600' : ''}">${n.noiDung}</div>
                                <div style="font-size:11px; color: var(--text-muted); margin-top:4px;">📅 ${n.ngayGui}</div>
                            </div>
                        </div>`;
                }).join('');
            } catch (e) {
                document.getElementById('notiList').innerHTML = '<div style="color: var(--text-muted);">Lỗi tải dữ liệu.</div>';
            }
        };
    }
});

// 3. Mở quản lý chương
async function openChapters(maTruyen, tenTruyen) {
    document.getElementById('modalChapters').style.display = 'flex';
    document.getElementById('modalChapterTitle').innerText = `Truyện: ${tenTruyen}`;
    document.getElementById('btnAddChapter').href = `/NguoiDung/NguoiDung/DangChuong?maTruyen=${maTruyen}`;

    const container = document.getElementById('chapterListContent');
    container.innerHTML = '<div style="color: var(--text-muted);">Đang tải...</div>';

    try {
        const res = await fetch(`${BASE_API}/GetChapters/${maTruyen}`);
        const data = await res.json();

        if (!data.length) {
            container.innerHTML = '<div style="color: var(--text-muted);">Chưa có chương.</div>';
            return;
        }

        container.innerHTML = data.map(c => `
            <div style="display:flex; justify-content:space-between; padding:10px; background: var(--hover-bg); border-radius:8px; align-items:center; border: 1px solid var(--header-border); margin-bottom:8px;">
                <div>
                    <div style="font-weight:600; color: var(--text-main);">${c.tieuDe}</div>
                    <div style="font-size:11px; color: var(--text-muted);">${c.ngayDang}</div>
                </div>
                <a href="/NguoiDung/NguoiDung/SuaChuong?id=${c.maChuongTruyen}" class="btn btn-blue" style="padding:4px 10px;">Sửa</a>
            </div>
        `).join('');
    } catch (e) { container.innerHTML = 'Lỗi tải.'; }
}

// 4. Mở thống kê chi tiết
async function openStats(maTruyen) {
    document.getElementById('modalStats').style.display = 'flex';
    const chartBox = document.getElementById('chartContainer');
    const labelBox = document.getElementById('chartLabels');

    chartBox.innerHTML = '<div style="color: var(--text-muted);">Đang tải...</div>';
    labelBox.innerHTML = '';

    try {
        const res = await fetch(`${BASE_API}/GetChartStats/${maTruyen}`);
        const data = await res.json();

        if (!data.length) {
            chartBox.innerHTML = '<div style="color: var(--text-muted);">Không có dữ liệu.</div>';
            return;
        }

        const maxVal = Math.max(...data.map(d => d.value)) || 1;
        chartBox.innerHTML = data.map(d => `
            <div class="bar" title="${d.value} lượt" style="height:${(d.value / maxVal) * 100}%; background:var(--accent); width:10%; border-radius:4px 4px 0 0;"></div>
        `).join('');

        labelBox.innerHTML = data.map(d => `<span style="width:14%; text-align:center; color: var(--text-muted); font-size:11px;">${d.label}</span>`).join('');
    } catch (e) { chartBox.innerHTML = 'Lỗi.'; }
}

// 5. Xóa truyện
async function deleteStory(id) {
    if (!confirm("Xác nhận xóa truyện?")) return;
    const res = await fetch(`${BASE_API}/DeleteStory/${id}`, { method: 'POST' });
    const result = await res.json();
    if (result.success) {
        alert("Thành công");
        document.getElementById(`story-row-${id}`).remove();
    }
}

async function markAsRead(id, element) {
    try {
        const res = await fetch(`${BASE_API}/MarkAsRead/${id}`, { method: 'POST' });
        const result = await res.json();

        if (result.success) {
            element.style.background = 'var(--card)';
            const dot = element.querySelector('.unread-dot');
            if (dot) dot.style.background = 'transparent';
            const content = element.querySelector('.noti-content');
            if (content) content.style.fontWeight = 'normal';
            updateNotificationBadge();
        }
    } catch (e) {
        console.error("Lỗi khi đánh dấu đã đọc:", e);
    }
}
// ... Các hàm cũ giữ nguyên ...

async function openDetailModal(type, maTruyen, tenTruyen) {
    const modal = document.getElementById('modalDetails');
    const title = document.getElementById('modalDetailTitle');
    const body = document.getElementById('modalDetailBody');

    modal.style.display = 'flex';
    body.innerHTML = '<div style="text-align:center; padding:20px;">⏳ Đang tải dữ liệu...</div>';

    let apiUrl = '';
    let titleText = '';

    // Xác định API và tiêu đề dựa trên loại click
    switch (type) {
        case 'follow':
            apiUrl = `${BASE_API}/GetFollowers/${maTruyen}`;
            titleText = `❤️ Người theo dõi: ${tenTruyen}`;
            break;
        case 'save':
            apiUrl = `${BASE_API}/GetSavers/${maTruyen}`;
            titleText = `💾 Người đã lưu: ${tenTruyen}`;
            break;
        case 'comment':
            apiUrl = `${BASE_API}/GetComments/${maTruyen}`;
            titleText = `💬 Bình luận: ${tenTruyen}`;
            break;
        case 'rating':
            apiUrl = `${BASE_API}/GetRatings/${maTruyen}`;
            titleText = `⭐ Đánh giá: ${tenTruyen}`;
            break;
    }

    title.innerText = titleText;

    try {
        const res = await fetch(apiUrl);
        const data = await res.json();

        if (!data || data.length === 0) {
            body.innerHTML = '<div style="text-align:center; padding:20px; color:#999;">Chưa có dữ liệu nào.</div>';
            return;
        }

        // Render HTML tùy theo loại
        if (type === 'follow' || type === 'save') {
            body.innerHTML = data.map(item => `
                <div class="detail-row" style="align-items:center;">
                    <img src="${item.avatar || '/NguoiDung/images/Avatar/default-avatar.jpg'}" class="detail-avatar" onerror="this.src='/NguoiDung/images/Avatar/default-avatar.jpg'">
                    <div class="detail-info">
                        <div class="detail-user">${item.taiKhoan}</div>
                        <div class="detail-date">🕒 ${item.ngayThucHien}</div>
                    </div>
                </div>
            `).join('');
        }
        else if (type === 'rating') {
            body.innerHTML = data.map(item => {
                const stars = '⭐'.repeat(Math.round(item.diem));
                const date = item.ngayDanhGia ? new Date(item.ngayDanhGia).toLocaleString('vi-VN') : '';
                return `
                <div class="detail-row">
                    <img src="${item.avatar || '/NguoiDung/images/Avatar/default-avatar.jpg'}" class="detail-avatar" onerror="this.src='/NguoiDung/images/Avatar/default-avatar.jpg'">
                    <div class="detail-info">
                        <div class="detail-user">${item.taiKhoan} <span class="star-yellow" style="font-size:12px; margin-left:5px;">${stars} (${item.diem})</span></div>
                        <div class="detail-content">${item.noiDung || 'Is marked.'}</div>
                        <div class="detail-date">🕒 ${date}</div>
                    </div>
                </div>`;
            }).join('');
        }
        else if (type === 'comment') {
            body.innerHTML = data.map(cmt => {
                const dateCmt = cmt.ngayGui ? new Date(cmt.ngayGui).toLocaleString('vi-VN') : '';

                // Xử lý các bình luận trả lời (Rep)
                let repliesHtml = '';
                if (cmt.repBinhLuans && cmt.repBinhLuans.length > 0) {
                    repliesHtml = cmt.repBinhLuans.map(rep => {
                        const dateRep = rep.ngayGui ? new Date(rep.ngayGui).toLocaleString('vi-VN') : '';
                        return `
                            <div class="reply-box">
                                <div style="display:flex; gap:10px; align-items:center; margin-bottom:5px;">
                                    <img src="${rep.avatar || '/NguoiDung/images/Avatar/default-avatar.jpg'}" style="width:25px; height:25px; border-radius:50%;" onerror="this.src='/NguoiDung/images/Avatar/default-avatar.jpg'">
                                    <span style="font-weight:bold; font-size:13px;">${rep.taiKhoan}</span>
                                    <span style="font-size:11px; color:#888;">${dateRep}</span>
                                </div>
                                <div style="font-size:13px;">${rep.noiDung}</div>
                            </div>
                         `;
                    }).join('');
                }

                return `
                <div class="detail-row">
                    <img src="${cmt.avatar || '/NguoiDung/images/Avatar/default-avatar.jpg'}" class="detail-avatar" onerror="this.src='/NguoiDung/images/Avatar/default-avatar.jpg'">
                    <div class="detail-info">
                        <div class="detail-user">
                            ${cmt.taiKhoan} 
                            <span style="font-weight:normal; color:var(--text-muted); font-size:12px;"> • ${cmt.tenChuong}</span>
                        </div>
                        <div class="detail-content">${cmt.noiDung}</div>
                        <div class="detail-date" style="margin-top:4px;">🕒 ${dateCmt}</div>
                        ${repliesHtml}
                    </div>
                </div>`;
            }).join('');
        }

    } catch (e) {
        console.error(e);
        body.innerHTML = '<div style="color:red; text-align:center;">Lỗi khi tải dữ liệu.</div>';
    }
}