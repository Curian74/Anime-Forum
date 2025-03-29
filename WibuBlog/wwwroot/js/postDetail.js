const submitBtn = document.getElementById('submit-btn');
const postId = document.getElementById('post-id');
const userId = document.getElementById('user-id');

const toggleBtn = document.getElementById('toggle-btn');
const cancelBtn = document.getElementById('cancel-btn');
const commentField = document.getElementById('comment-field');
const contentField = document.getElementById('content');
const commentForm = document.getElementById('commentForm');
const editCmtField = document.getElementById('edit-comment-section');
const commentSection = document.getElementById('comment-section');
const isBanned = document.getElementById('isBanned');

const isHiddenPost = document.getElementById('isHiddenPost').value;

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
        if (!mainEditor) return false;

        const htmlContent = mainEditor.getHTML().trim();
        const textContent = mainEditor.getText().trim();

        if (!htmlContent || (!textContent && !htmlContent.includes("<img"))) {
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
                    content: mainEditor.getHTML().trim()
                }),
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error(response.status);
            }

            const result = await response.json();
            console.log("Success:", result);
            window.location.reload();

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

    if (commentField) {
        commentField.style.display = "none";
        toggleBtn.addEventListener("click", toggleCommentInput);
        cancelBtn.addEventListener("click", toggleCommentInput)
    }
});


if (editCmtField) {
    editCmtField.style.display = 'none';
}

const openReplyCmt = (cmtId) => {
    const replyField = document.getElementById(`reply-comment-section-${cmtId}`);

    if (replyField) {
        replyField.style.display = 'block';
    }
};

const openEditCmt = (cmtId) => {
    const editCmtField = document.getElementById(`edit-comment-section-${cmtId}`);
    const commentText = document.getElementById(`comment-text-${cmtId}`);

    if (editCmtField && commentText) {
        editCmtField.style.display = 'block';
        commentText.classList.add('d-none');
    }
};

const cancelReply = (cmtId) => {
    const replyField = document.getElementById(`reply-comment-section-${cmtId}`);
    // const commentText = document.getElementById(`reply-text-${cmtId}`);

    if (replyField) {
        replyField.style.display = 'none';
        // commentText.classList.remove('d-none');
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

const replyComment = async (parentId, id) => {

    try {
        const editorId = `reply-cmt-${id}`;
        const editor = window.getEditor(editorId);

        if (!editor) return false;

        const htmlContent = editor.getHTML().trim();
        const textContent = editor.getText().trim();

        // Validate the editor content
        if (!htmlContent || (!textContent && !htmlContent.includes("<img"))) {
            Swal.fire({
                title: "Warning!",
                text: "Content is required.",
                icon: "warning",
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
            });
            return;
        }
        const endpoint = 'https://localhost:7186/api/Comment/PostComment';
        const response = await fetch(endpoint, {
            method: "POST",
            headers: {
                "Content-type": "application/json",
            },
            body: JSON.stringify({
                userId: userId.value,
                postId: postId.value,
                content: editor.getHTML().trim(),
                parentCommentId: parentId,
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

const editComment = async (cmtId) => {
    try {
        const editorId = `edit-cmt-${cmtId}`;
        const editor = window.getEditor(editorId);

        if (!editor) return;

        const htmlContent = editor.getHTML().trim();
        const textContent = editor.getText().trim();

        // Validate the editor content
        if (!htmlContent || (!textContent && !htmlContent.includes("<img"))) {
            Swal.fire({
                title: "Warning!",
                text: "Content is required.",
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

            // Filter to include only top-level (non-reply) comments that are not hidden
            const visibleComments = data.items.filter(comment => !comment.isHidden && comment.parentCommentId == null);
            allComments.push(...visibleComments);
            console.log(allComments)

            hasMore = data.items.length === size; // Stop if fewer than `size` items were returned
            page++; // Move to the next page
        }

        if (allComments.length > 0) {
            paginateComments(1, PAGE_SIZE); // Start with page 1
            return allComments.length;
        }

        const commentSection = document.getElementById("comment-section");

        // Clear existing comments
        commentSection.innerHTML = `<div class="d-flex align-items-center mt-5 gap-3">
                 <img src="https://www.redditstatic.com/shreddit/assets/thinking-snoo.png" 
                     alt="No comments yet" style="width: 50px; height: auto;" />
                 <div style="font-size: 14px;">
                     <h5 style="font-weight: 500; margin-bottom: 5px;">Be the first to comment</h5>
                     <p class="mb-0">Nobody's responded to this post yet.</p>
                     <p class="mb-0">Add your thoughts and get the conversation going.</p>
                 </div>
             </div>`;

        // Handle empty state
        commentSection.classList = '';

        return 0;
    } catch (error) {
        console.error("Error fetching comments:", error);
        document.getElementById("comment-list").innerHTML = `<p>Error loading comments.</p>`;
        return 0;
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

const fetchChildComments = async (parentCommentId) => {
    try {
        const url = `https://localhost:7186/api/Comment/GetAll?filterBy=parentCommentId&searchTerm=${parentCommentId}&descending=false`
        const response = await fetch(url);
        const result = await response.json();
        const data = result.value;

        return data;
    }

    catch (err) {
        console.log(err)
    }
}

async function renderComments(data, currentPage, size) {
    const commentList = document.getElementById("comment-list");
    const commentSection = document.getElementById("comment-section");

    // Clear existing comments
    commentList.innerHTML = "";

    // Handle empty state
    if (!data.items || data.items.length === 0) {
        commentSection.classList = ''; //Trick lo?d
        commentSection.innerHTML = `
             <div class="d-flex align-items-center mt-5 gap-3">
                 <img src="https://www.redditstatic.com/shreddit/assets/thinking-snoo.png" 
                     alt="No comments yet" style="width: 50px; height: auto;" />
                 <div style="font-size: 14px;">
                     <h5 style="font-weight: 500; margin-bottom: 5px;">Be the first to comment</h5>
                     <p class="mb-0">Nobody's responded to this post yet.</p>
                     <p class="mb-0">Add your thoughts and get the conversation going.</p>
                 </div>
             </div>`;
        return;
    }

    // Render parent comments
    for (const c of data.items.filter(c => !c.isHidden && c.parentCommentId == null)) {
        const childComments = await fetchChildComments(c.id);

        const commentHtml = `
            <div class="card p-2 mb-2 border">
                <div class="d-flex align-items-start">
                    <img class="rounded-circle avatar me-2"
                        src="${c.user?.profilePhoto?.url || '/images/defaults/user/default_avatar.jpg'}" 
                        alt="${c.user?.userName}'s avatar"  onclick="window.location.href='/MemberProfile/${c.user.id}'" />
                    <div class="w-100">
                        <div class="d-flex justify-content-between">
                            <div>
                                <div style="display: flex; align-items: center; gap: 10px;">
                                    <strong class="m-0">${c.user?.userName}</strong>
                                    <span class="badge bg-secondary">${c.user.rank?.name || "No Rank"}</span>
                                </div>
                                <small class="text-muted">${new Date(c.createdAt).toLocaleString()}</small>
                            </div>
                            
                            ${(c.userId === userId.value && !isHiddenPost && isBanned === false) ? `
                                <div class="dropdown">
                                    <button class="btn btn-sm" type="button" data-bs-toggle="dropdown">
                                        <i class="fa-solid fa-ellipsis"></i>
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li><a class="dropdown-item" onclick="openEditCmt('${c.id}'); return false;">Edit</a></li>
                                        <li><hr class="dropdown-divider border-top border-secondary"></li>
                                        <li><a class="dropdown-item text-danger" onclick="confirmDeleteComment('${c.id}'); return false;">Delete</a></li>
                                    </ul>
                                </div>
                            ` : ''}
                        </div>
                        <div class="mb-1 comment-text" id="comment-text-${c.id}">${c.content}</div>

                         ${(isBanned?.value === false && !isHiddenPost && userId?.value) ? `
                           <button onclick="openReplyCmt('${c.id}')">
                               <i style="font-size: 10px; margin-right: 5px;" class="fa-solid fa-reply"></i>
                               Reply
                           </button>
                        ` : ''}

                        <div id="reply-comment-section-${c.id}" style="display: none;">
                            <textarea id="reply-cmt-${c.id}" class="form-control"></textarea>
                            <div class="d-flex justify-content-end mt-2">
                                <button type="button" class="me-2" onclick="cancelReply('${c.id}')">Cancel</button>
                                <button type="button" style="background-color: #003584; color: white;" onclick="replyComment('${c.id}', '${c.id}')">Save</button>
                            </div>
                        </div>

                        <!-- Edit Comment Section -->
                        <div id="edit-comment-section-${c.id}" style="display: none;">
                            <textarea id="edit-cmt-${c.id}" class="form-control">${c.content}</textarea>
                            <div class="d-flex justify-content-end mt-2">
                                <button type="button" class="me-2" onclick="cancelEdit('${c.id}')">Cancel</button>
                                <button type="button" style="background-color: #003584; color: white;" onclick="editComment('${c.id}')">Save</button>
                            </div>
                        </div>

                        <!-- Child Comments Section -->
                        <div id="child-comments-${c.id}">
                            ${renderChildComments(childComments)}
                        </div>
                    </div>
                </div>
            </div>`;

        commentList.insertAdjacentHTML("beforeend", commentHtml);
    }

    initializeEditors();
}

// Helper function to render child comments
function renderChildComments(childComments, parentId) {
    if (!childComments || childComments.length === 0) {
        return '';
    }

    return childComments
        .filter(child => !child.isHidden)
        .map(child => `
        <div class="card p-2 mb-2 border ml-4">
            <div class="d-flex align-items-start">
                <img class="rounded-circle avatar me-2"
                    src="${child.user?.profilePhoto?.url || '/images/defaults/user/default_avatar.jpg'}" 
                    alt="${child.user?.userName}'s avatar" onclick="window.location.href='/MemberProfile/${child.user.id}'" />
                <div class="w-100">
                    <div class="d-flex justify-content-between">
                        <div>
                                <div style="display: flex; align-items: center; gap: 10px;">
                                    <strong class="m-0">${child.user?.userName}</strong>
                                    <span class="badge bg-secondary">${child.user.rank?.name || "No Rank"}</span>
                                </div>
                                <small class="text-muted">${new Date(child.createdAt).toLocaleString()}</small>
                            </div>
                        ${(child.userId === userId.value && !isHiddenPost && isBanned === false) ? `
                            <div class="dropdown">
                                <button class="btn btn-sm" type="button" data-bs-toggle="dropdown">
                                    <i class="fa-solid fa-ellipsis"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a class="dropdown-item" onclick="openEditCmt('${child.id}'); return false;">Edit</a></li>
                                    <li><hr class="dropdown-divider border-top border-secondary"></li>
                                    <li><a class="dropdown-item text-danger" onclick="confirmDeleteComment('${child.id}'); return false;">Delete</a></li>
                                </ul>
                            </div>
                        ` : ''}
                    </div>
                    <div class="mb-1 comment-text" id="comment-text-${child.id}">${child.content}</div>

                    <!-- Edit Comment Section for Child -->
                    <div id="edit-comment-section-${child.id}" style="display: none;">
                        <textarea id="edit-cmt-${child.id}" class="form-control">${child.content}</textarea>
                        <div class="d-flex justify-content-end mt-2">
                            <button type="button" class="me-2" onclick="cancelEdit('${child.id}')">Cancel</button>
                            <button type="button" style="background-color: #003584; color: white;" onclick="editComment('${child.id}')">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
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

    document.querySelectorAll("[id^='reply-cmt-']").forEach(textarea => {
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
    console.log(isBanned);
    // Event listeners
    if (userId.value && !isHiddenPost && isBanned?.value === false) {
        $('.upvote-btn').click(function (e) {
            e.preventDefault();
            toggleVote(true);
        });

        $('.downvote-btn').click(function (e) {
            e.preventDefault();
            toggleVote(false);
        });
    }

    // Initial fetch
    fetchTotalVotes();
    fetchUserVote();
});