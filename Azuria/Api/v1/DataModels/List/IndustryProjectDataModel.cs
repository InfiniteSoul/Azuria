﻿using Azuria.Api.v1.Converter;
using Azuria.Enums.Info;
using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels.List
{
    /// <summary>
    /// </summary>
    public class IndustryProjectDataModel : ProjectDataModel
    {
        /// <summary>
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(IndustryRoleConverter))]
        public IndustryType IndustryType { get; set; }
    }
}