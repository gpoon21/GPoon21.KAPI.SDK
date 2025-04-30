using System.Text.Json;

namespace GPoon21.KAPI.SDK.QRPayment;

public static partial class KBankQR {

    public class Client {
        private readonly CustomerInfo _customerInfo;

        public static async Task<Client> CreateAsync(string consumerId, string consumerSecret,
            IRequestMode headerModifier) {
            CustomerInfo clientInfo = await GetClientCredentials(consumerId, consumerSecret, headerModifier);
            return new Client(clientInfo);
        }

        private Client(CustomerInfo customerInfo) {
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


        public Task<QRSettlementResponse> GetSettlement(
            QRSettlementRequest request,
            IRequestMode headerModifier) {
            return KBankQR.GetSettlement(request, _customerInfo.AccessToken, headerModifier);
        }

        public Task<QRCancelResponse> CancelQR(
            QRCancelRequest request,
            IRequestMode headerModifier) {
            return KBankQR.CancelQR(request, _customerInfo.AccessToken, headerModifier);
        }

        public Task<QRVoidResponse> VoidPayment(
            QRVoidRequest request,
            IRequestMode headerModifier) {
            return KBankQR.VoidPayment(request, _customerInfo.AccessToken, headerModifier);
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


}