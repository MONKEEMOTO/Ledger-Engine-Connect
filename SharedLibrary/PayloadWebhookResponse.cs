using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    // this is the response you get from the 3rd party model 
    public  class PayloadWebhookResponse
    {
        public class Blob
        {
        }

        public class CustomMeta
        {
            public string identifier { get; set; }
            public Blob blob { get; set; }
            public string instruction { get; set; }
        }

        public class Meta
        {
            public string url { get; set; }
            public string application_uuidv4 { get; set; }
            public string payload_uuidv4 { get; set; }
            public bool opened_by_deeplink { get; set; }
        }

        public class PayloadResponse
        {
            public string payload_uuidv4 { get; set; }
            public string reference_call_uuidv4 { get; set; }
            public bool signed { get; set; }
            public bool user_token { get; set; }
            public ReturnUrl return_url { get; set; }
            public string txid { get; set; }
        }

        public class ReturnUrl
        {
            public string app { get; set; }
            public string web { get; set; }
        }

        public class Root
        {
            public Meta meta { get; set; }
            public CustomMeta custom_meta { get; set; }
            public PayloadResponse payloadResponse { get; set; }
            public UserToken userToken { get; set; }
        }

        public class UserToken
        {
            public string user_token { get; set; }
            public int token_issued { get; set; }
            public int token_expiration { get; set; }
        }
    }
}
