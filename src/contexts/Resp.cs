namespace webapi.src.contexts
{
  public class Resp
  {
    public static BaseResponse OK => new BaseResponse();
    public static BaseResponse ERROR => new BaseResponse(){ 
      status = "error",
      code = 503 ,
      message = "Error"
    };
  }
}