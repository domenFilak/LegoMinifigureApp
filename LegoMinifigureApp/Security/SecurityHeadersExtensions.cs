namespace LegoMinifigureApp.Security
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;

                //never guess files type (image always treated as image)
                headers["X-Content-Type-Options"] = "nosniff";

                //other sites cant show pages inside a frame
                headers["X-Frame-Options"] = "DENY";

                //other sites only know which site a visitor came from, not the full url
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

                //browser features this app should not use turned off
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=()";

                await next();
            });
        }
    }
}
