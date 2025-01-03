using System.Text;

namespace Uper.IntegrationTest.Turso.Tools;

public static class Util
{
    public static string GetInitials(this string camelCaseString)
    {
        if (string.IsNullOrWhiteSpace(camelCaseString))
            return string.Empty;

        var initials = new StringBuilder();
        foreach (char c in camelCaseString)
        {
            if (char.IsUpper(c))
            {
                initials.Append(c);
            }
        }

        return initials.ToString();
    }
}
