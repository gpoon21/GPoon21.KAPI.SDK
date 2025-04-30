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

    public static async Task<CustomerInfo> GetClientCredentials(string consumerId,
        string consumerSecret,
        IRequestMode requestMode) {
        // OAuth token endpoint
        UriBuilder builder = new(requestMode.BaseUrl);
        builder.Path = "v2/oauth/token";

        // Create a Basic Authorization header value
        string credentials = $"{consumerId}:{consumerSecret}";
        string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        // Create an HTTP request
        HttpRequestMessage request = new(HttpMethod.Post, builder.ToString());
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
        requestMode.Modify(request.Headers);

        // Set form body
        request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
            "application/x-www-form-urlencoded");

        // Send a request using SendRequestAsync
        return await SendRequestAsync<CustomerInfo>(request);
    }
}