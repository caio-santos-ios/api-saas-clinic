using System.Text.Json.Serialization;

namespace api_clinic.src.Requests.Signature
{
    public class AsaasWebhookRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        [JsonPropertyName("payment")]
        public AsaasWebhookPayment? Payment { get; set; }

        [JsonPropertyName("subscription")]
        public AsaasWebhookSubscription? Subscription { get; set; }
    }

    public class AsaasWebhookPayment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("customer")]
        public string Customer { get; set; } = string.Empty;

        [JsonPropertyName("subscription")]
        public string? Subscription { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("dueDate")]
        public string DueDate { get; set; } = string.Empty;

        [JsonPropertyName("paymentDate")]
        public string? PaymentDate { get; set; }
    }

    public class AsaasWebhookSubscription
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("customer")]
        public string Customer { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
