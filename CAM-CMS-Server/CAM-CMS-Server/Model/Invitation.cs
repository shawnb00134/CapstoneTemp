namespace CAMCMSServer.Model;


public class Invitation
{
    public int InvitationId { get; set; }
    public string Summary { get; set; }
    public int OrganizationId { get; set; }
    public int ModuleViewLimit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Organization Organization { get; set; }
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }
}
public class InvitationRequest
{
    public string Summary { get; set; }
    public int OrganizationId { get; set; }
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }
    public int ModuleViewLimit { get; set; }
    public int AppUserId { get; set; }
}

public class UserSignUpRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}