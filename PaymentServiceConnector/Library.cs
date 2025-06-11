using System.Net.Http.Headers;
using System.Text.Json;

namespace PaymentServiceConnector {
    public class HttpConnector {
        private readonly string baseURL;
        private readonly HttpClient client;

        public HttpConnector(string baseURL) {
            this.baseURL = baseURL;
            client = new HttpClient();
        }

        public void ConfirmPayment(string transactionId) {
            var payload = new { transactionId };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            client.PostAsync(baseURL + "/confirm", content);
        }

        public void CancelPayment(string transactionId) {
            var payload = new { transactionId };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            client.PostAsync(baseURL + "/cancel", content);
        }
    }
}
