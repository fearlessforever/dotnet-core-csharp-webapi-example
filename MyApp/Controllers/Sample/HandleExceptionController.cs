using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers.Sample;

[ApiController]
[Route("sample/[controller]")]
public class HandleExceptionController: ControllerBase{

  [HttpGet]
  [TypeFilter(typeof(Util.Filters.ExceptionInterceptor))]
  public void exceptionOnActionAttribute()
  {
    throw new Util.MyException("Forbidden: you are not allowed to access this feature" , 403);
  }

}