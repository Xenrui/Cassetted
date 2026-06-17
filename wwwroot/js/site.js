// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('click', function (e) {
    const card = e.target.closest('.c-review-card[data-href]');
    if (!card) return;
    if (e.target.closest('a, button, input, textarea, form')) return;
    window.location.href = card.dataset.href;
});
