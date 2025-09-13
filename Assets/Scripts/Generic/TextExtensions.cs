using UnityEngine;

/// <summary>
/// Extension methods for formatting text with TextMeshPro-style rich text tags.
/// Works with both TextMeshPro and Unity's built-in UI text where rich text is enabled.
/// </summary>
public static class TextExtensions
{
    /// <summary>
    /// Wraps the message in a color tag using an RGB hex code from a Unity Color.
    /// </summary>
    /// <param name="message">The object or text to display.</param>
    /// <param name="colour">The Unity Color to apply.</param>
    /// <returns>Rich text string with color applied.</returns>
    public static string ToColouredString(this object message, Color colour)
    {
        var colorHex = ColorUtility.ToHtmlStringRGB(colour);
        return $"<color=#{colorHex}>{message}</color>";
    }

    /// <summary>
    /// Wraps the message in a bold tag.
    /// </summary>
    /// <param name="message">The object or text to display.</param>
    /// <returns>Rich text string in bold.</returns>
    public static string ToBoldString(this object message)
    {
        return $"<b>{message}</b>";
    }

    /// <summary>
    /// Wraps the message in an italic tag.
    /// </summary>
    /// <param name="message">The object or text to display.</param>
    /// <returns>Rich text string in italics.</returns>
    public static string ToItalicString(this object message)
    {
        return $"<i>{message}</i>";
    }

    /// <summary>
    /// Wraps the message in a size tag to set an absolute font size (in points).
    /// </summary>
    /// <param name="message">The object or text to display.</param>
    /// <param name="size">Font size in points (absolute, not relative).</param>
    /// <returns>Rich text string with custom size applied.</returns>
    public static string ToSizeString(this object message, int size)
    {
        return $"<size={size}>{message}</size>";
    }

    /// <summary>
    /// Wraps the message in a size tag to set a relative font size change.
    /// </summary>
    /// <param name="message">The object or text to display.</param>
    /// <param name="sizeDelta">Relative change in size (e.g. +10, -5).</param>
    /// <returns>Rich text string with relative size adjustment applied.</returns>
    public static string ToRelativeSizeString(this object message, int sizeDelta)
    {
        return $"<size={(sizeDelta >= 0 ? "+" : "")}{sizeDelta}>{message}</size>";
    }
}
