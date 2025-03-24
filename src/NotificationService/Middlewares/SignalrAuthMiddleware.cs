namespace NotificationService.Middlewares
{
    public class SignalrAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SignalrAuthMiddleware> _logger;

        public SignalrAuthMiddleware(RequestDelegate next, ILogger<SignalrAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;

            if (!request.Path.Value!.Contains("negotiate"))
            {
                if (request.Query.TryGetValue("access_token", out var accessToken))
                {
                    // Append the token to the request headers
                    var token = accessToken.ToString();
                    if (!token.ToLower().StartsWith("bearer"))
                        token = $"Bearer {token}";

                    // Avoid adding the header if it already exists
                    if (!request.Headers.ContainsKey("Authorization"))
                    {
                        request.Headers.Add("Authorization", token);
                        _logger.LogInformation("Token added for route {0}", request.Path.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("Token missing for route {0}", request.Path.Value);
                }
            }

            await _next(httpContext); // Single invocation
        }
    }
}
