using System.Text;

namespace GithubRepositoryAnalyzer.Config.Extensions;

public static class StringX
{
    public static byte[] ToByteArray(this string s)
    {
        return string.IsNullOrEmpty(s) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(s);
    }
    
    public static bool EqualsIgnoreCase(this string first, string second)
    {
        return !string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(second) && first.Equals(second, StringComparison.InvariantCultureIgnoreCase);
    }
}