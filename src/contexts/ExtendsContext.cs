using Microsoft.EntityFrameworkCore;
using webapi.src.models;

namespace webapi.src.contexts
{
  public class ExtendsContext : DbContext
  {
    public DbSet<Category> Categories{ get; set;}
  }
}