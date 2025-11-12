const baseUrl = "http://127.0.0.1:5000/api/thanhvien";

// ----- Chuyển giữa 2 form -----
document.getElementById("show-register").onclick = () => {
  document.getElementById("login-form").style.display = "none";
  document.getElementById("register-form").style.display = "block";
  document.getElementById("form-title").innerText = "Đăng ký";
};

document.getElementById("show-login").onclick = () => {
  document.getElementById("register-form").style.display = "none";
  document.getElementById("login-form").style.display = "block";
  document.getElementById("form-title").innerText = "Đăng nhập";
};

// ----- Đăng nhập -----
document.getElementById("login-form").addEventListener("submit", async (e) => {
  e.preventDefault();
  const TaiKhoan = document.getElementById("login-username").value;
  const MatKhau = document.getElementById("login-password").value;

  const res = await fetch(`${baseUrl}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ TaiKhoan, MatKhau }),
  });

  const data = await res.json();
  alert(data.message);

  if (res.ok) {
    localStorage.setItem("user", TaiKhoan);
    window.location.href = "TrangChu.html";
  }
});

// ----- Đăng ký -----
document.getElementById("register-form").addEventListener("submit", async (e) => {
  e.preventDefault();
  const TaiKhoan = document.getElementById("reg-username").value;
  const MatKhau = document.getElementById("reg-password").value;
  const Avatar = document.getElementById("reg-avatar").value;

  const res = await fetch(baseUrl + "/", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ TaiKhoan, MatKhau, Avatar }),
  });


  if (res.ok) {
    document.getElementById("show-login").click();
  }
});
