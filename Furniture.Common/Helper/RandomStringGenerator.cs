namespace Furniture.Common.Helper;

public class RandomStringGenerator
{
    public string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const string specialChars = "!@#$%^&*()_-+=<>?";

        string allChars = chars + specialChars;

        var random = new Random();

        return new string(Enumerable.Repeat(allChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public string GenerateRandomOTP(int length)
    {
        const string chars = "0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
