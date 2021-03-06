﻿using System;
using System.Collections.Generic;
using System.Linq;
using Azuria.Enums.User;
using Azuria.Helpers;
using Newtonsoft.Json;

namespace Azuria.Api.v1.Converter
{
    internal class SubRatingsConverter : DataConverter<Dictionary<RatingCategory, int>>
    {
        /// <inheritdoc />
        public override Dictionary<RatingCategory, int> ConvertJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (string.IsNullOrEmpty(reader.Value.ToString()) || reader.Value.ToString().Equals("[]"))
                    return new Dictionary<RatingCategory, int>();
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(reader.Value.ToString())
                    .ToDictionary(
                        keyValuePair =>
                            EnumHelpers.ParseFromString(keyValuePair.Key, RatingCategory.Unknown),
                        keyValuePair => keyValuePair.Value);
            }
            catch (Exception)
            {
                return new Dictionary<RatingCategory, int>();
            }
        }
    }
}