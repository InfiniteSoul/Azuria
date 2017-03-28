﻿using Azuria.Api.Enums;
using Azuria.Api.v1.Converters;
using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels.Info
{
    /// <summary>
    /// </summary>
    public class ListInfoDataModel : IDataModel
    {
        #region Properties

        /// <summary>
        /// </summary>
        [JsonProperty("kat")]
        [JsonConverter(typeof(CategoryConverter))]
        public MediaEntryType Category { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("episodes")]
        public MediaContentDataModel[] ContentObjects { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("end")]
        public int EndIndex { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("start")]
        public int StartIndex { get; set; }

        #endregion
    }
}