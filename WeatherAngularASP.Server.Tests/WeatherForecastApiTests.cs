using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WeatherAngularASP.Server.Controllers;
using Xunit;

namespace WeatherAngularASP.Server.Tests;

public class WeatherForecastApiTests : IClassFixture<WebApplicationFactory<WeatherForecastController>>
{
    private readonly HttpClient _client;

    public WeatherForecastApiTests(WebApplicationFactory<WeatherForecastController> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_ReturnsFiveForecastsWithExpectedJsonContract()
    {
        using var response = await _client.GetAsync("/weatherforecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
        Assert.Equal(5, document.RootElement.GetArrayLength());

        foreach (var forecast in document.RootElement.EnumerateArray())
        {
            var propertyNames = forecast.EnumerateObject().Select(property => property.Name).Order();
            Assert.Equal(new[] { "date", "summary", "temperatureC", "temperatureF" }, propertyNames);
            Assert.True(DateOnly.TryParseExact(forecast.GetProperty("date").GetString(),
                "yyyy-MM-dd", out _));
            Assert.True(forecast.GetProperty("temperatureC").TryGetInt32(out _));
            Assert.True(forecast.GetProperty("temperatureF").TryGetInt32(out _));
            Assert.False(string.IsNullOrWhiteSpace(forecast.GetProperty("summary").GetString()));
        }
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        using var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Get_ReturnsNextFiveDaysWithTemperaturesWithinRange()
    {
        var dateBeforeRequest = DateOnly.FromDateTime(DateTime.Now);
        using var response = await _client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var dateAfterRequest = DateOnly.FromDateTime(DateTime.Now);
        Assert.Equal(5, document.RootElement.GetArrayLength());

        var day = 1;
        foreach (var forecast in document.RootElement.EnumerateArray())
        {
            var date = DateOnly.ParseExact(forecast.GetProperty("date").GetString()!, "yyyy-MM-dd");
            Assert.InRange(date, dateBeforeRequest.AddDays(day), dateAfterRequest.AddDays(day));
            Assert.InRange(forecast.GetProperty("temperatureC").GetInt32(), -20, 54);
            day++;
        }
    }
}
