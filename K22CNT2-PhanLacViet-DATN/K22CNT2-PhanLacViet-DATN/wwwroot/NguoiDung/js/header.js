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