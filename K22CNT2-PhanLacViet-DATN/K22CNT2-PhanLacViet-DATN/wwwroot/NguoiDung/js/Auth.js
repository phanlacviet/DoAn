document.addEventListener("DOMContentLoaded", function () {
    const authForm = document.querySelector(".auth-box form");
    if (authForm) {
        const submitBtn = authForm.querySelector("button");
        authForm.addEventListener("submit", function () {
            submitBtn.innerHTML = "Đang xử lý...";
            submitBtn.style.opacity = "0.7";
            submitBtn.style.pointerEvents = "none";
        });
    }
    const tabButtons = document.querySelectorAll(".tab-btn");
    const tabContents = document.querySelectorAll(".tab-content");

    if (tabButtons.length > 0) {
        tabButtons.forEach(button => {
            button.addEventListener("click", function () {
                const targetId = this.getAttribute("data-target");
                tabButtons.forEach(btn => btn.classList.remove("active"));
                tabContents.forEach(content => content.classList.remove("active"));
                this.classList.add("active");
                const targetContent = document.getElementById(targetId);
                if (targetContent) {
                    targetContent.classList.add("active");
                }
            });
        });
    }
});