using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace DiscordBot.Language;

public static class NullExtensions
{
    /// <summary>
    /// Throws an exception if the value is null, otherwise returns the value itself.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="name">The name of the member</param>
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    [ContractAnnotation("value:null => halt")]
    public static TMember ThrowIfNull<TMember>([NoEnumeration] this TMember value, [CallerArgumentExpression("value")] string? name = null)
    {
        ArgumentNullException.ThrowIfNull(value, name);
        return value;
    }

    /// <summary>
    /// Throws an exception if the string is null or whitespace, otherwise returns the value itself.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="name">The name of the member</param>
    [ContractAnnotation("value:null => halt")]
    public static string ThrowIfNullOrWhitespace(this string value, [CallerArgumentExpression("value")] string? name = null)
    {
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        name!.ThrowIfNullOrWhitespace();

        throw new ArgumentOutOfRangeException(name, value, "Value must not be null or whitespace");
    }
}