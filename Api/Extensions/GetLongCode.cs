namespace Api.Extensions;

public static class GetLongCode
{
    public static long ToLong(this string input)
    {
        long result = 0;
        foreach (char c in input)
        {
            result = result * 36 + (char.ToLower(c) - 'a' + 10);  // Convert each char to a base-36 number
        }
        return result;
    }

    // Converts a long back to the original string (Base-36 decoding)
    public static string LongToString(this long value)
    {
        string result = "";
        while (value > 0)
        {
            char c = (char)('a' + (value % 36) - 10);  // Convert back from base-36 to character
            result = c + result;
            value /= 36;
        }
        return result;
    }
}