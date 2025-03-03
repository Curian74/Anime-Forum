function showTab(tabId) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.add('d-none'));
    document.getElementById(tabId).classList.remove('d-none');
    document.querySelectorAll('.tabs button').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
}

document.addEventListener("DOMContentLoaded", function () {
    var avatarContainer = document.querySelector(".avatar-container");
    var avatarInput = document.getElementById("avatarInput");
    var avatarImage = document.getElementById("avatarImage");

    avatarContainer.addEventListener("click", function () {
        avatarInput.click();
    });

    avatarInput.addEventListener("change", function (event) {
        var file = event.target.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                avatarImage.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });
});