
namespace WeddingApp.Shared.DTO.GFileDTOs
{
    public class PaginatedFilesDTO
    {
        public List<GFileDTO> Files { get; set; } = new List<GFileDTO>();
        public string? NextPageToken { get; set; }
    }
}
