using System.Text.Json;

using PaymentWebhookModel;
using PaymentServiceConnector;
using PaymentWebhookController;


namespace PaymentWebhookView {
    public class WebhookEventHandler {
        // Limit for processed transactions
        private readonly int MaxTransactionsBuffer;
        private readonly HashSet<string> TransactionsBuffer;
        private readonly Queue<string> TransactionQueue;
        private readonly object TransactionLock;
        private readonly HttpConnector ServiceConnector;
        private bool pushTransactionId (string transactionId) {
            lock (TransactionLock) {
                if (TransactionsBuffer.Contains(transactionId)) {
                    return false;
                }
                TransactionsBuffer.Add(transactionId);
                TransactionQueue.Enqueue(transactionId);

                if (TransactionQueue.Count > MaxTransactionsBuffer) {
                    var oldest = TransactionQueue.Dequeue();
                    TransactionsBuffer.Remove(oldest);
                }
            }
            return true;
        }

        public WebhookEventHandler(string serviceUrl, int maxTransactions)
        {
            MaxTransactionsBuffer = maxTransactions;
            TransactionsBuffer = new HashSet<string>();
            TransactionQueue = new Queue<string>();
            TransactionLock = new object();
            ServiceConnector = new HttpConnector(serviceUrl);
        }

        public async Task<IResult> Handle(HttpContext context) {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

            WebhookEvent.WebhookEvent? model;
            try {
                model = JsonSerializer.Deserialize<WebhookEvent.WebhookEvent>(body);
            }
            catch (JsonException) {
                return Results.BadRequest("Invalid JSON payload.");
            }
            if (model == null || string.IsNullOrEmpty(model.transaction_id)) {
                return Results.BadRequest("Missing transaction_id.");
            }
            if (!pushTransactionId(model.transaction_id)) {
                return Results.BadRequest("Webhook event already processed.");
            }
            if (!WebhookEventController.handle(model, ServiceConnector)) {
                return Results.BadRequest("Failed to process the webhook event.");
            }

            return Results.Ok("Webhook event processed successfully.");
        }
    }
}
