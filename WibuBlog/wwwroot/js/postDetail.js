﻿const submitBtn = document.getElementById('submit-btn');
const postId = document.getElementById('post-id');
const userId = document.getElementById('user-id');

const toggleBtn = document.getElementById('toggle-btn');
const cancelBtn = document.getElementById('cancel-btn');
const commentField = document.getElementById('comment-field');
const contentField = document.getElementById('content');
const commentForm = document.getElementById('commentForm');
const editCmtField = document.getElementById('edit-comment-section');

const PAGE_SIZE = 10;

let isCommentFieldOpen = false;

document.addEventListener("DOMContentLoaded", function () {
    const editors = new Map();

    const mainEditor = new RichTextEditor("#content", configs);
    editors.set("content", mainEditor);

    //document.querySelectorAll("[id^='edit-cmt-']").forEach(textarea => {
    //    const editor = new RichTextEditor(`#${textarea.id}`, configs);
    //    editors.set(textarea.id, editor);
    //});

    // Validate main comment submission
    const validateComment = () => {
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
        submitBtn.addEventListener("click", async (e) => {
            if (!validateComment()) {
                e.preventDefault();
            } else {
                // commentForm.submit();
                await postComment();
                toggleCommentInput();
            }
        });
    }

    const postComment = async () => {
        try {
            const endpoint = 'https://localhost:7186/api/Comment/PostComment';
            const response = await fetch(endpoint, {
                method: "POST",
                headers: {
                    "Content-type": "application/json",
                },
                body: JSON.stringify({
                    userId: userId.value,
                    postId: postId.value,
                    content: mainEditor.getText().trim()
                }),
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error(response.status);
            }

            const result = await response.json();
            console.log("Success:", result);
            fetchComments('postId', postId.value, 'createdAt', true);
        }
        catch (err) {
            console.error("Error posting comment:", err);
        }
    };

    window.getEditor = (id) => editors.get(id);

    function toggleCommentInput() {
        isCommentFieldOpen = !isCommentFieldOpen;
        commentField.style.display = isCommentFieldOpen ? "block" : "none";
        mainEditor.setHTMLCode('');
        toggleBtn.style.display = !isCommentFieldOpen ? "block" : "none";
    };

    if(commentField){
        commentField.style.display = "none";
        toggleBtn.addEventListener("click", toggleCommentInput);
        cancelBtn.addEventListener("click", toggleCommentInput)
    }
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

const confirmDeleteComment = (cmtId) => {
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
            const result = await deleteComment(cmtId);
            if (result) {
                Swal.fire({
                    title: "Deleted!",
                    text: "This comment has been deleted.",
                    confirmButtonText: "OK",
                    icon: "success"
                }).then((res) => {
                    if (res.isConfirmed) {
                        fetchComments('postId', postId.value, 'createdAt', true);
                    }
                });
            }

            else {
                Swal.fire({
                    title: "Error!",
                    text: "Failed to delete comment.",
                    icon: "error"
                });
            }
        }
    });
}

const deleteComment = async (cmtId) => {
    try {
        const response = await fetch(`https://localhost:7186/api/Comment/Delete/${cmtId}`, {
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

document.addEventListener("DOMContentLoaded", () => {
    fetchComments('postId', postId.value, 'createdAt', true);
});

let allComments = []; // Store all fetched comments

async function fetchComments(filterBy, searchTerm, orderBy, descending = false) {
    try {
        let page = 1;
        let size = 50; // Fetch in larger chunks
        let hasMore = true;

        allComments = []; // Reset stored comments

        while (hasMore) {
            const response = await fetch(`https://localhost:7186/api/Comment/GetPaged?page=${page}&size=${size}&filterBy=${filterBy}&searchTerm=${searchTerm}&orderBy=${orderBy}&descending=${descending}`);
            const result = await response.json();
            const data = result.value;

            if (!response.ok) {
                throw new Error(data.message || "Failed to load comments.");
            }

            // Store only non-hidden comments
            const visibleComments = data.items.filter(comment => !comment.isHidden);
            allComments.push(...visibleComments);

            hasMore = data.items.length === size; // Stop if fewer than `size` items were returned
            page++; // Move to the next page
        }

        paginateComments(1, PAGE_SIZE); // Start with page 1
    } catch (error) {
        console.error("Error fetching comments:", error);
        document.getElementById("comment-list").innerHTML = `<p>Error loading comments.</p>`;
    }
}

function paginateComments(page, size) {
    const totalComments = allComments.length;
    const totalPages = Math.ceil(totalComments / size);

    const startIndex = (page - 1) * size;
    const endIndex = startIndex + size;
    const paginatedComments = allComments.slice(startIndex, endIndex);

    document.getElementById("comment-count").textContent = totalComments;

    renderComments({ items: paginatedComments }, page, size);
    renderPagination(totalPages, page, size);
}

function renderComments(data, currentPage, size) {
    const commentList = document.getElementById("comment-list");
    commentList.innerHTML = ""; // Clear existing comments

    if (!data.items || data.items.length === 0) {
        commentList.innerHTML = "<p>No comments yet.</p>";
        return;
    }

    data.items
        .filter(c => !c.isHidden) // Only include comments where isHidden is false
        .forEach(c => {
            const commentHtml = `
            <div class="card p-2 mb-2 border">
                <div class="d-flex align-items-start">
                    <img class="rounded-circle avatar me-2"
                        src="${c.user?.profilePhoto?.url || '/images/defaults/user/default_avatar.jpg'}" />
                    <div class="w-100">
                        <div class="d-flex justify-content-between">
                            <div class="d-flex flex-column mb-2">
                                <strong>${c.user?.userName}</strong>
                                <small class="fst-italic" style="font-size: 12px; color:#576f76;">
                                    ${new Date(c.createdAt).toLocaleString()}
                                </small>
                            </div>
                            ${c.userId === userId.value ? `
                                <div class="dropdown">
                                    <button class="btn btn-sm" type="button" data-bs-toggle="dropdown">
                                        <i class="fa-solid fa-ellipsis"></i>
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li><a class="dropdown-item" onclick="openEditCmt('${c.id}'); return false;">Edit</a></li>
                                        <li><hr class="dropdown-divider border-top border-secondary"></li>
                                        <li><a class="dropdown-item" style="color: red !important;" onclick="confirmDeleteComment('${c.id}'); return false;">Delete</a></li>
                                    </ul>
                                </div>
                            ` : ""}
                        </div>
                        <div class="mb-1 comment-text" id="comment-text-${c.id}">${c.content}</div>
                        
                        <!-- Edit Comment Section -->
                        <div id="edit-comment-section-${c.id}" style="display: none;">
                            <textarea id="edit-cmt-${c.id}">${c.content}</textarea>
                            <div class="d-flex justify-content-end mt-2">
                                <button type="button" class="btn btn-secondary me-2" onclick="cancelEdit('${c.id}')">Cancel</button>
                                <button type="button" class="btn btn-primary" onclick="editComment('${c.id}')">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

            commentList.insertAdjacentHTML("beforeend", commentHtml);
        });

    initializeEditors();
}

function renderPagination(totalPages, currentPage, size) {
    const paginationElement = document.getElementById("pagination");
    paginationElement.innerHTML = ""; // Clear pagination

    // if (totalPages <= 1) return;

    let paginationHtml = '';

    if (currentPage > 1) {
        paginationHtml += `<li class="page-item">
            <a class="page-link" href="#" onclick="paginateComments(${currentPage - 1}, ${size}); return false;">&laquo;</a>
        </li>`;
    }

    for (let i = 1; i <= totalPages; i++) {
        paginationHtml += `<li class="page-item ${i === currentPage ? 'active' : ''}">
            <a class="page-link" href="#" onclick="paginateComments(${i}, ${size}); return false;">${i}</a>
        </li>`;
    }

    if (currentPage < totalPages) {
        paginationHtml += `<li class="page-item">
            <a class="page-link" href="#" onclick="paginateComments(${currentPage + 1}, ${size}); return false;">&raquo;</a>
        </li>`;
    }

    paginationElement.innerHTML = paginationHtml;
}

function initializeEditors() {
    const editors = new Map();

    // // Main editor for new comments
    // const mainEditor = new RichTextEditor("#content", configs);
    // editors.set("content", mainEditor);

    // Editors for each dynamically loaded comment edit field
    document.querySelectorAll("[id^='edit-cmt-']").forEach(textarea => {
        const editor = new RichTextEditor(`#${textarea.id}`, configs);
        editors.set(textarea.id, editor);
    });

    window.getEditor = (id) => editors.get(id);
}

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