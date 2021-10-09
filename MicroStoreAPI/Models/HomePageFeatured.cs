using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroStoreAPI.Models
{
    public class HomePageFeatured
    {
        [JsonProperty("Carousel")]
        public List<string> Carousel { get; internal set; }
    }
}
