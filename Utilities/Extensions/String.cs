using System.Text.RegularExpressions;

namespace MainBot.Utilities.Extensions;
internal static class String
{
    public static string RemoveSpecialCharacters(this string str) => Regex.Replace(str, "[^a-zA-Z0-9_ ]+", "", RegexOptions.Compiled);
    public static bool ContainsSpecialCharacters(this string str) => new Regex("^[a-zA-Z0-9_ ]*$", RegexOptions.Compiled).IsMatch(str) is false;
}