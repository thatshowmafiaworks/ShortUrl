using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Models.DTOs
{
    public class UrlDTO
    {
        string Id { get; set; }
        [Required]
        public string UrlOriginal { get; set; }
    }
}
