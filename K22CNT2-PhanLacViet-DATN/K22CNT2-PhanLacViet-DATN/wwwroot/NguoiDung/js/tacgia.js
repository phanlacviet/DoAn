const BASE_API = '/api/nguoidung';

// 1. Đóng Modal chung
function closeModals() {
    document.querySelectorAll('.modal').forEach(m => m.style.display = 'none');
}

window.onclick = function (event) {
    if (event.target.classList.contains('modal')) closeModals();
}

// 2. Xử lý Thông báo
document.addEventListener('DOMContentLoaded', function () {
    const btnNoti = document.getElementById('btnNoti');
    const modalNoti = document.getElementById('modalNotifications');

    if (btnNoti) {
        btnNoti.onclick = async function () {
            modalNoti.style.display = 'flex';
            const currentUser = btnNoti.getAttribute('data-user');
            const container = modalNoti.querySelector('.modal-content');

            container.innerHTML = `
                <button class="close-modal" onclick="closeModals()">&times;</button>
                <h3 style="margin-bottom:15px">🔔 Thông báo của bạn</h3>
                <div id="notiList" style="max-height:400px; overflow-y:auto;">
                    <div style="text-align:center; padding:20px">Đang tải...</div>
                </div>`;

            try {
                const res = await fetch(`${BASE_API}/GetNotifications/${currentUser}`);
                const data = await res.json();
                const list = document.getElementById('notiList');

                if (!data || data.length === 0) {
                    list.innerHTML = '<p style="text-align:center; padding:20px;">Không có thông báo.</p>';
                    return;
                }

                list.innerHTML = data.map(n => {
                    const isUnread = n.daDoc === false || n.daDoc === 0;
                    return `
                    <div class="noti-item" style="padding:12px; border-bottom:1px solid #eee; background:${isUnread ? '#f0f7ff' : '#fff'}; display:flex; gap:10px;">
                        <div style="width:8px; height:8px; background:${isUnread ? '#007bff' : 'transparent'}; border-radius:50%; margin-top:6px;"></div>
                        <div style="flex:1">
                            <div style="font-size:14px; ${isUnread ? 'font-weight:600' : ''}">${n.noiDung}</div>
                            <div style="font-size:11px; color:#999; margin-top:4px;">📅 ${n.ngayGui}</div>
                        </div>
                    </div>`;
                }).join('');
            } catch (e) {
                document.getElementById('notiList').innerHTML = 'Lỗi tải dữ liệu.';
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
    container.innerHTML = 'Đang tải...';

    try {
        const res = await fetch(`${BASE_API}/GetChapters/${maTruyen}`);
        const data = await res.json();

        if (!data.length) {
            container.innerHTML = 'Chưa có chương.';
            return;
        }

        container.innerHTML = data.map(c => `
            <div style="display:flex; justify-content:space-between; padding:10px; background:#f8f9fa; border-radius:8px; align-items:center;">
                <div>
                    <div style="font-weight:600">${c.tieuDe}</div>
                    <div style="font-size:11px; color:#888">${c.ngayDang}</div>
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

    chartBox.innerHTML = 'Đang tải...';
    labelBox.innerHTML = '';

    try {
        const res = await fetch(`${BASE_API}/GetChartStats/${maTruyen}`);
        const data = await res.json();

        if (!data.length) {
            chartBox.innerHTML = 'Không có dữ liệu.';
            return;
        }

        const maxVal = Math.max(...data.map(d => d.value)) || 1;
        chartBox.innerHTML = data.map(d => `
            <div class="bar" title="${d.value} lượt" style="height:${(d.value / maxVal) * 100}%; background:var(--accent); width:10%; border-radius:4px 4px 0 0;"></div>
        `).join('');

        labelBox.innerHTML = data.map(d => `<span style="width:14%; text-align:center;">${d.label}</span>`).join('');
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