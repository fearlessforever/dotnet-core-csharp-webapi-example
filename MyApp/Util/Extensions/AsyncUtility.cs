namespace MyApp.Util.Extensions;

 public static class AsyncUtility {

    /**
    * Usage : MyAsyncMethod().MyAsyncTaskWithoutAwait(t => log.ErrorFormat("An error occurred while calling MyAsyncMethod:\n{0}", t.Exception) ); 
    */
    public static void MyAsyncTaskWithoutAwait(this Task task, Action<Task> exceptionHandler)
    {
      //  var dummy = task.ContinueWith(t => exceptionHandler(t), TaskContinuationOptions.OnlyOnFaulted); 
      task.ContinueWith(t => exceptionHandler(t), TaskContinuationOptions.OnlyOnFaulted); 
    }
} 