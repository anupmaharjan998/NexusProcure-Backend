namespace NexusProcure.Shared.Helper;

public static class TokenGenerator
{
    private static readonly Random _random = new Random();

    public static string GenerateToken(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}