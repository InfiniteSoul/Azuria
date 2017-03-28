﻿using Azuria.Api.Enums.Info;
using Azuria.Api.v1.Converters;
using Azuria.Api.v1.Converters.Info;
using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels.Info
{
    /// <summary>
    /// </summary>
    public class PublisherDataModel : IDataModel
    {
        #region Properties

        /// <summary>
        /// </summary>
        [JsonProperty("country")]
        [JsonConverter(typeof(CountryConverter))]
        public Country Country { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(IndustryTypeConverter))]
        public IndustryType Type { get; set; }

        #endregion
    }
}