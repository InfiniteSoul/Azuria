﻿using System;
using Azuria.Enums.Info;
using Newtonsoft.Json;

namespace Azuria.Api.v1.Converters
{
    internal class CountryConverter : DataConverter<Country>
    {
        #region Methods

        /// <inheritdoc />
        public override Country ConvertJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.Value.ToString())
            {
                case "de":
                    return Country.Germany;
                case "en":
                    return Country.England;
                case "us":
                    return Country.UnitedStates;
                case "jp":
                    return Country.Japan;
                default:
                    return Country.Misc;
            }
        }

        #endregion
    }
}