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
}