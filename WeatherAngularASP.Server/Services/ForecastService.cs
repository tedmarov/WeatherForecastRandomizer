namespace WeatherAngularASP.Server.Services
{
    public interface IForecastService
    {
        IReadOnlyList<WeatherForecast> GetForecasts(int days);
    }

    public sealed class RandomForecastService : IForecastService
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly TimeProvider _timeProvider;

        public RandomForecastService(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public IReadOnlyList<WeatherForecast> GetForecasts(int days)
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

            return Enumerable.Range(1, days).Select(index => new WeatherForecast
            {
                Date = today.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
