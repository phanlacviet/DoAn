// Hàm khởi tạo header
window.initHeader = function () {
  try {
    const user = localStorage.getItem("user");
    const container = document.getElementById("user-menu-container");

    // Nếu header chưa load xong → dừng
    if (!container) {
      console.warn("Header chưa có trong DOM → initHeader dừng");
      return;
    }

    // Nếu có user → thay giao diện
    if (user) {
      container.innerHTML = `
        <div class="user-menu">
          <img src="../Images/user.png" class="user-icon" alt="User" />
          <div class="dropdown">
            <p>${user}</p>
            <a href="#">Thông báo</a>
            <a href="#">Trang cá nhân</a>
            <a href="#">Cài đặt</a>
            <a id="logout">Đăng xuất</a>
          </div>
        </div>
      `;

      // Xử lý đăng xuất
      const logoutBtn = document.getElementById("logout");
      if (logoutBtn) {
        logoutBtn.onclick = async () => {
          try {
            await fetch("http://127.0.0.1:5000/api/thanhvien/logout", {
              method: "POST",
              credentials: "include",
            });
          } catch (err) {
            console.warn("Lỗi khi logout:", err);
          }

          // Xóa user
          localStorage.removeItem("user");
          location.reload();
        };
      }
    }

  } catch (error) {
    console.error("Lỗi initHeader:", error);
  }
};
