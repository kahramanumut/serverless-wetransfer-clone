using Amazon;

namespace WetransferClone.Core;

public class CommonContants
{
    public static RegionEndpoint DEFAULT_REGION = RegionEndpoint.EUCentral1;

    public const int EXPIRE_DAY = 7;
    
    public const string TEMP_BUCKET_NAME = "wetransfer-clone-temp";
    public const string METADATA_SENDER_EMAIL = "sender-email";
    public const string METADATA_RECEIVER_EMAIL = "receiver-email";
    
    public const string DYNAMO_DB_TABLE_NAME = "wetransfer-files";
}