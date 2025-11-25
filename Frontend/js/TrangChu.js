// Frontend/Js/TrangChu.js

// Frontend/Js/TrangChu.js (phần load header)
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


// Hàm tạo thẻ truyện
function createBookCard(truyen) {
  const card = document.createElement('div');
  card.className = 'book-card';

  const img = document.createElement('img');
  img.src = '/Images/template.jpg'; // tuyệt đối
  img.alt = 'Bìa truyện';
  img.style.cursor = 'pointer';
  img.addEventListener('click', () => {
    window.location.href = `Truyen.html?id=${encodeURIComponent(truyen.MaTruyen)}`;
  });

  const title = document.createElement('div');
  title.className = 'book-title';
  const a = document.createElement('a');
  a.href = `Truyen.html?id=${encodeURIComponent(truyen.MaTruyen)}`;
  a.textContent = truyen.TenTruyen || 'Không có tên';
  a.style.textDecoration = 'none';
  a.style.color = 'inherit';
  title.appendChild(a);

  card.appendChild(img);
  card.appendChild(title);
  return card;
}

function categorize(truyens) {
  const cat = { truyen_dich: [], sang_tac: [], ai_dich: [] };
  truyens.forEach(t => {
    const lt = (t.LoaiTruyen || '').toLowerCase();
    if (lt.includes('ai')) cat.ai_dich.push(t);
    else if (lt.includes('sáng') || lt.includes('sang')) cat.sang_tac.push(t);
    else if (lt.includes('dịch') || lt.includes('dich')) cat.truyen_dich.push(t);
    else cat.sang_tac.push(t);
  });
  return cat;
}

function renderGrid(gridId, list, limit = 8) {
  const grid = document.getElementById(gridId);
  if (!grid) return;
  grid.innerHTML = '';
  list.slice(0, limit).forEach(t => {
    grid.appendChild(createBookCard(t));
  });
}

const API_URL = '/api/truyen';

fetch(API_URL)
  .then(r => {
    if (!r.ok) throw new Error('Không thể lấy dữ liệu');
    return r.json();
  })
  .then(data => {
    const cat = categorize(data || []);
    renderGrid('grid-truyen-dich', cat.truyen_dich);
    renderGrid('grid-sang-tac', cat.sang_tac);
    renderGrid('grid-ai-dich', cat.ai_dich);
  })
  .catch(err => {
    console.error('Lỗi khi load truyện:', err);
  });
