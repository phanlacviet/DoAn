// Khi load trang, kiểm tra xem user đã đăng nhập chưa
document.addEventListener("DOMContentLoaded", () => {
  const user = localStorage.getItem("user");
  const container = document.getElementById("user-menu-container");

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

    // Đăng xuất
    document.getElementById("logout").onclick = async () => {
      await fetch("http://127.0.0.1:5000/api/thanhvien/logout", {
        method: "POST",
        credentials: "include",
      });
      localStorage.removeItem("user");
      location.reload();
    };
  }
});
