const submitBtn = document.getElementById('submit-btn');
const postId = document.getElementById('post-id');
const userId = document.getElementById('user-id');

const toggleBtn = document.getElementById('toggle-btn');
const cancelBtn = document.getElementById('cancel-btn');
const commentField = document.getElementById('comment-field');

let isCommentFieldOpen = false;
commentField.style.display = "none";

const toggleCommentInput = () => {
    const editor = tinymce.get('content'); //thg nay la async nen tranh khai bao ben ngoai
    isCommentFieldOpen = !isCommentFieldOpen;
    commentField.style.display = isCommentFieldOpen ? "block" : "none";
    toggleBtn.style.display = !isCommentFieldOpen ? "block" : "none";

    if (!isCommentFieldOpen) {
        editor.setContent('');
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
