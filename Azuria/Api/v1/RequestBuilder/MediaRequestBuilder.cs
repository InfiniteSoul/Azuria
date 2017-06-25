﻿using System;
using Azuria.Api.Builder;
using Azuria.Api.v1.DataModels.Media;
using Azuria.Enums.Media;
using Azuria.Helpers.Extensions;
using Azuria.Requests.Builder;

namespace Azuria.Api.v1.RequestBuilder
{
    /// <summary>
    /// Represents the media api class.
    /// </summary>
    public class MediaRequestBuilder : IApiClassRequestBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public MediaRequestBuilder(IProxerClient client)
        {
            this.ProxerClient = client;
        }

        /// <inheritdoc />
        public IProxerClient ProxerClient { get; }

        /// <summary>
        /// Builds a request that returns an array of all current headers.
        /// 
        /// Api permissions required (class - permission level):
        /// * Media - Level 0
        /// </summary>
        /// <returns>An instance of <see cref="IRequestBuilderWithResult{T}" /> that returns an array of headers.</returns>
        public IRequestBuilderWithResult<HeaderDataModel[]> GetHeaderList()
        {
            return new RequestBuilder<HeaderDataModel[]>(
                new Uri($"{ApiConstants.ApiUrlV1}/media/headerlist"), this.ProxerClient
            );
        }

        /// <summary>
        /// Builds a request that returns a random header for an optional specified style.
        /// 
        /// Api permissions required (class - permission level):
        /// * Media - Level 0
        /// </summary>
        /// <param name="style">Optional. The style of the returned header.</param>
        /// <returns>An instance of <see cref="IRequestBuilderWithResult{T}" /> that returns a header.</returns>
        public IRequestBuilderWithResult<HeaderDataModel> GetRandomHeader(HeaderStyle style = HeaderStyle.Gray)
        {
            return new RequestBuilder<HeaderDataModel>(
                new Uri($"{ApiConstants.ApiUrlV1}/media/randomheader"), this.ProxerClient
            ).WithGetParameter("style", style.ToTypeString());
        }
    }
}