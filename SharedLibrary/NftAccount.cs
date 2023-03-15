using Newtonsoft.Json;


namespace SharedLibrary
{
    [Serializable]
    public  class NftAccount
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public  class Result
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("account_nfts")]
        public AccountNft[] AccountNfts { get; set; }

        [JsonProperty("ledger_hash")]
        public string LedgerHash { get; set; }

        [JsonProperty("ledger_index")]
        public long LedgerIndex { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("validated")]
        public bool Validated { get; set; }
    }

    public  class AccountNft
    {
        [JsonProperty("Flags")]
        public long Flags { get; set; }

        [JsonProperty("Issuer")]
        public string Issuer { get; set; }

        [JsonProperty("NFTokenID")]
        public string NfTokenId { get; set; }

        [JsonProperty("NFTokenTaxon")]
        public long NfTokenTaxon { get; set; }

        [JsonProperty("TransferFee")]
        public long TransferFee { get; set; }

        [JsonProperty("nft_serial")]
        public long NftSerial { get; set; }
    }
}
