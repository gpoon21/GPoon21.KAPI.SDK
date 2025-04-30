using System.Net.Http.Headers;

namespace GPoon21.KAPI.SDK;

public interface IRequestMode {
    public string BaseUrl { get; }
    public void Modify(HttpRequestHeaders headers);

    /// <summary>
    /// Add 'x-test-mode: true' to <see cref="HttpRequestHeaders"/> and 'env-id' with a specific value.
    /// </summary>
    public class Test : IRequestMode {
        private readonly string? _envId;

        public Test(string? envId = null) {
            _envId = envId;
        }

        public string BaseUrl {
            get { return "https://openapi-sandbox.kasikornbank.com"; }
        }

        public void Modify(HttpRequestHeaders headers) {
            headers.Add("x-test-mode", "true");
            if (_envId != null) headers.Add("env-id", _envId);
        }
    }

    public class Default : IRequestMode {
        public required string BaseUrl { get; init; }
        public void Modify(HttpRequestHeaders headers) { }
    }
}