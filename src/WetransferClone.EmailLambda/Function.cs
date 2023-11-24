using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using WetransferClone.Core;
using WetransferClone.EmailLambda.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WetransferClone.EmailLambda
{
    public class Function
    {
        public async Task FunctionHandler(S3Event s3event, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("EmailLambda started.");
                var s3Client = new AmazonS3Client(CommonContants.DEFAULT_REGION);
                var s3Entity = s3event.Records[0].S3;
                
                var metaData = await s3Client.GetObjectMetadataAsync(CommonContants.TEMP_BUCKET_NAME, s3Entity.Object.Key);
                
                string senderEmail = metaData.Metadata[CommonContants.METADATA_SENDER_EMAIL];
                string receiverEmail = metaData.Metadata[CommonContants.METADATA_RECEIVER_EMAIL];
                
                var preSignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = CommonContants.TEMP_BUCKET_NAME,
                    Key = s3Entity.Object.Key,
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    Verb = HttpVerb.GET
                };
                string downloadUrl = s3Client.GetPreSignedURL(preSignedUrlRequest);

                context.Logger.LogInformation(
                    $"Download URL: {downloadUrl} Sender Email: {senderEmail} Receiver Email: {receiverEmail}");

                IEmailService emailService = new SmtpEmailService();
                await emailService.SendMail(senderEmail, receiverEmail, downloadUrl, context);
            }
            catch (Exception e)
            {
                context.Logger.LogInformation(e.StackTrace + e.Message + e.InnerException);
                context.Logger.LogError(e.StackTrace + e.Message + e.InnerException);
                throw;
            }
        }
    }
}