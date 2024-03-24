namespace MyApp.Util.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using MyApp.Util.CustomLogging;
// using Microsoft.AspNetCore.Builder.IApplicationBuilder;

public static class MyExceptionHandler {

  // public static void UseMyExceptionHandler(this IApplicationBuilder app , Microsoft.AspNetCore.Http.RequestDelegate handler ){
  //   // configure.
  //   app.Run( async context =>{
  //       context.Response.ContentType = "application/json";

  //       var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

  //       if( exception is MyApp.Util.MyException)
  //       {
  //           context.Response.StatusCode = (exception as MyApp.Util.MyException)?.statusCode ?? context.Response.StatusCode ;
  //       }
        
  //       if( exception == null ){
  //         exception = new Exception("Not Found");
  //       }
  //       MyLoggerService.LogStatic( LogLevel.Debug , JsonSerializer.Serialize( new { Oke = "Deh" } ));

  //       MyLoggerService.LogStatic( LogLevel.Error , exception?.Message == null ? "Something is Wrong" : exception.Message );
        
  //       await context.Response.WriteAsync( JsonSerializer.Serialize( new MyApp.Util.MyApiResponse{
  //            message =  exception?.Message == null ? "Something is Wrong" : exception.Message ,
  //            code = context.Response.StatusCode ,
  //            status = "error" ,
  //       }));
  //   });
  // }

  public static void callBackExceptionHandle( IApplicationBuilder app ){
    app.Run( callBackWriteAsync );
  }

  public static async Task<Task> callBackWriteAsync( HttpContext context ){
      context.Response.ContentType = "application/json";

      var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error ?? new Exception( context.Response.StatusCode == 404 ? "Route: Not Found" : "Something is Wrong" );

      if( exception is Util.MyException)
      {
        context.Response.StatusCode = (exception as Util.MyException)?.statusCode ?? context.Response.StatusCode ;
      }

      MyLoggerService.LogStatic( LogLevel.Critical , exception?.Message == null ? "Something is Wrong" : exception.Message );
      
      await context.Response.WriteAsync( new Util.MyApiResponse{
            message =  exception?.Message == null ? "Something is Wrong" : exception.Message ,
            code = context.Response.StatusCode ,
            status = "error" ,
      }.ToString() );

      return Task.CompletedTask;
  }
}