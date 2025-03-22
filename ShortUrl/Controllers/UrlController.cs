using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShortUrl.Models;
using ShortUrl.Models.DTOs;
using ShortUrl.Repositories;
using ShortUrl.Services;
using System.Diagnostics;

namespace ShortUrl.Controllers
{
    public class UrlController(
        IUrlRepository urlRepo,
        IAboutTextRepository aboutRepo,
        UserManager<IdentityUser> userManager,
        ILogger<UrlController> logger,
        IHashGenerator hashGenerator,
        IWebHostEnvironment env
        ) : Controller
    {
        public async Task<IActionResult> Index()
        {
            string rootPath = Path.Combine(env.WebRootPath, "urlindex", "build", "static", "js");
            string reactFile = Path.GetFileName(Directory.GetFiles(rootPath, "main.*").FirstOrDefault());
            TempData["reactFile"] = reactFile;
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
            var hash = hashGenerator.GenerateHash(model.UrlOriginal);
            var existing = await urlRepo.GetByHash(hash);
            if (existing != null)
            {
                TempData["errorMessage"] = "There is existing shortUrl for this url";
                return View(model);
            }
            try
            {
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
                TempData["succesfullMessage"] = "Short url was created succesfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Authorize]
        public async Task<IActionResult> Info(string id)
        {
            var url = await urlRepo.GetById(id);
            return View(url);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var url = await urlRepo.GetById(id);
            if (url is null)
            {
                logger.LogInformation($"not found url with Id: {id}");
                TempData["errorMessage"] = "Something went wrong";
                return RedirectToAction(nameof(Index));
            }
            await urlRepo.Delete(url);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> About()
        {
            var text = await aboutRepo.GetLastText();
            return View(text);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAbout()
        {
            var text = await aboutRepo.GetLastText();
            var model = new AboutDTO
            {
                Text = text.Text
            };
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditAbout(AboutDTO about)
        {
            var user = await userManager.GetUserAsync(User);
            var newText = new AboutText
            {
                Id = Guid.NewGuid().ToString(),
                Text = about.Text,
                CreatedBy = user.Id,
                Created = DateTime.UtcNow
            };
            await aboutRepo.UpdateText(newText);
            return RedirectToAction(nameof(About));
        }

        [Route("short/{hash}")]
        public async Task<IActionResult> Short(string hash)
        {
            var url = await urlRepo.GetByHash(hash);
            if (url is null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (url.UrlOriginal.StartsWith("https://"))
            {
                return Redirect(url.UrlOriginal);
            }
            return Redirect("https://" + url.UrlOriginal);
        }
    }
}
