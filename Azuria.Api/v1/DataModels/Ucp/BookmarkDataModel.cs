﻿using Azuria.Api.Enums;
using Azuria.Api.Enums.Info;
using Azuria.Api.v1.Converters;
using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels.Ucp
{
    /// <summary>
    /// </summary>
    public class BookmarkDataModel : IEntryInfoDataModel
    {
        #region Properties

        /// <summary>
        /// </summary>
        [JsonProperty("id")]
        public int BookmarkId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("episode")]
        public int ContentIndex { get; set; }

        /// <inheritdoc />
        [JsonProperty("eid")]
        public int EntryId { get; set; }

        /// <inheritdoc />
        [JsonProperty("medium")]
        public MediaMedium EntryMedium { get; set; }

        /// <inheritdoc />
        [JsonProperty("name")]
        public string EntryName { get; set; }

        /// <inheritdoc />
        [JsonProperty("kat")]
        public MediaEntryType EntryType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("language")]
        [JsonConverter(typeof(LanguageConverter))]
        public MediaLanguage Language { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("state")]
        public MediaStatus Status { get; set; }

        #endregion
    }
}