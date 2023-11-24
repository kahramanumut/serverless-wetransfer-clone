using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace WetransferClone.EmailLambda.Services;

public class FakeEmailService : IEmailService
{
    public Task SendMail(string fileSenderEmail, string toEmail, string downloadUrl, ILambdaContext context)
    {
        context.Logger.LogLine ($"{fileSenderEmail} sent you a new file. You can download right now, URL : {downloadUrl}");
        return Task.CompletedTask;
    }
}