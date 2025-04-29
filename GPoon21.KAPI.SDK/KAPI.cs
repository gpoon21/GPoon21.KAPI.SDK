using System.Net.Http.Headers;
using System.Text;

namespace GPoon21.KAPI.SDK;

public static class KAPI {

    public static async Task<string> GetAccessTokenAsync(string consumerId, string consumerSecret) {
        using HttpClient httpClient = new();
        // OAuth token endpoint
        string tokenUrl = "https://openapi-sandbox.kasikornbank.com/v2/oauth/token";

        // Step 1: Create Basic Authorization header value
        string credentials = $"{consumerId}:{consumerSecret}";
        string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        // Step 2: Create HTTP request
        HttpRequestMessage request = new(HttpMethod.Post, tokenUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
        request.Headers.Add("x-test-mode", "true");
        request.Headers.Add("env-id", "OAUTH2");

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

        return responseBody; // Contains access_token and expires_in
    }

}