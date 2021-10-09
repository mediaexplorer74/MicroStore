﻿using Newtonsoft.Json;

namespace MicroStoreAPI.Models.Firebase
{
    public class OAuthUserSignInResponse
    {
        /// <summary>
        /// The unique ID identifies the IdP account.
        /// </summary>
        [JsonProperty("federatedId")]
        public string FederatedID { get; set; }

        /// <summary>
        /// The linked provider ID (e.g. "google.com" for the Google provider).
        /// </summary>
        [JsonProperty("providerId")]
        public string ProviderID { get; set; }

        /// <summary>
        /// The uid of the authenticated user.
        /// </summary>
        [JsonProperty("localId")]
        public string LocalID { get; set; }

        /// <summary>
        /// Whether the sign-in email is verified.
        /// </summary>
        [JsonProperty("emailVerified")]
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// The email of the account.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The OIDC id token if available.
        /// </summary>
        [JsonProperty("oauthIdToken")]
        public string OAuthIDToken { get; set; }

        /// <summary>
        /// The OAuth access token if available.
        /// </summary>
        [JsonProperty("oauthAccessToken")]
        public string OAuthAccessToken { get; set; }

        /// <summary>
        /// The OAuth 1.0 token secret if available.
        /// </summary>
        [JsonProperty("oauthTokenSecret")]
        public string OAuthTokenSecret { get; set; }

        /// <summary>
        /// The stringified JSON response containing all the IdP data corresponding
        /// to the provided OAuth credential.
        /// </summary>
        [JsonProperty("rawUserInfo")]
        public string RawUserInfo { get; set; }

        /// <summary>
        /// The first name for the account.
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name for the account.
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// The full name for the account.
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// The display name for the account.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// The photo Url for the account.
        /// </summary>
        [JsonProperty("photoUrl")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// A Firebase Auth ID token for the authenticated user.
        /// </summary>
        [JsonProperty("idToken")]
        public string IDToken { get; set; }

        /// <summary>
        /// A Firebase Auth refresh token for the authenticated user.
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The number of seconds in which the ID token expires.
        /// </summary>
        [JsonProperty("expiresIn")]
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Whether another account with the same credential already exists.
        /// The user will need to sign in to the original account and then link the current credential to it.
        /// </summary>
        [JsonProperty("needConfirmation")]
        public bool NeedsConfirmation { get; set; }

        public static class CommonErrors
        {
            /// <summary>
            /// The supplied auth credential is malformed or has expired.
            /// </summary>
            public const string INVALID_IDP_RESPONSE = nameof(INVALID_IDP_RESPONSE);

            /// <summary>
            /// The corresponding provider is disabled for this project.
            /// </summary>
            public const string OPERATION_NOT_ALLOWED = nameof(OPERATION_NOT_ALLOWED);

            /// <summary>
            /// This credential is already associated with a different user account.
            /// </summary>
            public const string FEDERATED_USER_ID_ALREADY_LINKED = nameof(FEDERATED_USER_ID_ALREADY_LINKED);

            /// <summary>
            /// The email address is already in use by another account.
            /// </summary>
            public const string EMAIL_EXISTS = nameof(EMAIL_EXISTS);

            /// <summary>
            /// The user's credential is no longer valid. The user must sign in again.
            /// </summary>
            public const string INVALID_ID_TOKEN = nameof(INVALID_ID_TOKEN);
        }
    }
}
