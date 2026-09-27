
namespace WeddingApp.Server.Repository.GDriveRepo
{
    public interface IGDriveRepository
    {
        Task<GeneralResponse<List<GDriveDTO>>> GetAllDrives();
        Task<GeneralResponse<PaginatedFilesDTO>> GetPaginatedPhotos(GetPaginatedFileDTO request);
        Task<GeneralResponse<GFileContentDTO>> GetPhoto(string fileId);
        //Task<GeneralResponse<object>> UploadPhoto();
    }
}
