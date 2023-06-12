using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using My.Util.CustomLogging;
using MyDataEF;

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

//========================================= DB Entity Framework Service =================================================
builder.Services.AddDbContext<MyDB>( options =>{
    // options.UseInMemoryDatabase("Testing");
    // options.UseInMemoryDatabase("Testing");
    options.UseSqlite( builder.Configuration.GetConnectionString("MySqlLite") );
    options.UseSqlite( options =>{
        options.MigrationsAssembly("My");
    });
});
//=======================================================================================================================

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

using( var scope = app.Services.CreateScope() )
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MyDB>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

//======================================================= Error Handler ============================================================
// app.UseDeveloperExceptionPage();
// app.UseExceptionHandler("/Error");
app.UseExceptionHandler(configure: configApp =>{
    configApp.Run( async context =>{
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        // options => {
        //     options.InjectStylesheet("/swagger-ui/custom.css");
        //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        //     options.RoutePrefix = "swagger";
        // }
    );
    // app.UseStaticFiles();
}

// app.UseHttpsRedirection();

// app.UseRouting();
app.UseAuthorization();

app.UseResponseCompression(); // ini untuk aktifkan fitur compress response size

// ini contoh cara simple menambahkan route dan mengirim response
// untuk menggunakan ini , harus disertai dengan -> app.UseRouting();
// app.UseEndpoints(configure: check =>{
//     check.Map("/check" , async context => await context.Response.WriteAsync("Check Test") );
//     app.MapGet("/Hi" , () => "Hello!" );
//     app.MapGet( "{*url}" , context => throw new My.Util.MyException("Not Found" , 404 ) );
// });

app.MapControllers();



app.Run();
