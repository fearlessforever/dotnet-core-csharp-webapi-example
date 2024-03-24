namespace MyApp.Util.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

/**
* 
Can be registered as Attribute in controller or in controller method , or can be registered as MVC filter
  example : 

  - builder.Services.AddControllers( mvcOptions =>{
      mvcOptions.Filters.Add<ExceptionInterceptor>();
    });

  - [HttpGet]
    [TypeFilter(typeof(Util.Filters.ExceptionInterceptor))]
    public void exceptionOnActionAttribute()
  
  - [ApiController]
    [TypeFilter(typeof(Util.Filters.ExceptionInterceptor))]
    [Route("sample/[controller]")]
    public class HandleExceptionController: ControllerBase

*/
public class ExceptionInterceptor : IAsyncExceptionFilter
{
 
    public Task OnExceptionAsync(ExceptionContext context)
    {
      Util.CustomLogging.MyLoggerService.LogStatic( LogLevel.Error , context.Exception.ToString() );

      //Logs your technical exception with stack trace below
      var response = new MyApiResponse(){ status = "error" };
      response.message = context.Exception.Message ;
      response.code =  ( context.Exception is Util.MyException ) ?  (context.Exception as Util.MyException)?.statusCode ?? 500 : 500 ;
      // response.code = 400 ;

      var jsonResult = new Microsoft.AspNetCore.Mvc.JsonResult( response );
      jsonResult.StatusCode = response.code ;

      context.Result = jsonResult ;
      return Task.CompletedTask;
    }
}