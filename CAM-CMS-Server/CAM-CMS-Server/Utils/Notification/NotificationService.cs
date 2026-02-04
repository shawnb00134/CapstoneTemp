using CAMCMSServer.Model;

namespace CAMCMSServer.Utils.Notification;

/// <summary>
///     This interface defines the methods for sending notifications to users.
/// </summary>
public interface INotificationService
{
    #region Methods

    /// <summary>
    ///     Sends a notification to a user. Implementation should be able to handle
    ///     html and plain text messages.
    /// </summary>
    /// <param name="user">The user to send notification to</param>
    /// <param name="subject">The subject line of an email or SMS text</param>
    /// <param name="body">The body of an email.</param>
    /// <returns>True if successfully sent, false otherwise.</returns>
    Task<bool> SendNotification(User user, string subject, string body);

    #endregion
}

/// <summary>
///     A concrete implementation of the INotificationService interface.
/// </summary>
public class NotificationService : INotificationService
{
    #region Data members

    private readonly IConfiguration configuration;

    private readonly NotificationSender notificationSender;

    #endregion

    #region Constructors

    /// <summary>
    ///     Constructor for the NotificationService class.
    /// </summary>
    /// <param name="configuration">Configuration containing credentials.</param>
    public NotificationService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.notificationSender = new NotificationSender(configuration);
    }

    #endregion

    #region Methods

    public Task<bool> SendNotification(User user, string subject, string body)
    {
        return this.notificationSender.SendNotification(user, subject, body);
    }

    #endregion
}