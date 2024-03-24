public static class APP
{
    private static WebApplication? _instance;
    public static WebApplication instance { 
      get {
        if( _instance == null)
          throw new MyApp.Util.MyException("APP is NULL",500);
        
        return _instance;
      }
    }

    public static void set( WebApplication app ){
        if( _instance != null )return;
        _instance = app;
    }
}