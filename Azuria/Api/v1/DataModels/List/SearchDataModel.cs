﻿using Azuria.Api.v1.Converter;
using Azuria.Enums;
using Azuria.Enums.Info;
using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels.List
{
    /// <summary>
    /// </summary>
    public class SearchDataModel : DataModelBase, IEntryInfoDataModel
    {
        /// <summary>
        /// </summary>
        [JsonProperty("language")]
        [JsonConverter(typeof(MediaLanguageCommaCollectionConverter))]
        public MediaLanguage[] AvailableLanguages { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("count")]
        public int ContentCount { get; set; }

        /// <inheritdoc />
        [JsonProperty("id")]
        public int EntryId { get; set; }

        /// <inheritdoc />
        [JsonProperty("medium")]
        public MediaMedium EntryMedium { get; set; }

        /// <inheritdoc />
        [JsonProperty("name")]
        public string EntryName { get; set; }

        /// <inheritdoc />
        public MediaEntryType EntryType
            => (int) this.EntryMedium < 4 ? MediaEntryType.Anime : MediaEntryType.Manga;

        /// <summary>
        /// </summary>
        public Genre[] Genre => GenreConverter.ParseGenre(this.GenreRaw);

        /// <summary>
        /// Raw value of <see cref="Genre"/>.
        /// </summary>
        [JsonProperty("genre")]
        public string GenreRaw { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("rate_count")]
        public int RateCount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("rate_sum")]
        public int RateSum { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("state")]
        public MediaStatus Status { get; set; }
    }
}