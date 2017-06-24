﻿using System;
using System.Linq;
using System.Threading;
using Azuria.Api.v1.DataModels.Anime;
using Azuria.Api.v1.RequestBuilder;
using Azuria.Enums.Info;
using Azuria.Requests.Builder;
using Azuria.Test.Core;
using Xunit;

namespace Azuria.Test.Api.v1.RequestBuilder
{
    public class AnimeRequestBuilderTest : RequestBuilderTestBase<AnimeRequestBuilder>
    {
        [Fact]
        public void GetLinkTest()
        {
            int lRandomId = this.GetRandomNumber(4200);
            IRequestBuilderWithResult<string> lRequest = this.RequestBuilder.GetLink(lRandomId);
            this.CheckUrl(lRequest, "anime", "link");
            Assert.True(lRequest.GetParameters.ContainsKey("id"));
            Assert.Equal(lRandomId.ToString(), lRequest.GetParameters["id"]);
        }

        [Theory]
        [InlineData(AnimeLanguage.EngSub)]
        [InlineData(AnimeLanguage.EngDub)]
        [InlineData(AnimeLanguage.GerSub)]
        [InlineData(AnimeLanguage.GerDub)]
        public void GetProxerStreamTest(AnimeLanguage language)
        {
            int lRandomId = this.GetRandomNumber(4200);
            int lRandomEpisode = this.GetRandomNumber(42);
            IRequestBuilderWithResult<StreamDataModel[]> lRequest =
                this.RequestBuilder.GetProxerStreams(lRandomId, lRandomEpisode, language);
            this.CheckUrl(lRequest, "anime", "proxerstreams");
            Assert.True(lRequest.GetParameters.ContainsKey("id"));
            Assert.True(lRequest.GetParameters.ContainsKey("episode"));
            Assert.True(lRequest.GetParameters.ContainsKey("language"));
            Assert.Equal(lRandomId.ToString(), lRequest.GetParameters["id"]);
            Assert.Equal(lRandomEpisode.ToString(), lRequest.GetParameters["episode"]);
            Assert.Equal(language.ToString().ToLowerInvariant(), lRequest.GetParameters["language"]);
        }

        [Fact]
        public void GetProxerStreamUnkownLanguageErrorTest()
        {
            ArgumentException lException = Assert.Throws<ArgumentException>(
                () => this.RequestBuilder.GetProxerStreams(42, 42, AnimeLanguage.Unknown)
            );
            Assert.NotEmpty(lException.Message);
            Assert.Equal("language", lException.ParamName);
        }
        

        [Theory]
        [InlineData(AnimeLanguage.EngSub)]
        [InlineData(AnimeLanguage.EngDub)]
        [InlineData(AnimeLanguage.GerSub)]
        [InlineData(AnimeLanguage.GerDub)]
        public void GetStreamTest(AnimeLanguage language)
        {
            int lRandomId = this.GetRandomNumber(4200);
            int lRandomEpisode = this.GetRandomNumber(42);
            IRequestBuilderWithResult<StreamDataModel[]> lRequest =
                this.RequestBuilder.GetStreams(lRandomId, lRandomEpisode, language);
            this.CheckUrl(lRequest, "anime", "streams");
            Assert.True(lRequest.GetParameters.ContainsKey("id"));
            Assert.True(lRequest.GetParameters.ContainsKey("episode"));
            Assert.True(lRequest.GetParameters.ContainsKey("language"));
            Assert.Equal(lRandomId.ToString(), lRequest.GetParameters["id"]);
            Assert.Equal(lRandomEpisode.ToString(), lRequest.GetParameters["episode"]);
            Assert.Equal(language.ToString().ToLowerInvariant(), lRequest.GetParameters["language"]);
        }

        [Fact]
        public void GetStreamUnkownLanguageErrorTest()
        {
            ArgumentException lException = Assert.Throws<ArgumentException>(
                () => this.RequestBuilder.GetStreams(42, 42, AnimeLanguage.Unknown)
            );
            Assert.NotEmpty(lException.Message);
            Assert.Equal("language", lException.ParamName);
        }
        
        [Fact]
        public override void ProxerClientTest()
        {
            base.ProxerClientTest();
        }
    }
}