using Microsoft.EntityFrameworkCore;
using ShortUrl.Data;
using ShortUrl.Models;

namespace ShortUrl.Repositories
{
    public class UrlRepository(ApplicationDbContext context) : IUrlRepository
    {
        public async Task Add(Url url)
        {
            context.Add(url);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Url url)
        {
            context.Remove(url);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Url>> GetAll()
        {
            return await context.Urls.ToListAsync();
        }

        public async Task<Url?> GetById(string id)
        {
            return await context.Urls.FindAsync(id);
        }

        public async Task Update(Url url)
        {
            context.Urls.Update(url);
            await context.SaveChangesAsync();
        }
    }
}
