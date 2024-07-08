namespace BusTrackBookAPIs.Model
{
    public class UserDetails
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}