using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK;

public static partial class KBank {
    public class SslTestResponse {
        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("certificateObjs")]
        public required CertificateInfo CertificateInfo { get; init; }
    }

    public class CertificateInfo {
        [JsonPropertyName("subject")]
        public required string Subject { get; init; }
    }

    /// <summary>
    /// Tests two-way SSL authentication with the provided certificate
    /// API documentation: Exercise 16 - Two-way SSL authentication
    /// </summary>
    public static async Task<SslTestResponse> TestTwoWaySslAuthentication(
        string accessToken,
        X509Certificate2 clientCertificate) {

        // Configure the HTTP client handler with the client certificate
        HttpClientHandler handler = new();
        handler.ClientCertificates.Add(clientCertificate);
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        using HttpClient client = new(handler);
        IRequestMode.Test requestMode = new();
        // Build URL using requestMode.BaseUrl
        UriBuilder builder = new(requestMode.BaseUrl);
        builder.Path = "exercise/ssl";

        // Create an HTTP request with the built URL
        HttpRequestMessage httpRequest = new(HttpMethod.Get, builder.ToString());
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMode.Modify(httpRequest.Headers);

        // Add required headers
        httpRequest.Headers.Add("x-test-mode", "True");

        // Send request and handle response
        using HttpResponseMessage response = await client.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        string jsonResponse = await response.Content.ReadAsStringAsync();
        SslTestResponse? result = JsonSerializer.Deserialize<SslTestResponse>(jsonResponse);

        if (result == null)
            throw new JsonException("Failed to deserialize response");

        return result;
    }
}