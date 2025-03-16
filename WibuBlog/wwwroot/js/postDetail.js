const submitBtn = document.getElementById('submit-btn');
const postId = document.getElementById('post-id');
const userId = document.getElementById('user-id');

const toggleBtn = document.getElementById('toggle-btn');
const cancelBtn = document.getElementById('cancel-btn');
const commentField = document.getElementById('comment-field');
const contentField = document.getElementById('content');
const commentForm = document.getElementById('commentForm');
const editCmtField = document.getElementById('edit-comment-section');

let isCommentFieldOpen = false;
commentField.style.display = "none";

document.addEventListener("DOMContentLoaded", function () {
    // Store all editors in a map where key is the editor id and value is the editor instance
    const editors = new Map();

    const mainEditor = new RichTextEditor("#content", configs);
    editors.set("content", mainEditor);

    document.querySelectorAll("[id^='edit-cmt-']").forEach(textarea => {
        const editor = new RichTextEditor(`#${textarea.id}`, configs);
        editors.set(textarea.id, editor);
    });

    // Validate main comment submission
    const validateComment = () => {
        const mainEditor = editors.get("content");
        if (!mainEditor || !mainEditor.getText().trim()) {
            Swal.fire({
                title: "Warning!",
                text: "Content is required.",
                icon: "warning",
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
            });
            return false;
        }
        return true;
    };

    if (submitBtn) {
        submitBtn.addEventListener("click", (e) => {
            if (!validateComment()) {
                e.preventDefault();
            } else {
                commentForm.submit();
            }
        });
    }

    window.getEditor = (id) => editors.get(id);
});


if (editCmtField) {
    editCmtField.style.display = 'none';
}

const openEditCmt = (cmtId) => {
    const editCmtField = document.getElementById(`edit-comment-section-${cmtId}`);
    const commentText = document.getElementById(`comment-text-${cmtId}`);

    if (editCmtField && commentText) {
        editCmtField.style.display = 'block';
        commentText.classList.add('d-none');
    }
};

const cancelEdit = (cmtId) => {
    const editCmtField = document.getElementById(`edit-comment-section-${cmtId}`);
    const commentText = document.getElementById(`comment-text-${cmtId}`);

    if (editCmtField && commentText) {
        editCmtField.style.display = 'none';
        commentText.classList.remove('d-none');
    }
};

const editComment = async (cmtId) => {
    try {
        const editorId = `edit-cmt-${cmtId}`;
        const editor = window.getEditor(editorId);

        // Validate the editor content
        if (!editor || !editor.getText().trim()) {
            Swal.fire({
                title: "Warning!",
                text: "Comment content cannot be empty.",
                icon: "warning",
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
            });
            return;
        }

        const editedContent = editor.getHTML();
        const url = `https://localhost:7186/api/Comment/Update/${cmtId}`;
        const response = await fetch(url, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ content: editedContent }),
            credentials: 'include'
        });

        const result = await response.json();
        if (response.ok) {
            // Update the comment text in the UI
            const commentTextElement = document.querySelector(`#comment-text-${cmtId}`);

            commentTextElement.innerHTML = editedContent;

            // Hide the edit section and show the updated comment
            document.querySelector(`#edit-comment-section-${cmtId}`).style.display = 'none';
            commentTextElement.classList.remove('d-none');
        } else {
            console.error("Error updating comment:", result);
            alert(`Failed to update comment: ${result.message}`);
        }
    } catch (err) {
        console.error("Error:", err);
        alert("An error occurred while updating the comment.");
    }
};

const toggleCommentInput = () => {
    isCommentFieldOpen = !isCommentFieldOpen;
    commentField.style.display = isCommentFieldOpen ? "block" : "none";
    toggleBtn.style.display = !isCommentFieldOpen ? "block" : "none";
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
toggleBtn.addEventListener("click", toggleCommentInput);
cancelBtn.addEventListener("click", toggleCommentInput)

$(document).ready(function () {
    const postId = $('#post').data('post-id');

    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', '...');
        }
    });

    // Fetch total post votes
    function fetchTotalVotes() {
        $.ajax({
            url: `https://localhost:7186/api/Vote/GetTotalPostVotes?postId=${postId}`,
            type: 'GET',
            success: function (response) {
                $('#total-vote-count').text(response.value);
            },
            error: function (xhr) {
                console.error('Error fetching total votes:', xhr.responseText);
            }
        });
    }

    // Fetch current user vote
    function fetchUserVote() {
        $.ajax({
            url: `https://localhost:7186/api/Vote/GetCurrentUserVote?postId=${postId}`,
            type: 'GET',
            xhrFields: {
                withCredentials: true
            },
            success: function (response) {
                if (response.value?.isUpvote === true) {
                    $('.upvote-btn').addClass('active');
                    $('.downvote-btn').removeClass('active');
                } else if (response.value?.isUpvote === false) {
                    $('.downvote-btn').addClass('active');
                    $('.upvote-btn').removeClass('active');
                } else {
                    $('.upvote-btn, .downvote-btn').removeClass('active');
                }
            },
            error: function (xhr) {
                console.error('Error fetching user vote:', xhr.responseText);
            }
        });
    }

    // Toggle vote
    function toggleVote(isUpvote) {
        $.ajax({
            url: `https://localhost:7186/api/Vote/ToggleVote`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ postId: postId, isUpvote: isUpvote }),
            xhrFields: {
                withCredentials: true
            },
            success: function (response) {
                fetchTotalVotes();
                fetchUserVote();
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    // User is not logged in - redirect to login page
                    window.location.href = '@Url.Content("~/Auth/Login")';
                } else {
                    console.error('Error modifying vote:', xhr.responseText);
                }
            }
        });
    }

    // Event listeners
    $('.upvote-btn').click(function (e) {
        e.preventDefault();
        toggleVote(true);
    });

    $('.downvote-btn').click(function (e) {
        e.preventDefault();
        toggleVote(false);
    });

    // Initial fetch
    fetchTotalVotes();
    fetchUserVote();
});