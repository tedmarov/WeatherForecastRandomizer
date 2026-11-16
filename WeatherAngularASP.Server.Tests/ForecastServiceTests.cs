using WeatherAngularASP.Server;
using WeatherAngularASP.Server.Services;
using Xunit;

namespace WeatherAngularASP.Server.Tests;

public class ForecastServiceTests
{
    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTimeProvider(DateTimeOffset now) => _now = now;

        public override DateTimeOffset GetUtcNow() => _now;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private static readonly string[] KnownSummaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly RandomForecastService _service =
        new(new FixedTimeProvider(new DateTimeOffset(2026, 11, 16, 12, 0, 0, TimeSpan.Zero)));

    [Fact]
    public void GetForecasts_ReturnsConsecutiveDaysStartingTomorrow()
    {
        var forecasts = _service.GetForecasts(5);

        Assert.Equal(5, forecasts.Count);
        Assert.Equal(
            new[]
            {
                new DateOnly(2026, 11, 17),
                new DateOnly(2026, 11, 18),
                new DateOnly(2026, 11, 19),
                new DateOnly(2026, 11, 20),
                new DateOnly(2026, 11, 21)
            },
            forecasts.Select(forecast => forecast.Date));
    }

    [Fact]
    public void GetForecasts_ProducesValuesWithinExpectedRanges()
    {
        foreach (var forecast in _service.GetForecasts(100))
        {
            Assert.InRange(forecast.TemperatureC, -20, 54);
            Assert.Equal(32 + (int)(forecast.TemperatureC / 0.5556), forecast.TemperatureF);
            Assert.Contains(forecast.Summary, KnownSummaries);
        }
    }
}
