
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
    }
}
