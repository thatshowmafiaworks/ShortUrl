using Microsoft.EntityFrameworkCore;
using ShortUrl.Data;
using ShortUrl.Models;
using ShortUrl.Repositories;

namespace ShortURL.Tests
{
    public class AboutTextRepositoryTest
    {
        private readonly DbContextOptions<ApplicationDbContext> opts;

        public AboutTextRepositoryTest()
        {
            opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
        }
        [Fact]
        public async Task GetLastText_ShouldReturnLastTextByDate()
        {
            // Arrange
            using (var context = new ApplicationDbContext(opts))
            {
                var repo = new AboutTextRepository(context);

                var first = new AboutText
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "firstText",
                    CreatedBy = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow.AddDays(1)
                };
                var second = new AboutText
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "secondText",
                    CreatedBy = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow.AddDays(2)
                };
                var third = new AboutText
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "thirdText",
                    CreatedBy = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow.AddDays(3)
                };

                context.About.Add(first);
                context.About.Add(second);
                context.About.Add(third);
                await context.SaveChangesAsync();

                // Act

                var result = await repo.GetLastText();

                // Assert
                Assert.NotNull(result);
                Assert.Equal("thirdText", result.Text);
            }
        }
        [Fact]
        public async Task UpdateText_ShouldAddNewText()
        {
            // Arrange
            using (var context = new ApplicationDbContext(opts))
            {
                var repo = new AboutTextRepository(context);


                var old = new AboutText
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "oldText",
                    CreatedBy = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow
                };
                context.About.Add(old);
                await context.SaveChangesAsync();

                // Act
                var newText = new AboutText
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "newText",
                    CreatedBy = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow.AddDays(1)
                };
                repo.UpdateText(newText);

                // Assert
                var result = await repo.GetLastText();
                Assert.NotNull(result);
                Assert.Equal("newText", result.Text);
            }
        }
    }
}
