
namespace WeddingApp.Client.Services.GDriveClientService
{
    public class GDriveService : IGDriveService
    {
        private readonly HttpClient _http;

        public GDriveService(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResponse<List<GDriveDTO>>> GetAllDrives()
        {
            var response = await _http.GetAsync("api/GDrive/get-all-drives");
            return await response.Content.ReadFromJsonAsync<GeneralResponse<List<GDriveDTO>>>() ?? new GeneralResponse<List<GDriveDTO>>();
        }
    }
}
