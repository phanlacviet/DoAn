let synth = window.speechSynthesis;
let voices = [];
let currentParagraphIndex = 0;
let paragraphs = [];
let isPaused = false;
let currentUtterance = null;

document.addEventListener("DOMContentLoaded", function () {
    highlightReadChapters();
    initTTS();
    const shouldAutoStart = localStorage.getItem('autoStartTTS');
    if (shouldAutoStart === 'true') {
        localStorage.removeItem('autoStartTTS');
        setTimeout(() => {
            startReading();
        }, 1000);
    }
});

function initTTS() {
    loadVoices();
    if (speechSynthesis.onvoiceschanged !== undefined) {
        speechSynthesis.onvoiceschanged = loadVoices;
    }
    const contentDiv = document.querySelector('.chapter-content');
    if (contentDiv) {
        paragraphs = Array.from(contentDiv.querySelectorAll('p'));
        if (paragraphs.length === 0) {
            const tempP = document.createElement('p');
            tempP.innerText = contentDiv.innerText;
            paragraphs = [tempP];
        }
        paragraphs.forEach((p, index) => {
            p.style.cursor = "pointer";
            p.addEventListener('click', () => {
                stopReading();
                currentParagraphIndex = index;
                startReading();
            });
        });
    }

    // Gán sự kiện cho các nút điều khiển
    const btnOpen = document.getElementById('btnOpenTTS');
    const btnSpeak = document.getElementById('btnSpeak');
    const btnPause = document.getElementById('btnPause');
    const btnStop = document.getElementById('btnStop');
    const rateRange = document.getElementById('rateRange');

    if (btnOpen) btnOpen.addEventListener('click', toggleTTSPanel);
    if (btnSpeak) btnSpeak.addEventListener('click', startReading);
    if (btnPause) btnPause.addEventListener('click', pauseReading);
    if (btnStop) btnStop.addEventListener('click', stopReading);

    if (rateRange) {
        rateRange.addEventListener('input', function () {
            document.getElementById('rateValue').innerText = this.value + 'x';
            if (synth.speaking && !isPaused) {
                synth.cancel();
                speakParagraph(currentParagraphIndex);
            }
        });
    }
}

function loadVoices() {
    voices = synth.getVoices();
    const voiceSelect = document.getElementById('voiceSelect');
    if (!voiceSelect) return;
    voiceSelect.innerHTML = '';
    const vietnameseVoices = voices.filter(v =>
        v.lang === 'vi-VN' ||
        v.lang === 'vi_VN' ||
        v.lang.startsWith('vi')
    );
    if (vietnameseVoices.length === 0) {
        const option = document.createElement('option');
        option.textContent = '⚠ Trình duyệt không hỗ trợ giọng tiếng Việt';
        option.disabled = true;
        voiceSelect.appendChild(option);
        return;
    }
    vietnameseVoices.forEach((voice, index) => {
        const option = document.createElement('option');
        option.value = voice.name;
        option.textContent = voice.name;
        if (index === 0) option.selected = true;
        voiceSelect.appendChild(option);
    });
    voices = vietnameseVoices;
}


function toggleTTSPanel() {
    const panel = document.getElementById('ttsPanel');
    if (panel) panel.style.display = (panel.style.display === 'none') ? 'block' : 'none';
}

function startReading() {
    if (isPaused) {
        synth.resume();
        isPaused = false;
        return;
    }
    if (synth.speaking) return;
    speakParagraph(currentParagraphIndex);
}

function speakParagraph(index) {
    if (index >= paragraphs.length) {
        const nextBtn = document.querySelector('a.btn-nav[href*="ChiTietChuong"]:last-child');
        if (nextBtn && !nextBtn.classList.contains('disabled')) {
            localStorage.setItem('autoStartTTS', 'true');
            window.location.href = nextBtn.href;
        } else {
            console.log("Đây là chương cuối cùng của truyện.");
            stopReading();
        }
        return;
    }

    if (index < 0) return;
    paragraphs.forEach(p => p.classList.remove('reading-active'));
    const p = paragraphs[index];
    p.classList.add('reading-active');
    p.scrollIntoView({ behavior: 'smooth', block: 'center' });

    const text = p.innerText.trim();
    if (!text) {
        currentParagraphIndex++;
        speakParagraph(currentParagraphIndex);
        return;
    }

    const utterance = new SpeechSynthesisUtterance(text);
    const selectedVoiceName = document.getElementById('voiceSelect').value;
    const voice = voices.find(v => v.name === selectedVoiceName);
    if (voice) utterance.voice = voice;
    utterance.rate = parseFloat(document.getElementById('rateRange').value);

    utterance.onend = function () {
        currentParagraphIndex++;
        speakParagraph(currentParagraphIndex);
    };

    currentUtterance = utterance;
    synth.speak(utterance);
}

function pauseReading() {
    if (synth.speaking && !isPaused) {
        synth.pause();
        isPaused = true;
    }
}

function stopReading() {
    synth.cancel();
    isPaused = false;
    currentParagraphIndex = 0;
    paragraphs.forEach(p => p.classList.remove('reading-active'));
}

window.onbeforeunload = function () {
    synth.cancel();
};

function highlightReadChapters() {
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
}

function toggleReply(id) {
    const form = document.getElementById("replyForm-" + id);
    if (form.style.display === "block") {
        form.style.display = "none";
    } else {
        document.querySelectorAll('.reply-form').forEach(el => el.style.display = 'none');
        form.style.display = "block";
    }
}

async function postComment(maChuong) {
    if (!currentUser) {
        alert("Vui lòng đăng nhập để bình luận!");
        window.location.href = "/NguoiDung/Truyen/Auth";
        return;
    }
    const content = document.getElementById("txtMainComment").value;
    if (!content.trim()) {
        alert("Nội dung không được để trống!");
        return;
    }
    try {
        const response = await fetch('/api/Truyen/BinhLuan/Them', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaChuongId: maChuong,
                NoiDung: content
            })
        });
        if (response.ok) location.reload();
        else alert("Lỗi khi gửi bình luận.");
    } catch (error) {
        console.error(error);
        alert("Lỗi kết nối.");
    }
}

async function postReply(maBinhLuanGoc) {
    if (!currentUser) {
        alert("Vui lòng đăng nhập!");
        window.location.href = "/NguoiDung/Truyen/Auth";
        return;
    }
    const content = document.getElementById("txtReply-" + maBinhLuanGoc).value;
    if (!content.trim()) {
        alert("Nội dung không được để trống!");
        return;
    }
    try {
        const response = await fetch('/api/Truyen/BinhLuan/TraLoi', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaBinhLuanGoc: maBinhLuanGoc,
                NoiDung: content
            })
        });
        if (response.ok) location.reload();
        else alert("Lỗi khi gửi trả lời.");
    } catch (error) {
        console.error(error);
    }
}
async function saveStory(btn) {
    if (!currentUser) {
        alert("Vui lòng đăng nhập để lưu truyện!");
        window.location.href = "/NguoiDung/Truyen/Auth";
        return;
    }
    const maTruyen = btn.getAttribute('data-truyen');
    try {
        const response = await fetch('/api/Truyen/LuuTruyen', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                TaiKhoan: currentUser,
                MaTruyen: parseInt(maTruyen)
            })
        });
        const result = await response.json();
        if (response.ok && result.success) {
            const icon = btn.querySelector('i');
            if (icon) icon.className = 'fa-solid fa-bookmark';
            btn.classList.add('saved');

            alert("Đã thêm vào tủ truyện của bạn!");
        } else {
            alert(result.message || "Truyện này đã có trong tủ truyện.");
        }
    } catch (error) {
        console.error("Lỗi lưu truyện:", error);
        alert("Lỗi kết nối máy chủ.");
    }
}