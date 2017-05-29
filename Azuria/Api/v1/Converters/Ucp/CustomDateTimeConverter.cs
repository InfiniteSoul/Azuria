﻿using System;
using Newtonsoft.Json;

namespace Azuria.Api.v1.Converters.Ucp
{
    internal class CustomDateTimeConverter : DataConverter<DateTime>
    {
        #region Methods

        /// <inheritdoc />
        public override DateTime ConvertJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }

        #endregion
    }
}