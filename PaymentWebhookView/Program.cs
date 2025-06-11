using System.Text.Json;
using PaymentWebhookModel;
using PaymentWebhookController;
using PaymentServiceConnector;

namespace PaymentWebhookView {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = builder.Configuration;
            var webhookSecret = configuration?["Webhook:SecretToken"] ?? string.Empty;
            var serviceUrl = configuration?["PaymentService:BaseUrl"] ?? string.Empty;

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            var serviceConnector = new HttpConnector(serviceUrl);

            var webhookRoute = app.MapPost("/webhook", async (HttpContext context) => {
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

                WebhookEvent.WebhookEvent? model = null;
                try {
                    model = JsonSerializer.Deserialize<WebhookEvent.WebhookEvent>(body);
                }
                catch (JsonException) {
                    return Results.BadRequest("Invalid JSON payload.");
                }

                var result = WebhookEventController.handle(model, serviceConnector);

                return Results.Ok(new { success = result });
            })
            .WithTags("Webhook")
            .WithName("PostWebhookEvent")
            .WithSummary("Handles incoming webhook events")
            .WithDescription("Processes a event.")
            .WithOpenApi(WebhookAuth.AddTokenHeaderParameter)
            .AddEndpointFilter(WebhookAuth.TokenFilter)
            .Accepts<WebhookEvent.WebhookEvent>("application/json")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

            app.Run();
        }
    }
}
