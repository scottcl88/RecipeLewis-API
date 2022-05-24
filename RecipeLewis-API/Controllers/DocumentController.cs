using AutoMapper;
using RecipeLewis.Business;
using RecipeLewis.Models;
using RecipeLewis.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Models.Results;

namespace RecipeLewis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/documents")]
    [EnableCors("MyPolicy")]
    public class DocumentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IDocumentService _documentService;
        private readonly LogService _logService;
        private readonly string[] permittedExtensions = new string[] { ".txt", ".pdf", ".png", ".jpeg", ".jpg", ".gif", ".jfif" };
        private readonly long _fileSizeLimit = 26214400;

        public DocumentController(IConfiguration configuration, IDocumentService documentService, LogService logService, IHostEnvironment environment) : base(environment)
        {
            _configuration = configuration;
            _logService = logService;
            _documentService = documentService;
        }

        [HttpGet("download/{recipeId}/{documentId}")]
        public FileResult? Download(int recipeId, int documentId)
        {
            if (recipeId <= 0 || documentId <= 0) return null;
            var doc = _documentService.Get(new RecipeId(recipeId), documentId);
            return File(doc.Bytes, doc.ContentType, doc.FileName);
        }

        [AllowAnonymous]
        [HttpGet("view/{recipeId}/{documentId}")]
        public FileResult? View(int recipeId, int documentId)
        {
            if (recipeId <= 0 || documentId <= 0) return null;
            var doc = _documentService.Get(new RecipeId(recipeId), documentId);
            return File(doc.Bytes, doc.ContentType);
        }

        [HttpPost("upload/multiple/{recipeId}")]
        public async Task<ActionResult<List<UploadResult>>> Multiple([FromForm] IEnumerable<IFormFile> files, int recipeId)
        {
            List<UploadResult> uploadResults = new();
            try
            {
                List<DocumentModel> documentModels = new List<DocumentModel>();
                foreach (var file in files)
                {
                    var uploadResult = new UploadResult();

                    var untrustedFileName = file.FileName;
                    uploadResult.FileName = untrustedFileName;

                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                        {
                            uploadResult.ErrorCode = 2;
                            uploadResult.Message = "Invalid file extension";
                            uploadResults.Add(uploadResult);
                            return StatusCode(500, uploadResults);
                        }
                        if (file.Length > _fileSizeLimit)
                        {
                            uploadResult.ErrorCode = 3;
                            uploadResult.Message = "Invalid file size";
                            uploadResults.Add(uploadResult);
                            return StatusCode(500, uploadResults);
                        }

                        var documentKey = Guid.NewGuid();
                        DateTime createdDate = DateTime.UtcNow;

                        byte[] fileBytes = new byte[0];
                        if (file.Length > 0)
                        {
                            using (var ms = file.OpenReadStream())
                            {
                                using (var ms2 = new MemoryStream())
                                {
                                    await ms.CopyToAsync(ms2);
                                    fileBytes = ms2.ToArray();
                                }
                            }
                        }
                        uploadResult.Uploaded = true;
                        uploadResults.Add(uploadResult);
                        var documentModel = new DocumentModel()
                        {
                            ByteSize = (int)file.Length,
                            Bytes = fileBytes,
                            FileName = untrustedFileName,
                            ContentType = file.ContentType,
                            DocumentKey = documentKey,
                            CreatedDateTime = createdDate,
                        };
                        documentModels.Add(documentModel);
                    }
                }
                var result = _documentService.AddDocuments(documentModels, new RecipeId(recipeId), User);
                if (!result)
                {
                    var uploadResult = new UploadResult();
                    uploadResult.ErrorCode = 4;
                    uploadResult.Message = "Failure saving documents";
                    uploadResults.Add(uploadResult);
                    return StatusCode(500, uploadResults);
                }
                return StatusCode(200, uploadResults);
            }
            catch (Exception ex)
            {
                var uploadResult = new UploadResult();
                uploadResult.ErrorCode = 4;
                uploadResult.Message = ex.Message;
                uploadResults.Add(uploadResult);
                return StatusCode(500, uploadResults);
            }
        }


        private string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
    }
}