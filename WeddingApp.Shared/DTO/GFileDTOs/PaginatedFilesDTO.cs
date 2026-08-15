
namespace WeddingApp.Shared.DTO.GFileDTOs
{
    public class PaginatedFilesDTO
    {
        public List<string>? Photos { get; set; } = new List<string>();
        public string NextPageToken { get; set; } = string.Empty;
    }
}
