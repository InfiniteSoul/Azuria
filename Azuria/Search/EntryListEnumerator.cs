﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azuria.Api.v1;
using Azuria.Api.v1.DataModels.Search;
using Azuria.Api.v1.Enums;
using Azuria.ErrorHandling;
using Azuria.Media;
using Azuria.Search.Input;
using Azuria.Utilities;

namespace Azuria.Search
{
    internal class EntryListEnumerator<T> : PageEnumerator<T> where T : class, IAnimeMangaObject
    {
        private const int ResultsPerPage = 100;
        private readonly EntryListInput _input;

        internal EntryListEnumerator(EntryListInput input) : base(ResultsPerPage)
        {
            if ((typeof(T) != typeof(Anime)) && (typeof(T) != typeof(Manga))) throw new ArgumentException(nameof(T));
            this._input = input;
        }

        #region Methods

        private static IEnumerable<T> GetAnimeList(IEnumerable<SearchDataModel> dataModels)
        {
            return (from searchDataModel in dataModels
                where searchDataModel.EntryType == AnimeMangaEntryType.Anime
                select new Anime(searchDataModel)).Cast<T>();
        }

        private static IEnumerable<T> GetMangaList(IEnumerable<SearchDataModel> dataModels)
        {
            return (from searchDataModel in dataModels
                where searchDataModel.EntryType == AnimeMangaEntryType.Manga
                select new Manga(searchDataModel)).Cast<T>();
        }

        /// <inheritdoc />
        internal override async Task<ProxerResult<IEnumerable<T>>> GetNextPage(int nextPage)
        {
            ProxerResult<ProxerApiResponse<SearchDataModel[]>> lResult =
                await
                    RequestHandler.ApiRequest(ApiRequestBuilder.ListEntryList(this._input,
                        typeof(T).GetTypeInfo().Name.ToLowerInvariant(), ResultsPerPage, nextPage));
            if (!lResult.Success || (lResult.Result == null))
                return new ProxerResult<IEnumerable<T>>(lResult.Exceptions);
            SearchDataModel[] lData = lResult.Result.Data;

            return typeof(T) == typeof(Anime)
                ? new ProxerResult<IEnumerable<T>>(GetAnimeList(lData))
                : new ProxerResult<IEnumerable<T>>(GetMangaList(lData));
        }

        #endregion
    }
}