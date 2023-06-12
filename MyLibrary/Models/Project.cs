namespace MyLibrary.Models;

public class Project
{
  public int ProjectId;
  public List<Ticket>? Tickets { get; set; }
}