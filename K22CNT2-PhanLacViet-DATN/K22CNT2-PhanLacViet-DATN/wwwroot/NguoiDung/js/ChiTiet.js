document.addEventListener("DOMContentLoaded", function () {
    const btnLoadMore = document.getElementById("btnLoadMore");

    if (btnLoadMore) {
        btnLoadMore.addEventListener("click", function () {
            // Chọn tất cả các chương đang bị ẩn
            const hiddenChapters = document.querySelectorAll(".hidden-chapter");

            // Hiển thị chúng
            hiddenChapters.forEach(function (el) {
                el.classList.remove("hidden-chapter");
            });

            // Ẩn nút xem thêm sau khi đã show hết
            btnLoadMore.style.display = "none";
        });
    }
});