using System.Net;
using System.Net.Mail;

namespace EducatITion.Services
{
    public class EmailService
    {
        const string SENDER = "gejdelevgenij@gmail.com";
        const string PASSWORD = "dmojhxuivzpessth";
        const string SERVICE_NAME = "smtp.gmail.com";
        const int PORT = 587;

        public static void SendEmail(string toEmail, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(SENDER);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(SERVICE_NAME, PORT))
                {
                    smtp.Credentials = new NetworkCredential(SENDER, PASSWORD);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}
