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

        KAPI.CustomerInfo result =
            await KAPI.GetClientCredentials(customerId, customerSecret, new KAPI.IHeaderModifier.Test("OAUTH2"));
        _outputHelper.WriteLine(JsonSerializer.Serialize(result));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GenerateThaiQRCode_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get required environment variables
        KAPI.CustomerInfo credentials =
            await KAPI.GetClientCredentials(customerId, customerSecret, new KAPI.IHeaderModifier.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create QR request with specified parameters
        KAPI.QRRequest qrRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0001",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            QRType = "3",
            TransactionAmount = 100.00m,
            TransactionCurrencyCode = "THB",
            Reference1 = "INV001",
            Reference2 = "HELLOWORLD",
            Reference3 = "INV001",
            Reference4 = "INV001",
        };

        // Request QR code
        KAPI.QRResponse result =
            await KAPI.RequestQR(qrRequest, credentials.AccessToken, new KAPI.IHeaderModifier.Test("QR002"));

        // Log the response
        _outputHelper.WriteLine(JsonSerializer.Serialize(result));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(qrRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.NotNull(result.QRCode);
    }

    [Fact]
    public async Task GetQRCreditCard_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get access token
        KAPI.CustomerInfo credentials =
            await KAPI.GetClientCredentials(customerId, customerSecret, new KAPI.IHeaderModifier.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create QR request with specified parameters
        KAPI.QRRequest qrRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0001-2",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            QRType = "4",                // QR Credit Card type
            TransactionAmount = 100.00m, // Example amount, can be modified as needed
            TransactionCurrencyCode = "THB",
            Reference1 = "INV001",
            Reference2 = "HELLOWORLD",
            Reference3 = "INV001",
            Reference4 = "INV001"
        };

        // Request QR code with specified environment
        KAPI.QRResponse result =
            await KAPI.RequestQR(qrRequest, credentials.AccessToken, new KAPI.IHeaderModifier.Test("QR003"));

        // Log the response
        _outputHelper.WriteLine(JsonSerializer.Serialize(result));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(qrRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.NotNull(result.QRCode);
    }

}