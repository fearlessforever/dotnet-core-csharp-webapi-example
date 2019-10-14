using System;
namespace webapi.src.contexts
{
  public class BaseResponse 
  {
    public string version { get; }
    public string datetime { get; }
    public long timestamp { get; }
    public string status { set; get; }
    public int code { set; get; }
    public string message { set; get; }
    public dynamic data { set; get; }
    public dynamic errors { set; get; }

    public BaseResponse()
    {
        DateTime now = DateTime.UtcNow;
        version = "1.0";
        datetime = now.ToString("u");
        timestamp = ((DateTimeOffset)now).ToUnixTimeSeconds();
        status = "success";
        code = 200;
        message = "Ok";
        data = null;
        errors = null;
    }
  }
}