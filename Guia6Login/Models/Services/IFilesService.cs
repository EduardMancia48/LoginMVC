namespace Guia6Login.Models.Services
{
    public interface IFileHelper
    {
        Task<string> SubirArchivo(Stream archivo, string nombre);
    }
}
