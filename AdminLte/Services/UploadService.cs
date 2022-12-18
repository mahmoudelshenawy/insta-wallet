namespace AdminLte.Services
{
    public class UploadService : IUploadService
    {
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public UploadService(IWebHostEnvironment webHostEnvironment)
        {
            _WebHostEnvironment = webHostEnvironment;
        }
        public async Task<string> UploadImage(string path, IFormFile file)
        {
            string folder = path;
            folder += Guid.NewGuid().ToString() + "_" + file.FileName;
            string serverFolder = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folder;
        }
    }
}
