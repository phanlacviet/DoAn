// Frontend/js/chuongtruyen.js
// load header 
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

// Utils
function formatDate(sqlDate) {
  if (!sqlDate) return "";
  const d = new Date(sqlDate);
  if (isNaN(d)) return sqlDate;
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  return `${dd}/${mm}/${yyyy}`;
}

// Read params
const params = new URLSearchParams(window.location.search);
const truyenId = params.get('truyenId');
const chuongId = params.get('chuongId');
let thuTuParam = params.get('thuTu'); // may be null

let currentChapter = null; // store loaded chapter object

// DOM refs
const titleEl = document.querySelector('.chapter-title');
const authorEl = document.querySelector('.chapter-author');
const dateEl = document.querySelector('.chapter-date');
const contentEl = document.querySelector('.chapter-content');
const prevBtn = document.querySelector('.prev-btn');
const tocBtn = document.querySelector('.toc-btn');
const nextBtn = document.querySelector('.next-btn');

const commentInput = document.getElementById('commentInput');
const commentSubmit = document.getElementById('commentSubmit');
const commentList = document.getElementById('commentList');

// Load chapter data
async function loadChapter() {
  try {
    let res, data;
    if (chuongId) {
      res = await fetch(`/api/chuongtruyen/${encodeURIComponent(chuongId)}`);
      if (!res.ok) throw new Error('Không tìm thấy chương (theo id).');
      data = await res.json();
    } else if (truyenId && thuTuParam) {
      res = await fetch(`/api/chuongtruyen/truyen/${encodeURIComponent(truyenId)}/thuTu/${encodeURIComponent(thuTuParam)}`);
      if (!res.ok) throw new Error('Không tìm thấy chương (theo truyen+thuTu).');
      data = await res.json();
    } else if (truyenId) {
      // fallback: lấy chương thuTu=1
      res = await fetch(`/api/chuongtruyen/truyen/${encodeURIComponent(truyenId)}/thuTu/1`);
      data = await res.json();
    } else {
      throw new Error('Thiếu tham số truyenId hoặc chuongId.');
    }

    currentChapter = data;
    renderChapter(data);
    setupNavigation();
    loadCommentsForChapter(data.MaChuongTruyen);
  } catch (err) {
    console.error(err);
    titleEl.textContent = 'Không tìm thấy chương';
    authorEl.textContent = '';
    dateEl.textContent = '';
    contentEl.innerHTML = `<p style="color:#c00;">Lỗi khi tải chương: ${err.message}</p>`;
  }
}

function renderChapter(ch) {
  // ch expected fields: MaChuongTruyen, MaTruyen, ThuTuChuong, TieuDe, NgayDang, NoiDung
  titleEl.textContent = `Chương ${ch.ThuTuChuong}: ${ch.TieuDe || ''}`;
  dateEl.textContent = ch.NgayDang ? `Ngày đăng: ${formatDate(ch.NgayDang)}` : '';
  // get author from truyện if available
  if (truyenId) {
    fetch(`/api/truyen/${encodeURIComponent(truyenId)}`)
      .then(r => r.json())
      .then(t => {
        authorEl.textContent = `Tác giả: ${t.TacGia || ''}`;
      }).catch(() => {
        authorEl.textContent = '';
      });
  } else {
    authorEl.textContent = '';
  }
  // display content (NoiDung may contain HTML)
  contentEl.innerHTML = ch.NoiDung || '<p>(Chưa có nội dung)</p>';
}

// Navigation setup: prev / next / toc
function setupNavigation() {
  if (!currentChapter) return;
  const matruyen = currentChapter.MaTruyen;
  const thuTu = Number(currentChapter.ThuTuChuong);

  // Prev
  const prevThuTu = thuTu - 1;
  if (prevThuTu >= 1) {
    // check existence
    fetch(`/api/chuongtruyen/truyen/${encodeURIComponent(matruyen)}/thuTu/${encodeURIComponent(prevThuTu)}`)
      .then(r => {
        if (r.ok) {
          prevBtn.disabled = false;
          prevBtn.onclick = () => {
            window.location.href = `ChuongTruyen.html?truyenId=${encodeURIComponent(matruyen)}&thuTu=${encodeURIComponent(prevThuTu)}`;
          };
        } else {
          prevBtn.disabled = true;
        }
      }).catch(() => prevBtn.disabled = true);
  } else {
    prevBtn.disabled = true;
  }

  // Next
  const nextThuTu = thuTu + 1;
  fetch(`/api/chuongtruyen/truyen/${encodeURIComponent(matruyen)}/thuTu/${encodeURIComponent(nextThuTu)}`)
    .then(r => {
      if (r.ok) {
        nextBtn.disabled = false;
        nextBtn.onclick = () => {
          window.location.href = `ChuongTruyen.html?truyenId=${encodeURIComponent(matruyen)}&thuTu=${encodeURIComponent(nextThuTu)}`;
        };
      } else {
        nextBtn.disabled = true;
      }
    }).catch(() => nextBtn.disabled = true);

  // Table of contents button
  tocBtn.onclick = () => {
    // go back to Truyen.html with truyenId
    if (matruyen) {
      window.location.href = `Truyen.html?id=${encodeURIComponent(matruyen)}`;
    } else if (truyenId) {
      window.location.href = `Truyen.html?id=${encodeURIComponent(truyenId)}`;
    } else {
      // nothing
      alert('Không xác định mã truyện.');
    }
  };
}

// Comment submit
commentSubmit && commentSubmit.addEventListener('click', async () => {
  const t = (commentInput && commentInput.value || '').trim();
  if (!t) return alert('Vui lòng nhập nội dung bình luận.');
  const user = localStorage.getItem('user');
  if (!user) {
    // redirect to login page (same folder)
    window.location.href = 'Login_Resgister.html';
    return;
  }
  if (!currentChapter || !currentChapter.MaChuongTruyen) {
    return alert('Không xác định chương để bình luận.');
  }

  try {
    const body = {
      MaChuongTruyen: currentChapter.MaChuongTruyen,
      TaiKhoan: user,
      NoiDung: t
    };
    const res = await fetch('/api/binhluan', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    });
    if (!res.ok) throw new Error('Lỗi khi gửi bình luận');
    commentInput.value = '';
    loadCommentsForChapter(currentChapter.MaChuongTruyen);
  } catch (err) {
    console.error(err);
    alert('Gửi bình luận thất bại.');
  }
});

// Load comments for this MaChuongTruyen (and replies)
function loadCommentsForChapter(maChuong) {
  if (!maChuong) return;
  commentList.innerHTML = '<p style="color:#666;">Đang tải bình luận...</p>';
  Promise.all([
    fetch('/api/binhluan').then(r => r.json()),
    fetch('/api/replybinhluan').then(r => r.json())
  ]).then(([binhluanList, repliesList]) => {
    const comments = (Array.isArray(binhluanList) ? binhluanList : []).filter(c => String(c.MaChuongTruyen) === String(maChuong));
    if (comments.length === 0) {
      commentList.innerHTML = '<div class="review-empty"><p>Chưa có bình luận cho chương này.</p></div>';
      return;
    }
    commentList.innerHTML = '';
    comments.forEach(cmt => {
      const div = document.createElement('div');
      div.className = 'comment-item';
      const meta = `<div class="comment-meta"><strong>${escapeHtml(cmt.TaiKhoan)}</strong> — <span>${formatDate(cmt.NgayGui)}</span></div>`;
      const text = `<div class="comment-text">${escapeHtml(cmt.NoiDung)}</div>`;
      const repliesFor = (repliesList || []).filter(r => String(r.MaBinhLuan) === String(cmt.MaBinhLuan));
      let repliesHtml = '';
      if (repliesFor.length) {
        repliesHtml = '<div class="comment-replies">';
        repliesFor.forEach(rep => {
          repliesHtml += `<div class="reply-comment"><div class="comment-meta"><strong>${escapeHtml(rep.TaiKhoan)}</strong> — <span>${formatDate(rep.NgayGui)}</span></div><div class="comment-text">${escapeHtml(rep.NoiDung)}</div></div>`;
        });
        repliesHtml += '</div>';
      }
      div.innerHTML = meta + text + repliesHtml;
      commentList.appendChild(div);
    });
  }).catch(err => {
    console.error('Lỗi khi load bình luận:', err);
    commentList.innerHTML = '<div class="review-empty"><p>Không thể tải bình luận.</p></div>';
  });
}

// minimal escape
function escapeHtml(s) {
  if (s === null || s === undefined) return '';
  return String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;').replace(/'/g,'&#039;');
}

// init
loadChapter();