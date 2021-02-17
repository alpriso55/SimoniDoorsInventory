#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace SimoniDoorsInventory.Services
{
    public class FilePickerService : IFilePickerService
    {
        public async Task<ImagePickerResult> OpenImagePickerAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var bytes = await GetImageBytesAsync(file);
                return new ImagePickerResult
                {
                    FileName = file.Name,
                    ContentType = file.ContentType,
                    ImageBytes = bytes,
                    ImageSource = await BitmapTools.LoadBitmapAsync(bytes)
                };
            }
            return null;
        }

        public async Task<ExcelFilePickerResult> OpenExcelFilePickerAsync(string defaultFileName)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Excel File", new List<string>() { ".xlsx" });
            
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = defaultFileName;
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                return new ExcelFilePickerResult
                {
                    FileName = file.Name,
                    ParentFolder = Path.GetDirectoryName(file.Path),
                    ContentType = file.ContentType
                };
            }

            return null;
        }

        public async Task<Stream> GetExcelFileStreamAsync(string defaultFileName)
        {
            FileSavePicker filePicker = new FileSavePicker();
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            filePicker.FileTypeChoices.Add("Excel File", new List<string>() { ".xlsx" });

            // Default file name if the user does not type one in or select a file to replace
            // filePicker.SuggestedFileName = $"{ViewModel.OrderDetails.Item.OrderID}_Order_Details.xlsx";
            filePicker.SuggestedFileName = defaultFileName;
            StorageFile newStorageFile = await filePicker.PickSaveFileAsync();
            
            if (newStorageFile != null)
            {
                Stream newFileStream = await newStorageFile.OpenStreamForWriteAsync();
                return newFileStream;
            }

            return Stream.Null;
        }

        static private async Task<byte[]> GetImageBytesAsync(StorageFile file)
        {
            using (var randomStream = await file.OpenReadAsync())
            {
                using (var stream = randomStream.AsStream())
                {
                    byte[] buffer = new byte[randomStream.Size];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }
    }
}
