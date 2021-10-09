using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroStoreAPI.Models.Firebase
{
    public class UserDataResponse
    {

        [JsonProperty("users")]
        public IReadOnlyList<User> Users { get; set; }
    }
}
