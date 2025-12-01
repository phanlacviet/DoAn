// LOAD HEADER
fetch('../NguoiDung/header.html')
  .then(r => r.text())
  .then(html => {
    document.getElementById("header").innerHTML = html;
    if (!document.getElementById('header-js')) {
      const script = document.createElement('script');
      script.src = '../js/header.js';
      script.id = 'header-js';
      script.onload = () => window.initHeader && window.initHeader();
      document.body.appendChild(script);
    } else {
      window.initHeader && window.initHeader();
    }
  });
let hasRated = false;
let currentRating = 0;
// lấy ID truyện
const params = new URLSearchParams(window.location.search);
const truyenId = params.get("id");
const user = localStorage.getItem("user");

if (!truyenId) console.error("Không tìm thấy ID truyện.");

// Load truyện
fetch(`/api/truyen/${truyenId}`)
  .then(r => r.json())
  .then(data => {
    document.getElementById('bookTitle').textContent = data.TenTruyen;
    document.getElementById('bookAuthor').textContent = data.TacGia;
    document.getElementById('bookStatus').textContent = data.TrangThai || "Đang ra";
    document.getElementById('bookCategory').textContent = data.LoaiTruyen;
    document.getElementById('bookDesc').textContent = data.MoTa;
  });

// load danh sách chương
fetch(`/api/chuongtruyen`)
  .then(r => r.json())
  .then(data => {
    const danhSach = data.filter(c => c.MaTruyen == truyenId);
    const list = document.getElementById('chapterList');

    danhSach.forEach((c, i) => {
      const li = document.createElement('li');
      li.textContent = `Chương ${c.ThuTuChuong}: ${c.TieuDe}`;
      li.classList.add("chapter");
      if (i >= 10) li.classList.add("hidden");
      list.appendChild(li);
    });
  });

document.getElementById("toggleChapters").addEventListener("click", () => {
  document.querySelectorAll(".chapter.hidden").forEach(c => c.classList.toggle("hidden"));
});

// Format ngày
function formatDate(sqlDate) {
  const d = new Date(sqlDate);
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  const hh = String(d.getHours()).padStart(2, '0');
  const mi = String(d.getMinutes()).padStart(2, '0');
  return `${yyyy}-${mm}-${dd} ${hh}:${mi}`;
}

// Bình luận

const commentList = document.getElementById("commentList");

// Load bình luận (chỉ lấy BinhLuan của truyện hiện tại)
function loadComments() {
  commentList.innerHTML = "";

  // 1) Lấy tất cả chương, lọc ra các MaChuongTruyen của truyện hiện tại
  fetch('/api/chuongtruyen')
    .then(r => r.json())
    .then(chs => {
      const chapterIds = chs
        .filter(ch => ch.MaTruyen == truyenId)   // == để cho phép string/number
        .map(ch => ch.MaChuongTruyen);

      // 2) Lấy tất cả bình luận rồi lọc theo chapterIds
      fetch('/api/binhluan')
        .then(r => r.json())
        .then(list => {
          const comments = list.filter(c => chapterIds.includes(c.MaChuongTruyen));

          comments.forEach(cmnt => {
            const div = document.createElement("div");
            div.className = "comment-item";

            div.innerHTML = `
              <p class="cmt-user"><strong>${cmnt.TaiKhoan}</strong></p>
              <p class="cmt-date">${formatDate(cmnt.NgayGui)}</p>
              <p class="cmt-text">${cmnt.NoiDung}</p>
              <div class="replies" id="replies-${cmnt.MaBinhLuan}"></div>
            `;

            commentList.appendChild(div);
            loadReplies(cmnt.MaBinhLuan);
          });
        });
    })
    .catch(err => console.error('Lỗi khi load bình luận/chương:', err));
}

// Load reply
function loadReplies(cmtId) {
  fetch('/api/replybinhluan')
    .then(r => r.json())
    .then(replies => {
      const list = replies.filter(r => r.MaBinhLuan == cmtId);
      const div = document.getElementById(`replies-${cmtId}`);
      div.innerHTML = "";

      list.forEach(rep => {
        const repDiv = document.createElement("div");
        repDiv.className = "reply-comment";

        repDiv.innerHTML = `
          <p class="cmt-user"><strong>${rep.TaiKhoan}</strong></p>
          <p class="cmt-date">${formatDate(rep.NgayGui)}</p>
          <p class="cmt-text">${rep.NoiDung}</p>
        `;

        div.appendChild(repDiv);
      });
    });
}

loadComments();

// Đánh giá sao
const starBtns = document.querySelectorAll(".star");
starBtns.forEach((star, i) => {
  star.style.cursor = "pointer";
  star.onclick = () => {
    if (!user) return alert("Bạn phải đăng nhập!");
    if (hasRated) return;
    openRatingModal(i + 1);
  };
});
function openRatingModal() {
  const modal = document.getElementById("ratingModal");
  if (!modal) return alert("Modal đánh giá chưa được thêm vào HTML.");
  modal.style.display = "flex";
  const mstars = modal.querySelectorAll(".mstar");
  mstars.forEach((s, idx) => s.classList.toggle("selected", idx < initialRating));
  modal.selectedRating = initialRating;
  document.getElementById("ratingText").value = "";
}

// hiển thị bảng đánh giá / xử lý nút trong modal
document.addEventListener("click", (e) => {
  // chọn điểm sao trong modal
  if (e.target && e.target.classList.contains("mstar")) {
    const val = Number(e.target.getAttribute("data-value"));
    const mstars = document.querySelectorAll("#modalStars .mstar");
    mstars.forEach((s, idx) => s.classList.toggle("selected", idx < val));
    document.getElementById("ratingModal").selectedRating = val;
  }

  // Hủy
  if (e.target && e.target.id === "ratingCancelBtn") {
    const modal = document.getElementById("ratingModal");
    if (modal) modal.style.display = "none";
  }

  // Gửi
  if (e.target && e.target.id === "ratingSubmitBtn") {
    submitRatingFromModal();
  }
});

function submitRatingFromModal() {
  const modal = document.getElementById("ratingModal");
  if (!modal) return;
  const rating = modal.selectedRating || 0;
  if (rating < 1 || rating > 5) return alert("Vui lòng chọn số sao (1-5).");
  const text = document.getElementById("ratingText").value || "Đánh giá";

  // POST lên API danhgia
  fetch("/api/danhgia", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      NoiDung: text,
      Diem: rating,
      TaiKhoan: user,
      MaTruyen: truyenId
    })
  })
    .then(resp => {
      if (!resp.ok) throw new Error("Lỗi khi gửi đánh giá");
      return resp.json().catch(() => ({})); // response body không cần
    })
    .then(() => {
      // đóng bảng đánh giá, cập nhật trạng thái
      modal.style.display = "none";
      hasRated = true;
      currentRating = rating;
      updateStars();
      document.getElementById("ratingStatus").textContent = "Bạn đã đánh giá";
      document.getElementById("ratingStatus").style.color = "orange";
      refreshRatingAverage();
    })
    .catch(err => {
      console.error(err);
      alert("Gửi đánh giá thất bại.");
    });
}


function refreshRatingAverage() {
  fetch(`/api/danhgia/avg/${truyenId}`)
    .then(r => r.json())
    .then(data => {
      // data: { avg: number, count: number }
      document.getElementById("ratingScore").textContent = data.avg !== undefined ? data.avg : "0";
      document.getElementById("ratingCount").textContent = data.count !== undefined ? data.count : "0";
    })
    .catch(err => {
      console.error("Lỗi load điểm trung bình:", err);
      document.getElementById("ratingScore").textContent = "0";
      document.getElementById("ratingCount").textContent = "0";
    });
}

function checkUserRated() {
  fetch('/api/danhgia')
    .then(r => r.json())
    .then(list => {
      const rated = list.find(d => d.MaTruyen == truyenId && d.TaiKhoan == user);
      if (rated) {
        hasRated = true;
        currentRating = rated.Diem || 0;
        updateStars(); // hiển thị sao phù hợp
        document.getElementById("ratingStatus").textContent = "Bạn đã đánh giá";
        document.getElementById("ratingStatus").style.color = "orange";

        // không cho hủy/đánh lại: loại bỏ hành vi onclick (nếu có)
        const rc = document.getElementById("ratingContainer");
        if (rc) rc.onclick = null;
      } else {
        hasRated = false;
        currentRating = 0;
        updateStars();
        document.getElementById("ratingStatus").textContent = "";
      }

      // Cập nhật điểm trung bình sau khi biết trạng thái
      refreshRatingAverage();
    })
    .catch(err => console.error("Lỗi load đánh giá:", err));
}
checkUserRated();

function updateStars() {
  starBtns.forEach((s, i) => s.classList.toggle("selected", i < currentRating));
}

// lưu truyện

const btnSave = document.getElementById("followBtn");
checkSaved();

function checkSaved() {
  fetch('/api/luutruyen')
    .then(r => r.json())
    .then(list => {
      const saved = list.find(t => t.MaTruyen == truyenId && t.TaiKhoan == user);

      if (saved) {
        btnSave.textContent = "Đã theo dõi (bấm để hủy)";
        btnSave.style.background = "gray";

        btnSave.onclick = () => {
          if (confirm("Bạn có muốn hủy theo dõi")) {
            fetch(`/api/luutruyen/${saved.IDLuuTruyen}`, { method: "DELETE" })
              .then(() => location.reload());
          }
        };
      } else {
        btnSave.onclick = saveStory;
      }
    });
}

function saveStory() {
  if (!user) return alert("Bạn phải đăng nhập!");

  fetch("/api/luutruyen", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      TaiKhoan: user,
      MaTruyen: truyenId
    })
  }).then(() => location.reload());
}
