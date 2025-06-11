using Microsoft.OpenApi.Models;

namespace PaymentWebhookView {
    public static class WebhookAuth {
        // OpenAPI operation parameter for webhook token header
        public static OpenApiOperation AddTokenHeaderParameter(OpenApiOperation operation) {
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter {
                Name = "X-Webhook-Token",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                },
                Description = "Secret token for webhook authentication"
            });

            return operation;
        }

        public static async ValueTask<object?> TokenFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var httpContext = context.HttpContext;
            var configuration = httpContext.RequestServices.GetService<IConfiguration>();
            var webhookSecret = configuration?["Webhook:SecretToken"] ?? string.Empty;
            var hasToken = httpContext.Request.Headers.TryGetValue("X-Webhook-Token", out var token);

            if (!hasToken || !string.Equals(token, webhookSecret, StringComparison.Ordinal)) {
                return Results.Unauthorized();
            }

            return await next(context);
        }
    }
}
