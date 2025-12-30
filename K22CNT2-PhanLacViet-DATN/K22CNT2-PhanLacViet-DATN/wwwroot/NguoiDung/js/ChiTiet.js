document.addEventListener("DOMContentLoaded", function () {
    const btnLoadMore = document.getElementById("btnLoadMore");
    highlightReadChapters();
    if (btnLoadMore) {
        btnLoadMore.addEventListener("click", function () {
            const hiddenChapters = document.querySelectorAll(".hidden-chapter");
            hiddenChapters.forEach(function (el) {
                el.classList.remove("hidden-chapter");
            });
            btnLoadMore.style.display = "none";
        });
    }
});
// 1. CHỨC NĂNG THEO DÕI
async function toggleFollow(maTruyen) {
    if (!currentUser) {
        alert("Vui lòng đăng nhập để theo dõi truyện!");
        return;
    }

    try {
        const response = await fetch('/api/Truyen/TheoDoi', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaTruyen: maTruyen
            })
        });

        if (response.ok) {
            const data = await response.json();
            updateFollowButton(data.daTheoDoi);
        } else {
            alert("Có lỗi xảy ra khi theo dõi.");
        }
    } catch (error) {
        console.error("Lỗi:", error);
    }
}
function highlightReadChapters() {
    // maxReadOrder được lấy từ biến toàn cục định nghĩa ở View
    if (typeof maxReadOrder === 'undefined') return;

    const chapters = document.querySelectorAll('.chapter-item');
    chapters.forEach(chap => {
        const orderAttr = chap.getAttribute('data-thutu');
        if (orderAttr) {
            const order = parseInt(orderAttr);
            if (order <= maxReadOrder && maxReadOrder > 0) {
                chap.classList.add('read');
            } else {
                chap.classList.remove('read');
            }
        }

    });
};
function updateFollowButton(isFollowed) {
    const btn = document.getElementById("btnFollow");
    const icon = btn.querySelector("i");
    const text = document.getElementById("txtFollow");

    if (isFollowed) {
        btn.classList.add("active");
        icon.classList.remove("fa-regular"); 
        icon.classList.add("fa-solid");      
        text.innerText = "Đang theo dõi";
    } else {
        btn.classList.remove("active");
        icon.classList.remove("fa-solid");   
        icon.classList.add("fa-regular");    
        text.innerText = "Theo dõi";
    }
}

// 2. CHỨC NĂNG ĐÁNH GIÁ (MODAL)
function openRatingModal() {
    if (!currentUser) {
        alert("Vui lòng đăng nhập để đánh giá!");
        window.location.href = "/NguoiDung/Auth";
        return;
    }
    document.getElementById("ratingModal").style.display = "flex";
}

function closeRatingModal() {
    document.getElementById("ratingModal").style.display = "none";
    // Reset form
    document.getElementById("reviewContent").value = "";
    const checked = document.querySelector('input[name="rating"]:checked');
    if (checked) checked.checked = false;
}

async function submitReview(maTruyen) {
    const ratingInput = document.querySelector('input[name="rating"]:checked');
    const content = document.getElementById("reviewContent").value;

    if (!ratingInput) {
        alert("Vui lòng chọn số sao!");
        return;
    }

    const diem = parseInt(ratingInput.value);

    try {
        const response = await fetch('/api/Truyen/DanhGia', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaTruyen: maTruyen,
                Diem: diem,
                NoiDung: content
            })
        });

        if (response.ok) {
            alert("Cảm ơn bạn đã đánh giá!");
            closeRatingModal();
            location.reload(); // Tải lại trang để cập nhật điểm TB
        } else {
            alert("Lỗi khi gửi đánh giá.");
        }
    } catch (error) {
        console.error("Lỗi:", error);
    }
}

// Đóng modal khi click ra ngoài vùng trắng
window.onclick = function (event) {
    const modal = document.getElementById("ratingModal");
    if (event.target == modal) {
        closeRatingModal();
    }
}