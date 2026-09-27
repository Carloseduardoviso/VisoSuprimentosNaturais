using System.Globalization;
using System.Text.RegularExpressions;

namespace VisoERP.Infra.Helper;

public static class DecimalPtBrParser
{
    public static bool TryParse(string rawValue, out decimal value)
    {
        var temVirgula = rawValue.Contains(',');
        var formatoValido = temVirgula
            ? Regex.IsMatch(rawValue, @"^[+-]?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$", RegexOptions.CultureInvariant)
            : Regex.IsMatch(rawValue, @"^[+-]?\d+(?:\.\d+)?$", RegexOptions.CultureInvariant);
        if (!formatoValido)
        {
            value = default;
            return false;
        }

        var normalized = temVirgula
            ? rawValue.Replace(".", string.Empty, StringComparison.Ordinal).Replace(',', '.')
            : rawValue;
        return decimal.TryParse(normalized, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out value);
    }
}
