
namespace WeddingApp.Shared.DTO.GDriveDTOs
{
    public class GDriveDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;    
    }
}
