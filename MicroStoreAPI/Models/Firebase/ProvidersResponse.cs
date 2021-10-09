﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroStoreAPI.Models.Firebase
{
    public class ProvidersResponse
    {
        /// <summary>
        /// The list of providers that the user has previously signed in with.
        /// </summary>
        [JsonProperty("allProviders")]
        public IReadOnlyList<string> AllProviders { get; set; }

        /// <summary>
        /// Whether the email is for an existing account.
        /// </summary>
        [JsonProperty("registered")]
        public bool IsRegistered { get; set; }

        public static class CommonErrors
        {
            /// <summary>
            /// The email address is badly formatted.
            /// </summary>
            public const string INVALID_EMAIL = nameof(INVALID_EMAIL);
        }
    }
}
