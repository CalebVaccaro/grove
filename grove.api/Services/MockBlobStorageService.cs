using Azure;

namespace grove.Services;

public class MockBlobStorageService : IBlobStorageService
{
    private readonly Dictionary<string, string> mockBlobData = new Dictionary<string, string>();

    public MockBlobStorageService()
    {
        // Initialize the mock data with sample files
        mockBlobData.Add("image1.jpg", "image1.jpg");
        mockBlobData.Add("image2.jpg", "image2.jpg");
    }

    public async Task<IEnumerable<string>> ListBlobNamesAsync()
    {
        return await Task.FromResult(mockBlobData.Keys);
    }

    public async Task<string> GetBlobStreamAsync(string blobName)
    {
        if (mockBlobData.TryGetValue(blobName, out var formFile)){
            return mockBlobData[blobName];
        }
        else
        {
            throw new RequestFailedException(404, "Blob not found.");
        }
    }

    public async Task UploadBlobAsync(string formFile)
    {
        var blobName = formFile;
        mockBlobData[blobName] = formFile.ToString();
    }

    private IFormFile GetSampleFormFile(string fileName)
    {
        // Create a mock IFormFile with a sample content
        byte[] content = new byte[1024]; // Replace with actual file content
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, fileName, fileName);
    }
}