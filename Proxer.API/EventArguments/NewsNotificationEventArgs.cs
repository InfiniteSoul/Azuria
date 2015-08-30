﻿using System.Collections.Generic;
using Proxer.API.Notifications;

namespace Proxer.API.EventArguments
{
    /// <summary>
    /// </summary>
    public class NewsNotificationEventArgs : INotificationEventArgs
    {
        private readonly Senpai _senpai;

        /// <summary>
        /// </summary>
        /// <param name="count"></param>
        /// <param name="senpai"></param>
        internal NewsNotificationEventArgs(int count, Senpai senpai)
        {
            this._senpai = senpai;
            this.Type = NotificationEventArgsType.News;
            this.NotificationCount = count;
        }

        /// <summary>
        /// </summary>
        public List<NewsObject> Benachrichtigungen
        {
            get { return this._senpai.News; }
        }

        /// <summary>
        /// </summary>
        public int NotificationCount { get; private set; }

        /// <summary>
        /// </summary>
        public NotificationEventArgsType Type { get; private set; }
    }
}