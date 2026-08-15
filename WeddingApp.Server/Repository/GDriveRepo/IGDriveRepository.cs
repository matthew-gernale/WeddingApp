
namespace WeddingApp.Server.Repository.GDriveRepo
{
    public interface IGDriveRepository
    {
        Task<GeneralResponse<List<GDriveDTO>>> GetAllDrives();
        //Task<PaginatedFilesDTO>? GetPaginatedPhotos(GetPaginatedFileDTO request);
        //Task<GeneralResponse<object>> UploadPhoto();
    }
}
