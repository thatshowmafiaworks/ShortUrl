using ShortUrl.Models;

namespace ShortUrl.Repositories
{
    public interface IAboutTextRepository
    {
        Task<AboutText> GetLastText();
        Task UpdateText(AboutText text);
    }
}