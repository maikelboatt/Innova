namespace Innova.Presentation.Core.Services.Abstractions
{
    public interface IFileSaveService
    {
        Task<bool> SaveAsync( byte[] content, string defaultFileName, string filter );
    }
}
