namespace ReactProjectsAuthApi.Middlewares
{
    public static class WSRequestAuthExts
    {
        public static void UseWSRequestAuth(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<WSRequestAuthMiddleware>();
        }
    }
}
