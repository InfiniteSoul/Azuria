﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Azuria.Api.v1.Converters.List
{
    internal class TagIdConverter : DataConverter<Tuple<int[], int[]>>
    {
        #region Methods

        public override Tuple<int[], int[]> ConvertJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int[] lTagIds = new int[0];
            int[] lNoTagIds = new int[0];

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                switch (reader.Value)
                {
                    case "tags":
                        lTagIds = GetIdArray().ToArray();
                        break;
                    case "notags":
                        lNoTagIds = GetIdArray().ToArray();
                        break;
                }

            return new Tuple<int[], int[]>(lTagIds, lNoTagIds);

            IEnumerable<int> GetIdArray()
            {
                if (!reader.Read() || reader.TokenType != JsonToken.StartArray) yield break;
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    yield return Convert.ToInt32(reader.Value);
            }
        }

        #endregion
    }
}