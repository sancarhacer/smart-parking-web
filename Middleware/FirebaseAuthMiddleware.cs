using FirebaseAdmin.Auth;

public class FirebaseAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FirebaseAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Authorization header var mı?
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) &&
            authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "");

            try
            {
                var decodedToken =
                    await FirebaseAuth.DefaultInstance
                        .VerifyIdTokenAsync(token);

                // 🔥 USER CONTEXT'E KOYULUYOR
                context.Items["uid"] = decodedToken.Uid;
                context.Items["email"] = decodedToken.Claims["email"];
            }
            catch
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Firebase Token");
                return;
            }
        }

        await _next(context);
    }
}
