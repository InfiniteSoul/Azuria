﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Proxer.API.Exceptions;
using Proxer.API.Utilities;
using Proxer.API.Utilities.Net;

namespace Proxer.API.Community
{
    /// <summary>
    ///     Repräsentiert eine Proxer-Konferenz.
    /// </summary>
    public class Conference
    {
        /// <summary>
        ///     Wird ausgelöst, wenn neue Nachrichten in der Konferenz vorhanden sind.
        /// </summary>
        /// <param name="sender">Die Konferenz, die das Event aufgerufen hat</param>
        /// <param name="e">Die neuen Nachrichten. Beim ersten mal werden hier alle Nachrichten aufgeführt</param>
        public delegate void NeuePmEventHandler(Conference sender, List<Message> e);

        private readonly Timer _getMessagesTimer;
        private readonly Func<Task<ProxerResult>>[] _initFuncs;
        private readonly Senpai _senpai;

        /// <summary>
        ///     Standard-Konstruktor der Klasse.
        /// </summary>
        /// <param name="id">Die ID der Konferenz</param>
        /// <param name="senpai">Muss Teilnehmer der Konferenz sein. Darf nicht null sein.</param>
        public Conference(int id, Senpai senpai)
        {
            this._initFuncs = new Func<Task<ProxerResult>>[] {this.GetAllParticipants, this.GetLeader, this.GetTitle};

            this.Id = id;
            this._senpai = senpai;

            this._getMessagesTimer = new Timer {Interval = (new TimeSpan(0, 0, 15)).TotalMilliseconds};
            this._getMessagesTimer.Elapsed += async (s, eArgs) =>
            {
                this._getMessagesTimer.Interval = (new TimeSpan(0, 0, 15)).TotalMilliseconds;
                Timer timer = s as Timer;
                timer?.Stop();
                if (this.IstInitialisiert)
                {
                    if (this.Nachrichten != null && this.Nachrichten.Any())
                        await this.GetMessages(this.Nachrichten.Last().NachrichtId);
                    else await this.GetAllMessages();
                }
                Timer timer1 = s as Timer;
                timer1?.Start();
            };

            this.Aktiv = false;
        }

        internal Conference(string title, int id, Senpai senpai)
        {
            this._initFuncs = new Func<Task<ProxerResult>>[] {this.GetAllParticipants, this.GetLeader, this.GetTitle};

            this.Titel = title;
            this.Id = id;
            this._senpai = senpai;

            this._getMessagesTimer = new Timer {Interval = (new TimeSpan(0, 0, 15)).TotalMilliseconds};
            this._getMessagesTimer.Elapsed += async (s, eArgs) =>
            {
                this._getMessagesTimer.Interval = (new TimeSpan(0, 0, 15)).TotalMilliseconds;
                Timer timer = s as Timer;
                timer?.Stop();
                if (this.IstInitialisiert)
                {
                    if (this.Nachrichten != null && this.Nachrichten.Any())
                        await this.GetMessages(this.Nachrichten.Last().NachrichtId);
                    else await this.GetAllMessages();
                }
                Timer timer1 = s as Timer;
                timer1?.Start();
            };

            this.Aktiv = false;
        }

        #region Properties

        /// <summary>
        ///     Gibt einen Wert an, ob die Privatnachrichten in einem bestimmten Intervall abgerufen werden, oder legt diesen fest.
        /// </summary>
        public bool Aktiv
        {
            get { return this._getMessagesTimer.Enabled; }
            set
            {
                if (value)
                {
                    this._getMessagesTimer.Interval = 1;
                    this._getMessagesTimer.Start();
                }
                else
                {
                    this._getMessagesTimer.Stop();
                }
            }
        }

        /// <summary>
        ///     Gibt die ID der Konferenz zurück
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Gibt zurück, ob die Konferenz bereits initialisiert ist.
        /// </summary>
        public bool IstInitialisiert { get; private set; }

        /// <summary>
        ///     Gibt den Leiter der Konferenz zurück. (<see cref="InitConference" /> muss dafür zunächst einmal aufgerufen werden)
        /// </summary>
        public User Leiter { get; private set; }

        /// <summary>
        ///     Gibt alle Nachrichten aus der Konferenz in chronoligischer Ordnung zurück.
        /// </summary>
        public List<Message> Nachrichten { get; private set; }

        /// <summary>
        ///     Gibt alle Teilnehmer der Konferenz zurück (<see cref="InitConference" /> muss dafür zunächst einmal aufgerufen
        ///     werden)
        /// </summary>
        public List<User> Teilnehmer { get; private set; }

        /// <summary>
        ///     Gibt den Titel der Konferenz zurück (<see cref="InitConference" /> muss dafür zunächst einmal aufgerufen werden)
        /// </summary>
        public string Titel { get; private set; }


        private bool IsConference { get; set; }

        #endregion

        #region

        /// <summary>
        ///     Wird immer aufgerufen, wenn neue Nachrichten in der Konferenz vorhanden sind.
        /// </summary>
        public event NeuePmEventHandler NeuePmRaised;


        /// <summary>
        ///     Initialisiert die Konferenz.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <seealso cref="Senpai.Login" />
        public async Task<ProxerResult> InitConference()
        {
            if (!this._senpai.LoggedIn)
                return new ProxerResult(new Exception[] {new NotLoggedInException(this._senpai)});

            ProxerResult<bool> lIstTeilnehmer = await IstTeilnehmner(this.Id, this._senpai);
            if (lIstTeilnehmer.Success && !lIstTeilnehmer.Result)
                return new ProxerResult
                {
                    Success = false
                };

            ProxerResult<bool> lIsConference = await this.CheckIsConference();
            if (lIsConference.Success) this.IsConference = lIsConference.Result;

            int lFailedInits = 0;
            ProxerResult lReturn = new ProxerResult();
            foreach (Func<Task<ProxerResult>> initFunc in this._initFuncs)
            {
                try
                {
                    ProxerResult lResult;
                    if ((lResult = await initFunc.Invoke()).Success) continue;

                    lReturn.AddExceptions(lResult.Exceptions);
                    lFailedInits++;
                }
                catch
                {
                    return new ProxerResult
                    {
                        Success = false
                    };
                }
            }

            this.IstInitialisiert = true;
            if (lFailedInits < this._initFuncs.Length)
                lReturn.Success = true;

            return lReturn;
        }

        /// <summary>
        ///     Sendet eine Nachricht an die Konferenz.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="nachricht">Die Nachricht, die gesendet werden soll</param>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Gibt zurück, ob die Aktion erfolgreich war</returns>
        public async Task<ProxerResult<bool>> SendeNachricht(string nachricht)
        {
            this._getMessagesTimer.Stop();

            Dictionary<string, string> lPostArgs = new Dictionary<string, string>
            {
                {"message", nachricht}
            };
            ProxerResult<string> lResult =
                await
                    HttpUtility.PostResponseErrorHandling(
                        "https://proxer.me/messages?id=" + this.Id + "&format=json&json=answer",
                        lPostArgs,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            try
            {
                Dictionary<string, string> lResponseJson =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(lResponse);

                if (lResponseJson.Keys.Contains("message"))
                {
                    this.NeuePmRaised?.Invoke(this,
                        new List<Message>
                        {
                            new Message(User.System, -1, lResponseJson["message"], DateTime.Now,
                                Message.Action.GetAction)
                        });
                    return new ProxerResult<bool>(true);
                }
                if (lResponseJson["msg"].Equals("Erfolgreich!"))
                {
                    await this.GetMessages(this.Nachrichten.Last().NachrichtId);
                    this._getMessagesTimer.Start();
                    return new ProxerResult<bool>(true);
                }
            }
            catch
            {
                return
                    new ProxerResult<bool>((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
            }

            this._getMessagesTimer.Start();

            return new ProxerResult<bool>(new Exception[] {new WrongResponseException {Response = lResponse}});
        }

        /// <summary>
        ///     Markiert die Konferenz als ungelesen.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Gibt zurück, ob die Aktion erfolgreich war</returns>
        public async Task<ProxerResult<bool>> AlsUngelesenMarkieren()
        {
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=setUnread&id=" + this.Id,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            return new ProxerResult<bool>(lResponse.StartsWith("{\"error\":0"));
        }

        /// <summary>
        ///     Markiert die Konferenz als Favorit.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Gibt zurück, ob die Aktion erfolgreich war</returns>
        public async Task<ProxerResult<bool>> FavoritHinzufuegen()
        {
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=favour&id=" + this.Id,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            return new ProxerResult<bool>(lResponse.StartsWith("{\"error\":0"));
        }

        /// <summary>
        ///     Entfernt die Konferenz aus den Favoriten.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Gibt zurück, ob die Aktion erfolgreich war</returns>
        public async Task<ProxerResult<bool>> FavoritEntfernen()
        {
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=unfavour&id=" + this.Id,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            return new ProxerResult<bool>(lResponse.StartsWith("{\"error\":0"));
        }

        /// <summary>
        ///     Blockiert die Konferenz.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Gibt zurück, ob die Aktion erfolgreich war</returns>
        public async Task<ProxerResult<bool>> BlockHinzufuegen()
        {
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=block&id=" + this.Id,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            return new ProxerResult<bool>(lResponse.StartsWith("{\"error\":0"));
        }

        /// <summary>
        ///     Entblockt die Konferenz.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Gibt zurück, ob die Aktion erfolgreich war</returns>
        public async Task<ProxerResult<bool>> BlockEntfernen()
        {
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=unblock&id=" + this.Id,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            return new ProxerResult<bool>(lResponse.StartsWith("{\"error\":0"));
        }


        private async Task<ProxerResult> GetLeader()
        {
            if (!this.IsConference) return new ProxerResult();

            Dictionary<string, string> lPostArgs = new Dictionary<string, string>
            {
                {"message", "/leader"}
            };
            ProxerResult<string> lResult =
                await
                    HttpUtility.PostResponseErrorHandling(
                        "https://proxer.me/messages?id=" + this.Id + "&format=json&json=answer",
                        lPostArgs,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult(lResult.Exceptions);

            string lResponse = lResult.Result;

            try
            {
                Dictionary<string, string> lDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(lResponse);
                if (!lDict["msg"].Equals("Erfolgreich!"))
                    return new ProxerResult
                    {
                        Success = false
                    };

                User[] lLeiterArray = this.Teilnehmer.Where(
                    x => x.UserName.Equals(lDict["message"].Remove(0, "Konferenzleiter: ".Length)))
                                          .ToArray();
                if (lLeiterArray.Any()) this.Leiter = lLeiterArray[0];

                return new ProxerResult();
            }
            catch
            {
                return new ProxerResult((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
            }
        }

        private async Task<ProxerResult> GetAllParticipants()
        {
            HtmlDocument lDocument = new HtmlDocument();
            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "https://proxer.me/messages?id=" + this.Id + "&format=raw",
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult(lResult.Exceptions);

            string lResponse = lResult.Result;

            try
            {
                lDocument.LoadHtml(lResponse);

                HtmlNodeCollection lNodes = lDocument.DocumentNode.SelectNodes("//div[@id='conferenceUsers']");
                this.Teilnehmer = new List<User>();

                foreach (HtmlNode curTeilnehmer in lNodes[0].ChildNodes[1].ChildNodes)
                {
                    string lUserName = curTeilnehmer.ChildNodes[1].InnerText;
                    int lUserId =
                        Convert.ToInt32(
                            Utility.GetTagContents(
                                curTeilnehmer.ChildNodes[1].FirstChild.Attributes["href"].Value, "/user/",
                                "#top")[0]);

                    this.Teilnehmer.Add(new User(lUserName, lUserId, this._senpai));
                }

                return new ProxerResult();
            }
            catch
            {
                return new ProxerResult((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
            }
        }

        private async Task<ProxerResult> GetTitle()
        {
            if (this.IsConference)
            {
                Dictionary<string, string> lPostArgs = new Dictionary<string, string>
                {
                    {"message", "/topic"}
                };

                ProxerResult<string> lResult =
                    await
                        HttpUtility.PostResponseErrorHandling(
                            "https://proxer.me/messages?id=" + this.Id + "&format=json&json=answer",
                            lPostArgs,
                            this._senpai.LoginCookies,
                            this._senpai.ErrHandler,
                            this._senpai);

                if (!lResult.Success)
                    return new ProxerResult(lResult.Exceptions);

                string lResponse = lResult.Result;

                try
                {
                    Dictionary<string, string> lDict =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(lResponse);
                    if (lDict["msg"].Equals("Erfolgreich!")) this.Titel = lDict["message"];

                    return new ProxerResult();
                }
                catch
                {
                    return new ProxerResult((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
                }
            }

            if (this.Teilnehmer.Count > 1)
                this.Titel = this.Teilnehmer.Where(x => x.Id != this._senpai.Me.Id).ToArray()[0].UserName;

            return new ProxerResult();
        }

        private async Task<ProxerResult> GetAllMessages()
        {
            if (this.Nachrichten != null && this.Nachrichten.Count > 0)
                return await this.GetMessages(this.Nachrichten.Last().NachrichtId);

            if (this.Nachrichten != null && this.Nachrichten.Count != 0)
                return new ProxerResult
                {
                    Success = false
                };

            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=messages&id=" + this.Id,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult(lResult.Exceptions);

            string lResponse = lResult.Result;

            if (lResponse.Equals("{\"uid\":\"" + this._senpai.Me.Id +
                                 "\",\"error\":1,\"msg\":\"Ein Fehler ist passiert.\"}"))
                return new ProxerResult
                {
                    Success = false
                };
            try
            {
                string lMessagesJson = Utility.GetTagContents(lResponse, "\"messages\":[", "],\"favour")[0];

                List<Dictionary<string, string>> lMessages =
                    JsonConvert.DeserializeObject<List<Dictionary<string, string>>>("[" + lMessagesJson + "]");

                this.Nachrichten = new List<Message>();
                foreach (Dictionary<string, string> curMessage in lMessages)
                {
                    Message.Action lMessageAction;

                    switch (curMessage["action"])
                    {
                        case "addUser":
                            lMessageAction = Message.Action.AddUser;
                            break;
                        case "removeUser":
                            lMessageAction = Message.Action.RemoveUser;
                            break;
                        case "setTopic":
                            lMessageAction = Message.Action.SetTopic;
                            break;
                        case "setLeader":
                            lMessageAction = Message.Action.SetLeader;
                            break;
                        default:
                            lMessageAction = Message.Action.NoAction;
                            break;
                    }

                    User[] lSender =
                        this.Teilnehmer.Where(x => x.Id == Convert.ToInt32(curMessage["fromid"])).ToArray();

                    if (lSender.Any())
                        this.Nachrichten.Insert(0,
                            new Message(lSender[0], Convert.ToInt32(curMessage["id"]), curMessage["message"],
                                Convert.ToInt32(curMessage["timestamp"]), lMessageAction));
                    else
                        this.Nachrichten.Insert(0,
                            new Message(
                                new User(curMessage["username"], Convert.ToInt32(curMessage["fromid"]),
                                    this._senpai),
                                Convert.ToInt32(curMessage["id"]), curMessage["message"],
                                Convert.ToInt32(curMessage["timestamp"]), lMessageAction));
                }
                if (this.Nachrichten.Any(
                    x => x.Aktion == Message.Action.AddUser || x.Aktion == Message.Action.RemoveUser))
                    await this.GetAllParticipants();
                if (this.Nachrichten.Any(x => x.Aktion == Message.Action.SetLeader) &&
                    this.IstInitialisiert)
                    await this.GetLeader();
                if (this.Nachrichten.Any(x => x.Aktion == Message.Action.SetTopic) &&
                    this.IstInitialisiert)
                    await this.GetTitle();

                this.NeuePmRaised?.Invoke(this, this.Nachrichten);
            }
            catch
            {
                return new ProxerResult((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
            }

            return new ProxerResult();
        }

        private async Task<ProxerResult> GetMessages(int mid)
        {
            if (this.Nachrichten == null || this.Nachrichten.Count(x => x.NachrichtId == mid) == 0)
                return await this.GetAllMessages();
            if (!this._senpai.LoggedIn)
                return new ProxerResult(new Exception[] {new NotLoggedInException(this._senpai)});

            ProxerResult<string> lResult =
                await
                    HttpUtility.GetResponseErrorHandling(
                        "http://proxer.me/messages?format=json&json=newmessages&id=" + this.Id + "&mid=" + mid,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult(lResult.Exceptions);

            string lResponse = lResult.Result;

            if (lResponse.Equals("{\"uid\":\"" + this._senpai.Me.Id +
                                 "\",\"error\":1,\"msg\":\"Ein Fehler ist passiert.\"}"))
                return new ProxerResult
                {
                    Success = false
                };

            List<Message> lNewMessages = new List<Message>();
            try
            {
                string lMessagesJson = Utility.GetTagContents(lResponse, "\"messages\":[", "]}")[0];

                List<Dictionary<string, string>> lMessages =
                    JsonConvert.DeserializeObject<List<Dictionary<string, string>>>("[" + lMessagesJson +
                                                                                    "]");

                foreach (Dictionary<string, string> curMessage in lMessages)
                {
                    Message.Action lMessageAction;

                    switch (curMessage["action"])
                    {
                        case "addUser":
                            lMessageAction = Message.Action.AddUser;
                            break;
                        case "removeUser":
                            lMessageAction = Message.Action.RemoveUser;
                            break;
                        case "setTopic":
                            lMessageAction = Message.Action.SetTopic;
                            break;
                        case "setLeader":
                            lMessageAction = Message.Action.SetLeader;
                            break;
                        default:
                            lMessageAction = Message.Action.NoAction;
                            break;
                    }

                    User[] lSender =
                        this.Teilnehmer.Where(x => x.Id == Convert.ToInt32(curMessage["fromid"])).ToArray();

                    if (lSender.Any())
                        lNewMessages.Insert(0,
                            new Message(lSender[0], Convert.ToInt32(curMessage["id"]), curMessage["message"],
                                Convert.ToInt32(curMessage["timestamp"]), lMessageAction));
                    else
                        lNewMessages.Insert(0,
                            new Message(
                                new User(curMessage["username"], Convert.ToInt32(curMessage["fromid"]),
                                    this._senpai), Convert.ToInt32(curMessage["id"]), curMessage["message"],
                                Convert.ToInt32(curMessage["timestamp"]), lMessageAction));
                }

                if (
                    lNewMessages.Count(
                        x => x.Aktion == Message.Action.AddUser || x.Aktion == Message.Action.RemoveUser) >
                    0)
                    await this.GetAllParticipants();
                if (lNewMessages.Count(x => x.Aktion == Message.Action.SetLeader) > 0 &&
                    this.IstInitialisiert)
                    await this.GetLeader();
                if (lNewMessages.Count(x => x.Aktion == Message.Action.SetTopic) > 0 &&
                    this.IstInitialisiert)
                    await this.GetTitle();

                this.Nachrichten = this.Nachrichten.Concat(lNewMessages).ToList();

                this.NeuePmRaised?.Invoke(this, lNewMessages);
            }
            catch
            {
                return new ProxerResult((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
            }

            return new ProxerResult();
        }

        private async Task<ProxerResult<bool>> CheckIsConference()
        {
            Dictionary<string, string> lPostArgs = new Dictionary<string, string>
            {
                {"message", "/ping"}
            };
            ProxerResult<string> lResult =
                await
                    HttpUtility.PostResponseErrorHandling(
                        "https://proxer.me/messages?id=" + this.Id + "&format=json&json=answer",
                        lPostArgs,
                        this._senpai.LoginCookies,
                        this._senpai.ErrHandler,
                        this._senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;
            try
            {
                Dictionary<string, string> lDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(lResponse);
                return new ProxerResult<bool>(lDict["msg"].Equals("Erfolgreich!") &&
                                              !lDict["message"].Equals("Befehle sind nur in Konferenzen verfügbar."));
            }
            catch
            {
                return
                    new ProxerResult<bool>((await ErrorHandler.HandleError(this._senpai, lResponse, false)).Exceptions);
            }
        }


        /// <summary>
        ///     Gibt zurück, ob Senpai ein Teilnehmer einer Konferenz mit der bestimmten ID ist.
        ///     <para>Mögliche Fehler, die <see cref="ProxerResult" /> enthalten kann:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Ausnahme</term>
        ///             <description>Beschreibung</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="NotLoggedInException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn der <see cref="Senpai">Benutzer</see> nicht eingeloggt ist.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="WrongResponseException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn die Antwort des Servers nicht der Erwarteten entspricht.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="ArgumentNullException" />
        ///             </term>
        ///             <description>Wird ausgelöst, wenn <paramref name="senpai" /> null (oder Nothing in Visual Basic) ist.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="id">ID der Konferenz</param>
        /// <param name="senpai">Muss eingeloggt sein</param>
        /// <seealso cref="Senpai.Login" />
        /// <returns>Benutzer ist Teilnehmer der Konferenz. True oder False.</returns>
        public static async Task<ProxerResult<bool>> IstTeilnehmner(int id, Senpai senpai)
        {
            if (senpai == null)
                return new ProxerResult<bool>(new Exception[] {new ArgumentNullException(nameof(senpai))});

            if (!senpai.LoggedIn) return new ProxerResult<bool>(new Exception[] {new NotLoggedInException(senpai)});

            Dictionary<string, string> lPostArgs = new Dictionary<string, string>
            {
                {"message", "/ping"}
            };
            ProxerResult<string> lResult =
                await
                    HttpUtility.PostResponseErrorHandling(
                        "https://proxer.me/messages?id=" + id + "&format=json&json=answer",
                        lPostArgs,
                        senpai.LoginCookies,
                        senpai.ErrHandler,
                        senpai);

            if (!lResult.Success)
                return new ProxerResult<bool>(lResult.Exceptions);

            string lResponse = lResult.Result;

            try
            {
                Dictionary<string, string> lDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(lResponse);
                return new ProxerResult<bool>(!lDict["msg"].Equals("Ein Fehler ist passiert."));
            }
            catch
            {
                return new ProxerResult<bool>((await ErrorHandler.HandleError(senpai, lResponse, false)).Exceptions);
            }
        }

        #endregion

        /// <summary>
        ///     Repräsentiert die jeweilige einzelne Nachricht in der Konferenz.
        /// </summary>
        public class Message
        {
            /// <summary>
            ///     Die Aktion der Nachricht.
            /// </summary>
            public enum Action
            {
                /// <summary>
                ///     Normale Nachricht, nur Text.
                /// </summary>
                NoAction,

                /// <summary>
                ///     Ein Benutzer wurde hinzugefügt.
                /// </summary>
                AddUser,

                /// <summary>
                ///     Ein Benutzer wurde entfernt.
                /// </summary>
                RemoveUser,

                /// <summary>
                ///     Der Leiter der Konferenz wurde geändert.
                /// </summary>
                SetLeader,

                /// <summary>
                ///     Das Thema der Konferenz wurde geändert.
                /// </summary>
                SetTopic,

                /// <summary>
                ///     Immer wenn von der JSON direkt etwas zurückgegeben wird z.B. bei /leader
                /// </summary>
                GetAction
            }

            internal Message(User sender, int mid, string nachricht, int unix, Action aktion)
            {
                this.Sender = sender;
                this.NachrichtId = mid;
                this.Nachricht = nachricht;
                this.TimeStamp = Utility.UnixTimeStampToDateTime(unix);
                this.Aktion = aktion;
            }

            internal Message(User sender, int mid, string nachricht, DateTime date, Action aktion)
            {
                this.Sender = sender;
                this.NachrichtId = mid;
                this.Nachricht = nachricht;
                this.TimeStamp = date;
                this.Aktion = aktion;
            }

            #region Properties

            /// <summary>
            ///     Gibt die Aktion der Nachricht zurück.
            /// </summary>
            public Action Aktion { get; }

            /// <summary>
            ///     Gibt den Text der Nachricht zurück.
            /// </summary>
            public string Nachricht { get; private set; }

            /// <summary>
            ///     Gibt die ID der Nachricht zurück.
            /// </summary>
            public int NachrichtId { get; }

            /// <summary>
            ///     Gibt den Sender der Nachricht zurück.
            /// </summary>
            public User Sender { get; private set; }

            /// <summary>
            ///     Gibt das Datum der Nachricht zurück.
            /// </summary>
            public DateTime TimeStamp { get; private set; }

            #endregion
        }
    }
}