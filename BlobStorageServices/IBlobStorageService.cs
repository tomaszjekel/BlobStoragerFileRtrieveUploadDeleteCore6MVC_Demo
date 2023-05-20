using BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.BlobStorageServices
{
    public interface IBlobStorageService
    {
        Task<List<BlobStorage>> GetAllBlobFiles();
        Task UploadBlobFileAsync(FileStream files);
        Task DeleteDocumentAsync(string blobName);
    }
}
