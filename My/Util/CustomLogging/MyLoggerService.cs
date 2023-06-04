namespace My.Util.CustomLogging;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

public interface IMyLoggerService : IHostedService
{
	void Log(LogLevel logLevel, string message);
}

public class MyLoggerService : BackgroundService, IMyLoggerService
{
	private static Channel<LogMessage>? logMessageQueueStatic;
	private readonly Channel<LogMessage> logMessageQueue;
	private readonly IHostApplicationLifetime HostApplicationLifetime;
	private const int MAX_BATCH_SIZE = 10;
	private readonly ILogRepository LogRepository;
	public MyLoggerService( IHostApplicationLifetime hostApplicationLifetime)
	{
		logMessageQueue = Channel.CreateUnbounded<LogMessage>();
		LogRepository = new LogRepository( "" );
		HostApplicationLifetime = hostApplicationLifetime;
		logMessageQueueStatic = logMessageQueue;
	}
	public async override Task StopAsync(CancellationToken cancellationToken)
	{
		await base.StopAsync(cancellationToken);
	}
	protected async override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while(!stoppingToken.IsCancellationRequested)
		{

			try
			{
				Console.WriteLine("Waiting for log messages");
				await Task.Delay( 3000 , cancellationToken:stoppingToken );
				var batch = await GetBatch(stoppingToken);

				Console.WriteLine($"Got a batch with {batch.Count}(s) log messages. Bulk inserting them now.");

				//Let non-retryable errors from this bubble up and crash the service
				await LogRepository.Insert(batch);
			}
			catch (TaskCanceledException)
			{
				Console.WriteLine("Stopping token was canceled, which means the service is shutting down.");
				return;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n Force Stop APP : Fatal exception in database logger. Crashing service. Error={ex}");
				HostApplicationLifetime.StopApplication();
				return;
			}
		}
	}
	public void Log(LogLevel logLevel, string message)
	{
		//The reason to use Writer.TryWrite() is because it's synchronous.
		//We want the logging to be as fast as possible for the client, so
		//we don't want the overhead of async

		//Note: We're using an unbounded Channel, so TryWrite() *should* only fail 
		//if we call writer.Complete().
		//Guard against it anyway


		var logMessage = new LogMessage()
		{
			Message = message,
			ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId,
			Timestamp = DateTimeOffset.Now ,
			LogLevel = logLevel ,
		};

		if (!logMessageQueue.Writer.TryWrite(logMessage))
		{
			throw new InvalidOperationException("Failed to write the log message");
		}
	}

	public static void LogStatic(LogLevel logLevel, string message)
	{
		var logMessage = new LogMessage()
		{
			Message = message,
			ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId,
			Timestamp = DateTimeOffset.Now ,
			LogLevel = logLevel ,
		};

		if( logMessageQueueStatic != null ){
			if (!logMessageQueueStatic.Writer.TryWrite(logMessage))
			{
				throw new InvalidOperationException("Failed to write the log message");
			}
		}
	}
	private async Task<List<LogMessage>> GetBatch(CancellationToken cancellationToken)
	{
		await logMessageQueue.Reader.WaitToReadAsync(cancellationToken);

		var batch = new List<LogMessage>();

		while (batch.Count < MAX_BATCH_SIZE && logMessageQueue.Reader.TryRead(out LogMessage? message))
		{
			batch.Add(message);
		}

		return batch;
	}
}