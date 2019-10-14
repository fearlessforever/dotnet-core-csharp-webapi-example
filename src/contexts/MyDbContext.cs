using Microsoft.EntityFrameworkCore;

namespace webapi.src.contexts
{
  public class MyDbContext : ExtendsContext
  {
    
    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
    {
      // var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyDb.db" };
      //   var connectionString = connectionStringBuilder.ToString();
      //   var connection = new SqliteConnection(connectionString);

        optionsBuilder.UseSqlite("Filename=MyDatabase.db");
    }
  }
}