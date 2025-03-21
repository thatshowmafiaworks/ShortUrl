using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShortUrl.Models;
using ShortUrl.Models.DTOs;
using ShortUrl.Repositories;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ShortUrl.Controllers
{
    public class UrlController(IUrlRepository urlRepo, UserManager<IdentityUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var urls = await urlRepo.GetAll();
            return View(urls);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(UrlDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var hash = GenerateHash(model.UrlOriginal);
                Url url = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = User.Identity.IsAuthenticated ? (await userManager.GetUserAsync(User)).Id : "",
                    UrlOriginal = model.UrlOriginal,
                    UrlNormalized = model.UrlOriginal.ToLower(),
                    Hash = hash,
                    UrlShort = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/short/{hash}",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };
                await urlRepo.Add(url);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        private string GenerateHash(string originalUrl)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(originalUrl));
                var stringHash = Convert.ToBase64String(hash)
                    .Replace("+", "")
                    .Replace("=", "")
                    .Replace("/", "")
                    .Substring(0, 8);
                return stringHash;
            }
        }
    }
}
