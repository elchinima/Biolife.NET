namespace Biolife.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var from = emailSettings["From"]!.Trim();
            var password = emailSettings["Password"]!.Replace(" ", string.Empty);
            var host = emailSettings["Host"]!.Trim();
            var port = emailSettings.GetValue<int>("Port");

            using var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var client = new SmtpClient(host, port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
                await client.SendMailAsync(message);
        }
    }
}
