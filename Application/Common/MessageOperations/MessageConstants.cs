namespace Application.Common.MessageOperations
{
    public static class MessageConstants
    {
        //message chinh thuc trong srs (message nghiep vu)
        public const string ME001 = "Invalid username/ password. Please try again";
        public const string ME002 = "Required field";
        public const string ME003 = "We've sent an email with the link to reset your password.";
        public const string ME004 = "This link has expired. Please go back to Homepage and try again.";
        public const string ME005 = "The email address doesn’t exist. Please try again.";
        public const string ME006 = "Password and Confirm password don’t match. Please try again.";
        public const string ME007 = "Password must contain at least one number, one numeral, and seven characters.";
        public const string ME008 = "Field must be unique";
        public const string ME009 = "Invalid email address";
        public const string ME010 = "Posting is permitted whilst banned.";
        public const string ME011 = "Post must not contain any inappropriate content";
        public const string ME012 = "Post must not exceed 255 characters";
        public const string ME013 = "Please choose at least one category";
        public const string ME014 = "Please choose at least one genre";
        public const string ME015 = "You’re currently banned, temporarily can only view comments";
        public const string ME016 = "Comment must not contain inappropriate content";
        public const string ME017 = "Comment must not be empty";
        public const string ME018 = "This comment has been deleted.";
        public const string ME019 = "This post has been deleted";
        public const string ME020 = "You’ve reported this post, thanks for your opinion";
        public const string ME021 = "You’re banned, voting is prohibited temporarily";
        public const string ME022 = "The rejection is not approved";
        public const string ME023 = "This post has been deleted";
        public const string ME024 = "Moderator has been demoted to Member";
        public const string ME025 = "Member has been promoted to Moderator";
        public const string ME026 = "Member has been banned";
        public const string ME027 = "Member has been unbanned";
        public const string ME028 = "Ticket verify successful";
        public const string ME029 = "You need to log in first in order to post comments.";

        //message chua co (da phan la validation thi them chu 'N' vao) (cai nay khong co trong BR nhung no thuoc nghiep vu)
        public const string MEN001 = "Username must not exceed 50 characters";
        public const string MEN002 = "Username must have at least 5 characters";
        public const string MEN003 = "Successfully registered, please login again.";
        public const string MEN004 = "This email have already been used";
        public const string MEN005 = "OTP must length must be 6";
        public const string MEN006 = "Invalid OTP";
        public const string MEN007 = "OTP expired";
        public const string MEN008 = "Username taken";
        public const string MEN009 = "Post title must be at least 2 characters.";
        public const string MEN010 = "Must be loggeed in to do this action";
       

        //message nay dung de debug chu khong lien quan gi den BR hay nghiep vu
        public const string MEO001 = "Error reading expiry date";
        public const string MEO002 = "Missing session data";
        public const string MEO003 = "Bio must not exceed 255 characters";
    }
}