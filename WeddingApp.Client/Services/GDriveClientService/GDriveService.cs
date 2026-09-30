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

        public async Task<GeneralResponse<GFileDTO>> UploadPhoto(byte[] content, string fileName)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                using var file = new ByteArrayContent(content);
                form.Add(file, "photo", fileName);
                using var response = await _http.PostAsync("api/GDrive/photo", form);
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse<GFileDTO>>()
                    ?? new GeneralResponse<GFileDTO> { ErrorMessage = "The upload did not return a result." };
                if (result.Data is not null)
                    result.Data.ImageUrl = new Uri(_http.BaseAddress!, result.Data.ImageUrl).ToString();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse<GFileDTO> { ErrorMessage = "Could not upload the photo. Please try again." };
            }
        }

        public async Task<GeneralResponse<GFileContentDTO>> PreviewPhoto(byte[] content, string fileName)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                using var file = new ByteArrayContent(content);
                form.Add(file, "photo", fileName);
                using var response = await _http.PostAsync("api/GDrive/photo/preview", form);
                if (!response.IsSuccessStatusCode)
                    return new GeneralResponse<GFileContentDTO>
                    {
                        ErrorMessage = "Could not preview this photo. Try a different file."
                    };

                return new GeneralResponse<GFileContentDTO>
                {
                    IsSuccess = true,
                    Data = new GFileContentDTO
                    {
                        Content = await response.Content.ReadAsByteArrayAsync(),
                        MimeType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg"
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse<GFileContentDTO> { ErrorMessage = "Could not preview this photo. Please try again." };
            }
        }
    }
}
