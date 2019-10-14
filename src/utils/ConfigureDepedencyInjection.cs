using Microsoft.Extensions.DependencyInjection;
using webapi.src.services;

namespace webapi.src.utils
{
  public static class ConfigureDepedencyInjection
  {
    public static IServiceCollection ConfigureDepedencyInjectionMyServices( this IServiceCollection services )
    {
      services.AddScoped<ICategoryService , CategoryService>();
      return services;
    }
  }
}