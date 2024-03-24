// using System.Text.Json;
// using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyApp.Util.CustomLogging;
using MyDataEF;
using MyExceptionMiddleware = MyApp.Util.Middlewares.MyExceptionMiddleware;

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
    options.UseSqlite( builder.Configuration.GetConnectionString("MySqlLite") , sqLiteOptions => {
        sqLiteOptions.MigrationsAssembly("MyApp");
    });
    // options.UseSqlite( options =>{ options.MigrationsAssembly("MyApp"); });
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
APP.set(app);

//======================================================= Error Handler ============================================================
// app.UseDeveloperExceptionPage();
// app.UseExceptionHandler("/Error");
// app.UseStatusCodePagesWithReExecute("/StatusCode", "?statusCode={0}");
app.UseMiddleware<MyExceptionMiddleware>();
app.UseStatusCodePages( MyExceptionMiddleware.callBackExceptionHandle );
// app.UseExceptionHandler( MyApp.Util.Extensions.MyExceptionHandler.callBackExceptionHandle );
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
//     app.MapGet( "{*url}" , context => throw new MyApp.Util.MyException("Not Found" , 404 ) );
// });

app.MapControllers();
app.Run();
