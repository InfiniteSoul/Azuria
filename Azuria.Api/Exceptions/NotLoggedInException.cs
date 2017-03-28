﻿using System;

namespace Azuria.Api.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a <see cref="IProxerUser" /> object, which was not logged in, was passed to
    /// a method or a constructor which required it to be logged in.
    /// </summary>
    public class NotLoggedInException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="NotLoggedInException" /> class.
        /// </summary>
        public NotLoggedInException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NotLoggedInException" /> class.
        /// </summary>
        /// <param name="user">The user which is not logged in.</param>
        public NotLoggedInException(IProxerUser user)
        {
            this.User = user;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotLoggedInException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message string.</param>
        public NotLoggedInException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotLoggedInException" /> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="inner">The inner exception reference.</param>
        public NotLoggedInException(string message, Exception inner) : base(message, inner)
        {
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="IProxerUser" /> object which is connected to this exception.
        /// </summary>
        public IProxerUser User { get; set; }

        #endregion
    }
}