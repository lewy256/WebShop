namespace ProductApi.Shared.FilesDtos;
public class FileDto {
    public int TotalFilesUploaded { get; set; }
    public string TotalSizeUploaded { get; set; }
    public IList<string> FileNames { get; set; }
    public IList<string> NotUploadedFiles { get; set; }
}
