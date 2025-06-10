using System.Text.Json;

using PaymentWebhookModel;
using PaymentWebhookController;

namespace PaymentWebhookView {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapPost("/webhook", async (HttpContext context) => {
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var model = JsonSerializer.Deserialize<WebhookEvent.WebhookEvent>(body);

                // Check for required properties being null or empty
                if (model == null ||
                    string.IsNullOrWhiteSpace(model.transaction_id) ||
                    string.IsNullOrWhiteSpace(model.@event) ||
                    string.IsNullOrWhiteSpace(model.timestamp.ToString())) // Add other required fields as needed
                {
                    return Results.BadRequest("Missing or empty required fields.");
                }

                return Results.Ok(Say.hello(model.transaction_id));
            })
            .WithName("PostWebhookEvent")
            .WithSummary("Handles incoming webhook events")
            .WithDescription("Processes a event.")
            .Accepts<WebhookEvent.WebhookEvent>("application/json")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Webhook")
            .WithOpenApi();

            app.Run();
        }
    }
}
