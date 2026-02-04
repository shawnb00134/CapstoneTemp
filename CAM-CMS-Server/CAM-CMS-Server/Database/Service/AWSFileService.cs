//using CAMCMSServer.Model.ElementTypes;
//using CAMCMSServer.Model;
//using Newtonsoft.Json;

//namespace CAMCMSServer.Database.Service
//{
//    public class AWSFileService : IFileService
//    {
//        #region Data members

//        private readonly string bucketName;
//        private readonly IAmazonS3 s3Client;

//        #endregion

//        #region Constructors

//        public FileService(IAmazonS3 s3Client, string bucketName)
//        {
//            this.s3Client = s3Client;
//            this.bucketName = bucketName;
//        }

//        #endregion

//        #region Methods

//        public async Task UploadFileAsync(IFormFile file, Element element)
//        {
//            var bucketExists =
//                await AmazonS3Util.DoesS3BucketExistV2Async(this.s3Client, this.bucketName);
//            if (bucketExists)
//            {
//                var request = new PutObjectRequest
//                {
//                    BucketName = this.bucketName,
//                    Key = element.ElementId + "-" + file.FileName,
//                    InputStream = file.OpenReadStream()
//                };
//                request.Metadata.Add("Content-Type", file.ContentType);
//                await this.s3Client.PutObjectAsync(request);
//            }
//        }

//        public async Task DeleteFileAsync(string key)
//        {
//            var bucketExists =
//                await AmazonS3Util.DoesS3BucketExistV2Async(this.s3Client, this.bucketName);
//            if (bucketExists)
//            {
//                var deleteRequest = new DeleteObjectRequest
//                {
//                    BucketName = this.bucketName,
//                    Key = key
//                };
//                await this.s3Client.DeleteObjectAsync(deleteRequest);
//            }
//        }

//        public async Task<string> GetFileAsync(string key)
//        {
//            var bucketExists =
//                await AmazonS3Util.DoesS3BucketExistV2Async(this.s3Client, this.bucketName);
//            var url = "";
//            if (!bucketExists)
//            {
//                return url;
//            }

//            try
//            {
//                var request = new GetPreSignedUrlRequest
//                {
//                    BucketName = this.bucketName,
//                    Key = key,
//                    Expires = DateTime.UtcNow.AddSeconds(30),
//                    Verb = HttpVerb.GET,
//                    Protocol = Protocol.HTTPS
//                };
//                url = this.s3Client.GetPreSignedURL(request);
//            }
//            catch (AmazonS3Exception ex)
//            {
//            }

//            return url;
//        }

//        public IElementType CheckFileType(IFormFile file, Element element)
//        {
//            var split = file.ContentType.Split('/');
//            if (split[0].Equals("image"))
//            {
//                var image = new ImageElement
//                {
//                    Key = element.ElementId + "-" + file.FileName,
//                    Link = false
//                };
//                return image;
//            }

//            if (split[1].Equals("pdf"))
//            {
//                var pdf = new PdfElement
//                {
//                    Key = element.ElementId + "-" + file.FileName,
//                    Link = false
//                };
//                return pdf;
//            }

//            return null;
//        }

//        public async Task<string> GetFileUrlAsync(Element element, int baseElement)
//        {
//            string? url;
//            switch (baseElement)
//            {
//                case (int)ElementType.Image:
//                    {
//                        var elementImageJson = JsonSerializer.Deserialize<ImageElement>(element.Content);
//                        var fileUrl = await this.GetFileAsync(elementImageJson.Key);
//                        elementImageJson.Source = fileUrl;
//                        elementImageJson.Key = null;
//                        var content = elementImageJson.ConvertToJsonForUI();
//                        var result = JsonSerializer.Serialize(content.RootElement);
//                        return result;
//                    }
//                case (int)ElementType.Pdf:
//                    {
//                        var elementPdfJson = JsonSerializer.Deserialize<PdfElement>(element.Content);
//                        var fileUrl = await this.GetFileAsync(elementPdfJson.Key);
//                        elementPdfJson.Source = fileUrl;
//                        elementPdfJson.Key = null;
//                        var content = elementPdfJson.ConvertToJsonForUI();
//                        var result = JsonSerializer.Serialize(content.RootElement);
//                        return result;
//                    }
//                default:
//                    return null;
//            }
//        }
//    }
