using PaymentWebhookModel;

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

            var handler = new WebhookEventHandler(serviceUrl, 1000);

            app.MapPost("/webhook", async (HttpContext context) => {
                return await handler.Handle(context);
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
