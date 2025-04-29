using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK;

public static partial class KAPI {
    public class QRInquiryRequest {
        public required string PartnerTransactionUid { get; init; }
        public required string PartnerId { get; init; }
        public required string PartnerSecret { get; init; }
        public required string MerchantId { get; init; }
        public string? TerminalId { get; init; }
        public required string OriginalPartnerTransactionUid { get; init; }
        public string? TransactionNumber { get; init; }
    }

    private class SerializableQRInquiryRequest {
        [JsonPropertyName("partnerTxnUid")]
        public required string PartnerTransactionUid { get; init; }

        [JsonPropertyName("partnerId")]
        public required string PartnerId { get; init; }

        [JsonPropertyName("partnerSecret")]
        public required string PartnerSecret { get; init; }

        [JsonPropertyName("merchantId")]
        public required string MerchantId { get; init; }

        [JsonPropertyName("terminalId")]
        public string? TerminalId { get; init; }

        [JsonPropertyName("origPartnerTxnUid")]
        public required string OriginalPartnerTransactionUid { get; init; }

        [JsonPropertyName("txnNo")]
        public string? TransactionNumber { get; init; }

        [JsonPropertyName("requestDt")]
        public required string RequestDateTime { get; init; }

        public static SerializableQRInquiryRequest FromRequest(QRInquiryRequest request) {
            return new SerializableQRInquiryRequest {
                PartnerTransactionUid = request.PartnerTransactionUid,
                PartnerId = request.PartnerId,
                PartnerSecret = request.PartnerSecret,
                MerchantId = request.MerchantId,
                TerminalId = request.TerminalId,
                OriginalPartnerTransactionUid = request.OriginalPartnerTransactionUid,
                TransactionNumber = request.TransactionNumber,
                RequestDateTime = DateTimeOffset.Now.ToString("o")
            };
        }
    }
    

    public class QRInquiryResponse {
        [JsonPropertyName("partnerTxnUid")]
        public required string PartnerTransactionUid { get; init; }

        [JsonPropertyName("partnerId")]
        public required string PartnerId { get; init; }

        [JsonPropertyName("statusCode")]
        [JsonConverter(typeof(StatusCodeJsonConverter))]
        public required StatusCode StatusCode { get; init; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; init; }

        [JsonPropertyName("errorDesc")]
        public string? ErrorDescription { get; init; }

        [JsonPropertyName("txnStatus")]
        public required string TransactionStatus { get; init; }

        [JsonPropertyName("txnNo")]
        public string? TransactionNumber { get; init; }

        [JsonPropertyName("loyaltyId")]
        public string? LoyaltyId { get; init; }

        [JsonPropertyName("cardScheme")]
        public string? CardScheme { get; init; }

        [JsonPropertyName("cardNo")]
        public string? CardNumber { get; init; }

        [JsonPropertyName("approvalCode")]
        public string? ApprovalCode { get; init; }

        [JsonPropertyName("additionalTxnNo")]
        public string[]? AdditionalTransactionNumbers { get; init; }

        [JsonPropertyName("channel")]
        public string? Channel { get; init; }

        [JsonPropertyName("merchantId")]
        public required string MerchantId { get; init; }

        [JsonPropertyName("terminalId")]
        public string? TerminalId { get; init; }

        [JsonPropertyName("qrType")]
        [JsonConverter(typeof(QRTypeJsonConverter))]
        public required QRType QRType { get; init; }

        [JsonPropertyName("txnAmount")]
        public required string TransactionAmount { get; init; }

        [JsonPropertyName("txnCurrencyCode")]
        public required string TransactionCurrencyCode { get; init; }

        [JsonPropertyName("reference1")]
        public required string Reference1 { get; init; }

        [JsonPropertyName("reference2")]
        public required string Reference2 { get; init; }

        [JsonPropertyName("reference3")]
        public required string Reference3 { get; init; }

        [JsonPropertyName("reference4")]
        public required string Reference4 { get; init; }
    }

    /// <summary>
    /// API documentation: https://openapi-sandbox.kasikornbank.com/v1/qrpayment/v4/inquiry
    /// </summary>
    public static async Task<QRInquiryResponse> InquiryQR(
        QRInquiryRequest request,
        string accessToken,
        IHeaderMode? headerModifier = null) {
        using HttpClient httpClient = new();
        const string apiUrl = "https://openapi-sandbox.kasikornbank.com/v1/qrpayment/v4/inquiry";

        // Create HTTP request
        HttpRequestMessage httpRequest = new(HttpMethod.Post, apiUrl);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        headerModifier ??= new IHeaderMode.Default();
        headerModifier.Modify(httpRequest.Headers);

        // Convert to serializable request and add timestamp
        SerializableQRInquiryRequest serializableRequest = SerializableQRInquiryRequest.FromRequest(request);

        // Set JSON body
        string jsonContent = JsonSerializer.Serialize(serializableRequest);
        httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) {
            throw new ApplicationException(
                $"Failed to inquire QR payment status. Status: {response.StatusCode}, Response: {responseBody}");
        }

        return JsonSerializer.Deserialize<QRInquiryResponse>(responseBody)!;
    }
}