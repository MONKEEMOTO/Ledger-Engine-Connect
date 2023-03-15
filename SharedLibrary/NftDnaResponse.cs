using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    [Serializable]

    public class NftDnaResponse
    {
        [JsonProperty("dna")]
        public string dna { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("image")]
        public string image { get; set; }
        [JsonProperty("imageHash")]
        public string imageHash { get; set; }
        [JsonProperty("edition")]
        public int edition { get; set; }
        [JsonProperty("date")]
        public long date { get; set; }
        [JsonProperty("attributes")]
        public Attribute[] attributes { get; set; }
        [JsonProperty("compiler")]
        public string compiler { get; set; }
        [JsonProperty("schema")]
        public string schema { get; set; }
        [JsonProperty("nftType")]
        public string nftType { get; set; }
        [JsonProperty("collection")]
        public Collection collection { get; set; }
    }


    public class Collection
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
    }

    public class Attribute
    {
        [JsonProperty("trait_type")]
        public string trait_type { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
    }
}
