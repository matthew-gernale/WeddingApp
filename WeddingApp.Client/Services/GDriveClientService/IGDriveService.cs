
namespace WeddingApp.Client.Services.GDriveClientService
{
    public interface IGDriveService
    {
        Task<GeneralResponse<List<GDriveDTO>>> GetAllDrives();
        Task<GeneralResponse<PaginatedFilesDTO>> GetAllPhotos(GetPaginatedFileDTO payload);
        Task<GeneralResponse<GFileDTO>> UploadPhoto(byte[] content, string fileName);
        Task<GeneralResponse<GFileContentDTO>> PreviewPhoto(byte[] content, string fileName);
    }
}
