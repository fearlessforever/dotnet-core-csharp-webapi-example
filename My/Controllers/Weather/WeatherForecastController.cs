using Microsoft.AspNetCore.Mvc;
using My.Util;
using My.Util.CustomLogging;

using NewWeatherForecast = MyLibrary.Weather.WeatherForecast;

namespace My.Controllers.Weather;

[ApiController]
[Route("weather/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMyLoggerService Logger;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger , IMyLoggerService loggerDB )
    {
        _logger = logger;
        Logger = loggerDB;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<NewWeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new NewWeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("[action]")]
    public async Task<IResult> test2()
    {
        var listData = Enumerable.Range(1, 5).Select(index => new NewWeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
        
        Logger.Log(LogLevel.Debug, $"GET /Recipes/{listData}");
        Logger.Log(LogLevel.Debug, System.Text.Json.JsonSerializer.Serialize(listData) );

        return await Task.Run(() => MyApiResponse.generate( data:listData ) );
    }
    
    [HttpGet("[action]")]
    public async Task<IResult> test()
    {
        // throw new System.Web(404);
        throw new MyException("Invalid: your request is not complete" , 401 );
        throw new MyException("Not Allowed" , 403 );
        throw new Exception("Oke deh");
        throw new InvalidDataException();
        return await Task.Run(() => MyApiResponse.generate( data:Summaries , code:401 , isHeaderStatus:true ) );
        // return MyApiResponse.generate( data:Summaries , code:404 , isHeaderStatus:true );
    }
}
