using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace WetransferClone.EmailLambda.Services;

public class SmtpEmailService : IEmailService
{
    public Task SendMail(string fileSenderEmail, string toEmail, string downloadUrl, ILambdaContext context)
    {
        context.Logger.LogInformation("Sending email...");
        
        string senderEmail = "";
        string senderPassword = "";

        string smtpServer = "";
        int smtpPort = 587;

        string subject = "WeTransfer Clone";
        string body = $"{fileSenderEmail} size bir dosya gönderdi. Dosyanız hazır indirebilirsiniz. " + downloadUrl;

        try
        {
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                using (MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, body))
                {
                    client.Send(mailMessage);
                    context.Logger.LogInformation("Email sent.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            context.Logger.LogError(e.StackTrace + e.Message + e.InnerException);
            throw;
        }
        
        context.Logger.LogInformation("SendMail done");
        return Task.CompletedTask;
    }
}