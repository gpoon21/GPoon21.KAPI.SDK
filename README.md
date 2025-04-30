
# KAPI SKD (non official)

C# SDK for KBank KAPI QR Payment https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Identity%20confirmation

By running the unit test with envirment variable customerId, customerSecret, and SSL_CERTIFICATE_PEM, you should be able to pass all 16 QRPayment excercises.

This code currently have not been tested againist a real API outside of the sandbox since I have no access to it.
 
## Usage/Examples
Call actual API with KBankQR.Client
```csharp
KBankQR.Client client = await KBankQR.Client.CreateAsync(customerId, customerSecret, new IRequestMode.Test());
// Create a QR request with specified parameters
KBankQR.QRRequest qrRequest = new() {
    PartnerTransactionUid = "{parameter}",
    PartnerId = "{parameter}",
    PartnerSecret = "{parameter}",
    MerchantId = "{parameter}",
    QRType = QRType.ThaiQR,
    TransactionAmount = 100,
    TransactionCurrencyCode = "{parameter}",
    Reference1 = "{parameter}",
    Reference2 = "{parameter}",
    Reference3 = "{parameter}",
    Reference4 = "{parameter}",
};
IRequestMode.Default requestMode = new() {
    BaseUrl = "{URL}",
};
// Request QR code
KBankQR.QRResponse result =  await client.RequestQR(qrRequest, requestMode);

```
Call direcly without KBankQR.Client
```csharp
IRequestMode.Default requestMode = new() {
    BaseUrl = "{URL}",
};
KBankQR.CustomerInfo credentials = await KBankQR.GetClientCredentials(customerId, customerSecret, requestMode);
KBankQR.QRResponse result = await KBankQR.RequestQR(qrRequest, credentials.AccessToken, requestMode);
```
