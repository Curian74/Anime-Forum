const confirmBanUser = (userId) => {
    Swal.fire({
        title: "Are you sure?",
        text: "Are you sure you want to continue?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then(async (result) => {
        if (result.isConfirmed) {
            const result = await banUser(userId);
            console.log(result);
            if (result) {
                Swal.fire({
                    title: "Success!",
                    text: "Success.",
                    confirmButtonText: "OK",
                    icon: "success"
                }).then((res) => {
                    if (res.isConfirmed) {
                        window.location.href = 'https://localhost:7139/User/UserList'
                    }
                });
            }

            else {
                Swal.fire({
                    title: "Error!",
                    text: "Failed to ban user.",
                    icon: "error"
                });
            }
        }
    });
}

const banUser = async (userId) => {
    try {
        const response = await fetch(`https://localhost:7186/api/User/ToggleBan/${userId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include' // Allow cookies to be sent with the request
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        console.log("Success");
        return true;
    } catch (error) {
        console.error("Error:", error);
        return false;
    }
};