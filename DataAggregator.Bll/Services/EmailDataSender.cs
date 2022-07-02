using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.Bll.Infrastructure;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace DataAggregator.Bll.Services
{
    public class EmailDataSender : IEmailDataSender
    {
        private readonly EmailCredentials emailCredentials;

        public EmailDataSender(EmailCredentials emailCredentials)
        {
            this.emailCredentials = emailCredentials ?? 
                                    throw new ArgumentNullException(nameof(emailCredentials));
        }

        public async Task SendDataOnEmailAsync(MessageDetails messageDetails)
        {
            if (messageDetails is null)
            {
                throw new ArgumentNullException(nameof(messageDetails));
            }

            if (string.IsNullOrWhiteSpace(messageDetails.TargetEmail))
            {
                throw new ArgumentException("Target email is null or white space.", nameof(messageDetails));
            }

            var emailMessage = this.CreateEmailMessage(messageDetails);

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(this.emailCredentials.SmtpServer, emailCredentials.Port, true);
            await smtpClient.AuthenticateAsync(this.emailCredentials.From, this.emailCredentials.Password);
            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);
        }

        private MimeMessage CreateEmailMessage(MessageDetails messageDetails)
        {
            var csvData = new MimePart()
            {
                ContentType = {
                    MediaType = "text",
                    MediaSubtype = "csv",
                    Charset = "utf8"
                },
                Content = new MimeContent(messageDetails.Data),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName("Updates.csv")
            };

            var body = new TextPart("plain")
            {
                Text = $"Hello {messageDetails.TargetUserName}, here the latest update for your task."
            };

            var multipartContent = new Multipart("mixed");
            multipartContent.Add(body);
            multipartContent.Add(csvData);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(this.emailCredentials.UserName, this.emailCredentials.From));
            emailMessage.To.Add(new MailboxAddress(messageDetails.TargetUserName, messageDetails.TargetEmail));
            emailMessage.Subject = messageDetails.Subject;

            emailMessage.Body = multipartContent;

            return emailMessage;
        }
    }
}
