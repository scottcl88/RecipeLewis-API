namespace Models.Results;

public class UploadResult
{
    public bool Uploaded { get; set; }
    public string? FileName { get; set; }
    public int? ErrorCode { get; set; }
    public string? Message { get; set; }
}