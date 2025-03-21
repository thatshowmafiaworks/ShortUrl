using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Models.DTOs
{
    public class UrlDTO
    {
        [Required]
        public string UrlOriginal { get; set; }
    }
}
