using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK;

public static partial class KAPI {
    public class QRRequest {
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
        public required string QRType { get; init; }

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
    }

    public class QRResponse {
        [JsonPropertyName("partnerTxnUid")]
        public required string PartnerTransactionUid { get; init; }

        [JsonPropertyName("qrCode")]
        public required string QRCode { get; init; }

        [JsonPropertyName("qrImage")]
        public required string QRImage { get; init; }
    }

    /// <summary>
    /// Requests a QR code for payment from K-Bank API
    /// API documentation: https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Request%20QR
    /// </summary>
    /// <param name="request">The QR request details</param>
    /// <param name="accessToken">The OAuth access token obtained from GetClientCredentials</param>
    /// <returns>QR response containing the QR code and image</returns>
    public static async Task<QRResponse> RequestQR(
        QRRequest request,
        string accessToken,
        IHeaderModifier? headerModifier = null) {
        using HttpClient httpClient = new();
        string apiUrl = "https://openapi-sandbox.kasikornbank.com/v1/qrpayment/request";

        // Create HTTP request
        HttpRequestMessage httpRequest = new(HttpMethod.Post, apiUrl);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        headerModifier ??= new IHeaderModifier.Default();
        headerModifier.Modify(httpRequest.Headers);

        // Set JSON body
        string jsonContent = JsonSerializer.Serialize(request);
        httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) {
            throw new ApplicationException(
                $"Failed to request QR code. Status: {response.StatusCode}, Response: {responseBody}");
        }

        return JsonSerializer.Deserialize<QRResponse>(responseBody)!;
    }
}