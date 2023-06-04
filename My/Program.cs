using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using My.Util.CustomLogging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression( configureOptions: options => {
    // ini untuk aktifkan service fitur compress response size , di function ini hnyak tidak di set opsi tambahan
    // misal opsi
    // options.EnableForHttps = true;
});

//======================================== Background Task ( in this case , Custom Logger )
// builder.Services.AddSingleton<ILogRepository, LogRepository>( _ => new LogRepository( builder.Configuration.GetConnectionString("DefaultConnection") ?? "" ));

// line ini menambahkan service tapi ada warning kemungkinan reference null
// builder.Services.AddSingleton<IMyLoggerService, MyLoggerService>();
// builder.Services.AddHostedService<MyLoggerService>( sp => sp.GetService<IMyLoggerService>() as MyLoggerService );

// ini yang benar , Custom Logger
builder.Services.AddSingleton<MyLoggerService>();
builder.Services.AddSingleton<IMyLoggerService>( implementationFactory: provider => provider.GetRequiredService<MyLoggerService>() );
builder.Services.AddHostedService<MyLoggerService>( implementationFactory: provider => provider.GetRequiredService<MyLoggerService>() );
//====================================================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// app.UseRouting();
app.UseAuthorization();


//======================================================= Error Handler ============================================================
// app.UseDeveloperExceptionPage();
// app.UseExceptionHandler("/Error");
app.UseExceptionHandler( _appExceptionHandler =>{
    _appExceptionHandler.Run( async context =>{
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        if( exception is My.Util.MyException)
        {
            context.Response.StatusCode = (exception as My.Util.MyException)?.statusCode ?? context.Response.StatusCode ;
        }

        MyLoggerService.LogStatic( LogLevel.Error , exception?.Message == null ? "Something is Wrong" : exception.Message );
        
        await context.Response.WriteAsync( JsonSerializer.Serialize( new My.Util.MyApiResponse{
             message =  exception?.Message == null ? "Something is Wrong" : exception.Message ,
             code = context.Response.StatusCode ,
             status = "error" ,
        }));
    });
});
//====================================================================================================================================

app.UseResponseCompression(); // ini untuk aktifkan fitur compress response size

// ini contoh cara simple menambahkan route dan mengirim response
// untuk menggunakan ini , harus disertai dengan -> app.UseRouting();
// app.UseEndpoints( _=>{
//     app.MapGet("/check" , async context => await context.Response.WriteAsync("Check Test") );
//     app.MapGet("/Hi" , () => "Hello!" );
//     app.MapGet( "{*url}" , context => throw new My.Util.MyException("Not Found" , 404 ) );
// });

app.MapControllers();



app.Run();
