namespace grove.Services;

public interface  IBlobStorageService
{
    Task<IEnumerable<string>> ListBlobNamesAsync();
    Task UploadBlobAsync(string file);
    Task<string> GetBlobStreamAsync(string blobName);
}