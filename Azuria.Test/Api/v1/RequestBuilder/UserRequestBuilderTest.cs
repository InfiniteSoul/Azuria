using Azuria.Api.v1.RequestBuilder;
using Xunit;

namespace Azuria.Test.Api.v1.RequestBuilder
{
    public class UserRequestBuilderTest : RequestBuilderTestBase<UserRequestBuilder>
    {
        [Fact]
        public override void ProxerClientTest()
        {
            base.ProxerClientTest();
        }
    }
}