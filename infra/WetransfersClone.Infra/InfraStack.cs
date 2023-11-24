using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.S3;
using Constructs;
using WetransferClone.Core;

namespace WetransfersClone.Infra;

public class InfraStack : Stack
{
    public InfraStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var appRole = new Role(this, "wetransfer-clone-role", new RoleProps
        {
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
            ManagedPolicies = new []
            {
                ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"),
                ManagedPolicy.FromAwsManagedPolicyName("AmazonS3FullAccess"),
            }
        });
        
        var tempS3Bucket = new Bucket(this, "wetransfer-temp-files", new BucketProps
        {
            BucketName = CommonContants.TEMP_BUCKET_NAME,
            BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            PublicReadAccess = false,
            LifecycleRules = new []{ new LifecycleRule { Expiration = Duration.Days(7) } }, //We don't need to keep files, remove files after 7 days.
            Cors = new []{ new CorsRule
            {
                AllowedMethods = new []{ HttpMethods.HEAD, HttpMethods.GET, HttpMethods.PUT }, 
                AllowedOrigins = new []{ "*" },
                AllowedHeaders = new []{ "Authorization", "*" }
            }}
        });

        var uploadServiceLambda = new Function(this, "wetransfer-clone-upload-lambda", new FunctionProps
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "WetransferClone.UploadLambda::WetransferClone.UploadLambda.Function::UploadFunctionHandler",
            Code = Code.FromAsset(Path.Combine(Directory.GetCurrentDirectory(), "publish/upload_lambda/")),
            Role = appRole,
            Timeout = Duration.Minutes(1)
        });
        
        tempS3Bucket.GrantPut(uploadServiceLambda);

        uploadServiceLambda.AddFunctionUrl(new FunctionUrlOptions
        {
            AuthType = FunctionUrlAuthType.NONE
        });
        
        var emailServiceLambda = new Function(this, "wetransfer-clone-email-lambda", new FunctionProps
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "WetransferClone.EmailLambda::WetransferClone.EmailLambda.Function::FunctionHandler",
            Code = Code.FromAsset(Path.Combine(Directory.GetCurrentDirectory(), "publish/email_lambda/")),
            Role = appRole,
            Timeout = Duration.Minutes(1)
        });

        tempS3Bucket.GrantRead(emailServiceLambda);
        
        emailServiceLambda.AddEventSource(new S3EventSource(tempS3Bucket, new S3EventSourceProps
        {
            Events = new []{ EventType.OBJECT_CREATED }
        }));

    }
}