
namespace WeddingApp.Server.Repository.GDriveRepo
{
    public class GDriveRepository : IGDriveRepository
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public GDriveRepository(IConfiguration config, HttpClient http)
        {
            _config = config;
            _http = http;
        }

        private async Task<string> GetAccessToken()
        { 
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

            return document.RootElement.
                GetProperty("access_token")
                .GetString()!;
        }

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
 