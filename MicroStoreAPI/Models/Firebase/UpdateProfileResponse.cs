using Newtonsoft.Json;
namespace MicroStoreAPI.Models.Firebase
{
    public class UpdateProfileResponse : UpdateAccountResponse
    {
        /// <summary>
        /// User's new display name.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// User's new photo url.
        /// </summary>
        [JsonProperty("photoUrl")]
        public string PhotoUrl { get; set; }
    }
}
