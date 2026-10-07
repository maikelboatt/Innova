using System.IO;
using Innova.Presentation.Core.Services.Abstractions;
using Microsoft.Win32;

namespace Innova.Presentation.WPF.Services
{
    public sealed class FileSaveService:IFileSaveService
    {
        public async Task<bool> SaveAsync( byte[] content, string defaultFileName, string filter )
        {
            SaveFileDialog dialog = new()
                                    {
                                        FileName = defaultFileName,
                                        Filter = filter
                                    };

            bool? result = dialog.ShowDialog();

            if (result != true)
                return false;

            await File.WriteAllBytesAsync(dialog.FileName, content);
            return true;
        }
    }
}
