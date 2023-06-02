using Microsoft.AspNetCore.Mvc;

namespace My.Controllers.Weather;

[ApiController]
[Route("{*url}", Order = 999)] // catch all url
public class NoneRouteController : ControllerBase
{
  public void Get()
  {
    throw new My.Util.MyException("Route: Not Found" , 404);
  }
}