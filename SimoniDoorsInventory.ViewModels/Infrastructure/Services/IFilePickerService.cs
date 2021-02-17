using System;
using System.IO;
using System.Threading.Tasks;

namespace SimoniDoorsInventory.Services
{
    public class ImagePickerResult
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageBytes { get; set; }
        public object ImageSource { get; set; }
    }

    public class ExcelFilePickerResult
    {
        public string FileName { get; set; }
        public string ParentFolder { get; set; }
        public string ContentType { get; set; }
    }

    public interface IFilePickerService
    {
        Task<ImagePickerResult> OpenImagePickerAsync();
        Task<ExcelFilePickerResult> OpenExcelFilePickerAsync(string defaultFileName);
        Task<Stream> GetExcelFileStreamAsync(string defaultFileName);
    }
}
