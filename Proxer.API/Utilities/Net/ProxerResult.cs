﻿using System;

namespace Proxer.API.Utilities.Net
{
    /// <summary>
    ///     Eine Klasse, die ein Resultat einer Methode des API darstellt.
    /// </summary>
    /// <typeparam name="T">Der Typ des Resultats.</typeparam>
    public class ProxerResult<T>
    {
        internal ProxerResult()
        {
            this.Success = true;
        }

        internal ProxerResult(T result)
        {
            this.Success = true;
            this.Result = result;
        }

        internal ProxerResult(Exception[] exceptions)
        {
            this.Success = false;
            this.Exceptions = exceptions;
        }

        #region Properties

        /// <summary>
        ///     Gibt die Fehler zurück, die während der Ausführung aufgetreten sind, oder legt diese fest.
        /// </summary>
        /// <value>Ist null, wenn <see cref="Success" /> == true</value>
        public Exception[] Exceptions { get; set; }

        /// <summary>
        ///     Gibt das Resultat zurück, das die Klasse repräsentiert, oder legt dieses fest.
        /// </summary>
        /// <value>Ist null, wenn <see cref="Success" /> == false</value>
        public T Result { get; set; }

        /// <summary>
        ///     Gibt zurück, ob die Methode erfolg hatte, oder legt dieses fest.
        /// </summary>
        public bool Success { get; set; }

        #endregion
    }

    /// <summary>
    ///     Eine Klasse, die ein Resultat einer Methode des API darstellt.
    /// </summary>
    public class ProxerResult
    {
        internal ProxerResult()
        {
            this.Success = true;
        }

        internal ProxerResult(Exception[] exceptions)
        {
            this.Success = false;
            this.Exceptions = exceptions;
        }

        #region Properties

        /// <summary>
        ///     Gibt die Fehler zurück, die während der Ausführung aufgetreten sind, oder legt diese fest.
        /// </summary>
        /// <value>Ist null, wenn <see cref="Success" /> == true</value>
        public Exception[] Exceptions { get; set; }

        /// <summary>
        ///     Gibt zurück, ob die Methode erfolg hatte, oder legt dieses fest.
        /// </summary>
        public bool Success { get; set; }

        #endregion
    }
}