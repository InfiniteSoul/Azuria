﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azuria.Api.v1.DataModels.Info;
using Azuria.Api.v1.Enums;
using Azuria.Exceptions;
using Azuria.User.Comment;
using Azuria.User.ControlPanel;
using Azuria.Utilities.ErrorHandling;
using Azuria.Utilities.Extensions;
using Azuria.Utilities.Properties;
using Azuria.Utilities.Web;
using HtmlAgilityPack;
using JetBrains.Annotations;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Azuria.AnimeManga
{
    /// <summary>
    ///     Represents an anime.
    /// </summary>
    [DebuggerDisplay("Anime: {Name} [{Id}]")]
    public class Anime : IAnimeMangaObject
    {
        private readonly Senpai _senpai;

        [UsedImplicitly]
        internal Anime()
        {
            this.AnimeTyp = new InitialisableProperty<AnimeType>(() => this.InitMainInfoApi(this._senpai));
            this.AvailableLanguages =
                new InitialisableProperty<IEnumerable<AnimeLanguage>>(() => this.InitAvailableLangApi(this._senpai));
            this.Clicks = new InitialisableProperty<int>(() => this.InitMainInfoApi(this._senpai));
            this.ContentCount = new InitialisableProperty<int>(() => this.InitMainInfoApi(this._senpai));
            this.Description = new InitialisableProperty<string>(() => this.InitMainInfoApi(this._senpai));
            this.EnglishTitle = new InitialisableProperty<string>(() => this.InitNamesApi(this._senpai), string.Empty)
            {
                IsInitialisedOnce = false
            };
            this.Fsk = new InitialisableProperty<IEnumerable<FskType>>(() => this.InitMainInfoApi(this._senpai));
            this.Genre = new InitialisableProperty<IEnumerable<GenreType>>(() => this.InitMainInfoApi(this._senpai));
            this.GermanTitle = new InitialisableProperty<string>(() => this.InitNamesApi(this._senpai), string.Empty)
            {
                IsInitialisedOnce = false
            };
            this.Groups = new InitialisableProperty<IEnumerable<Group>>(() => this.InitGroupsApi(this._senpai));
            this.Industry = new InitialisableProperty<IEnumerable<Industry>>(() => this.InitIndustryApi(this._senpai));
            this.IsLicensed = new InitialisableProperty<bool>(() => this.InitMainInfoApi(this._senpai));
            this.IsHContent = new InitialisableProperty<bool>(() => this.InitIsHContentApi(this._senpai));
            this.JapaneseTitle = new InitialisableProperty<string>(() => this.InitNamesApi(this._senpai), string.Empty)
            {
                IsInitialisedOnce = false
            };
            this.Name = new InitialisableProperty<string>(() => this.InitMainInfoApi(this._senpai), string.Empty)
            {
                IsInitialisedOnce = false
            };
            this.Rating = new InitialisableProperty<AnimeMangaRating>(() => this.InitMainInfoApi(this._senpai));
            this.Season = new InitialisableProperty<AnimeMangaSeasonInfo>(() => this.InitSeasonsApi(this._senpai));
            this.Status = new InitialisableProperty<AnimeMangaStatus>(() => this.InitMainInfoApi(this._senpai));
            this.Synonym = new InitialisableProperty<string>(() => this.InitNamesApi(this._senpai), string.Empty)
            {
                IsInitialisedOnce = false
            };
        }

        internal Anime([NotNull] string name, int id, [NotNull] Senpai senpai) : this()
        {
            this.Id = id;
            this._senpai = senpai;

            this.Name.SetInitialisedObject(name);
        }

        internal Anime([NotNull] string name, int id, [NotNull] Senpai senpai,
            [NotNull] IEnumerable<GenreType> genreList, AnimeMangaStatus status,
            AnimeType type) : this(name, id, senpai)
        {
            this.Genre.SetInitialisedObject(genreList);
            this.Status.SetInitialisedObject(status);
            this.AnimeTyp.SetInitialisedObject(type);
        }

        internal Anime(EntryDataModel entryDataModel, Senpai senpai) : this()
        {
            if (entryDataModel.EntryType != AnimeMangaEntryType.Anime)
                throw new ArgumentException(nameof(entryDataModel.EntryType));

            this._senpai = senpai;
            this.Id = entryDataModel.EntryId;

            this.AnimeTyp.SetInitialisedObject((AnimeType) entryDataModel.Medium);
            this.Clicks.SetInitialisedObject(entryDataModel.Clicks);
            this.ContentCount.SetInitialisedObject(entryDataModel.ContentCount);
            this.Description.SetInitialisedObject(entryDataModel.Description);
            this.Fsk.SetInitialisedObject(entryDataModel.Fsk);
            this.Genre.SetInitialisedObject(entryDataModel.Genre);
            this.IsLicensed.SetInitialisedObject(entryDataModel.IsLicensed);
            this.Name.SetInitialisedObject(entryDataModel.Name);
            this.Rating.SetInitialisedObject(entryDataModel.Rating);
            this.Status.SetInitialisedObject(entryDataModel.State);
        }

        #region Properties

        /// <summary>
        ///     Gets the type of the <see cref="Anime" />.
        /// </summary>
        [NotNull]
        public InitialisableProperty<AnimeType> AnimeTyp { get; }

        /// <summary>
        ///     Gets the languages the <see cref="Anime" /> is available in.
        /// </summary>
        [NotNull]
        public InitialisableProperty<IEnumerable<AnimeLanguage>> AvailableLanguages { get; }

        /// <summary>
        ///     Gets the total amount of clicks the <see cref="Anime" /> recieved. Is reset every 3 months.
        /// </summary>
        public InitialisableProperty<int> Clicks { get; }

        /// <summary>
        ///     Gets the count of the <see cref="Episode">Episodes</see> the <see cref="Anime" /> contains.
        /// </summary>
        public InitialisableProperty<int> ContentCount { get; }

        /// <summary>
        ///     Gets the link to the cover of the <see cref="Anime" />.
        /// </summary>
        public Uri CoverUri => new Uri($"https://cdn.proxer.me/cover/{this.Id}.jpg");

        /// <summary>
        ///     Gets the description of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<string> Description { get; }

        /// <summary>
        ///     Gets the english title of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<string> EnglishTitle { get; }

        /// <summary>
        ///     Gets an enumeration of the age restrictions of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<IEnumerable<FskType>> Fsk { get; }

        /// <summary>
        ///     Gets an enumeration of all the genre of the <see cref="Anime" /> contains.
        /// </summary>
        public InitialisableProperty<IEnumerable<GenreType>> Genre { get; }

        /// <summary>
        ///     Gets the german title of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<string> GermanTitle { get; }

        /// <summary>
        ///     Gets an enumeration of all the groups that translated the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<IEnumerable<Group>> Groups { get; }

        /// <summary>
        ///     Gets the Id of the <see cref="Anime" />.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Gets an enumeration of all the companies that were involved in making the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<IEnumerable<Industry>> Industry { get; }

        /// <summary>
        ///     Gets whether the <see cref="Anime" /> contains H-Content (Adult).
        /// </summary>
        public InitialisableProperty<bool> IsHContent { get; }

        /// <summary>
        ///     Gets if the <see cref="Anime" /> is licensed by a german company.
        /// </summary>
        public InitialisableProperty<bool> IsLicensed { get; }

        /// <summary>
        ///     Gets the japanese title of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<string> JapaneseTitle { get; }

        /// <summary>
        ///     Gets the original title of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<string> Name { get; }

        /// <summary>
        ///     Gets the rating of the <see cref="Anime" /> or <see cref="Manga" />.
        /// </summary>
        public InitialisableProperty<AnimeMangaRating> Rating { get; }

        /// <summary>
        ///     Gets the seasons the <see cref="Anime" /> aired in.
        /// </summary>
        public InitialisableProperty<AnimeMangaSeasonInfo> Season { get; }

        /// <summary>
        ///     Gets the status of the <see cref="Anime" />.
        /// </summary>
        public InitialisableProperty<AnimeMangaStatus> Status { get; }

        /// <summary>
        ///     Gets the synonym the <see cref="Anime" /> is also known as.
        /// </summary>
        public InitialisableProperty<string> Synonym { get; }

        #endregion

        #region Inherited

        /// <summary>
        ///     Adds the <see cref="Anime" /> to the planned list. If <paramref name="userControlPanel" /> is specified the object
        ///     is also added to the corresponding <see cref="UserControlPanel.Anime" />-enumeration.
        /// </summary>
        /// <param name="userControlPanel">The object which, if specified, this object is added to.</param>
        /// <returns>If the action was successful.</returns>
        async Task<ProxerResult> IAnimeMangaObject.AddToPlanned(UserControlPanel userControlPanel)
        {
            return await this.AddToPlanned(userControlPanel);
        }

        #endregion

        #region

        /// <summary>
        ///     Adds the <see cref="Anime" /> to the planned list. If <paramref name="userControlPanel" /> is specified the object
        ///     is also added to the corresponding <see cref="UserControlPanel.Anime" />-enumeration.
        /// </summary>
        /// <param name="userControlPanel">The object which, if specified, this object is added to.</param>
        /// <returns>If the action was successful.</returns>
        public async Task<ProxerResult<AnimeMangaUcpObject<Anime>>> AddToPlanned(
            UserControlPanel userControlPanel = null)
        {
            userControlPanel = userControlPanel ?? new UserControlPanel(this._senpai);
            return await userControlPanel.AddToPlanned(this);
        }

        /// <summary>
        ///     Gets the comments of the <see cref="Anime" /> in a chronological order.
        /// </summary>
        /// <param name="startIndex">The offset of the comments parsed.</param>
        /// <param name="count">The count of the returned comments starting at <paramref name="startIndex" />.</param>
        /// <returns>If the action was successful and if it was, an enumeration of the comments.</returns>
        public async Task<ProxerResult<IEnumerable<Comment<Anime>>>> GetCommentsLatest(int startIndex, int count)
        {
            return
                await
                    Comment<Anime>.GetCommentsFromUrl(startIndex, count,
                        "https://proxer.me/info/" + this.Id + "/comments/",
                        "latest", this._senpai, this);
        }

        /// <summary>
        ///     Gets the comments of the <see cref="Anime" /> ordered by rating.
        /// </summary>
        /// <param name="startIndex">The offset of the comments parsed.</param>
        /// <param name="count">The count of the returned comments starting at <paramref name="startIndex" />.</param>
        /// <returns>If the action was successful and if it was, an enumeration of the comments.</returns>
        public async Task<ProxerResult<IEnumerable<Comment<Anime>>>> GetCommentsRating(int startIndex, int count)
        {
            return
                await
                    Comment<Anime>.GetCommentsFromUrl(startIndex, count,
                        "https://proxer.me/info/" + this.Id + "/comments/",
                        "rating", this._senpai, this);
        }

        /// <summary>
        ///     Returns all <see cref="Episode">epsiodes</see> of the <see cref="Anime" /> in a specified language.
        /// </summary>
        /// <param name="language">The language of the episodes.</param>
        /// <seealso cref="Episode" />
        /// <returns>An enumeration of <see cref="Episode">episodes</see> with a count of <see cref="ContentCount" />.</returns>
        [NotNull]
        [ItemNotNull]
        public async Task<ProxerResult<IEnumerable<Episode>>> GetEpisodes(AnimeLanguage language)
        {
            if (!(await this.AvailableLanguages.GetObject(new AnimeLanguage[0])).Contains(language))
                return new ProxerResult<IEnumerable<Episode>>(new Exception[] {new LanguageNotAvailableException()});

            List<Episode> lEpisodes = new List<Episode>();
            for (int i = 1; i <= await this.ContentCount.GetObject(-1); i++)
            {
                lEpisodes.Add(new Episode(this, i, language, this._senpai));
            }

            return new ProxerResult<IEnumerable<Episode>>(lEpisodes.ToArray());
        }

        /// <summary>
        ///     Returns the currently most popular <see cref="Anime" />.
        /// </summary>
        /// <param name="senpai">The user that makes the request.</param>
        /// <returns>An enumeration of the currently most popular <see cref="Anime" />.</returns>
        [ItemNotNull]
        public static async Task<ProxerResult<IEnumerable<Anime>>> GetPopularAnime([NotNull] Senpai senpai)
        {
            HtmlDocument lDocument = new HtmlDocument();
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        new Uri("https://proxer.me/anime?format=raw"),
                        senpai);

            if (!lResult.Success)
                return new ProxerResult<IEnumerable<Anime>>(lResult.Exceptions);

            string lResponse = lResult.Result;

            try
            {
                lDocument.LoadHtml(lResponse);

                return
                    new ProxerResult<IEnumerable<Anime>>(
                        (from childNode in lDocument.DocumentNode.ChildNodes[5].FirstChild.FirstChild.ChildNodes
                            let lId =
                                Convert.ToInt32(
                                    childNode.FirstChild.GetAttributeValue("href", "/info/-1#top").Split('/')[2].Split(
                                        '#')
                                        [0])
                            select new Anime(childNode.FirstChild.GetAttributeValue("title", "ERROR"), lId, senpai))
                            .ToArray());
            }
            catch
            {
                return new ProxerResult<IEnumerable<Anime>>(ErrorHandler.HandleError(senpai, lResponse).Exceptions);
            }
        }

        #endregion

        /// <summary>
        ///     Represents an episode of an <see cref="Anime" />.
        /// </summary>
        public class Episode : IAnimeMangaContent<Anime>
        {
            private readonly Senpai _senpai;

            internal Episode([NotNull] Anime anime, int index, AnimeLanguage lang, [NotNull] Senpai senpai)
            {
                this.ParentObject = anime;
                this.ContentIndex = index;
                this.Language = lang;
                this._senpai = senpai;

                this.IsAvailable = new InitialisableProperty<bool>(this.InitInfo);
                this.Streams =
                    new InitialisableProperty<IEnumerable<KeyValuePair<Stream.StreamPartner, Stream>>>(
                        this.InitInfo);
            }

            internal Episode([NotNull] Anime anime, int index, AnimeLanguage lang, bool isAvailable,
                [NotNull] Senpai senpai)
                : this(anime, index, lang, senpai)
            {
                this.IsAvailable = new InitialisableProperty<bool>(this.InitInfo, isAvailable);
            }

            #region Properties

            /// <summary>
            ///     Gets the <see cref="Episode" />-number.
            /// </summary>
            public int ContentIndex { get; }

            /// <summary>
            ///     Gets the general language (english/german) of the <see cref="Episode" />.
            /// </summary>
            public Language GeneralLanguage
                =>
                    this.Language == AnimeLanguage.GerSub || this.Language == AnimeLanguage.GerDub
                        ? AnimeManga.Language.German
                        : this.Language == AnimeLanguage.EngSub || this.Language == AnimeLanguage.EngDub
                            ? AnimeManga.Language.English
                            : AnimeManga.Language.Unkown;

            /// <summary>
            ///     Gets if the <see cref="Episode" /> is available.
            /// </summary>
            public InitialisableProperty<bool> IsAvailable { get; }

            /// <summary>
            ///     Gets the language of the episode
            /// </summary>
            public AnimeLanguage Language { get; }

            /// <summary>
            ///     Gets the <see cref="Anime" /> this <see cref="Episode" /> belongs to.
            /// </summary>
            public Anime ParentObject { get; }

            /// <summary>
            ///     Gets the available streams of the episode.
            /// </summary>
            [NotNull]
            public InitialisableProperty<IEnumerable<KeyValuePair<Stream.StreamPartner, Stream>>> Streams { get; }

            #endregion

            #region Inherited

            /// <summary>
            ///     Adds the <see cref="Episode" /> to the bookmarks. If <paramref name="userControlPanel" /> is specified the object
            ///     is also added to the corresponding <see cref="UserControlPanel.AnimeBookmarks" />-enumeration.
            /// </summary>
            /// <param name="userControlPanel">The object which, if specified, this object is added to.</param>
            /// <returns>If the action was successful.</returns>
            public async Task<ProxerResult<AnimeMangaBookmarkObject<Anime>>> AddToBookmarks(
                UserControlPanel userControlPanel = null)
            {
                userControlPanel = userControlPanel ?? new UserControlPanel(this._senpai);
                return await userControlPanel.AddToBookmarks(this);
            }

            #endregion

            #region

            /// <summary>
            ///     Initialises the object.
            /// </summary>
            [ItemNotNull, Obsolete("Bitte benutze die Methoden der jeweiligen Eigenschaften, um sie zu initalisieren!")]
            public async Task<ProxerResult> Init()
            {
                return await this.InitAllInitalisableProperties();
            }

            [ItemNotNull]
            private async Task<ProxerResult> InitInfo()
            {
                HtmlDocument lDocument = new HtmlDocument();
                ProxerResult<string> lResult =
                    await
                        HttpUtility.GetResponseErrorHandling(
                            new Uri("https://proxer.me/watch/" + this.ParentObject.Id + "/" + this.ContentIndex + "/" +
                                    this.Language.ToString().ToLower()),
                            this._senpai, new Func<string, ProxerResult>[0], useMobileCookies: true);

                if (!lResult.Success)
                    return new ProxerResult(lResult.Exceptions);

                string lResponse = lResult.Result;

                try
                {
                    lDocument.LoadHtml(lResponse);

                    HtmlNode[] lAllHtmlNodes = lDocument.DocumentNode.DescendantsAndSelf().ToArray();

                    if (
                        lAllHtmlNodes.Any(
                            x =>
                                x.Name.Equals("img") && x.HasAttributes &&
                                x.GetAttributeValue("src", "").Equals("/images/misc/stopyui.jpg")))
                        return new ProxerResult(new Exception[] {new CaptchaException()});

                    if (
                        lAllHtmlNodes.Any(
                            x =>
                                x.Name.Equals("img") && x.HasAttributes &&
                                x.GetAttributeValue("src", "").Equals("/images/misc/404.png")))
                        return new ProxerResult(new Exception[] {new WrongResponseException {Response = lResponse}});

                    List<KeyValuePair<Stream.StreamPartner, Stream>> lStreams =
                        new List<KeyValuePair<Stream.StreamPartner, Stream>>();

                    foreach (
                        HtmlNode childNode in
                            lDocument.DocumentNode.ChildNodes[1].ChildNodes[2].FirstChild.ChildNodes[1].ChildNodes[1]
                                .ChildNodes.Where(childNode => !childNode.FirstChild.Name.Equals("#text")))
                    {
                        switch (childNode.InnerText)
                        {
                            case "Viewster":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.Viewster,
                                        new Stream(
                                            new Uri("https:" +
                                                    childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.Viewster)));
                                break;

                            case "Crunchyroll (EN)":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.Crunchyroll,
                                        new Stream(
                                            new Uri("https:" +
                                                    childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.Crunchyroll)));
                                break;

                            case "Dailymotion":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.Dailymotion,
                                        new Stream(
                                            new Uri(childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.Dailymotion)));
                                break;
                            case "MP4Upload":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.Mp4Upload,
                                        new Stream(
                                            new Uri(childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.Mp4Upload)));
                                break;
                            case "Streamcloud":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.Streamcloud,
                                        new Stream(
                                            new Uri(childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.Streamcloud)));
                                break;
                            case "Videobam":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.Videobam,
                                        new Stream(
                                            new Uri(childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.Videobam)));
                                break;
                            case "YourUpload":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.YourUpload,
                                        new Stream(
                                            new Uri(childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.YourUpload)));
                                break;
                            case "Proxer-Stream":
                                lStreams.Add(
                                    new KeyValuePair<Stream.StreamPartner, Stream>(Stream.StreamPartner.ProxerStream,
                                        new Stream(
                                            new Uri(childNode.FirstChild.GetAttributeValue("href", "http://proxer.me/")),
                                            Stream.StreamPartner.ProxerStream)));
                                break;
                        }
                        this.Streams.SetInitialisedObject(lStreams);
                        this.IsAvailable.SetInitialisedObject(lStreams.Any());
                    }

                    return new ProxerResult();
                }
                catch
                {
                    return new ProxerResult(ErrorHandler.HandleError(this._senpai, lResponse, false).Exceptions);
                }
            }

            /// <summary>
            ///     Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            ///     A string that represents the current object.
            /// </returns>
            public override string ToString()
            {
                return "Episode " + this.ContentIndex;
            }

            #endregion

            /// <summary>
            ///     Represents a stream of an <see cref="Episode" /> of an <see cref="Anime" />.
            /// </summary>
            public class Stream
            {
                /// <summary>
                ///     Represents a streampartner.
                /// </summary>
                public enum StreamPartner
                {
                    /// <summary>
                    ///     Represents the streampartner Crunchyroll.
                    /// </summary>
                    Crunchyroll,

                    /// <summary>
                    ///     Represents the streampartner Viewster.
                    /// </summary>
                    Viewster,

                    /// <summary>
                    ///     Represents the streampartner Streamcloud.
                    /// </summary>
                    Streamcloud,

                    /// <summary>
                    ///     Represents the streampartner MP4Upload.
                    /// </summary>
                    Mp4Upload,

                    /// <summary>
                    ///     Represents the streampartner Dailymotion.
                    /// </summary>
                    Dailymotion,

                    /// <summary>
                    ///     Represents the streampartner Videobam.
                    /// </summary>
                    Videobam,

                    /// <summary>
                    ///     Represents the streampartner YourUpload.
                    /// </summary>
                    YourUpload,

                    /// <summary>
                    ///     Represents the internal streampartner of Proxer.Me.
                    /// </summary>
                    ProxerStream,

                    /// <summary>
                    ///     Represents no streampartner.
                    /// </summary>
                    None
                }

                internal Stream([NotNull] Uri link, StreamPartner streamPartner)
                {
                    this.Link = link;
                    this.Partner = streamPartner;
                }

                #region Properties

                /// <summary>
                ///     Gets the link of the stream.
                /// </summary>
                [NotNull]
                public Uri Link { get; private set; }

                /// <summary>
                ///     Gets the streampartner of the stream.
                /// </summary>
                public StreamPartner Partner { get; private set; }

                #endregion
            }
        }
    }
}