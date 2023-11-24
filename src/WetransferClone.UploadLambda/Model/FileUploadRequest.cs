namespace WetransferClone.UploadLambda.Model;

public class FileUploadRequest
{
    public string FileKey { get; set; }
    public string SenderEmail { get; set; }
    public string ReceiverEmail { get; set; }
}