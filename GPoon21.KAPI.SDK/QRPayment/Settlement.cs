using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK.QRPayment;

public static partial class KBankQR {
    public class QRSettlementRequest {
        public required string PartnerTransactionUid { get; init; }
        public required string PartnerId { get; init; }
        public required string PartnerSecret { get; init; }
        public required string MerchantId { get; init; }
        public required string TerminalId { get; init; }
        public required QRType QRType { get; init; }
    }

    private class SerializableQRSettlementRequest {
        [JsonPropertyName("partnerTxnUid")]
        public required string PartnerTransactionUid { get; init; }

        [JsonPropertyName("partnerId")]
        public required string PartnerId { get; init; }

        [JsonPropertyName("partnerSecret")]
        public required string PartnerSecret { get; init; }

        [JsonPropertyName("merchantId")]
        public required string MerchantId { get; init; }

        [JsonPropertyName("terminalId")]
        public required string TerminalId { get; init; }

        [JsonPropertyName("qrType")]
        [JsonConverter(typeof(QRTypeJsonConverter))]
        public required QRType QRType { get; init; }

        [JsonPropertyName("requestDt")]
        public required string RequestDateTime { get; init; }

        public static SerializableQRSettlementRequest FromRequest(QRSettlementRequest request) {
            return new SerializableQRSettlementRequest {
                PartnerTransactionUid = request.PartnerTransactionUid,
                PartnerId = request.PartnerId,
                PartnerSecret = request.PartnerSecret,
                MerchantId = request.MerchantId,
                TerminalId = request.TerminalId,
                QRType = request.QRType,
                RequestDateTime = DateTimeOffset.Now.ToString("o")
            };
        }
    }

    public class QRSettlementResponse {
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

        [JsonPropertyName("settlementAmount")]
        public required decimal SettlementAmount { get; init; }

        [JsonPropertyName("settlementCurrencyCode")]
        public required string SettlementCurrencyCode { get; init; }

        [JsonPropertyName("accountNo")]
        public required string AccountNumber { get; init; }

        [JsonPropertyName("accountName")]
        public required string AccountName { get; init; }
    }

    /// <summary>
    /// API documentation: https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Settlement
    /// </summary>
    public static async Task<QRSettlementResponse> GetSettlement(
        QRSettlementRequest request,
        string accessToken,
        IRequestMode requestMode) {

        // Build URL using requestMode.BaseUrl
        UriBuilder builder = new(requestMode.BaseUrl);
        builder.Path = "v1/qrpayment/settlement";

        // Create an HTTP request with the built URL
        HttpRequestMessage httpRequest = new(HttpMethod.Post, builder.ToString());
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMode.Modify(httpRequest.Headers);

        // Convert to serializable request and add timestamp
        SerializableQRSettlementRequest serializableRequest = SerializableQRSettlementRequest.FromRequest(request);

        // Set JSON body
        string jsonContent = JsonSerializer.Serialize(serializableRequest);
        httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send a request using SendRequestAsync
        return await SendRequestAsync<QRSettlementResponse>(httpRequest);
    }
}