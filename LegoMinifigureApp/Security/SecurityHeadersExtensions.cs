namespace LegoMinifigureApp.Security
{
    public static class SecurityHeadersExtensions
    {
        public static WebApplication UseSecurityHeaders(this WebApplication app)
        {
            var contentSecurityPolicy = BuildContentSecurityPolicy(app.Environment.IsDevelopment());

            app.Use(async (context, next) =>
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

                //only load and run from allowed sources
                headers["Content-Security-Policy"] = contentSecurityPolicy;

                await next();
            });

            return app;
        }

        private static string BuildContentSecurityPolicy(bool isDevelopment)
        {
            var devScriptSources = isDevelopment ? " http://localhost:* https://localhost:*" : "";
            var devConnectSources = isDevelopment ? " ws://localhost:* wss://localhost:* http://localhost:* https://localhost:*" : "";

            return string.Join("; ",
                "default-src 'self'",
                "script-src 'self'" + devScriptSources, //js only from own files
                "style-src 'self'", //css only from own files
                "img-src 'self' data:", //images from this site, and data: images boostrap
                "font-src 'self'", //fonts only from this site
                "connect-src 'self'" + devConnectSources, //js can only communicate to this site
                "form-action 'self'", //forms can only submit to this site
                "frame-ancestors 'none'", //no other sites can embed these pages
                "base-uri 'self'", //blocks injected <base> tag
                "object-src 'none'"); //no plugins
        }
    }
}
