namespace MyApp.Util.CustomLogging;

using System.Data;
using System.Data.SqlClient;

public interface ILogRepository
{
  public Task Insert(List<LogMessage> logMessages);
}

public class LogRepository : ILogRepository
{
	private const string TABLE = "Log";
	private readonly string ConnectionString;
	private readonly HashSet<int> transientSqlErrors = new HashSet<int>()
	{
		-2, 258, 4060
	};
	private const int MAX_RETRIES = 3;
	private const int RETRY_SECONDS = 5;
	public LogRepository(string connectionString)
	{
		ConnectionString = connectionString;
	}
	public async Task Insert(List<LogMessage> logMessages)
	{
		string folderPath = "./../logs/";
		System.IO.FileInfo file = new System.IO.FileInfo(folderPath);
		file.Directory?.Create(); // If the directory already exists, this method does nothing.

		using( StreamWriter fileWriter = new StreamWriter( Path.Combine( folderPath ,"MyLog.txt") , append:true ) )
		{
			foreach (var logMessage in logMessages)
			{
				await fileWriter.WriteLineAsync( System.Text.Json.JsonSerializer.Serialize( logMessage ) );
			}
			
		}
	}
}