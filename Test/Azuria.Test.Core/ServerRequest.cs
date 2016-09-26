﻿using System.Collections.Generic;

namespace Azuria.Test.Core
{
    public class ServerRequest
    {
        internal ServerRequest(string url)
        {
            this.Url = url;
        }

        #region Properties

        public bool ContainsSenpai { get; set; }

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string> PostArguments { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();

        public RequestType RequestType { get; set; }

        public string Url { get; set; }

        #endregion

        #region Methods

        public string GetQuery()
        {
            string lQuery = "?";
            foreach (KeyValuePair<string, string> keyValuePair in this.QueryParams)
                lQuery += $"{keyValuePair.Key}={keyValuePair.Value}&";
            return lQuery.Remove(lQuery.Length - 1);
        }

        public ServerRequest WithHeader(string key, string value)
        {
            this.Headers.Add(key, value);
            return this;
        }

        public ServerRequest WithPostArgument(string key, string value)
        {
            this.PostArguments.Add(key, value);
            return this;
        }

        public ServerRequest WithQueryParameter(string key, string value)
        {
            this.QueryParams.Add(key, value);
            return this;
        }

        public ServerRequest WithSenpai(bool withSenpai)
        {
            this.ContainsSenpai = withSenpai;
            return this;
        }

        #endregion
    }
}