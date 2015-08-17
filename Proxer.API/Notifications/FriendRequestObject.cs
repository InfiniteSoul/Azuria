﻿using Proxer.API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxer.API.Notifications
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendRequestObject : INotificationObject
    {
        private Senpai senpai;
        private bool accepted;
        private bool denied;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <param name="senpai"></param>
        internal FriendRequestObject(string userName, int userID, Senpai senpai)
        {
            this.senpai = senpai;
            this.Type = NotificationObjectType.FriendRequest;
            this.Message = userName;
            this.UserName = userName;
            this.ID = userID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <param name="userDescription"></param>
        /// <param name="requestDate"></param>
        /// <param name="userOnline"></param>
        /// <param name="senpai"></param>
        internal FriendRequestObject(string userName, int userID, string userDescription, DateTime requestDate, bool userOnline, Senpai senpai)
        {
            this.senpai = senpai;
            this.Type = NotificationObjectType.FriendRequest;
            this.Message = userName;
            this.UserName = userName;
            this.ID = userID;
            this.Description = userDescription;
            this.Date = requestDate;
            this.Online = userOnline;
        }

        /// <summary>
        /// 
        /// </summary>
        public NotificationObjectType Type { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Online { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Ob die Aktion erfolgreich war</returns>
        public bool acceptRequest()
        {
            if (senpai.LoggedIn && !this.accepted && !this.denied)
            {
                Dictionary<string, string> lPostArgs = new Dictionary<string, string> { { "type", "accept" } };
                string lResponse = HttpUtility.PostWebRequestResponse("https://proxer.me/user/my?format=json&cid=" + this.ID, senpai.LoginCookies, lPostArgs);

                if (lResponse.StartsWith("{\"error\":0"))
                {
                    this.accepted = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Ob die Aktion erfolgreich war</returns>
        public bool denyRequest()
        {
            if (senpai.LoggedIn && !this.accepted && !this.denied)
            {
                Dictionary<string, string> lPostArgs = new Dictionary<string, string> { { "type", "deny" } };
                string lResponse = HttpUtility.PostWebRequestResponse("https://proxer.me/user/my?format=json&cid=" + this.ID, senpai.LoginCookies, lPostArgs);

                if (lResponse.StartsWith("{\"error\":0"))
                {
                    this.denied = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Ob die Aktion erfolgreich war</returns>
        public bool editDescription(string pNewDescription)
        {
            if (senpai.LoggedIn)
            {
                Dictionary<string, string> lPostArgs = new Dictionary<string, string> { { "type", "desc" } };
                string lResponse = HttpUtility.PostWebRequestResponse("https://proxer.me/user/my?format=json&desc=" + System.Web.HttpUtility.JavaScriptStringEncode(pNewDescription) + "&cid=" + this.ID, senpai.LoginCookies, lPostArgs);

                if (lResponse.StartsWith("{\"error\":0"))
                {
                    this.Description = pNewDescription;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
