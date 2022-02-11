namespace ReactProjectsAuthApi.Middlewares
{
    public class WSRequestAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public WSRequestAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,ILogger<WSRequestAuthMiddleware> logger)
        {
            if (context.Request.QueryString.ToString().Contains("access_token"))
            {
                string query = context.Request.QueryString.Value.ToString();
                string token = query.Substring(query.IndexOf("access_token=")+"access_token".Length+1);
                logger.LogError(token);
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }

            await _next(context);

        }
    }
}
