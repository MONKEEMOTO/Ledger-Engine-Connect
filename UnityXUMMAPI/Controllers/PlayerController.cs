using Microsoft.AspNetCore.Mvc;
using KEEGames.UnityXUMMAPI.Services;
using Newtonsoft.Json;
using SharedLibrary;


namespace KEEGames.UnityXUMMAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase {
        private readonly IPlayerService _playerservice;

        public PlayerController(IPlayerService playerService)
        {
            _playerservice = playerService;
        }

        /// <summary>
        /// test route to get player from service to unity
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        [HttpGet("{playerID}")]
        public Player Get([FromRoute] int playerID) {
            
            //not using this one 
            _playerservice.StartAuthentication();
            
            //test code to see if route works
            var player = new Player
                () { playerGuid = playerID };
            
            return player;
        }

        /// <summary>
        /// Sign in  to start the sign in process for a player into service
        /// </summary>
       
        /// <returns></returns>
        [HttpGet]
        [Route("signin/{playerID}")]
        public Player Signin([FromRoute] int playerID)
        {
            // call the sign in process and returns the QR code 
            var playerResult = _playerservice.StartAuthentication();
            playerResult.playerGuid = playerID;
            //assigning the responses to the model
            //var playerResponse = new Player() { playerGuid = playerID, playerName = "", qrurl = playerResult };
            return playerResult;

        }

        [HttpGet]
        [Route("nfts/{account}")]
        public List<NftDnaResponse> GetNFTS([FromRoute] string account )
        {
            var playerNFTS = _playerservice.GetPlayerNFTS(account);
         var s =    JsonConvert.SerializeObject(playerNFTS);
            return playerNFTS;
        }


        /// <summary>
        /// test route to post new player into service
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost]
        public Player Post(Player player)
        {
            Console.WriteLine("Player data has been posted");
            return player;
        }


        [HttpGet]
        [Route("getstatus/{payloadUUID}")]
        public string GetStatus([FromRoute] string payloadUUID)
        {
            var playerAccountNumber = _playerservice.CheckStatus(payloadUUID);
            if(playerAccountNumber == null)
            {
                return "";
               
            }
            return playerAccountNumber.ToString();
            
        }
        /// <summary>
        /// call back allow the 3rd party app to send data for the  service
        /// this should be player/callback
        /// </summary>
        /// <param name="incoming"></param>
        /// <returns></returns>
        [HttpPost("callback")]
  //this should
        public void Callback(object incoming)
        {
            //Opens up an endpoint that can be configured in the portal to point all responses to
            //after they scanned it should come here and then return the identifier to your game 
            if (incoming != null)
            {
                var model = JsonConvert.DeserializeObject<PayloadWebhookResponse.Root>(incoming.ToString());

                _playerservice.GetPayloadByUUID(model.payloadResponse.payload_uuidv4);
            }
          
         
            //needs to send this to the app when reciveed and signed in so that the unity app can then call get nfts list 

           // return model.custom_meta.identifier;
        }
    }


}
