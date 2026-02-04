using CAMCMSServer.Model;
using MailKit.Net.Smtp;
using MimeKit;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CAMCMSServer.Utils.Notification;

public class NotificationSender
{
    #region Data members

    private readonly MailboxAddress fromAddress = new("", "");

    private readonly SmtpClient smtpClient;

    private readonly string? hostName;
    private readonly int port;
    private readonly IConfiguration configuration;

    #endregion

    #region Constructors

    /// <summary>
    ///     Constructor for the NotificationSender class.
    /// </summary>
    /// <param name="configuration">Configuration containing credentials.</param>
    /// <exception cref="ArgumentException">Throws an argument exception if the configuration is invalid.</exception>
    public NotificationSender(IConfiguration configuration)
    {
        this.smtpClient = new SmtpClient();
        this.hostName = configuration.GetSection("NotificationSettings")["Host"];
        if (this.hostName == null)
        {
            throw new ArgumentException("Invalid host name");
        }

        var portString = configuration.GetSection("NotificationSettings")["Port"];
        if (!int.TryParse(portString, out this.port))
        {
            throw new ArgumentException("Invalid port number");
        }

        this.configuration = configuration;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Sends a notification to a user through email or SMS.
    ///     Attempts to send an email if the user has an email address,
    ///     then attempts to send an SMS if the user has a phone number.
    ///     Supports html and plain text messages.
    /// </summary>
    /// <param name="user">The user to send notification</param>
    /// <param name="subject">The subject line of an email or SMS text</param>
    /// <param name="body">The body of an email.</param>
    /// <returns>True if successfully sent, false otherwise.</returns>
    public async Task<bool> SendNotification(User user, string subject, string body)
    {

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            return await this.sendEmail(user, subject, body); 
        }
        else if (!string.IsNullOrWhiteSpace(user.Phone))
        {
            return await this.sendSms(user, subject);
        }

        return false;
    }

    private async Task<bool> sendSms(User user, string subject)
    {
        var accountSid = this.configuration.GetSection("NotificationSettings")["SMS Sid"];
        var authToken = this.configuration.GetSection("NotificationSettings")["SMS Token"];
        var fromNumber = this.configuration.GetSection("NotificationSettings")["SMS From"];
        TwilioClient.Init(accountSid, authToken);

        try
        {
            var message = await MessageResource.CreateAsync(
                body: subject,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber(user.Phone)

            );
            return message.ErrorCode == null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> sendEmail(User user, string subject, string body)
    {
        var toAddress = new MailboxAddress(user.Firstname + " " + user.Lastname, user.Email);
        var message = new MimeMessage();
        message.From.Add(this.fromAddress);
        message.To.Add(toAddress);
        message.Subject = subject;
        message.Body = this.buildBody(body);
        // message.Body = new TextPart("html")
        // {
        //     Text = body
        // };

        var username = this.configuration.GetSection("NotificationSettings")["Username"];
        var password = this.configuration.GetSection("NotificationSettings")["Password"];
        try
        {
            using (this.smtpClient)
            {
                await this.smtpClient.ConnectAsync(this.hostName, this.port);
                await this.smtpClient.AuthenticateAsync(username, password);
                await this.smtpClient.SendAsync(message);
                await this.smtpClient.DisconnectAsync(true);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    private MimeEntity buildBody(string body)
    {
        var builder = new BodyBuilder
        {
            HtmlBody = body,
            TextBody = body
        };
        return builder.ToMessageBody();
    }

    #endregion
}