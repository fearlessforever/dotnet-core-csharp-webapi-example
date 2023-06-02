namespace My.Util;

public class MyException : Exception {
  public int statusCode { get; private set; } = 500;

  public MyException( string message , int code = 500 ) : base ( message)
  {
    statusCode = code;
  }
}