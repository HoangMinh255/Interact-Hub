using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace InteractHub.Application.Common;

public static class StringNormalizationExtensions
{
    public static string NormalizeForSearch(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var trimmed = input.Trim().ToLowerInvariant();
        var noDiacritics = RemoveDiacritics(trimmed);
        var normalizedSpaces = Regex.Replace(noDiacritics, @"\s+", " ");

        return normalizedSpaces;
    }

    public static string RemoveDiacritics(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var character in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(character);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string Truncate(this string? input, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        if (maxLength <= 0)
        {
            return string.Empty;
        }

        return input.Length <= maxLength ? input : input[..maxLength];
    }
}