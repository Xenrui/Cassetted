document.addEventListener('click', function (e) {
    const card = e.target.closest('[data-href]');
    if (!card) return;
    if (e.target.closest('a, button, input, textarea, form')) return;
    window.location.href = card.dataset.href;
});

document.addEventListener('click', async function (e) {
    const btn = e.target.closest('.c-like-btn');
    if (!btn) return;

    const reviewId = btn.dataset.reviewId;
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const body = new FormData();
    body.append('reviewId', reviewId);
    if (token) body.append('__RequestVerificationToken', token);

    btn.disabled = true;
    try {
        const res = await fetch('/Review/ToggleLike', { method: 'POST', body });
        if (!res.ok) return;
        const data = await res.json();

        btn.classList.toggle('c-like-btn--liked', data.liked);
        const icon = btn.querySelector('.c-like-btn__icon');
        if (icon) icon.setAttribute('fill', data.liked ? 'currentColor' : 'none');
        const count = btn.querySelector('.c-like-count');
        if (count) count.textContent = data.likeCount > 0 ? ` · ${data.likeCount}` : '';
    } catch {
    } finally {
        btn.disabled = false;
    }
});

document.addEventListener('click', async function (e) {
    const btn = e.target.closest('.c-fav-btn');
    if (!btn) return;

    const reviewId = btn.dataset.reviewId;
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const body = new FormData();
    body.append('reviewId', reviewId);
    if (token) body.append('__RequestVerificationToken', token);

    btn.disabled = true;
    try {
        const res = await fetch('/Review/ToggleFavorite', { method: 'POST', body });
        if (!res.ok) return;
        const data = await res.json();

        const icon = btn.querySelector('.c-fav-btn__icon');
        // Restart the tuck animation by removing & re-adding the saved class
        btn.classList.remove('c-fav-btn--saved');
        if (data.favorited) {
            void btn.offsetWidth; // force reflow so the animation re-fires
            btn.classList.add('c-fav-btn--saved');
        }
        if (icon) icon.setAttribute('fill', data.favorited ? 'currentColor' : 'none');
        const count = btn.querySelector('.c-fav-count');
        if (count) count.textContent = data.favoriteCount > 0 ? ` · ${data.favoriteCount}` : '';
    } catch {
    } finally {
        btn.disabled = false;
    }
});
