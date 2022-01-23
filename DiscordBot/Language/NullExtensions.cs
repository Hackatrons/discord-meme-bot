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
    public static TMember ThrowIfNull<TMember>([NoEnumeration] this TMember value, string name)
    {
        if (value != null) return value;

        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        throw new ArgumentNullException(name);
    }
}