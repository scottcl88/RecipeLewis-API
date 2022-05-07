using AutoMapper;
using RecipeLewis.Business;
using RecipeLewis.Models;
using RecipeLewis.Services;
using RecipeLewis.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RecipeLewis.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("MyPolicy")]
    public class DocumentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly LogService _logService;

        public DocumentController(IConfiguration configuration, LogService logService, IHostEnvironment environment) : base(environment)
        {
            _configuration = configuration;
            _logService = logService;
        }
        public enum ResumeType
        {
            Unknown = 0,
            Docx = 1,
            HTML = 2,
            PDF = 3,
            RichText = 4,
            Text = 5,
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("SCLewisResume")]
        [SwaggerOperation(Summary = "Used to view my pdf resume")]
        public ActionResult SCLewisResume()
        {
            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                _logService.Info("SCLewisResume called", UserId, new { ipAddress });
                string filePath = Directory.GetCurrentDirectory() + "/Public_SCLewis_Resume.pdf";
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var mimeType = GetMimeTypeForFileExtension(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "Public_SCLewis_Resume.pdf",
                    Inline = true,
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                return File(fileBytes, mimeType);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on SCLewisResume", UserId);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("DownloadResume")]
        [SwaggerOperation(Summary = "Used to download my resume")]
        public async Task<FileResult> DownloadResume(ResumeType resumeType)
        {
            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                _logService.Info("DownloadResume called", UserId, new { ipAddress });
                string recaptchaToken = Request.Headers["RecaptchaToken"];
                if (!HostingEnvironment.IsDevelopment() || (HostingEnvironment.IsDevelopment() && !string.IsNullOrWhiteSpace(recaptchaToken)))
                {
                    var recaptchaResponse = await Business.GoogleRecaptcha.Verify(_configuration, recaptchaToken, false);
                    if (recaptchaResponse.IsSuccessful)
                    {
                        var result = JsonConvert.DeserializeObject<GoogleRecaptchaSiteVeirfyModel>(recaptchaResponse.Content);
                        if (result.Score < 0.5)
                        {
                            _logService.Error("DownloadResume recaptcha too low", UserId);
                            throw new RecaptchaException();
                        }
                    }
                    else
                    {
                        _logService.Error("DownloadResume failed to verify recaptcha", UserId, new { recaptchaResponse?.IsSuccessful, recaptchaResponse?.StatusCode, recaptchaResponse.Content });
                        throw new RecaptchaException();
                    }
                }
                string filePath = Directory.GetCurrentDirectory() + "/Public_SCLewis_Resume.docx";
                string extension = ".docx";
                switch (resumeType)
                {
                    case ResumeType.HTML:
                        {
                            extension = ".html";
                            filePath = Directory.GetCurrentDirectory() + "/Public_SCLewis_Resume.html";
                            break;
                        }
                    case ResumeType.PDF:
                        {
                            extension = ".pdf";
                            filePath = Directory.GetCurrentDirectory() + "/Public_SCLewis_Resume.pdf";
                            break;
                        }
                    case ResumeType.RichText:
                        {
                            extension = ".rtf";
                            filePath = Directory.GetCurrentDirectory() + "/Public_SCLewis_Resume.rtf";
                            break;
                        }
                    case ResumeType.Text:
                        {
                            extension = ".txt";
                            filePath = Directory.GetCurrentDirectory() + "/Public_SCLewis_Resume.txt";
                            break;
                        }
                }
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var mimeType = GetMimeTypeForFileExtension(filePath);
                return File(fileBytes, mimeType, "SCLewis_Resume" + extension);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on DownloadResume", UserId);
                throw;
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