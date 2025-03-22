using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShortUrl.Models;
using ShortUrl.Models.DTOs;
using ShortUrl.Repositories;
using ShortUrl.Services;

namespace ShortUrl.Controllers
{
    [Route("api")]
    public class ApiController(
        IUrlRepository urlRepo,
        UserManager<IdentityUser> userManager,
        IHashGenerator hashGenerator,
        ILogger<ApiController> logger
        ) : ControllerBase
    {

        [HttpGet("allurls")]
        public async Task<IActionResult> GetUrls()
        {
            var urls = await urlRepo.GetAll();
            return Ok(urls);
        }

        [HttpGet("auth")]
        public async Task<IActionResult> GetAuth()
        {
            if (User.Identity.IsAuthenticated)
                return Ok(new
                {
                    isAuthenticated = User.Identity.IsAuthenticated,
                    isAdmin = User.IsInRole("Admin"),
                    id = (await userManager.GetUserAsync(User)).Id
                });
            return Ok(new
            {
                isAuthenticated = false,
                isAdmin = false,
                id = ""
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUrl([FromBody] UrlDTO newUrl)
        {
            if (newUrl.UrlOriginal.IsNullOrEmpty()) return BadRequest(new { error = "empty UrlOriginal" });
            var hash = hashGenerator.GenerateHash(newUrl.UrlOriginal);
            var existing = await urlRepo.GetByHash(hash);
            if (existing != null) return BadRequest(new { error = "this url already exist" });
            try
            {
                Url url = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = User.Identity.IsAuthenticated ? (await userManager.GetUserAsync(User)).Id : "",
                    UrlOriginal = newUrl.UrlOriginal,
                    UrlNormalized = newUrl.UrlOriginal.ToLower(),
                    Hash = hash,
                    UrlShort = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/short/{hash}",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                await urlRepo.Add(url);
                return Ok(new { message = "Succesfully created" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + "\n" + ex.InnerException?.Message);
                return BadRequest(new { error = "something went wrong" });
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUrl([FromBody] string id)
        {
            var url = await urlRepo.GetById(id);
            if (url is null)
            {
                logger.LogInformation($"not found url with Id: {id}");
                return BadRequest(new { error = "Not found this url" });
            }
            await urlRepo.Delete(url);
            return Ok(new { success = "sucesfully deleted" });
        }
    }
}
