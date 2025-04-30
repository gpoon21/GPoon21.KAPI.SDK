using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK.QRPayment;

public static partial class KBankQR {
    public class QRVoidRequest {
        public required string PartnerTransactionUid { get; init; }
        public required string PartnerId { get; init; }
        public required string PartnerSecret { get; init; }
        public required string MerchantId { get; init; }
        public string? TerminalId { get; init; }
        public required string OriginalPartnerTransactionUid { get; init; }
        public string? TransactionNumber { get; init; }
    }

    private class SerializableQRVoidRequest {
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

        public static SerializableQRVoidRequest FromRequest(QRVoidRequest request) {
            return new SerializableQRVoidRequest {
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

    public class QRVoidResponse {
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

        [JsonPropertyName("txnNo")]
        public required string TransactionNumber { get; init; }
    }

    /// <summary>
    /// API documentation: https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Void%20Payment
    /// </summary>
    public static async Task<QRVoidResponse> VoidPayment(
        QRVoidRequest request,
        string accessToken,
        IRequestMode requestMode) {

        // Build URL using requestMode.BaseUrl
        UriBuilder builder = new(requestMode.BaseUrl);
        builder.Path = "v1/qrpayment/void";

        // Create an HTTP request with the built URL
        HttpRequestMessage httpRequest = new(HttpMethod.Post, builder.ToString());
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMode.Modify(httpRequest.Headers);

        // Convert to serializable request and add timestamp
        SerializableQRVoidRequest serializableRequest = SerializableQRVoidRequest.FromRequest(request);

        // Set JSON body
        string jsonContent = JsonSerializer.Serialize(serializableRequest);
        httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send a request using SendRequestAsync
        return await SendRequestAsync<QRVoidResponse>(httpRequest);
    }
}