namespace SwiftlyS2.Shared.Menus;

/// <summary>
/// Provides a fluent builder interface for creating and configuring menus.
/// All methods support chaining for convenient menu construction.
/// </summary>
public interface IMenuBuilderAPI
{
    /// <summary>
    /// Gets the design interface for this menu.
    /// </summary>
    public IMenuDesignAPI Design { get; }

    /// <summary>
    /// Creates a hierarchical menu by setting a parent menu that players can navigate back to.
    /// </summary>
    /// <param name="parent">The parent menu.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI WithParent( IMenuAPI parent );

    /// <summary>
    /// Adds an option to the menu.
    /// </summary>
    /// <param name="option">The menu option to add.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI AddOption( IMenuOption option );

    /// <summary>
    /// Enables or disables sound effects for menu interactions.
    /// </summary>
    /// <param name="playSound">True to play sounds, false to remain silent.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI PlaySound( bool playSound = false );

    /// <summary>
    /// Controls whether players can move while the menu is open.
    /// </summary>
    /// <param name="freeze">True to freeze player movement, false to allow movement.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI FreezePlayer( bool freeze = false );

    /// <summary>
    /// Sets how long the menu stays open before automatically closing.
    /// </summary>
    /// <param name="seconds">Time in seconds before auto-close. Set to 0 to disable.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI AutoClose( float seconds = 0f );

    /// <summary>
    /// Customizes which button(s) players press to select menu options.
    /// </summary>
    /// <param name="keyBind">Key bind for selection.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI OverrideSelectButton( KeyBind keyBind );

    /// <summary>
    /// Customizes which button(s) players press to navigate through menu options.
    /// </summary>
    /// <param name="keyBind">Key bind for navigation.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI OverrideMoveButton( KeyBind keyBind );

    /// <summary>
    /// Customizes which button(s) players press to close the menu.
    /// </summary>
    /// <param name="keyBind">Key bind for closing the menu.</param>
    /// <returns>This builder for method chaining.</returns>
    public IMenuBuilderAPI OverrideExitButton( KeyBind keyBind );

    /// <summary>
    /// Builds the menu and returns the final menu instance.
    /// </summary>
    /// <returns>The built menu instance.</returns>
    public IMenuAPI Build();
}