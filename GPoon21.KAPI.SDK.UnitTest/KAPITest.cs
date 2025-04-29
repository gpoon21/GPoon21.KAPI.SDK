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
        
        KAPI.CustomerInfo result = await KAPI.GetClientCredentials(customerId, customerSecret);
        _outputHelper.WriteLine(JsonSerializer.Serialize(result));
        Assert.NotNull(result);
    }
}