namespace AdminLte.Services
{
    public interface IUploadService
    {
        Task<string> UploadImage(string path, IFormFile file);
    }
}