using Microsoft.EntityFrameworkCore;
using ShortUrl.Data;
using ShortUrl.Models;
using ShortUrl.Repositories;
using ShortUrl.Services;

namespace ShortURL.Tests
{
    public class UrlRepositoryTest
    {
        private readonly IHashGenerator _hashGeneratorMock;

        private readonly DbContextOptions<ApplicationDbContext> dbContextOptions;

        public UrlRepositoryTest()
        {
            _hashGeneratorMock = new HashGenerator();

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
        }

        [Fact]
        public async Task GetById_ShouldReturnUrl_WhenUrlExists()
        {
            // Arrange
            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var repo = new UrlRepository(context);


                var id = Guid.NewGuid().ToString();
                var url = "google.com";
                var hash = _hashGeneratorMock.GenerateHash(url);
                var expectedUrl = new Url
                {
                    Id = id,
                    UserId = Guid.NewGuid().ToString(),
                    UrlOriginal = url,
                    UrlNormalized = url.ToLower(),
                    UrlShort = $"https://site.com/short/{hash}",
                    Hash = hash,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                context.Urls.Add(expectedUrl);
                await context.SaveChangesAsync();

                // Act
                var result = await repo.GetById(id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(url, result.UrlOriginal);
            }
        }

        [Fact]
        public async Task GetByHash_ShouldReturnUrl_WhenUrlExists()
        {
            // Arrange
            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var repo = new UrlRepository(context);

                var id = Guid.NewGuid().ToString();
                var url = "google.com";
                var hash = _hashGeneratorMock.GenerateHash(url);
                var expectedUrl = new Url
                {
                    Id = id,
                    UserId = Guid.NewGuid().ToString(),
                    UrlOriginal = url,
                    UrlNormalized = url.ToLower(),
                    UrlShort = $"https://site.com/short/{hash}",
                    Hash = hash,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                context.Urls.Add(expectedUrl);
                await context.SaveChangesAsync();

                // Act

                var result = await repo.GetByHash(hash);

                // Assert

                Assert.NotNull(result);
                Assert.Equal(hash, result.Hash);
            }

        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUrls_WhenUrlsExists()
        {
            // Arrange
            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var repo = new UrlRepository(context);


                var id = Guid.NewGuid().ToString();
                var url = "google.com";
                var hash = _hashGeneratorMock.GenerateHash(url);
                var expectedUrl1 = new Url
                {
                    Id = id,
                    UserId = Guid.NewGuid().ToString(),
                    UrlOriginal = url,
                    UrlNormalized = url.ToLower(),
                    UrlShort = $"https://site.com/short/{hash}",
                    Hash = hash,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                var id2 = Guid.NewGuid().ToString();
                var url2 = "google.com";
                var hash2 = _hashGeneratorMock.GenerateHash(url);
                var expectedUrl2 = new Url
                {
                    Id = id2,
                    UserId = Guid.NewGuid().ToString(),
                    UrlOriginal = url2,
                    UrlNormalized = url2.ToLower(),
                    UrlShort = $"https://site.com/short/{hash2}",
                    Hash = hash2,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                context.Urls.Add(expectedUrl1);
                context.Urls.Add(expectedUrl2);
                await context.SaveChangesAsync();

                // Act
                var result = await repo.GetAll();

                // Assert
                Assert.NotNull(result);
                foreach (var item in result)
                {
                    Assert.NotNull(item);
                }
                Assert.Equal(url, result.First().UrlOriginal);
                Assert.Equal(url2, result.Last().UrlOriginal);
            }
        }

        [Fact]
        public async Task Add_ShouldCreateNewUrl()
        {
            // Arrange
            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var repo = new UrlRepository(context);

                var id = Guid.NewGuid().ToString();
                var url = "google.com";
                var hash = _hashGeneratorMock.GenerateHash(url);
                var newUrl = new Url
                {
                    Id = id,
                    UserId = Guid.NewGuid().ToString(),
                    UrlOriginal = url,
                    UrlNormalized = url.ToLower(),
                    UrlShort = $"https://site.com/short/{hash}",
                    Hash = hash,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                // Act

                await repo.Add(newUrl);

                // Assert
                var result = await context.Urls.FirstOrDefaultAsync(x => x.UrlOriginal == url);
                Assert.NotNull(result);
                Assert.Equal(newUrl.UrlOriginal, result.UrlOriginal);
            }
        }

        [Fact]
        public async Task Delete_ShouldDeleteExisting()
        {
            // Arrange
            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var repo = new UrlRepository(context);

                var id = Guid.NewGuid().ToString();
                var url = "google.com";
                var hash = _hashGeneratorMock.GenerateHash(url);
                var existingUrl = new Url
                {
                    Id = id,
                    UserId = Guid.NewGuid().ToString(),
                    UrlOriginal = url,
                    UrlNormalized = url.ToLower(),
                    UrlShort = $"https://site.com/short/{hash}",
                    Hash = hash,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                context.Urls.Add(existingUrl);
                await context.SaveChangesAsync();

                // Act

                var toDelete = await repo.GetById(id);
                repo.Delete(toDelete);

                // Assert
                var result = await context.Urls.FirstOrDefaultAsync(x => x.Id == id);
                Assert.Null(result);
            }
        }
    }
}
