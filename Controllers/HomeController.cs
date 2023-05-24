using BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.BlobStorageServices;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using static System.Collections.Specialized.BitVector32;

namespace BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlobStorageService _blobStorage;
        public HomeController(IBlobStorageService blobStorage)
        {
            _blobStorage = blobStorage;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _blobStorage.GetAllBlobFiles());
        }

        [HttpGet]
        public IActionResult Upload1()
        {
            return View();
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            var request = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    // Get the temporary folder, and combine a random file name with it
                    var saveToPath = contentDisposition.FileName.Value;// Path.Combine(Path.GetTempPath(), fileName);

                    using (var requestBodyStream = new MemoryStream())
                    {
                        await section.Body.CopyToAsync(requestBodyStream);
                        requestBodyStream.Seek(0, SeekOrigin.Begin);
                        var fileByte = requestBodyStream.ToArray();
                        await _blobStorage.UploadBlobFileAsync(fileByte, saveToPath);
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }
            return Ok();

            // If the code runs to this location, it means that no files have been saved
            //return BadRequest("No files data in the request.");
        }

        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobStorage.DeleteDocumentAsync(blobName);
            return RedirectToAction("Index", "Home");
        }   
    }
}