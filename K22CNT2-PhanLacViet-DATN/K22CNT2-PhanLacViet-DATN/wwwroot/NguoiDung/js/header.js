document.addEventListener("DOMContentLoaded", function () {
    // --- PHẦN 1: XỬ LÝ DARK MODE (ĐÃ CẬP NHẬT) ---
    const themeToggleBtn = document.getElementById('theme-toggle');
    const rootElement = document.documentElement;
    function applyTheme(theme) {
        const iconTheme = themeToggleBtn ? themeToggleBtn.querySelector('i') : null;

        if (theme === 'dark') {
            rootElement.setAttribute('data-theme', 'dark');
            if (iconTheme) iconTheme.className = 'fa-regular fa-sun';
        } else {
            rootElement.removeAttribute('data-theme');
            if (iconTheme) iconTheme.className = 'fa-regular fa-moon';
        }
    }
    const savedTheme = localStorage.getItem('theme') || 'light';
    applyTheme(savedTheme);
    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', () => {
            const isDark = rootElement.getAttribute('data-theme') === 'dark';
            const newTheme = isDark ? 'light' : 'dark';

            applyTheme(newTheme);
            localStorage.setItem('theme', newTheme);
        });
    }
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    let timeout = null;

    if (searchInput && searchResults) {
        searchInput.addEventListener('input', function (e) {
            const keyword = e.target.value.trim();
            clearTimeout(timeout);

            if (keyword.length === 0) {
                searchResults.style.display = 'none';
                return;
            }

            timeout = setTimeout(async () => {
                try {
                    const response = await fetch(`/api/Truyen/TimKiem?keyword=${encodeURIComponent(keyword)}`);
                    if (response.ok) {
                        const data = await response.json();
                        renderDropdown(data);
                    }
                } catch (error) {
                    console.error("Lỗi tìm kiếm:", error);
                }
            }, 300);
        });
    }

    function renderDropdown(truyens) {
        if (!truyens || truyens.length === 0) {
            searchResults.innerHTML = '<div style="padding:15px;text-align:center;color:var(--text-muted)">Không tìm thấy truyện nào</div>';
            searchResults.style.display = 'block';
            return;
        }

        let html = '';
        truyens.slice(0, 5).forEach(t => {
            const imgUrl = t.anhBia ? t.anhBia : '/images/AnhBia/default.jpg';
            const link = `/NguoiDung/Truyen/ChiTiet/${t.maTruyen}`;

            html += `
                <a href="${link}" class="search-item" style="display:flex; gap:10px; padding:10px; border-bottom:1px solid var(--header-border); align-items:center;text-decoration:none">
                    <img src="${imgUrl}" style="width:40px; height:55px; object-fit:cover; border-radius:4px;" alt="${t.tenTruyen}">
                    <div class="search-info">
                        <h5 style="margin:0; font-size:14px; font-weight:bold; color:var(--text-main)">${t.tenTruyen}</h5>
                        <p style="margin:2px 0 0; font-size:12px; color:var(--text-muted)">${t.tacGia || 'Đang cập nhật'}</p>
                    </div>
                </a>
            `;
        });

        searchResults.innerHTML = html;
        searchResults.style.display = 'block';
    }

    // Đóng kết quả tìm kiếm khi click ra ngoài
    document.addEventListener('click', function (e) {
        if (searchInput && !searchInput.contains(e.target) && !searchResults.contains(e.target)) {
            searchResults.style.display = 'none';
        }
    });
});
document.addEventListener("DOMContentLoaded", function () {
    const bellBtn = document.getElementById('ntfBellBtn');
    const ntfDropdown = document.getElementById('ntfDropdown');
    const ntfBadge = document.getElementById('ntfBadge');
    const ntfList = document.getElementById('ntfList');
    const currentUser = bellBtn ? bellBtn.getAttribute('data-user') : '';

    if (!currentUser) return;

    let notificationsData = [];

    // 1. Hàm load thông báo
    async function loadNotifications() {
        try {
            const res = await fetch(`/api/nguoidung/GetNotifications/${currentUser}`);
            if (res.ok) {
                notificationsData = await res.json();
                updateBadge();
            }
        } catch (err) {
            console.error(err);
        }
    }

    // 2. Hàm cập nhật Badge và render dropdown
    function updateBadge() {
        // Đếm số chưa đọc (Giả sử API trả về daDoc là boolean hoặc 0/1)
        const unreadCount = notificationsData.filter(n => n.daDoc === false || n.daDoc === 0).length;

        // Cập nhật số trên badge
        ntfBadge.innerText = unreadCount > 99 ? '99+' : unreadCount;

        if (unreadCount > 0) {
            ntfBadge.style.display = 'block';
        } else {
            ntfBadge.style.display = 'none';
        }
    }

    function renderDropdownList() {
        if (notificationsData.length === 0) {
            ntfList.innerHTML = '<div class="ntf-empty">Bạn không có thông báo nào.</div>';
            return;
        }

        // Chỉ lấy 5 thông báo mới nhất
        const top5 = notificationsData.slice(0, 5);

        ntfList.innerHTML = top5.map(n => {
            const isUnread = n.daDoc === false || n.daDoc === 0;
            const unreadClass = isUnread ? 'unread' : '';

            return `
                <div class="ntf-item ${unreadClass}" onclick="readNotification(${n.maThongBao}, '${n.link || '#'}')">
                    <div class="ntf-content">${n.noiDung}</div>
                    <div class="ntf-time">${n.ngayGui}</div>
                </div>
            `;
        }).join('');
    }

    // 3. Sự kiện click chuông
    bellBtn.addEventListener('click', function (e) {
        e.stopPropagation(); 
        const isVisible = ntfDropdown.style.display === 'block';

        if (!isVisible) {
            renderDropdownList(); 
            ntfDropdown.style.display = 'block';
        } else {
            ntfDropdown.style.display = 'none';
        }
    });

    // 4. Đóng dropdown khi click ra ngoài
    document.addEventListener('click', function (e) {
        if (!bellBtn.contains(e.target) && !ntfDropdown.contains(e.target)) {
            ntfDropdown.style.display = 'none';
        }
    });

    // 5. Hàm global để gọi khi click vào 1 thông báo
    window.readNotification = async function (id, link) {
        try {
            await fetch(`/api/nguoidung/MarkAsRead/${id}`, { method: 'POST' });
            const noti = notificationsData.find(n => n.maThongBao === id);
            if (noti) noti.daDoc = true;
            updateBadge();
            if (link && link !== '#') {
                window.location.href = link;
            } else {
                renderDropdownList();
            }
        } catch (e) {
            console.error(e);
        }
    };
    window.markAllRead = async function () {
        if (!confirm('Đánh dấu tất cả là đã đọc?')) return;
        try {
            await fetch(`/api/nguoidung/MarkAllRead/${currentUser}`, { method: 'POST' });
            notificationsData.forEach(n => n.daDoc = true);
            updateBadge();
            renderDropdownList();
        } catch (e) { console.error(e); }
    }
    loadNotifications();
    setInterval(loadNotifications, 60000);
});
document.addEventListener("DOMContentLoaded", function () {
    const avatarImgElement = document.getElementById('userHeaderAvatar');
    const bellBtn = document.getElementById('ntfBellBtn');
    const currentUser = bellBtn ? bellBtn.getAttribute('data-user') : '';

    if (currentUser && avatarImgElement) {
        fetch(`/api/nguoidung/GetAvatar/${currentUser}`)
            .then(response => response.json())
            .then(data => {
                if (data && data.avatar) {
                    avatarImgElement.src = data.avatar;
                }
            })
            .catch(err => console.error("Lỗi khi lấy avatar:", err));
    }
});