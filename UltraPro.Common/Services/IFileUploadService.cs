using System.Drawing;
using System.Threading.Tasks;
using UltraPro.Common.Enums;

namespace UltraPro.Common.Services
{
    public interface IFileUploadService
    {
        Task<string> SaveFileAsync(object file, string fileName, EnumFileUploadFolderCode type);
        Task<string> SaveImageAsync(object file, string fileName, EnumFileUploadFolderCode type);
        Task<string> SaveImageAsync(object file, string fileName, EnumFileUploadFolderCode type, int width, int height);
        Task<string> SaveImageAsync(string base64String, string fileName, EnumFileUploadFolderCode type);
        Task<string> SaveImageAsync(string base64String, string fileName, EnumFileUploadFolderCode type, int width, int height);
        Task<Image> ResizeImageAsync(Image image, int width, int height);
        Task DeleteImageAsync(string filePath);
        Task DeleteFileAsync(string filePath);
        Task<bool> IsValidImage(object file);
    }
}
