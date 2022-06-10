using System.ComponentModel;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Helper for converting an enum type into a collection of
/// <see cref="ListOption"/> types.
/// </summary>
public static class EnumListOptionHelper
{
    /// <summary>
    /// Converts the specified enum <paramref name="type"/> to a collection
    /// of <see cref="ListOption"/>, using the the string name as the value
    /// and trying to convert the name to a human readable description.
    /// </summary>
    public static ICollection<ListOption> ConvertToOptions(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsEnum)
        {
            throw new ArgumentException("Enum type expected.", nameof(type));
        }

        var values = Enum.GetValues(type);
        var options = new List<ListOption>(values.Length);

        foreach (Enum value in values)
        {
            var stringValue = value.ToString();
            string description = GetDescription(value, stringValue);

            options.Add(new ListOption(description, stringValue));
        }

        return options;
    }

    private static string GetDescription(Enum value, string stringValue)
    {
        var field = value.GetType().GetField(stringValue);
        var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Any())
        {
            return attributes[0].Description;
        }
        else
        {
            return TextFormatter.PascalCaseToSentence(stringValue);
        }
    }
}
