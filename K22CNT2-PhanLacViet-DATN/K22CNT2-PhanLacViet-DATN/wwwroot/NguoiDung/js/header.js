const searchInput = document.getElementById('searchInput');
const searchResults = document.getElementById('searchResults');
let timeout = null;

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

function renderDropdown(truyens) {
    if (!truyens || truyens.length === 0) {
        searchResults.innerHTML = '<div style="padding:10px;text-align:center;color:#666">Không tìm thấy truyện nào</div>';
        searchResults.style.display = 'block';
        return;
    }

    let html = '';
    truyens.slice(0, 5).forEach(t => {
        const imgUrl = "https://via.placeholder.com/50x70?text=No+Img";
        const link = `/NguoiDung/Truyen/ChiTiet/${t.maTruyen}`;

        html += `
                <a href="${link}" class="search-item">
                    <img src="${imgUrl}" alt="${t.tenTruyen}">
                    <div class="search-info">
                        <h5>${t.tenTruyen}</h5>
                        <p>Tác giả: ${t.tacGia || 'Đang cập nhật'}</p>
                    </div>
                </a>
            `;
    });
    if (truyens.length > 5) {
        html += `<div style="padding:10px;text-align:center;border-top:1px solid #eee;">
                        <small>Nhấn Enter để xem thêm ${truyens.length - 5} kết quả...</small>
                      </div>`;
    }

    searchResults.innerHTML = html;
    searchResults.style.display = 'block';
}
document.addEventListener('click', function (e) {
    if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
        searchResults.style.display = 'none';
    }
});