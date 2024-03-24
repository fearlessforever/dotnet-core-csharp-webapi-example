namespace MyDataEF;

using Microsoft.EntityFrameworkCore;
using MyLibrary.Models;

public class MyDB : DbContext
{
  public MyDB( DbContextOptions options ) : base(options)
  {
    
  }

  public DbSet<Project> Projects { get; set; }
  public DbSet<Ticket> Tickets { get; set; }

  public Random Randomize { get{ return new Random(); } }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Project>().HasKey( a => a.ProjectId );
    modelBuilder.Entity<Ticket>().HasKey( a => a.TicketId );
    modelBuilder.Entity<Project>().HasMany<Ticket>( p => p.Tickets ).WithOne().HasForeignKey( a => a.ProjectId ).OnDelete( DeleteBehavior.Restrict );

    // modelBuilder.Entity<Project>().HasNoKey();
    // modelBuilder.Entity<Ticket>().HasNoKey();

    // modelBuilder.Entity<Project>().HasData(
    //   new Project{ ProjectId = 1 },
    //   new Project{ ProjectId = 2 }
    // );

    // modelBuilder.Entity<Ticket>().HasData(
    //   new Ticket{ ProjectId = 1 , TicketId = 1 },
    //   new Ticket{ ProjectId = 1 , TicketId = 2 },
    //   new Ticket{ ProjectId = 2 , TicketId = 3 }
    // );

  }
  
  public Guid Random(){
    return new Guid();
    // return new Random();
  }

}
