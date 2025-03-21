using Microsoft.EntityFrameworkCore;
using ShortUrl.Data;
using ShortUrl.Models;

namespace ShortUrl.Repositories
{
    public class AboutTextRepository(ApplicationDbContext context) : IAboutTextRepository
    {
        public async Task<AboutText> GetLastText()
        {
            var text = (await context.About.ToListAsync()).OrderByDescending(x => x.Created).First();
            return text;
        }

        public async Task UpdateText(AboutText text)
        {
            await context.About.AddAsync(text);
            await context.SaveChangesAsync();
        }
    }
}
