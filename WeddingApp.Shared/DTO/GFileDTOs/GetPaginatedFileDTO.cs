
namespace WeddingApp.Shared.DTO.GFileDTOs
{
    public class GetPaginatedFileDTO
    {
        public int PageSize { get; set; } = 10;
        public int PageToken { get; set; }
        public string OrderBy { get; set; } = "createdTime";
    }
}
