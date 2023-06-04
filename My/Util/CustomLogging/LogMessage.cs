namespace My.Util.CustomLogging;

public class LogMessage
{
  public int ThreadId { get; set; }
	public string? Message { get; set; }
	public DateTimeOffset Timestamp { get; set; }

	public LogLevel LogLevel { get; set; }
}