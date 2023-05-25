using BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.BlobStorageServices
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
        }
        public async Task<List<BlobStorage>> GetAllBlobFiles()
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_storageContainerName);
                CloudBlobDirectory dirb = container.GetDirectoryReference(_storageContainerName);


                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(string.Empty,
                    true, BlobListingDetails.Metadata, 100, null, null, null);
                List<BlobStorage> fileList = new List<BlobStorage>();

                foreach (var blobItem in resultSegment.Results)
                {
                    // A flat listing operation returns only blobs, not virtual directories.
                    var blob = (CloudBlob)blobItem;
                    fileList.Add(new BlobStorage()
                    {
                        FileName = blob.Name,
                        FileSize = Math.Round((blob.Properties.Length / 1024f) / 1024f, 2).ToString(),
                        Modified = DateTime.Parse(blob.Properties.LastModified.ToString()).ToLocalTime().ToString()
                    });
                }
                return fileList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task UploadBlobFileAsync(byte[] file, string name)
        {
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                // Create the blob client.
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageContainerName);

                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                string systemFileName = Path.GetFileName(name);
                await cloudBlobContainer.SetPermissionsAsync(permissions);
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(systemFileName);
                //await cloudBlockBlob.UploadFromByteArrayAsync(dataFiles, 0, dataFiles.Length);
                await cloudBlockBlob.UploadFromByteArrayAsync(file,0,file.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DeleteDocumentAsync(string blobName)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageContainerName);
                var blob = cloudBlobContainer.GetBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<FileResult > DownloadAsync(string fileName)
        {
            try
            {
                CloudBlockBlob blockBlob;
                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageContainerName);
                    blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    await blockBlob.DownloadToStreamAsync(memoryStream);
                }
                return  new FileResult{ FileStream = blockBlob.OpenReadAsync().Result, ContentType= blockBlob.Properties.ContentType };
               
                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
