using System.Net.Http.Headers;

namespace GPoon21.KAPI.SDK;

public static partial class KAPI {

    public class Client {
        private readonly CustomerInfo _customerInfo;

        public static async Task<Client> CreateAsync(string consumerId, string consumerSecret,
            IHeaderMode? headerModifier = null) {
            CustomerInfo clientInfo = await GetClientCredentials(consumerId, consumerSecret, headerModifier);
            return new Client(clientInfo);
        }

        private Client(CustomerInfo customerInfo) {
            _customerInfo = customerInfo;
        }

        public Task<QRResponse> RequestQR(
            QRRequest request,
            IHeaderMode? headerModifier = null) {
            return KAPI.RequestQR(request, _customerInfo.AccessToken, headerModifier);
        }

        public Task<QRInquiryResponse> InquiryQR(
            QRInquiryRequest request,
            IHeaderMode? headerModifier = null) {
            return KAPI.InquiryQR(request, _customerInfo.AccessToken, headerModifier);
        }

    }

    public interface IHeaderMode {

        public void Modify(HttpRequestHeaders headers);

        /// <summary>
        /// Add 'x-test-mode: true' to <see cref="HttpRequestHeaders"/> and 'env-id' with a specific value.
        /// </summary>
        public class Test : IHeaderMode {
            private readonly string _envId;

            public Test(string envId) {
                _envId = envId;
            }

            public void Modify(HttpRequestHeaders headers) {
                headers.Add("x-test-mode", "true");
                headers.Add("env-id", _envId);
            }
        }

        public class Default : IHeaderMode {
            public void Modify(HttpRequestHeaders headers) { }
        }

    }
}