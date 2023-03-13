using XUMM.NET.SDK.Models.Payload;
using XUMM.NET.SDK.Enums;
using XUMM.NET.SDK.Models.Payload.Xumm;
using XUMM.NET.SDK.Extensions;
using XUMM.NET.SDK;
using Newtonsoft.Json;
using RestSharp;
using UnityXUMMAPI.Models;
using SharedLibrary;
using UnityXUMMAPI.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using System.Drawing;
using UnityXUMMAPI.Repository;
using UnityXUMMAPI.Entities;

namespace KEEGames.UnityXUMMAPI.Services
{
    //interface for the player's stuff
    //standards would have this in its own folder and interface class 
    public interface IPlayerService
    {
        Task<Player> AuthenticateUser(XummPostJsonPayload payload);
        Player StartAuthentication();
      List<NftDnaResponse> GetPlayerNFTS(string account);
        void GetPayloadByUUID(string payloaduuid);
        string CheckStatus(string payloadUUID);
    }

    //real service
    public class PlayerService : IPlayerService
    {
        private readonly IHelperEngine _helperEngine;
        // here i register a read only varible of the sdk
        private readonly XummSdk _sdk;
        private readonly XummConnections _xummConnections;
        private readonly IRepository<PayloadStatus> _sqlRepository;
        
        public PlayerService(IOptions<XummConnections> xummOptions, IHelperEngine helperEngine, IRepository<PayloadStatus> sqlRepository)
        {
            
            _helperEngine = helperEngine;
            _xummConnections = xummOptions.Value;
            _sqlRepository = sqlRepository;
            // here i make assign it to a new instance 
            // this can be pulled from the appsettings so you only change it in one place 
            //_sdk = new XummSdk("d4ff9437-d8e5-46fa-a2fc-b76f8b62009d", "41d06e96-2b29-4ef0-b6e7-e5b9a25ce0b1");
            _sdk = new XummSdk(_xummConnections.ApiKey, _xummConnections.ApiSecret);

        }
        
        /// <summary>
        /// Starts the authentication process
        /// </summary>
        /// <returns>Player object with authentication packet</returns>
        public Player StartAuthentication()
        {
            // Create the Sign in Transaction and ad Custom Meta to it. 
            var payload = new XummPayloadTransaction(XummTransactionType.SignIn).ToXummPostJsonPayload();
            payload.CustomMeta = new XummPayloadCustomMeta { Instruction = "Authenticate payload" };
            
            //Call the athenticate user method async and return a task. 
            var result =  AuthenticateUser(payload);

            //returns the qrcode 
            return result.Result;

        }

        /// <summary>
        /// Async task for authentication of the user
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>Task to calling function</returns>
        public async Task<Player> AuthenticateUser(XummPostJsonPayload payload)
        {
            
            // Call XUMM API's create payload method (true flag there incase errors happen - it should return that fact.
            var result = await _sdk.Payload.CreateAsync(payload, true);
           
            // Check if the result is not null, then return the QR png
            if (result != null)
                return new Player
                {
                    uuid = result.Uuid,
                    qrurl = result.Refs.QrPng
                };

            //for testing
            /*  return result.Refs.QrPng;  
            *///result.Uuid WVD  "254bc6e6-3c3f-47f4-8b93-7749c3fae076"
            
            //return the player
            return new Player(); 
        }

        /// <summary>
        /// Gets a list of the players NFT's
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<NftDnaResponse> GetPlayerNFTS(string account)
        {
            // preparing API request
            Param[] paramsList = new Param[1];
            paramsList[0] = new Param
            {
                Account = account,
                LedgerIndex = "validated"
            };

            var nftObject = new NftRequestModel
            {
                Method = "account_nfts",
                Params = paramsList
            };

            //Serializing account object
            var serializationNftObject = JsonConvert.SerializeObject(nftObject);
            
            //Rest call to XRPcluster.com
            var client = new RestClient("https://xrplcluster.com");
            var request = new RestRequest("resource", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", serializationNftObject, ParameterType.RequestBody);
            
            //getting response
            var response = client.Execute(request);
            
            //Deserializing response into NftAccount Model
            var result = JsonConvert.DeserializeObject<NftAccount>(response.Content);

            // Retrieve the player's NFTs from NftAccount MOdel
            var nftList = new List<NftDnaResponse>();
            foreach (var nft in result.Result.AccountNfts)
            {
                var dna = GetNftMetaData(nft.NfTokenId);
                if (dna != null)
                    nftList.Add(dna);
            }
       
            // return nftlist for account
            return nftList;
        }

        /// <summary>
        /// Gets the NFT metadata and prepares response
        /// </summary>
        /// <param name="nftTokenId"></param>
        /// <returns></returns>
        public NftDnaResponse GetNftMetaData(string nftTokenId)
        {
            var client = new RestClient("https://marketplace-api.onxrp.com");
            var request = new RestRequest("/api/metadata/" + nftTokenId, Method.Get);
            request.AddHeader("accept", "application/json");
            var response = client.Get(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<NftDnaResponse>(response.Content.ToString());
                if (result.collection.name == "MONKEE MONKEE")
                {
                   result.image = GetImage(result.image);
                    return result;
                }
            }
            return new NftDnaResponse();
        }

        /// <summary>
        /// Helper function to get the image from the interplanetary file system
        /// </summary>
        /// <param name="image"></param>
        /// <returns>path to the image on ipfs</returns>
        public string GetImage(string image)
        {
            //replace image url to make a proper 1 
            var result = image.Replace("ipfs://", "https://ipfs.io/ipfs/");

            return result;
        }

        /// <summary>
        /// Check the status for the specific  UUID
        /// </summary>
        /// <param name="payloadUUID"></param>
        /// <returns></returns>
        public string CheckStatus(string payloadUUID)
        {
            //easy exit
            if (payloadUUID == null)
                return "" ;

            //check if there is an account stored with payloadUUID
            var statusModel = _sqlRepository.GetAll().Where(x => x.PayloadUUid == payloadUUID).FirstOrDefault();
            if (statusModel != null && statusModel.Signed && statusModel.IsActive)
            {
                return statusModel.Account;
            }

            //catch
            return "";
        }

        /// <summary>
        /// Gets the payload and stores it into the database.
        /// </summary>
        /// <param name="payloaduuid"></param>
        public void  GetPayloadByUUID(string payloaduuid)
        {

            //call the xumm endpoint with payloaduuidv4
            var client = new RestClient(_xummConnections.RestClientAddress);
            var request = new RestRequest("/platform/payload/"+payloaduuid,Method.Get);
            request.AddHeader("accept", "application/json");
            request.AddHeader("X-API-Key", _xummConnections.ApiKey);
            request.AddHeader("X-API-Secret", _xummConnections.ApiSecret);
       
            //get the response and parse the result
            var response = client.Get(request);
            var result = JsonConvert.DeserializeObject<PayloadDetails>(response.Content.ToString());
            
            //save status to DB
            var saveData = new PayloadStatus
            {
                Account = result.response.account,
                PayloadUUid = result.meta.uuid,
                Signed = result.meta.signed,
                IsActive = true,
                UpdatedOn = DateTime.Now,

            };
            _sqlRepository.Create(saveData);
           
        }


    }

    //for testing service - leave these for now pls;
    //public class TestPlayerService : IPlayerService
    //{
    //    public string AuthenticateUser()
    //    {
    //        Console.WriteLine("todo from test service");
    //        //this is where you need to authenticate them.  I need ot return the 
    //        return "r-address here";
    //    }

    //    public void StartAuthentication()
    //    {
    //        Console.WriteLine("todo");

    //    }
    //}
}



