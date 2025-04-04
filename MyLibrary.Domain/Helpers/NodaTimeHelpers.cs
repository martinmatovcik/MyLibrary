using NodaTime;

namespace MyLibrary.Domain.Helpers;

public static class NodaTimeHelpers
{
    private static readonly SystemClock Clock = SystemClock.Instance;
    private static readonly DateTimeZone DefaultTimeZone = DateTimeZoneProviders.Tzdb["Europe/Prague"];

    public static Instant NowInstant(DateTimeZone? timeZone = null) => 
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).ToInstant();
    
    public static LocalDateTime Now(DateTimeZone? timeZone = null) => 
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).LocalDateTime;

    public static LocalDate Today(DateTimeZone? timeZone = null) =>
        Clock.GetCurrentInstant().InZone(timeZone ?? DefaultTimeZone).Date;
}