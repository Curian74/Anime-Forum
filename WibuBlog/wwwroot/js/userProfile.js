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

let updatedFields = {};

function openEditModal(value, label, field) {
	document.getElementById("editModalLabel").innerText = "Edit " + label;
	document.getElementById("editLabel").innerText = label;
	document.getElementById("editInput").value = value;
	document.getElementById("editField").value = field;

	var editModal = new bootstrap.Modal(document.getElementById("editModal"));
	editModal.show();
}

document.getElementById("editModal").addEventListener("shown.bs.modal", function () {
	document.getElementById("editInput").focus();
});

document.querySelector("#editModal .btn-primary").addEventListener("click", function () {
	let field = document.getElementById("editField").value;
	let newValue = document.getElementById("editInput").value;

	if (field) {
		updatedFields[field] = newValue;

		let fieldDisplay = document.getElementById(field + "Display");
		if (fieldDisplay) {
			fieldDisplay.textContent = newValue;
		}

		let inputField = document.querySelector("input[name='" + field + "']");
		if (inputField) {
			inputField.value = newValue; // Cập nhật giá trị input ẩn
		}
	}

	var editModal = bootstrap.Modal.getInstance(document.getElementById("editModal"));
	editModal.hide();
});

document.querySelector("#editPasswordModal .btn-primary").addEventListener("click", function () {
	let oldPassword = document.getElementById("editInput").value;
	let newPassword = document.getElementById("editInput2").value;
	let confirmPassword = document.getElementById("editInput3").value;

	if (newPassword !== confirmPassword) {
		alert("New password and confirmation do not match!");
		return;
	}
	var editPasswordModal = bootstrap.Modal.getInstance(document.getElementById("editPasswordModal"));
	editPasswordModal.hide();
});

document.querySelector("#editModal .btn-primary").addEventListener("click", function () {
	let field = document.getElementById("editField").value;
	let newValue = document.getElementById("editInput").value;

	if (field) {
		updatedFields[field] = newValue;

		let fieldDisplay = document.getElementById(field + "Display");
		if (fieldDisplay) {
			fieldDisplay.textContent = newValue;
		}

		let inputField = document.querySelector("input[name='" + field + "']");
		if (inputField) {
			inputField.value = newValue; // Cập nhật giá trị input ẩn
		}
	}

	var editModal = bootstrap.Modal.getInstance(document.getElementById("editModal"));
	editModal.hide();
});