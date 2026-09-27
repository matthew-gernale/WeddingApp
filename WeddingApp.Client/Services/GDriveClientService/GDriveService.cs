using System.Net;

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

        public async Task<GeneralResponse<PaginatedFilesDTO>> GetAllPhotos(GetPaginatedFileDTO payload)
        {
            var url = $"api/GDrive/get-all-photos?PageSize={payload.PageSize}&OrderBy={Uri.EscapeDataString(payload.OrderBy)}";

            if (!string.IsNullOrEmpty(payload.PageToken))
                url += $"&PageToken={Uri.EscapeDataString(payload.PageToken)}";

            try
            {
                var response = await _http.GetAsync(url);
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse<PaginatedFilesDTO>>();

                if (result is null)
                    return new GeneralResponse<PaginatedFilesDTO> { StatusCode = response.StatusCode, ErrorMessage = "Failed to load photos." };

                // The API may be on a different origin than the WASM app, so resolve image URLs against it.
                foreach (var file in result.Data?.Files ?? [])
                    file.ImageUrl = new Uri(_http.BaseAddress!, file.ImageUrl).ToString();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse<PaginatedFilesDTO> { StatusCode = HttpStatusCode.ServiceUnavailable, ErrorMessage = "Failed to load photos." };
            }
        }
    }
}
