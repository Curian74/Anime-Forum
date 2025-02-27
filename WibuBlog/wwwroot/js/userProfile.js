function showTab(tabId) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.add('d-none'));
    document.getElementById(tabId).classList.remove('d-none');
    document.querySelectorAll('.tabs button').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
}