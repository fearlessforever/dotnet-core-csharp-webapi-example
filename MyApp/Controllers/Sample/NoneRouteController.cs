using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers;

[ApiController]
// [Route("{*url}", Order = 999)] // catch all url
// [TypeFilter(typeof(Util.Filters.ExceptionInterceptor))]
[Route("sample/[controller]", Order = 999)]
public class NoneRouteController : ControllerBase
{
  [HttpGet]
  public void Index()
  {
    using( var scope = APP.instance.Services.CreateScope() )
    {
        var services = scope.ServiceProvider;
        var myDbContext = services.GetRequiredService<MyDataEF.MyDB>();
        myDbContext.Database.EnsureDeleted();
        myDbContext.Database.EnsureCreated();
    }
    
    throw new Util.MyException("Route: Not Found" , 404);
  }

  [HttpGet("[action]")]
  public async Task<IResult> test3(){
    
    return await Task.Run( () => {
      Task.Delay(1000);
      return Util.MyApiResponse.generate( data: new List<int>(){1,2} );
    });
  }
}