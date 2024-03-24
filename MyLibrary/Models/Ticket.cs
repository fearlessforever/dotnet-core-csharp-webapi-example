namespace MyLibrary.Models;

public record Ticket
{
  public int TicketId;
  public int ProjectId { get; set; }
}