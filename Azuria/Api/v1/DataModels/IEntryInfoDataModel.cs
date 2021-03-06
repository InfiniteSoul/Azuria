﻿using Azuria.Api.v1.Converter;
using Azuria.Enums;
using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels
{
    /// <summary>
    /// </summary>
    public interface IEntryInfoDataModel : IDataModel
    {
        /// <summary>
        /// </summary>
        int EntryId { get; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(MediumConverter))]
        MediaMedium EntryMedium { get; }

        /// <summary>
        /// </summary>
        string EntryName { get; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(CategoryConverter))]
        MediaEntryType EntryType { get; }
    }
}