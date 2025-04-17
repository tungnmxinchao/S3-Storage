using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3.Model;
using System.Runtime;

namespace S3Storage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _settings;

        public StorageController(IAmazonS3 s3Client, S3Settings settings)
        {
            _s3Client = s3Client;
            _settings = settings;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                using var stream = file.OpenReadStream();
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = fileName,
                    BucketName = _settings.BucketName,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                var fileUrl = $"{_settings.Endpoint}/{_settings.BucketName}/{fileName}";
                return Ok(new { Url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading to CloudFly S3: {ex.Message}");
            }
        }
    }
}
