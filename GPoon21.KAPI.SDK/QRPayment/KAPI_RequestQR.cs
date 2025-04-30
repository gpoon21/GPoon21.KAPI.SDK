using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK.QRPayment;

public static partial class KApi {
    public class QRRequest {
        public required string PartnerTransactionUid { get; init; }
        public required string PartnerId { get; init; }
        public required string PartnerSecret { get; init; }
        public required string MerchantId { get; init; }
        public string? TerminalId { get; init; }
        public required QRType QRType { get; init; }
        public required decimal TransactionAmount { get; init; }
        public required string TransactionCurrencyCode { get; init; }
        public required string Reference1 { get; init; }
        public string? Reference2 { get; init; }
        public string? Reference3 { get; init; }
        public string? Reference4 { get; init; }
    }

    private class SerializableQRRequest {
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

        [JsonPropertyName("qrType")]
        [JsonConverter(typeof(QRTypeJsonConverter))]
        public required QRType QRType { get; init; }

        [JsonPropertyName("txnAmount")]
        public required decimal TransactionAmount { get; init; }

        [JsonPropertyName("txnCurrencyCode")]
        public required string TransactionCurrencyCode { get; init; }

        [JsonPropertyName("reference1")]
        public required string Reference1 { get; init; }

        [JsonPropertyName("reference2")]
        public string? Reference2 { get; init; }

        [JsonPropertyName("reference3")]
        public string? Reference3 { get; init; }

        [JsonPropertyName("reference4")]
        public string? Reference4 { get; init; }

        [JsonPropertyName("requestDt")]
        public required string RequestDateTime { get; init; }

        public static SerializableQRRequest FromRequest(QRRequest request) {
            return new SerializableQRRequest {
                PartnerTransactionUid = request.PartnerTransactionUid,
                PartnerId = request.PartnerId,
                PartnerSecret = request.PartnerSecret,
                MerchantId = request.MerchantId,
                TerminalId = request.TerminalId,
                QRType = request.QRType,
                TransactionAmount = request.TransactionAmount,
                TransactionCurrencyCode = request.TransactionCurrencyCode,
                Reference1 = request.Reference1,
                Reference2 = request.Reference2,
                Reference3 = request.Reference3,
                Reference4 = request.Reference4,
                RequestDateTime = DateTimeOffset.Now.ToString("o")
            };
        }
    }

    public class QRResponse {
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

        [JsonPropertyName("accountName")]
        public required string AccountName { get; init; }

        [JsonPropertyName("qrCode")]
        public required string QRCode { get; init; }

        [JsonPropertyName("sof")]
        [JsonConverter(typeof(ReturnedQRTypeArrayJsonConverter))]
        public required ReturnedQRType[] SourceOfFunds { get; init; }
    }

    /// <summary>
    /// API documentation: https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Request%20QR
    /// </summary>
    public static async Task<QRResponse> RequestQR(
        QRRequest request,
        string accessToken,
        IRequestMode requestMode) {

        // Build URL using requestMode.BaseUrl
        UriBuilder builder = new(requestMode.BaseUrl);
        builder.Path = "v1/qrpayment/request";

        // Create an HTTP request with the built URL
        HttpRequestMessage httpRequest = new(HttpMethod.Post, builder.ToString());
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMode.Modify(httpRequest.Headers);

        // Convert to serializable request and add timestamp
        SerializableQRRequest serializableRequest = SerializableQRRequest.FromRequest(request);

        // Set JSON body
        string jsonContent = JsonSerializer.Serialize(serializableRequest);
        httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send a request using SendRequestAsync
        return await SendRequestAsync<QRResponse>(httpRequest);
    }
}