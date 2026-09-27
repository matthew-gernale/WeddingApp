
namespace WeddingApp.Client.Services.GDriveClientService
{
    public interface IGDriveService
    {
        Task<GeneralResponse<List<GDriveDTO>>> GetAllDrives();
        Task<GeneralResponse<PaginatedFilesDTO>> GetAllPhotos(GetPaginatedFileDTO payload);
    }
}
