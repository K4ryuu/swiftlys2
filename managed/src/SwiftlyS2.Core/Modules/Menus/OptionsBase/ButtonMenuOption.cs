namespace SwiftlyS2.Core.Menus.OptionsBase;

/// <summary>
/// Represents a clickable button menu option.
/// </summary>
public sealed class ButtonMenuOption : MenuOptionBase
{
    /// <summary>
    /// Creates an instance of <see cref="ButtonMenuOption"/> with dynamic text updating capabilities.
    /// </summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    public ButtonMenuOption(
        string text,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : base(updateIntervalMs, pauseIntervalMs)
    {
        Text = text;
        PlaySound = true;
    }
}