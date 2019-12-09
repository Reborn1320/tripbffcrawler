using System.Collections.Generic;
using Newtonsoft.Json;

namespace CFF.Crawler
{
    public class LocationModel
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        public string Url { get; set; }

        [JsonProperty(PropertyName = "long")]
        public string Long { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public string Lat { get; set; }
    }

    public class LocationResult
    {
        public List<LocationModel> Locations { get; set; }
    }
}
