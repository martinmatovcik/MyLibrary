using NodaTime;

namespace MyLibrary.Domain.Helpers;

/// <summary>
/// Provides utility methods for working with NodaTime to retrieve current time information.
/// </summary>
public static class NodaTimeHelpers
{
    private static readonly SystemClock Clock = SystemClock.Instance;
    
    /// <summary>
    /// The default time zone used when none is specified (Europe/Prague).
    /// </summary>
    private static readonly DateTimeZone DefaultTimeZone = DateTimeZoneProviders.Tzdb["Europe/Prague"];

    /// <summary>
    /// Gets the current instant in the specified time zone (or default if none provided).
    /// </summary>
    /// <param name="timeZone">Optional time zone. If not specified, the default time zone (Europe/Prague) is used.</param>
    /// <returns>The current instant.</returns>
    public static Instant NowInstant(DateTimeZone? timeZone = null) => 
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).ToInstant();
    
    /// <summary>
    /// Gets the current local date and time in the specified time zone (or default if none provided).
    /// </summary>
    /// <param name="timeZone">Optional time zone. If not specified, the default time zone (Europe/Prague) is used.</param>
    /// <returns>The current local date and time.</returns>
    public static LocalDateTime Now(DateTimeZone? timeZone = null) => 
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).LocalDateTime;

    /// <summary>
    /// Gets the current date in the specified time zone (or default if none provided).
    /// </summary>
    /// <param name="timeZone">Optional time zone. If not specified, the default time zone (Europe/Prague) is used.</param>
    /// <returns>The current date.</returns>
    public static LocalDate Today(DateTimeZone? timeZone = null) =>
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).Date;
}