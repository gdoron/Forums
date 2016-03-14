//using System.Threading.Tasks;
//using Microsoft.Extensions.OptionsModel;

//namespace Forums.Services
//{
//    // This class is used by the application to send Email and SMS
//    // when you turn on two-factor authentication in ASP.NET Identity.
//    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    
//    public class AuthMessageSender : IEmailSender, ISmsSender
//    {
//        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
//        {
//            Options = optionsAccessor.Value;
//        }

//        public AuthMessageSenderOptions Options { get; }


//        public Task SendEmailAsync(string email, string subject, string message)
//        {
//            // Plug in your email service here to send an email.
//            var myMessage = new SendGrid.SendGridMessage();
//            myMessage.AddTo(email);
//            myMessage.From = new System.Net.Mail.MailAddress("doron@jifiti.com", "Doron Grinzaig");
//            myMessage.Subject = subject;
//            myMessage.Text = message;
//            myMessage.Html = message;
//            var credentials = new System.Net.NetworkCredential(Options.SendGridUser, Options.SendGridKey);
//            // Create a Web transport for sending email.
//            var transportWeb = new SendGrid.Web(credentials);
//            // Send the email.
//            return transportWeb.DeliverAsync(myMessage);
//        }

//        public Task SendSmsAsync(string number, string message)
//        {
//            // Plug in your SMS service here to send a text message.
//            return Task.FromResult(0);
//        }
//    }
//}
