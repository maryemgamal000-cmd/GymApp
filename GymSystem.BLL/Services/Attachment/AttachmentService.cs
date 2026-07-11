using GymSystem.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileize = 5 * 1024 * 1024; //5 MB
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".png" , ".jpg" , ".jpeg" };

        public AttachmentService(ILogger<AttachmentService>  logger , IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public Result Delete(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName); 
            try {
                if (!File.Exists(fullPath)) return Result.NotFound("File Not Exist");   
                File.Delete(fullPath);
                return Result.Ok();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex ,$" Failed To Delete Attachment {fileName}");
             return Result.Fail($" Failed To Delete Attachment {fileName}");    
            
            }
        }

        public Result<(Stream stream, string contentType)?> GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
            {
                return Result<(Stream stream, string contentType)?>.Fail("Invalid file name");
            }

            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);

            if (!File.Exists(fullPath))
                return Result<(Stream stream, string contentType)?>.NotFound("File not found.");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            var extension = Path.GetExtension(fullPath).ToLower();

            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return Result<(Stream stream, string contentType)?>.Ok((stream, contentType));
        }

        public async Task<Result<string>?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {

            if (fileStream == null)
                return Result<string>.Fail("Validation Error - fileStream is null");

            if (!fileStream.CanRead)
                return Result<string>.Fail("Validation Error - Stream is closed or cannot be read");

            if (fileStream.Length == 0)
                return Result<string>.Fail("Validation Error - File is empty");

            if (fileStream.Length > _maxFileize)
            {
                _logger.LogError($"File Rejected :  File Too Large {fileStream.Length} Bytes");
                return Result<string>.Fail("you exceed the max length");
            }

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (String.IsNullOrWhiteSpace(extension)  || !_allowedExtensions.Contains(extension) )
            {
                _logger.LogError($"File Rejected :  Extension {extension} Not Allowed");
                return Result<string>.Fail($" Extension {extension} Not Allowed");
            }

            var uploadsFolder =  Path.Combine(_env.ContentRootPath, folderName);    
            Directory.CreateDirectory(uploadsFolder);


            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            var filePath = Path.Combine(uploadsFolder,  storedFileName);

            try {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await  fileStream.CopyToAsync(fs,ct);
                return Result<string>.Ok(storedFileName);
          
            }
            catch(Exception ex) {
                _logger.LogError(ex, $"Failed To Upload File {fileName}");
                return Result<string>.Fail("failed To Upload File");
            }


        }
    }
}
