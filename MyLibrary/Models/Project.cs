namespace MyLibrary.Models;

public record Project
{
  public int ProjectId;
  public List<Ticket>? Tickets { get; set; }
}