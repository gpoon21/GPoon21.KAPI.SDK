
# KAPI SKD (non official)

C# SDK for KBank KAPI QR Payment https://apiportal.kasikornbank.com/product/public/All/QR%20Payment/Documentation/Identity%20confirmation

By running the unit test with envirment variable customerId, customerSecret, and SSL_CERTIFICATE_PEM, you should be able to pass the QRPayment excercise.
 
## Usage/Examples

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
        // Request QR code
        KBankQR.QRResponse result =
            await client.RequestQR(qrRequest, new IRequestMode.Default() {
                BaseUrl = "URL",
            });

```

