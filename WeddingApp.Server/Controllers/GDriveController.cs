
using Microsoft.AspNetCore.RateLimiting;

namespace WeddingApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GDriveController : ControllerBase
    {
        private readonly IGDriveRepository _driveRepo;

        public GDriveController(IGDriveRepository driveRepo)
        {
            _driveRepo = driveRepo; 
        }

        [HttpGet("get-all-drives")]
        public async Task<ActionResult<GeneralResponse<List<GDriveDTO>>>> GetAllDrives()
        {
            var response = await _driveRepo.GetAllDrives();
            return ResponseHelper.GetStatusResponseWData(response);
        }

        [HttpGet("get-all-photos")]
        public async Task<ActionResult<GeneralResponse<PaginatedFilesDTO>>> GetAllPhotos([FromQuery] GetPaginatedFileDTO request)
        {
            var response = await _driveRepo.GetPaginatedPhotos(request);
            return ResponseHelper.GetStatusResponseWData(response);
        }

        [HttpGet("photo/{fileId}")]
        public async Task<IActionResult> GetPhoto(string fileId)
        {
            var response = await _driveRepo.GetPhoto(fileId);

            if (!response.IsSuccess || response.Data is null)
                return ResponseHelper.GetStatusResponseWData(response);

            Response.Headers.CacheControl = "private, max-age=86400";
            Response.Headers.XContentTypeOptions = "nosniff";
            return File(response.Data.Content, response.Data.MimeType);
        }

        [HttpPost("photo")]
        [EnableRateLimiting("PhotoUpload")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<ActionResult<GeneralResponse<GFileDTO>>> UploadPhoto([FromForm] IFormFile photo)
        {
            if (photo is null || photo.Length == 0 || photo.Length > 10 * 1024 * 1024)
                return BadRequest(ResponseHelper.ErrorResponseWData<GFileDTO>("Photo must be between 1 byte and 10 MB.", HttpStatusCode.BadRequest));

            using var stream = new MemoryStream();
            await photo.CopyToAsync(stream);
            var response = await _driveRepo.UploadPhoto(stream.ToArray(), photo.FileName);
            return ResponseHelper.GetStatusResponseWData(response);
        }

        [HttpPost("photo/preview")]
        [EnableRateLimiting("PhotoPreview")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<IActionResult> PreviewPhoto([FromForm] IFormFile photo)
        {
            if (photo is null || photo.Length == 0 || photo.Length > 10 * 1024 * 1024)
                return BadRequest("Photo must be between 1 byte and 10 MB.");

            using var stream = new MemoryStream();
            await photo.CopyToAsync(stream);
            var result = _driveRepo.PreviewPhoto(stream.ToArray(), photo.FileName);
            if (!result.IsSuccess || result.Data is null)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            Response.Headers.CacheControl = "no-store";
            Response.Headers.XContentTypeOptions = "nosniff";
            return File(result.Data.Content, result.Data.MimeType);
        }

        [HttpGet("photo/{fileId}/download")]
        public async Task<IActionResult> DownloadPhoto(string fileId)
        {
            var response = await _driveRepo.GetPhoto(fileId);
            if (!response.IsSuccess || response.Data is null)
                return ResponseHelper.GetStatusResponseWData(response);

            Response.Headers.XContentTypeOptions = "nosniff";
            var extension = response.Data.MimeType switch
            {
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => ".jpg"
            };
            return File(response.Data.Content, response.Data.MimeType, $"wedding-photo-{fileId}{extension}");
        }
    }
}
