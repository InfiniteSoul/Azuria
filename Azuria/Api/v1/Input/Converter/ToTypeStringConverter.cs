﻿using Azuria.Enums.Info;
using Azuria.Enums.Media;
using Azuria.Helpers.Extensions;

namespace Azuria.Api.v1.Input.Converter
{
    internal class ToTypeStringConverter
        : IInputDataConverter<IndustryType?>, IInputDataConverter<UserList?>, IInputDataConverter<MediaLanguage?>,
            IInputDataConverter<HeaderStyle?>
    {
        /// <inheritdoc />
        public string Convert(IndustryType? toConvert)
        {
            return toConvert?.ToTypeString();
        }

        /// <inheritdoc />
        public string Convert(UserList? toConvert)
        {
            return toConvert?.ToTypeString();
        }

        /// <inheritdoc />
        public string Convert(MediaLanguage? toConvert)
        {
            return toConvert?.ToTypeString();
        }

        /// <inheritdoc />
        public string Convert(HeaderStyle? toConvert)
        {
            return toConvert?.ToTypeString();
        }
    }
}