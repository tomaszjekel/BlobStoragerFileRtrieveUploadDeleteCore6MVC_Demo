using BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.BlobStorageServices
{
    public interface IBlobStorageService
    {
        Task<List<BlobStorage>> GetAllBlobFiles();
        Task UploadBlobFileAsync(byte[] file, string name);
        Task DeleteDocumentAsync(string blobName);
        Task<FileResult> DownloadAsync(string fileName);
    }
}
