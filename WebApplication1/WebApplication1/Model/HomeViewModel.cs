namespace WebApplication1.Models
{
    public class HomeViewModel
    {
        public List<AppUserInfo> AppUsers { get; set; }
        public List<string> Organizations { get; set; }
    }

    public class AppUserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Add other properties based on your app_user table columns
    }
}