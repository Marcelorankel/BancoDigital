namespace Application.Interfaces.Service
{
    public interface IApiService
    {
        Task<T?> GetFromController<T>(string endpoint, string token);
        Task<T?> PostToController<T>(string endpoint, object data, string token);
    }
}