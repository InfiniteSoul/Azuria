﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;

namespace Proxer.API.Utilities
{
    /// <summary>
    /// </summary>
    internal class HttpUtility
    {
        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        internal static string GetWebRequestResponse(string url, CookieContainer cookies)
        {
            RestClient lClient = new RestClient(url);
            lClient.CookieContainer = cookies;
            RestRequest lRequest = new RestRequest(Method.GET);
            return lClient.Execute(lRequest).Content;
        }

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <param name="postArgs"></param>
        /// <returns></returns>
        internal static string PostWebRequestResponse(string url, CookieContainer cookies,
            Dictionary<string, string> postArgs)
        {
            RestClient lClient = new RestClient(url);
            lClient.CookieContainer = cookies;
            RestRequest lRequest = new RestRequest(Method.POST);
            postArgs.ToList().ForEach(x => lRequest.AddParameter(x.Key, x.Value));
            return lClient.Execute(lRequest).Content;
        }
    }
}