using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3.Model;

namespace S3Storage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private const string AccessKey = "xxx";
        private const string SecretKey = "yyy";
        private const string BucketName = "my-asp-bucket";
        private const string ServiceUrl = "https://s3.cloudfly.vn";

        [HttpPost]
        [Route("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = ServiceUrl,
                    ForcePathStyle = true,
                    SignatureVersion = "2"
                };

                var client = new AmazonS3Client(AccessKey, SecretKey, config);
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                using var stream = file.OpenReadStream();
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = fileName,
                    BucketName = BucketName,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);

                var fileUrl = $"{ServiceUrl}/{BucketName}/{fileName}";
                return Ok(new { Url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading to CloudFly S3: {ex.Message}");
            }
        }



    }
}
