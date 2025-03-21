using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Models.DTOs
{
    public class AboutDTO
    {
        [Required]
        public string Text { get; set; }
    }
}
