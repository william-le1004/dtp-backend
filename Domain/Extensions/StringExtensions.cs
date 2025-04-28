namespace Domain.Extensions;

public static class StringExtensions
{
    public static string Random(this string source)
    {
        Random random = new Random();
        char[] result = new char[source.Length];

        for (int i = 0; i < source.Length; i++)
        {
            result[i] = source[random.Next(source.Length)];
        }

        return new string(result);
    }
    
    public static long ToLong(this string input)
    {
        return Convert.ToInt64(input, 16);
    }
    
    public static string RemoveSubstring(this string source, string toRemove)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(toRemove))
            return source;

        return source.Replace(toRemove, string.Empty);
    }
}