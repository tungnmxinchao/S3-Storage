using System.Runtime;
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);


var cloudFlyConfig = builder.Configuration.GetSection("CloudFly");
var endpoint = cloudFlyConfig["Endpoint"];
var accessKey = cloudFlyConfig["AccessKey"];
var secretKey = cloudFlyConfig["SecretKey"];
var bucketName = cloudFlyConfig["BucketName"];

var s3Config = new AmazonS3Config
{
    ServiceURL = endpoint,
    ForcePathStyle = true,
    SignatureVersion = "2"
};

var s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);


builder.Services.AddSingleton<IAmazonS3>(s3Client);
builder.Services.AddSingleton(new S3Settings
{
    BucketName = bucketName,
    Endpoint = endpoint
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class S3Settings
{
    public string BucketName { get; set; }
    public string Endpoint { get; set; }
}