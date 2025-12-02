// Frontend/js/danhgia.js
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

// lấy ID truyện
const params = new URLSearchParams(window.location.search);
const truyenId = params.get("id");

// Format ngày (dùng dd/mm/yyyy)
function formatDate(sqlDate) {
  if (!sqlDate) return "";
  const d = new Date(sqlDate);
  if (isNaN(d)) return sqlDate;
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  return `${dd}/${mm}/${yyyy}`;
}

// XSS-safety minimal
function escapeHtml(str) {
  if (str === null || str === undefined) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;');
}

// render stars (filled / empty spans)
function renderStars(n) {
  n = Number(n) || 0;
  let out = '';
  for (let i = 1; i <= 5; i++) {
    out += `<span class="star ${i <= n ? 'filled' : 'empty'}">★</span>`;
  }
  return out;
}

// Load đánh giá 
fetch('/api/danhgia')
  .then(r => r.json())
  .then(data => {
    const reviewsList = document.getElementById('reviewsList');
    if (!reviewsList) return;


    const list = Array.isArray(data) ? data.filter(rv => String(rv.MaTruyen) === String(truyenId)) : [];

    reviewsList.innerHTML = '';
    if (list.length === 0) {
      reviewsList.innerHTML = `<div class="review-empty"><p>Chưa có đánh giá cho truyện này.</p></div>`;
      return;
    }

    list.forEach(review => {
      const author = review.TaiKhoan || 'Người dùng';
      const ratingNum = Number(review.Diem) || 0;
      const content = review.NoiDung || '';
      const date = formatDate(review.NgayDanhGia);

      const reviewDiv = document.createElement('div');
      reviewDiv.classList.add('review');

      reviewDiv.innerHTML = `
        <div class="review-header">
          <div class="review-author">${escapeHtml(author)}</div>
          <div class="review-rating">${renderStars(ratingNum)}</div>
        </div>
        <div class="review-content">
          <p>${escapeHtml(content)}</p>
          <p class="review-date"><em>${escapeHtml(date)}</em></p>
        </div>
      `;
      reviewsList.appendChild(reviewDiv);
    });
  })
  .catch(err => {
    console.error('Lỗi load đánh giá:', err);
    const reviewsList = document.getElementById('reviewsList');
    if (reviewsList) reviewsList.innerHTML = `<div class="review-empty"><p>Không thể tải đánh giá.</p></div>`;
  });
// quay lại trang truyện
function goBack() {
  window.location.href = `Truyen.html?id=${truyenId}`;
}