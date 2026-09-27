
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
    }
}
