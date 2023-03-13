using Newtonsoft.Json;

namespace UnityXUMMAPI.Models
{
    public partial class NftRequestModel
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public Param[] Params { get; set; }
    }

    public partial class Param
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("ledger_index")]
        public string LedgerIndex { get; set; }
    }
}
