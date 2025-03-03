document.getElementById("confirmLogout").addEventListener("click", function () {
    document.getElementById("logoutForm").submit();

    document.addEventListener("DOMContentLoaded", function () {
        var dropdownToggle = document.getElementById("userDropdown");
        if (dropdownToggle) {
            dropdownToggle.addEventListener("click", function () {
                var dropdown = new bootstrap.Dropdown(dropdownToggle);
                dropdown.toggle();
            });
        }
    })
});