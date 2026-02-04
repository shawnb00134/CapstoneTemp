namespace CAMCMSServer.Model.Requests
{
    public class UserAddRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
}
