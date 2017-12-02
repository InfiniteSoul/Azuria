﻿using Azuria.Api.v1.Input.Ucp;
using Azuria.Helpers.Attributes;

namespace Azuria.Api.v1.Input.User
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UserGetListInput : UcpGetListInputBase
    {
        /// <summary>
        /// </summary>
        [InputData("uid", Optional = true)]
        public int? UserId { get; set; }

        /// <summary>
        /// Ignored if <see cref="UserId"/> is given.
        /// </summary>
        [InputData("username", ConverterMethodName = nameof(GetUsernameString), Optional = true)]
        public string Username { get; set; }

        internal string GetUsernameString(string username)
        {
            return this.UserId == null ? username : null;
        }
    }
}