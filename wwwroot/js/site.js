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
