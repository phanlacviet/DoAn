
// Load header.html

fetch('../NguoiDung/header.html')
  .then(response => {
    if (!response.ok) throw new Error('Không load được header');
    return response.text();
  })
  .then(data => {
    document.getElementById("header").innerHTML = data;

    // Load header.js nếu chưa có
    if (!document.getElementById('header-js')) {

      const script = document.createElement('script');
      script.src = '../js/header.js';   // ⭐ SỬA ĐÚNG Ở ĐÂY
      script.id = 'header-js';

      script.onload = () => {
        if (window.initHeader) window.initHeader();
      };

      document.body.appendChild(script);

    } else {
      // Script đã load từ trước → chỉ cần chạy lại initHeader
      if (window.initHeader) window.initHeader();
    }
  })
  .catch(() => {
    console.warn("Không thể tải header.html");
  });


// Đánh giá sao

const stars = document.querySelectorAll('.star');
let rating = 4;
stars.forEach((star, idx) => {
  star.addEventListener('click', () => {
    rating = idx + 1;
    updateStars();
  });
});
function updateStars() {
  stars.forEach((s, i) => s.classList.toggle('selected', i < rating));
}
updateStars();


// Xem thêm chương

document.getElementById('toggleChapters').addEventListener('click', () => {
  document.querySelectorAll('.chapter.hidden').forEach(c => c.classList.toggle('hidden'));
});


// Lấy dữ liệu truyện & chương

const urlParams = new URLSearchParams(window.location.search);
const truyenId = urlParams.get('id');

// Lấy thông tin truyện
fetch(`/api/truyen/${truyenId}`)
  .then(r => r.json())
  .then(data => {
    document.getElementById('bookTitle').textContent = data.TenTruyen;
    document.getElementById('bookAuthor').textContent = data.TacGia;
    document.getElementById('bookStatus').textContent = data.TrangThai || 'Đang ra';
    document.getElementById('bookCategory').textContent = data.LoaiTruyen;
    document.getElementById('bookDesc').textContent = data.MoTa;
  })
  .catch(err => console.error('Lỗi load truyện:', err));

// Lấy danh sách chương
fetch(`/api/chuongtruyen`)
  .then(r => r.json())
  .then(chapters => {
    const list = chapters.filter(c => c.MaTruyen == truyenId);
    const ul = document.getElementById('chapterList');
    list.forEach((c, idx) => {
      const li = document.createElement('li');
      li.textContent = `Chương ${c.ThuTuChuong}: ${c.TieuDe}`;
      if (idx >= 10) li.classList.add('hidden');
      li.classList.add('chapter');
      ul.appendChild(li);
    });
  })
  .catch(err => console.error('Lỗi load chương:', err));


// Bình luận & phản hồi

const commentList = document.getElementById('commentList');
const commentInput = document.getElementById('commentText');
const sendCommentBtn = document.getElementById('sendComment');

// Lấy danh sách bình luận
function loadComments() {
  commentList.innerHTML = '';
  fetch('/api/danhgia')
    .then(r => r.json())
    .then(danhgias => {
      const comments = danhgias.filter(d => d.MaTruyen == truyenId);
      comments.forEach(c => {
        const div = document.createElement('div');
        div.className = 'comment-item';
        div.innerHTML = `
          <p><strong>${c.TaiKhoan}:</strong> ${c.NoiDung}</p>
          <button class="replyBtn" data-id="${c.MaDanhGia}">Trả lời</button>
          <div class="replies" id="replies-${c.MaDanhGia}"></div>
        `;
        commentList.appendChild(div);
        loadReplies(c.MaDanhGia);
      });
    })
    .catch(err => console.error('Lỗi load bình luận:', err));
}

// Lấy phản hồi của từng bình luận
function loadReplies(commentId) {
  const replyContainer = document.getElementById(`replies-${commentId}`);
  fetch('/api/replybinhluan')
    .then(r => r.json())
    .then(replies => {
      const list = replies.filter(r => r.MaRepBinhLuan == commentId);
      replyContainer.innerHTML = '';
      list.forEach(r => {
        const p = document.createElement('p');
        p.innerHTML = `<strong>${r.TaiKhoan}:</strong> ${r.NoiDung}`;
        replyContainer.appendChild(p);
      });
    })
    .catch(err => console.error('Lỗi load phản hồi:', err));
}
// Load lần đầu
loadComments();
