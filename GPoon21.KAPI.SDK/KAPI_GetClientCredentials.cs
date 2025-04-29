using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK;

public static partial class KAPI {
    public class CustomerInfo {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("client_id")]
        public required string ClientId { get; init; }

        [JsonPropertyName("developer.email")]
        public required string Email { get; init; }

        [JsonPropertyName("expires_in")]
        public required string ExpireIn { get; init; }

        [JsonPropertyName("scope")]
        public required string Scope { get; init; }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; init; }
    }

    /// <summary>
    /// API documentation: https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Identity%20confirmation
    /// </summary>
    public static async Task<CustomerInfo> GetClientCredentials(
        string consumerId,
        string consumerSecret,
        IHeaderModifier? headerModifier = null) {
        using HttpClient httpClient = new();
        // OAuth token endpoint
        string tokenUrl = "https://openapi-sandbox.kasikornbank.com/v2/oauth/token";

        // Step 1: Create Basic Authorization header value
        string credentials = $"{consumerId}:{consumerSecret}";
        string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        // Step 2: Create HTTP request
        HttpRequestMessage request = new(HttpMethod.Post, tokenUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
        headerModifier ??= new IHeaderModifier.Default();
        headerModifier.Modify(request.Headers);

        // Step 3: Set form body
        request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
            "application/x-www-form-urlencoded");

        // Step 4: Send request
        HttpResponseMessage response = await httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) {
            throw new ApplicationException(
                $"Failed to get token. Status: {response.StatusCode}, Response: {responseBody}");
        }

        return JsonSerializer.Deserialize<CustomerInfo>(responseBody)!;
    }
}