using Azuria.Api.v1.DataModels.Media;
using Azuria.Api.v1.RequestBuilder;
using Azuria.Enums.Media;
using Azuria.Helpers.Extensions;
using Azuria.Requests.Builder;
using NUnit.Framework;

namespace Azuria.Test.Api.v1.RequestBuilder
{
    [TestFixture]
    public class MediaRequestBuilderTest : RequestBuilderTestBase<MediaRequestBuilder>
    {
        [Test]
        public void GetHeaderListTest()
        {
            IRequestBuilderWithResult<HeaderDataModel[]> lRequest = this.RequestBuilder.GetHeaderList();
            this.CheckUrl(lRequest, "media", "headerlist");
            Assert.AreSame(this.ProxerClient, lRequest.Client);
            Assert.False(lRequest.CheckLogin);
        }

        [Test]
        [TestCase(HeaderStyle.Black)]
        [TestCase(HeaderStyle.Gray)]
        [TestCase(HeaderStyle.OldBlue)]
        [TestCase(HeaderStyle.Pantsu)]
        public void GetRandomHeaderTest(HeaderStyle style)
        {
            IRequestBuilderWithResult<HeaderDataModel> lRequest = this.RequestBuilder.GetRandomHeader(style);
            this.CheckUrl(lRequest, "media", "randomheader");
            Assert.AreSame(this.ProxerClient, lRequest.Client);
            Assert.True(lRequest.GetParameters.ContainsKey("style"));
            Assert.AreEqual(style.ToTypeString(), lRequest.GetParameters["style"]);
            Assert.False(lRequest.CheckLogin);
        }

        [Test]
        public override void ProxerClientTest()
        {
            base.ProxerClientTest();
        }
    }
}