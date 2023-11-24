using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace WetransferClone.EmailLambda.Services;

public interface IEmailService
{
    Task SendMail(string fileSenderEmail, string toEmail, string downloadUrl, ILambdaContext context);
}