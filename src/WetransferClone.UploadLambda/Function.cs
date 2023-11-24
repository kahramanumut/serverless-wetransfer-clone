using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.S3.Model;
using WetransferClone.Core;
using WetransferClone.UploadLambda.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WetransferClone.UploadLambda
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> UploadFunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                FileUploadRequest uploadRequest = JsonSerializer.Deserialize<FileUploadRequest>(request.Body);

                string preSignedUrl = GetPresignedUrl(uploadRequest);
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)System.Net.HttpStatusCode.OK,
                    Body = preSignedUrl,
                    Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)System.Net.HttpStatusCode.InternalServerError,
                    Body = $"Bir hata oluştu: {ex.Message}",
                    Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
                };
            }
        }

        private string GetPresignedUrl(FileUploadRequest uploadRequest)
        {
            var s3Client = new AmazonS3Client(CommonContants.DEFAULT_REGION);

            var preSignedUrlrequest = new GetPreSignedUrlRequest
            {
                BucketName = CommonContants.TEMP_BUCKET_NAME,
                Key = uploadRequest.FileKey,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Verb = HttpVerb.PUT,
            };
            
            preSignedUrlrequest.Metadata.Add(CommonContants.METADATA_SENDER_EMAIL, uploadRequest.SenderEmail);
            preSignedUrlrequest.Metadata.Add(CommonContants.METADATA_RECEIVER_EMAIL, uploadRequest.ReceiverEmail);

            return s3Client.GetPreSignedURL(preSignedUrlrequest);
        }
    }
}