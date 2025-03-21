namespace ShortUrl.Models
{
    public class Url
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string UrlOriginal { get; set; }
        public string UrlNormalized { get; set; }
        public string UrlShort { get; set; }
        public string Hash { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
