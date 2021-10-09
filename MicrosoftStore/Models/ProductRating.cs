﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MicrosoftStore.Models
{
    public class ProductRating
    {
        public string RatingSystem { get; set; }
        public string RatingSystemShortName { get; set; }
        public string RatingSystemId { get; set; }
        public string RatingSystemUrl { get; set; }
        public string RatingValue { get; set; }
        public string RatingId { get; set; }
        public string RatingValueLogoUrl { get; set; }
        public int RatingAge { get; set; }
        public bool RestrictMetadata { get; set; }
        public bool RestrictPurchase { get; set; }
        public List<string> RatingDescriptors { get; set; }
        public List<string> RatingDisclaimers { get; set; }
        public List<object> InteractiveElements { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public Uri RatingSystemUri
        {
            get
            {
                try
                {
                    return new Uri(RatingSystemUrl);
                }
                catch { return null; }
            }
        }
        [JsonIgnore]
        public Uri RatingValueLogoUri
        {
            get
            {
                try
                {
                    return new Uri(RatingValueLogoUrl);
                }
                catch { return null; }
            }
        }
    }
}
