using System.Net.Http.Headers;

namespace GPoon21.KAPI.SDK;

public static partial class KAPI {
    public interface IHeaderModifier {

        public void Modify(HttpRequestHeaders headers);

        public class Test : IHeaderModifier {
            private readonly string _envId;

            public Test(string envId) {
                _envId = envId;
            }

            public void Modify(HttpRequestHeaders headers) {
                headers.Add("x-test-mode", "true");
                headers.Add("env-id", _envId);
            }
        }

        public class Default : IHeaderModifier {
            public void Modify(HttpRequestHeaders headers) { }
        }

    }
}