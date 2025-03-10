function confirmReset(postId) {
    Swal.fire({
        title: "Reset the data?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Post/Edit/${postId}`;
        }
    });
}
