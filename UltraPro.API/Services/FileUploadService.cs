using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UltraPro.API.Core;
using UltraPro.Common.Enums;
using UltraPro.Common.Services;

namespace UltraPro.API.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _host;
        private readonly PhotoSettings _photoSettings;

        public FileUploadService(
            IWebHostEnvironment host,
            IOptions<PhotoSettings> photoOptions
            )
        {
            _host = host;
            _photoSettings = photoOptions.Value;
        }

        public async Task<string> SaveFileAsync(object fileObj, string fileName, EnumFileUploadFolderCode type)
        {
            var file = fileObj as IFormFile;
            string filePath = this.GetFileFolderPath(type);

            if (!Directory.Exists(Path.Combine(_host.WebRootPath, filePath)))
                Directory.CreateDirectory(Path.Combine(_host.WebRootPath, filePath));

            fileName += Path.GetExtension(file.FileName);
            filePath = Path.Combine(filePath, fileName);

            using (var stream = new FileStream(Path.Combine(_host.WebRootPath, filePath), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<string> SaveImageAsync(object fileObj, string fileName, EnumFileUploadFolderCode type)
        {
            var file = fileObj as IFormFile;
            string filePath = this.GetImageFolderPath(type);

            if (!Directory.Exists(Path.Combine(_host.WebRootPath, filePath)))
                Directory.CreateDirectory(Path.Combine(_host.WebRootPath, filePath));

            fileName += Path.GetExtension(file.FileName); 
            filePath = Path.Combine(filePath, fileName);

            using (var stream = new FileStream(Path.Combine(_host.WebRootPath, filePath), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<string> SaveImageAsync(object fileObj, string fileName, EnumFileUploadFolderCode type, int width, int height)
        {
            var file = fileObj as IFormFile;
            string filePath = this.GetImageFolderPath(type);

            if (!Directory.Exists(Path.Combine(_host.WebRootPath, filePath)))
                Directory.CreateDirectory(Path.Combine(_host.WebRootPath, filePath));

            fileName += Path.GetExtension(file.FileName); 
            filePath = Path.Combine(filePath, fileName);

            using (Image image = Image.FromStream(file.OpenReadStream()))
            {
                Image newImage = image;
                if(image.Width > width || image.Height > height)
                {
                    newImage = await ResizeImageAsync(image, width, height);
                }
                newImage.Save(Path.Combine(_host.WebRootPath, filePath));
            }

            return filePath;
        }

        public async Task<string> SaveImageAsync(string base64String, string fileName, EnumFileUploadFolderCode type)
        {
            string filePath = this.GetImageFolderPath(type);

            if (!Directory.Exists(Path.Combine(_host.WebRootPath, filePath)))
                Directory.CreateDirectory(Path.Combine(_host.WebRootPath, filePath));

            fileName += ".jpg";
            filePath = Path.Combine(filePath, fileName);

            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image image = Image.FromStream(ms);
                Image newImage = image;
                newImage.Save(Path.Combine(_host.WebRootPath, filePath));
            }

            return filePath;
        }

        public async Task<string> SaveImageAsync(string base64String, string fileName, EnumFileUploadFolderCode type, int width, int height)
        {
            string filePath = this.GetImageFolderPath(type);

            if (!Directory.Exists(Path.Combine(_host.WebRootPath, filePath)))
                Directory.CreateDirectory(Path.Combine(_host.WebRootPath, filePath));

            fileName += ".jpg";
            filePath = Path.Combine(filePath, fileName);

            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image image = Image.FromStream(ms);
                Image newImage = image;
                if (image.Width > width || image.Height > height)
                {
                    newImage = await ResizeImageAsync(image, width, height);
                }
                newImage.Save(Path.Combine(_host.WebRootPath, filePath));
            }

            return filePath;
        }

        public async Task<Image> ResizeImageAsync(Image image, int width, int height)
        {
            Image newImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return newImage;
        }

        public async Task DeleteImageAsync(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(Path.Combine(_host.WebRootPath, filePath)))
                File.Delete(Path.Combine(_host.WebRootPath, filePath));
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(Path.Combine(_host.WebRootPath, filePath)))
                File.Delete(Path.Combine(_host.WebRootPath, filePath));
        }

        public async Task<bool> IsValidImage(object fileObj)
        {
            var file = fileObj as IFormFile;
            if (file == null) throw new Exception("Null file");
            if (file.Length == 0) throw new Exception("Empty file");
            if (file.Length > _photoSettings.MaxBytes) throw new Exception("Max file size exceeded");
            if (!_photoSettings.IsSupported(file.FileName)) throw new Exception("Invalid file type.");
            return true;
        }

        private string GetImageFolderPath(EnumFileUploadFolderCode type)
        {
            string filePath = Path.Combine("uploads", "images");
            switch (type)
            {
                case EnumFileUploadFolderCode.ApplicationUserImage:
                    filePath = Path.Combine(filePath, "ApplicationUsers");
                    break;
                default:
                    break;
            }
            return filePath;
        }

        private string GetFileFolderPath(EnumFileUploadFolderCode type)
        {
            string filePath = Path.Combine("uploads", "files");
            switch (type)
            {
                case EnumFileUploadFolderCode.ApplicationUserImage:
                    filePath = Path.Combine(filePath, "ApplicationUsers");
                    break;
                default:
                    break;
            }
            return filePath;
        }
    }
}
