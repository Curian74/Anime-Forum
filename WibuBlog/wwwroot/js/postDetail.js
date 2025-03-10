const submitBtn = document.getElementById('submit-btn');
const postId = document.getElementById('post-id');
const userId = document.getElementById('user-id');

const toggleBtn = document.getElementById('toggle-btn');
const cancelBtn = document.getElementById('cancel-btn');
const commentField = document.getElementById('comment-field');

let isCommentFieldOpen = false;
commentField.style.display = "none";

const toggleCommentInput = () => {
    isCommentFieldOpen = !isCommentFieldOpen;
    commentField.style.display = isCommentFieldOpen ? "block" : "none";
    toggleBtn.style.display = !isCommentFieldOpen ? "block" : "none";

    if (!isCommentFieldOpen) {
        editor.setContent('');
    }
};

const confirmDeletePost = (postId) => {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then(async (result) => {
        if (result.isConfirmed) {
            const result = await deletePost(postId, true);
            console.log(result);
            if (result) {
                Swal.fire({
                    title: "Deleted!",
                    text: "This post has been deleted.",
                    confirmButtonText: "Go to post list",
                    icon: "success"
                }).then((res) => {
                    if (res.isConfirmed) {
                        window.location.href = 'https://localhost:7139/post/newPosts'
                    }
                });
            }

            else {
                Swal.fire({
                    title: "Error!",
                    text: "Failed to delete post.",
                    icon: "error"
                });
            }
        }
    });

}

const deletePost = async (postId, isHidden) => {
    try {
        const response = await fetch(`https://localhost:7186/api/Post/Deactivate/${postId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ isHidden }),
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



//const postComment = async () => {
//    const editor = tinymce.get('content');

//    const editorContent = editor.getContent().trim();

//    try {
//        const endpoint = 'https://localhost:7186/api/Comment/PostComment';
//        const response = await fetch(endpoint, {
//            method: "POST",
//            headers: {
//                "Content-type": "application/json",
//            },
//            body: JSON.stringify({
//                userId: userId.value,
//                postId: postId.value,
//                content: editorContent
//            })
//        });

//        if (!response.ok) {
//            throw new Error(response.status);
//        }

//        const result = await response.json();
//        console.log("Success:", result);
//    }
//    catch (err) {
//        console.error("Error posting comment:", err);
//    }
//};

//submitBtn.addEventListener("click", postComment);
toggleBtn.addEventListener("click", toggleCommentInput);
cancelBtn.addEventListener("click", toggleCommentInput)
