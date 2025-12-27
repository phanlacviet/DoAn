// Toggle hiển thị form trả lời
function toggleReply(id) {
    const form = document.getElementById("replyForm-" + id);
    if (form.style.display === "block") {
        form.style.display = "none";
    } else {
        // Ẩn tất cả các form khác trước khi hiện cái này (tùy chọn)
        document.querySelectorAll('.reply-form').forEach(el => el.style.display = 'none');
        form.style.display = "block";
    }
}

// Đăng bình luận cho chương
async function postComment(maChuong) {
    if (!currentUser) {
        alert("Vui lòng đăng nhập để bình luận!");
        window.location.href = "/NguoiDung/Auth";
        return;
    }

    const content = document.getElementById("txtMainComment").value;
    if (!content.trim()) {
        alert("Nội dung không được để trống!");
        return;
    }

    try {
        const response = await fetch('/api/Truyen/BinhLuan/Them', { // Đảm bảo API này tồn tại
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaChuongId: maChuong, // ID chương
                NoiDung: content
            })
        });

        if (response.ok) {
            location.reload();
        } else {
            alert("Lỗi khi gửi bình luận.");
        }
    } catch (error) {
        console.error(error);
        alert("Lỗi kết nối.");
    }
}

// Gửi trả lời bình luận (Rep)
async function postReply(maBinhLuanGoc) {
    if (!currentUser) {
        alert("Vui lòng đăng nhập!");
        window.location.href = "/NguoiDung/Auth";
        return;
    }

    const content = document.getElementById("txtReply-" + maBinhLuanGoc).value;
    if (!content.trim()) {
        alert("Nội dung không được để trống!");
        return;
    }

    try {
        const response = await fetch('/api/Truyen/BinhLuan/TraLoi', { // API Rep
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaBinhLuanGoc: maBinhLuanGoc,
                NoiDung: content
            })
        });

        if (response.ok) {
            location.reload();
        } else {
            alert("Lỗi khi gửi trả lời.");
        }
    } catch (error) {
        console.error(error);
    }
}