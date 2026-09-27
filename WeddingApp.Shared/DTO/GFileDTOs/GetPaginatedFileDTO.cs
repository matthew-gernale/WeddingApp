
namespace WeddingApp.Shared.DTO.GFileDTOs
{
    public class GetPaginatedFileDTO
    {
        public int PageSize { get; set; } = 10;
        public string? PageToken { get; set; }
        public string OrderBy { get; set; } = "createdTime";
    }
}
