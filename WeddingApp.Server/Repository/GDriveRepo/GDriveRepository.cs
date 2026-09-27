using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace WeddingApp.Server.Repository.GDriveRepo
{
    public class GDriveRepository : IGDriveRepository
    {
        private const string DriveFilesUrl = "https://www.googleapis.com/drive/v3/files";
        private const string AccessTokenCacheKey = "GDriveAccessToken";
        private const int MaxPageSize = 100;
        private const string SvgMimeType = "image/svg+xml";

        private static readonly Regex FileIdPattern = new("^[A-Za-z0-9_-]{10,200}$", RegexOptions.Compiled);

        // Whitelist prevents arbitrary input from being injected into the Drive query string.
        private static readonly HashSet<string> AllowedOrderBy = new(StringComparer.Ordinal)
        {
            "createdTime", "createdTime desc",
            "modifiedTime", "modifiedTime desc",
            "name", "name desc"
        };

        private readonly IConfiguration _config;
        private readonly string folderId;
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;

        public GDriveRepository(IConfiguration config, HttpClient http, IMemoryCache cache)
        {
            _config = config;
            _http = http;
            _cache = cache;
            folderId = _config["GDriveFolderID"]!;
        }

        private async Task<string> GetAccessToken()
        {
            if (_cache.TryGetValue(AccessTokenCacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
                return cachedToken;

            string clientId = _config["GDriveClientID"]!;
            string clientSecret = _config["GDriveClientSecret"]!;
            string refreshToken = _config["GDriveRefreshToken"]!;

            var requestBody = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            var requestContent = new FormUrlEncodedContent(requestBody);
            var response = await _http.PostAsync("https://oauth2.googleapis.com/token", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);

            var accessToken = document.RootElement
                .GetProperty("access_token")
                .GetString()!;

            var expiresIn = document.RootElement.TryGetProperty("expires_in", out var expiresElement)
                ? expiresElement.GetInt32()
                : 3600;

            // Refresh slightly before Google's expiry.
            _cache.Set(AccessTokenCacheKey, accessToken, TimeSpan.FromSeconds(Math.Max(expiresIn - 60, 30)));

            return accessToken;
        }

        private async Task<HttpResponseMessage> SendDriveRequest(string url)
        {
            var accessToken = await GetAccessToken();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await _http.SendAsync(requestMessage);
        }

        public async Task<GeneralResponse<PaginatedFilesDTO>> GetPaginatedPhotos(GetPaginatedFileDTO request)
        {
            if (request.PageSize < 1 || request.PageSize > MaxPageSize)
                return ResponseHelper.ErrorResponseWData<PaginatedFilesDTO>($"PageSize must be between 1 and {MaxPageSize}.", HttpStatusCode.BadRequest);

            if (!AllowedOrderBy.Contains(request.OrderBy))
                return ResponseHelper.ErrorResponseWData<PaginatedFilesDTO>("Invalid OrderBy value.", HttpStatusCode.BadRequest);

            try
            {
                var query = $"'{folderId}' in parents and mimeType contains 'image/' and mimeType != '{SvgMimeType}' and trashed = false";

                var url = $"{DriveFilesUrl}?q={Uri.EscapeDataString(query)}" +
                          $"&pageSize={request.PageSize}" +
                          $"&orderBy={Uri.EscapeDataString(request.OrderBy)}" +
                          $"&fields={Uri.EscapeDataString("nextPageToken,files(id,name,mimeType)")}" +
                          "&supportsAllDrives=true&includeItemsFromAllDrives=true";

                if (!string.IsNullOrWhiteSpace(request.PageToken))
                    url += $"&pageToken={Uri.EscapeDataString(request.PageToken)}";

                using var response = await SendDriveRequest(url);

                if (!response.IsSuccessStatusCode)
                    return ResponseHelper.ErrorResponseWData<PaginatedFilesDTO>($"Failed to retrieve photos. Status code: {response.StatusCode}", response.StatusCode);

                var fileList = await response.Content.ReadFromJsonAsync<DriveFileList>();

                var files = (fileList?.Files ?? [])
                    .Where(f => !string.IsNullOrEmpty(f.Id))
                    .Select(f => new GFileDTO
                    {
                        Id = f.Id,
                        Name = f.Name ?? string.Empty,
                        MimeType = f.MimeType ?? string.Empty,
                        ImageUrl = $"/api/GDrive/photo/{Uri.EscapeDataString(f.Id)}"
                    })
                    .ToList();

                return ResponseHelper.SuccessResponseWData(new PaginatedFilesDTO
                {
                    Files = files,
                    NextPageToken = string.IsNullOrEmpty(fileList?.NextPageToken) ? null : fileList.NextPageToken
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ResponseHelper.ErrorResponseWData<PaginatedFilesDTO>("Failed to retrieve photos.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GeneralResponse<GFileContentDTO>> GetPhoto(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId) || !FileIdPattern.IsMatch(fileId))
                return ResponseHelper.ErrorResponseWData<GFileContentDTO>("Invalid file id.", HttpStatusCode.BadRequest);

            try
            {
                // Verify the file is an image inside the wedding folder so this endpoint can't expose other Drive files.
                using var metadataResponse = await SendDriveRequest(
                    $"{DriveFilesUrl}/{fileId}?fields={Uri.EscapeDataString("id,mimeType,parents,trashed")}&supportsAllDrives=true");

                if (metadataResponse.StatusCode == HttpStatusCode.NotFound)
                    return ResponseHelper.ErrorResponseWData<GFileContentDTO>("Photo not found.", HttpStatusCode.NotFound);

                if (!metadataResponse.IsSuccessStatusCode)
                    return ResponseHelper.ErrorResponseWData<GFileContentDTO>($"Failed to retrieve photo. Status code: {metadataResponse.StatusCode}", metadataResponse.StatusCode);

                var metadata = await metadataResponse.Content.ReadFromJsonAsync<DriveFile>();

                if (metadata is null
                    || metadata.Trashed
                    || metadata.Parents?.Contains(folderId) != true
                    || string.IsNullOrEmpty(metadata.MimeType)
                    || !metadata.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                    || metadata.MimeType.Equals(SvgMimeType, StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseHelper.ErrorResponseWData<GFileContentDTO>("Photo not found.", HttpStatusCode.NotFound);
                }

                using var mediaResponse = await SendDriveRequest($"{DriveFilesUrl}/{fileId}?alt=media&supportsAllDrives=true");

                if (!mediaResponse.IsSuccessStatusCode)
                    return ResponseHelper.ErrorResponseWData<GFileContentDTO>($"Failed to retrieve photo. Status code: {mediaResponse.StatusCode}", mediaResponse.StatusCode);

                return ResponseHelper.SuccessResponseWData(new GFileContentDTO
                {
                    Content = await mediaResponse.Content.ReadAsByteArrayAsync(),
                    MimeType = metadata.MimeType
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ResponseHelper.ErrorResponseWData<GFileContentDTO>("Failed to retrieve photo.", HttpStatusCode.InternalServerError);
            }
        }

        private sealed record DriveFileList(string? NextPageToken, List<DriveFile>? Files);

        private sealed record DriveFile(string Id, string? Name, string? MimeType, List<string>? Parents, bool Trashed);

        public async Task<GeneralResponse<List<GDriveDTO>>> GetAllDrives()
        {
            try
            {
                Console.WriteLine("starting...");
                var accessToken = await GetAccessToken();
                //_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                //var response = await _http.GetAsync("https://www.googleapis.com/drive/v3/drives");

                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/drive/v3/drives");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _http.SendAsync(request);


                if (!response.IsSuccessStatusCode) return ResponseHelper.ErrorResponseWData<List<GDriveDTO>>($"Failed to retrieve drives. Status code: {response.StatusCode}", response.StatusCode);

                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Status: " + response.StatusCode);
                Console.WriteLine(json);

                var docs = JsonDocument.Parse(json);
                var drives = JsonSerializer.Deserialize<List<GDriveDTO>>(docs.RootElement.GetProperty("drives").GetRawText());

                Console.WriteLine("Success");

                return ResponseHelper.SuccessResponseWData(drives ?? new List<GDriveDTO>());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ResponseHelper.ErrorResponseWData<List<GDriveDTO>>(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
 