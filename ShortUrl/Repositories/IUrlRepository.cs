using ShortUrl.Models;

namespace ShortUrl.Repositories
{
    public interface IUrlRepository
    {
        Task Add(Url url);
        Task Update(Url url);
        Task Delete(Url url);
        Task<Url> GetById(string id);
        Task<Url> GetByHash(string hash);
        Task<IEnumerable<Url>> GetAll();

    }
}