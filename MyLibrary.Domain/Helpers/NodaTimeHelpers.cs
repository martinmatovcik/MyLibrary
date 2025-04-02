using NodaTime;

namespace MyLibrary.Domain.Helpers;

public static class NodaTimeHelpers
{
    private static readonly SystemClock Clock = SystemClock.Instance;
    private static readonly DateTimeZone DefaultTimeZone = DateTimeZoneProviders.Tzdb["Europe/Prague"];

    /// <summary>
    /// Gets actual time 'Now' as Instant
    /// </summary>
    /// <param name="timeZone">When not given, it defaults to "Europe/Prague"</param>
    /// <returns>Instant.Now</returns>
    public static Instant NowInstant(DateTimeZone? timeZone = null) => 
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).ToInstant();
    
    /// <summary>
    /// Gets actual time 'Now' as LocalDateTime
    /// </summary>
    /// <param name="timeZone">When not given, it defaults to "Europe/Prague"</param>
    /// <returns>LocalDateTime.Now</returns>
    public static LocalDateTime Now(DateTimeZone? timeZone = null) => 
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).LocalDateTime;

    /// <summary>
    /// Gets today 'Today' as LocalDate
    /// </summary>
    /// <param name="timeZone">When not given, it defaults to "Europe/Prague"</param>
    /// <returns>LocalDate.Today</returns>
    public static LocalDate Today(DateTimeZone? timeZone = null) =>
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).Date;
}