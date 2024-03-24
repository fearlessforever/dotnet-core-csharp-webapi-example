using Microsoft.AspNetCore.Mvc;
using MyApp.Util;
using MyApp.Util.CustomLogging;
using MyDataEF;
using Microsoft.EntityFrameworkCore;

using NewWeatherForecast = MyLibrary.Weather.WeatherForecast;

namespace MyApp.Controllers.Sample;

[ApiController]
[Route("sample/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMyLoggerService Logger;
    private readonly MyDB DB;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger , IMyLoggerService loggerDB , MyDB db )
    {
        _logger = logger;
        Logger = loggerDB;
        DB = db;
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
        // throw new MyException("Invalid: your request is not complete" , 401 );
        // throw new MyException("Not Allowed" , 403 );
        // throw new Exception("Oke deh");
        // throw new InvalidDataException();
        return await Task.Run( async () =>{
            for( var i=0; i< 100; i++){
                DB.Projects.Add(
                    new MyLibrary.Models.Project{}
                );
            }

            await DB.SaveChangesAsync();

           return MyApiResponse.generate( data: DB.Projects.ToList() , code:200 , isHeaderStatus:true );
        });
        // return MyApiResponse.generate( data:Summaries , code:404 , isHeaderStatus:true );
    }

    [HttpGet("[action]")]
    public IResult check(){
        // MyLoggerService.LogStatic( LogLevel.Critical , DB.Random() );
        // var _project = (from project in DB.Projects orderby Guid.NewGuid() select new { newID = project.ProjectId ,  oke= " project gaje z" }  ).ToQueryString();
        var data = new { 
            Tickets = (from ticket in DB.Tickets select new { newID = ticket.TicketId ,  oke= "DEH" } ).Take(27).ToArray() , 
            Projects = (from project in DB.Projects select new { newID = project.ProjectId ,  oke= " project gaje z" }  ).Take(27).ToList() ,
            ProjectLain = DB.Projects.FromSql($"SELECT ProjectId FROM projects ORDER BY RANDOM() LIMIT 3 ").Select( a => new { Newid = a.ProjectId } ).ToList() ,
            ProjectLain2 =(from project in DB.Projects.FromSql($"SELECT ProjectId FROM projects ORDER BY RANDOM() LIMIT 3 ") select new { NewID = project.ProjectId }  )/* .Select( a => new { Newid = a.ProjectId } ) */.ToList() ,
            // Test = (from ticket in DB.Tickets select ticket).ToQueryString()
        };
        
        return MyApiResponse.generate( data: data , code:200 , isHeaderStatus:true ) ;
    }
}
