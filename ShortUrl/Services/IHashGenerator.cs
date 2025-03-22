namespace ShortUrl.Services
{
    public interface IHashGenerator
    {
        string GenerateHash(string originalUrl);
    }
}