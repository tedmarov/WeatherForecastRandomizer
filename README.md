<p align="center">
<img src="https://github.com/user-attachments/assets/99fde7a0-8da3-444e-b21a-2ed484347eba">
</p>

# Weather Forecast Randomizer
[![CI](https://github.com/tedmarov/WeatherForecastRandomizer/actions/workflows/ci.yml/badge.svg)](https://github.com/tedmarov/WeatherForecastRandomizer/actions/workflows/ci.yml)

An Angular 17 frontend displaying five randomly generated forecasts from an ASP.NET Core 8 API. Data is simulated, not a real weather prediction.

## Architecture

- `WeatherAngularASP.Server` — Web API. `WeatherForecastController` delegates to an injected `IForecastService`; `RandomForecastService` generates forecasts using .NET 8's `TimeProvider` so dates are testable. Exposes `/weatherforecast` and a `/health` endpoint.
- `weatherangularasp.client` — Angular SPA that fetches and displays the forecasts.
- `WeatherAngularASP.Server.Tests` — xUnit tests, run by GitHub Actions on every push and pull request to `main` alongside the Angular Karma tests.

## Run locally

Prerequisites: .NET 8 SDK, Node.js and npm.

From the repository root, start the API without the Visual Studio SPA launch profile:

```sh
dotnet run --project WeatherAngularASP.Server --no-launch-profile -p:ShouldRunNpmInstall=false -- --urls http://localhost:5266
```

In another terminal (POSIX shell):

```sh
cd weatherangularasp.client
npm ci
ASPNETCORE_URLS=http://localhost:5266 npm start
```

Open http://localhost:4200. The frontend proxies `/weatherforecast` to the API. Check API liveness at http://localhost:5266/health.

## Tests

Backend — from the repository root:

```sh
dotnet test WeatherAngularASP.Server.Tests/WeatherAngularASP.Server.Tests.csproj -p:ShouldRunNpmInstall=false
```

HTTP tests host the API in memory and check status codes, JSON shape, five upcoming dates and temperature bounds; unit tests pin service behavior with a fixed `TimeProvider`. No browser or running server is required.

Frontend — from `weatherangularasp.client` (requires Chrome):

```sh
npm test
```
