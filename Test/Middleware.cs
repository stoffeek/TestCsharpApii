public class Middleware
{

  public Middleware(WebApplication app, string serverName)
  {
    app.Use(async (context, next) =>
    {
      context.Response.OnStarting(() =>
      {
        SetServerHeader(context, serverName);
        var session = new SessionHandler();
        session.Touch(context);
        return Task.CompletedTask;
      });

      await next(context);
    });
  }

  public static void SetServerHeader(HttpContext context, string serverName)
  {
    var res = context.Response;
    res.Headers.Append("Server", serverName);
  }

  // Return the cookie value used for sessions
  // (set it first if it doesn't exist already)
  public static string GetCookieValue(HttpContext context)
  {
    var req = context.Request;
    var res = context.Response;

    string? cookieValue;
    req.Cookies.TryGetValue("session", out cookieValue);

    if (cookieValue == null)
    {
      cookieValue = Guid.NewGuid().ToString();
      res.Cookies.Append("session", cookieValue);
    }

    return cookieValue;
  }

}