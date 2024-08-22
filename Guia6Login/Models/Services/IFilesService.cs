namespace Guia6Login.Models.Services
{
    public interface IFilesService
    {
        Task<string> SubirArchivo(Stream archivo, string nombre);
    }
}
