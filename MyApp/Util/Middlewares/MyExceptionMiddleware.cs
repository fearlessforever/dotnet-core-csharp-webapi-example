
namespace MyApp.Util.Middlewares;

public class MyExceptionMiddleware{
    private readonly RequestDelegate next;

    public MyExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
          await next(context);
      }
      catch (Exception ex)
      {
          await __handleException(context, ex);
      }
    }

    private static Task __handleException(HttpContext context, Exception ex)
    {
        // If the exception is not user based
        var response = new Util.MyApiResponse{ code = 500 , status = "error" };
        context.Response.StatusCode = response.code;
        context.Response.ContentType = "application/json";

        if (ex is Util.MyException)
        {
            response.code = context.Response.StatusCode = (ex as Util.MyException)?.statusCode ?? 500;
            response.message = ex.Message ;
                
        }else if( ex is not Util.MyException ){
            // 500 if unexpected
            response.message = ex.Message;
        }

        Util.CustomLogging.MyLoggerService.LogStatic( LogLevel.Critical , ex.ToString() );

        return context.Response.WriteAsync( response.ToString() );
    }

    public static void callBackExceptionHandle( IApplicationBuilder app ){
      app.Run( async context => {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error ?? new Exception( context.Response.StatusCode == 404 ? "Route: Not Found" : "Something is Wrong" );

        if( exception is Util.MyException)
        {
          context.Response.StatusCode = (exception as Util.MyException)?.statusCode ?? context.Response.StatusCode ;
        }

        Util.CustomLogging.MyLoggerService.LogStatic( LogLevel.Error , exception?.Message == null ? "Something is Wrong" : exception.Message );
      
        await context.Response.WriteAsync( new Util.MyApiResponse{
              message =  exception?.Message == null ? "Something is Wrong" : exception.Message ,
              code = context.Response.StatusCode ,
              status = "error" ,
        }.ToString() );

      });
    }

}