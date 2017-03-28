﻿using System;
using System.Collections.Generic;

namespace Azuria.Api.ErrorHandling
{
    /// <summary>
    /// Represents the result of a method including a returned object.
    /// </summary>
    /// <typeparam name="T">The type of the returned object.</typeparam>
    public interface IProxerResult<T> : IProxerResult
    {
        #region Properties

        /// <summary>
        /// Gets the returned object if the method executed successfully. If the method did not execute successfully the default
        /// value of the type <typeparamref name="T" /> will be returned.
        /// </summary>
        T Result { get; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="success"></param>
        /// <param name="exceptions"></param>
        /// <param name="result"></param>
        void Deconstruct(out bool success, out IEnumerable<Exception> exceptions, out T result);

        #endregion
    }

    /// <summary>
    /// Represents the result of a method without a returned object.
    /// </summary>
    public interface IProxerResult
    {
        #region Properties

        /// <summary>
        /// Gets an enumeration of the exceptions that were thrown during method execution.
        /// </summary>
        IEnumerable<Exception> Exceptions { get; }

        /// <summary>
        /// Gets a value indicating whether the method executed successfully.
        /// </summary>
        bool Success { get; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="success"></param>
        /// <param name="exceptions"></param>
        void Deconstruct(out bool success, out IEnumerable<Exception> exceptions);

        #endregion
    }
}