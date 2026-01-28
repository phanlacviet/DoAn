// Hàm sắp xếp thông báo
function sortNotifications() {
    const container = document.getElementById('notificationContainer');
    const sortValue = document.getElementById('sortSelect').value; // 'asc' hoặc 'desc'

    // Lấy tất cả các dòng thông báo
    const items = Array.from(container.getElementsByClassName('ntf-row'));

    // Sắp xếp mảng dựa trên thuộc tính data-date
    items.sort((a, b) => {
        const dateA = new Date(a.getAttribute('data-date'));
        const dateB = new Date(b.getAttribute('data-date'));

        if (sortValue === 'asc') {
            return dateA - dateB; // Tăng dần (Cũ nhất -> Mới nhất)
        } else {
            return dateB - dateA; // Giảm dần (Mới nhất -> Cũ nhất)
        }
    });

    // Xóa nội dung cũ và thêm lại theo thứ tự mới
    container.innerHTML = '';
    items.forEach(item => container.appendChild(item));
}

// Hàm đánh dấu đã đọc một tin khi click
async function markOneRead(id, element) {
    // Nếu đã đọc rồi (không còn class unread) thì không làm gì cả
    if (!element.classList.contains('unread')) return;

    try {
        const res = await fetch(`/api/nguoidung/MarkAsRead/${id}`, { method: 'POST' });
        const result = await res.json();

        if (result.success) {
            // Xử lý giao diện: Bỏ class unread, đổi icon, xóa chấm đỏ
            element.classList.remove('unread');

            // Đổi icon phong bì
            const icon = element.querySelector('.ntf-icon i');
            if (icon) {
                icon.className = 'fa-regular fa-envelope-open';
                icon.style.color = 'var(--text-muted)';
            }

            // Xóa chấm đỏ
            const dot = element.querySelector('.ntf-dot');
            if (dot) dot.remove();

            // Cập nhật lại số trên Header (gọi hàm từ header.js nếu có)
            if (typeof updateNotificationBadge === 'function') {
                updateNotificationBadge();
            }
        }
    } catch (e) {
        console.error("Lỗi khi đánh dấu đã đọc:", e);
    }
}

// Hàm đánh dấu đọc tất cả
async function markAllReadPage() {
    if (!confirm("Bạn có chắc muốn đánh dấu tất cả là đã đọc?")) return;

    // Lấy user từ session (hoặc lấy từ API/DOM hidden input). 
    // Ở đây ta có thể lấy user từ Sidebar nếu đã render
    const userNameElement = document.querySelector('.ntf-username');
    if (!userNameElement) return;

    const user = userNameElement.innerText.trim();

    try {
        const res = await fetch(`/api/nguoidung/MarkAllRead/${user}`, { method: 'POST' });
        const result = await res.json();

        if (result.success) {
            // Loại bỏ class unread ở tất cả các dòng
            document.querySelectorAll('.ntf-row.unread').forEach(row => {
                row.classList.remove('unread');

                // Đổi icon
                const icon = row.querySelector('.ntf-icon i');
                if (icon) {
                    icon.className = 'fa-regular fa-envelope-open';
                    icon.style.color = 'var(--text-muted)';
                }

                // Xóa dot
                const dot = row.querySelector('.ntf-dot');
                if (dot) dot.remove();
            });

            // Cập nhật header
            if (typeof updateNotificationBadge === 'function') {
                updateNotificationBadge();
            }
        }
    } catch (e) {
        console.error("Lỗi mark all:", e);
    }
}
async function deleteReadNotifications() {
    const userNameElement = document.querySelector('.ntf-username');
    if (!userNameElement) return;

    const user = userNameElement.innerText.trim();

    if (!confirm("Bạn có chắc chắn muốn xóa tất cả thông báo đã đọc? Hành động này không thể hoàn tác.")) {
        return;
    }

    try {
        const res = await fetch(`/api/nguoidung/DeleteReadNotifications/${user}`, {
            method: 'DELETE'
        });
        const result = await res.json();

        if (result.success) {
            // Xóa các phần tử DOM có trạng thái đã đọc (không có class unread)
            const rows = document.querySelectorAll('.ntf-row:not(.unread)');
            rows.forEach(row => {
                row.style.opacity = '0';
                row.style.transform = 'translateX(20px)';
                setTimeout(() => row.remove(), 300);
            });

            // Cập nhật lại tổng số thông báo hiển thị ở sidebar
            const statsB = document.querySelector('.ntf-stats b');
            if (statsB) {
                const currentTotal = parseInt(statsB.innerText);
                statsB.innerText = currentTotal - result.count;
            }

            // Nếu không còn thông báo nào sau khi xóa, hiển thị trạng thái trống
            setTimeout(() => {
                const container = document.getElementById('notificationContainer');
                if (container && container.querySelectorAll('.ntf-row').length === 0) {
                    container.innerHTML = `
                        <div class="ntf-empty-state">
                            <img src="https://via.placeholder.com/150?text=Empty" alt="Empty">
                            <p>Bạn không có thông báo nào.</p>
                        </div>`;
                }
            }, 400);

        } else {
            alert("Có lỗi xảy ra khi xóa thông báo.");
        }
    } catch (e) {
        console.error("Lỗi xóa thông báo:", e);
        alert("Không thể kết nối đến máy chủ.");
    }
}