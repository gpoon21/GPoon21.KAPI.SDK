using System.Net.Http.Headers;
using System.Text.Json;

namespace GPoon21.KAPI.SDK.QRPayment;

public static partial class KBankQR {

    public class QRPaymentClient {
        private readonly CustomerInfo _customerInfo;

        public static async Task<QRPaymentClient> CreateAsync(string consumerId, string consumerSecret,
            IRequestMode headerModifier) {
            CustomerInfo clientInfo = await GetClientCredentials(consumerId, consumerSecret, headerModifier);
            return new QRPaymentClient(clientInfo);
        }

        private QRPaymentClient(CustomerInfo customerInfo) {
            _customerInfo = customerInfo;
        }

        public Task<QRResponse> RequestQR(
            QRRequest request,
            IRequestMode headerModifier) {
            return KBankQR.RequestQR(request, _customerInfo.AccessToken, headerModifier);
        }

        public Task<QRInquiryResponse> InquiryQR(
            QRInquiryRequest request,
            IRequestMode headerModifier) {
            return KBankQR.InquiryPayment(request, _customerInfo.AccessToken, headerModifier);
        }

    }

    private static async Task<T> SendRequestAsync<T>(HttpRequestMessage request) {
        using HttpClient httpClient = new();

        // Step 4: Send a request
        HttpResponseMessage response = await httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) {
            throw new ApplicationException(
                $"Failed to get token. Status: {response.StatusCode}, Response: {responseBody}");
        }

        return JsonSerializer.Deserialize<T>(responseBody)!;
    }


    public interface IRequestMode {
        public string BaseUrl { get; }
        public void Modify(HttpRequestHeaders headers);

        /// <summary>
        /// Add 'x-test-mode: true' to <see cref="HttpRequestHeaders"/> and 'env-id' with a specific value.
        /// </summary>
        public class Test : IRequestMode {
            private readonly string _envId;

            public Test(string envId) {
                _envId = envId;
            }

            public string BaseUrl {
                get { return "https://openapi-sandbox.kasikornbank.com"; }
            }

            public void Modify(HttpRequestHeaders headers) {
                headers.Add("x-test-mode", "true");
                headers.Add("env-id", _envId);
            }
        }

        public class Default : IRequestMode {
            public required string BaseUrl { get; init; }
            public void Modify(HttpRequestHeaders headers) { }
        }
    }


}