
namespace SS.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "User","Admin","SuperAdmin"
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        //this is for email varification
        public bool IsEmailVerified { get; set; }
        public string EmailVerificationToken { get; set; }
        //Forgot password
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }


    }
}
