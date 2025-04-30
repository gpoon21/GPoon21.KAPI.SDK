using System.Text.Json;
using GPoon21.KAPI.SDK.QRPayment;
using Xunit.Abstractions;

namespace GPoon21.KAPI.SDK.UnitTest;

public class QRPaymentTest {
    private readonly ITestOutputHelper _outputHelper;

    public QRPaymentTest(ITestOutputHelper outputHelper) {
        _outputHelper = outputHelper;
    }


    [Fact]
    public async Task GetClientCredentials_Success() {

        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        KBankQR.CustomerInfo result =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        _outputHelper.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));
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
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR request with specified parameters
        KBankQR.QRRequest qrRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0001",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            QRType = QRType.ThaiQR,
            TransactionAmount = 100.00m,
            TransactionCurrencyCode = "THB",
            Reference1 = "INV001",
            Reference2 = "HELLOWORLD",
            Reference3 = "INV001",
            Reference4 = "INV001",
        };

        // Request QR code
        KBankQR.QRResponse result =
            await KBankQR.RequestQR(qrRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR002"));

        // Log the response
        _outputHelper.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

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

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR request with specified parameters
        KBankQR.QRRequest qrRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0001-2",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            QRType = QRType.CreditCard,  // QR Credit Card type
            TransactionAmount = 100.00m, // Example amount can be modified as needed
            TransactionCurrencyCode = "THB",
            Reference1 = "INV001",
            Reference2 = "HELLOWORLD",
            Reference3 = "INV001",
            Reference4 = "INV001"
        };

        // Request QR code with a specified environment
        KBankQR.QRResponse result =
            await KBankQR.RequestQR(qrRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR003"));

        // Log the response
        _outputHelper.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(qrRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.NotNull(result.QRCode);
    }

    [Fact]
    public async Task InquiryQR_StatusRequest_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR inquiry request with specified parameters
        KBankQR.QRInquiryRequest inquiryRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0002",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "PARTNERTEST0001"
        };

        // Perform QR inquiry
        KBankQR.QRInquiryResponse result =
            await KBankQR.InquiryPayment(inquiryRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR004"));

        // Log the response
        _outputHelper.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(inquiryRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(inquiryRequest.PartnerId, result.PartnerId);
    }


    [Fact]
    public async Task InquiryQR_CancelledStatus_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR inquiry request with specified parameters
        KBankQR.QRInquiryRequest inquiryRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0003",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "TESTCANCELQR001"
        };

        // Perform QR inquiry with a specified environment
        KBankQR.QRInquiryResponse result =
            await KBankQR.InquiryPayment(inquiryRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR005"));

        // Log the response
        _outputHelper.WriteLine(
            JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(inquiryRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(inquiryRequest.PartnerId, result.PartnerId);
    }

    [Fact]
    public async Task InquiryQR_PaidStatus_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR inquiry request with specified parameters
        KBankQR.QRInquiryRequest inquiryRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0004",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "PARTNERTEST0007"
        };

        // Perform QR inquiry with a specified environment
        KBankQR.QRInquiryResponse result =
            await KBankQR.InquiryPayment(inquiryRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR006"));

        // Log the response
        _outputHelper.WriteLine(
            JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(inquiryRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(inquiryRequest.PartnerId, result.PartnerId);
    }

    [Fact]
    public async Task InquiryQR_VoidedStatus_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR inquiry request with specified parameters
        KBankQR.QRInquiryRequest inquiryRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0005",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "PARTNERTEST0011"
        };

        // Perform QR inquiry with a specified environment
        KBankQR.QRInquiryResponse result =
            await KBankQR.InquiryPayment(inquiryRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR007"));

        // Log the response
        _outputHelper.WriteLine(
            JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(inquiryRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(inquiryRequest.PartnerId, result.PartnerId);
    }

    [Fact]
    public async Task CancelQR_StatusRequest_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR cancel request with specified parameters
        KBankQR.QRCancelRequest cancelRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0006",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "PARTNERTEST0001"
        };

        // Perform QR cancellation
        KBankQR.QRCancelResponse result =
            await KBankQR.CancelQR(cancelRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR008"));

        // Log the response
        _outputHelper.WriteLine(
            JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(cancelRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(cancelRequest.PartnerId, result.PartnerId);
    }

    [Fact]
    public async Task CancelQR_PaidStatus_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR cancel request with specified parameters
        KBankQR.QRCancelRequest cancelRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0007",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "PARTNERTEST0007"
        };

        // Perform QR cancellation
        KBankQR.QRCancelResponse result =
            await KBankQR.CancelQR(cancelRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR010"));

        // Log the response
        _outputHelper.WriteLine(
            JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(cancelRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(cancelRequest.PartnerId, result.PartnerId);
    }

    [Fact]
    public async Task CancelQR_VoidedStatus_Success() {
        // Get required credentials
        string? customerId = Environment.GetEnvironmentVariable(nameof(customerId));
        Assert.NotNull(customerId);
        string? customerSecret = Environment.GetEnvironmentVariable(nameof(customerSecret));
        Assert.NotNull(customerSecret);

        // Get an access token
        KBankQR.CustomerInfo credentials =
            await KBankQR.GetClientCredentials(customerId, customerSecret,
                new KBankQR.IRequestMode.Test("OAUTH2"));
        Assert.NotNull(credentials.AccessToken);

        // Create a QR cancel request with specified parameters from the exercise
        KBankQR.QRCancelRequest cancelRequest = new() {
            PartnerTransactionUid = "PARTNERTEST0008",
            PartnerId = "PTR1051673",
            PartnerSecret = "d4bded59200547bc85903574a293831b",
            MerchantId = "KB102057149704",
            OriginalPartnerTransactionUid = "PARTNERTEST0011"
        };

        // Perform QR cancellation
        KBankQR.QRCancelResponse result =
            await KBankQR.CancelQR(cancelRequest, credentials.AccessToken,
                new KBankQR.IRequestMode.Test("QR011"));

        // Log the response
        _outputHelper.WriteLine(
            JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));

        // Verify response
        Assert.NotNull(result);
        Assert.Equal(cancelRequest.PartnerTransactionUid, result.PartnerTransactionUid);
        Assert.Equal(cancelRequest.PartnerId, result.PartnerId);
    }


}