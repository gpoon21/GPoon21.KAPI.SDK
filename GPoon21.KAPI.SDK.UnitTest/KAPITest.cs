using System.Text.Json;
using Xunit.Abstractions;

namespace GPoon21.KAPI.SDK.UnitTest;

public class KAPITest {
    private readonly ITestOutputHelper _outputHelper;

    public KAPITest(ITestOutputHelper outputHelper) {
        _outputHelper = outputHelper;
    }


    [Fact]
    public async Task GetClientCredentials_Success() {

        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        KAPI.CustomerInfo result = await KAPI.GetClientCredentials(customerId, customerSecret, new KAPI.IHeaderModifier.Test("OAUTH2"));
        _outputHelper.WriteLine(JsonSerializer.Serialize(result));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GenerateThaiQRCode_Success() {
        // Get required environment variables
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);
        string? partnerId = Environment.GetEnvironmentVariable(nameof(partnerId));
        Assert.NotNull(partnerId);
        string? partnerSecret = Environment.GetEnvironmentVariable(nameof(partnerSecret));
        Assert.NotNull(partnerSecret);
        string? merchantId = Environment.GetEnvironmentVariable(nameof(merchantId));
        Assert.NotNull(merchantId);
        string? partnerUID = Environment.GetEnvironmentVariable(nameof(partnerUID));
        Assert.NotNull(partnerUID);
        
        // First, get the access token
        KAPI.CustomerInfo credentials = await KAPI.GetClientCredentials(customerId, customerSecret, new KAPI.IHeaderModifier.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create QR request
        KAPI.QRRequest qrRequest = new() {
            PartnerTransactionUid = partnerUID,
            PartnerId = partnerId,
            PartnerSecret = partnerSecret,
            MerchantId = merchantId,
            QRType = "3",
            TransactionAmount = 100.00m,
            TransactionCurrencyCode = "THB",
            Reference1 = "INV001",
            Reference2 = "HELLOWORLD",
            Reference3 = "INV001",
            Reference4 = "INV001",
        };

        // Request QR code
        KAPI.QRResponse result = await KAPI.RequestQR(qrRequest, credentials.AccessToken, new KAPI.IHeaderModifier.Test("QR002"));

        // Log the response
        _outputHelper.WriteLine(JsonSerializer.Serialize(result));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(qrRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.NotNull(result.QRCode);
    }

}