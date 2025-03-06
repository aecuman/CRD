using Microsoft.AspNetCore.Http;

namespace CRD.API.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile formFile);
        Task<bool> DeleteFileAsync(string requestModel);
    }
}
