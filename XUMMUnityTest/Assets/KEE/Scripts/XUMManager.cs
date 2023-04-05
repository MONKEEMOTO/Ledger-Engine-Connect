using UnityEngine;
using SharedLibrary;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

using System.Collections.Generic;

namespace KEEGames {
    public class XUMManager : MonoBehaviour
    {
        public Image qrImage;
        public List<Image> imageList;
        private GameObject _qrContainer;
        private int _retryCounter;
        private int _maxRetriesForXUMM = 10;

         // Start is called before the first frame update
         void Start()
        {
            _qrContainer = qrImage.gameObject;
            _qrContainer.SetActive(false);
            _retryCounter = 0;
        }

        /// <summary>
        /// calls our custom API and gets a login QR code, as well as setting up the callback coroutine.
        /// </summary>
        public async void StartSignin()
        {
            _qrContainer.SetActive(true);
            
            //call API
            var player = await HttpClient.Get<Player>("https://unityxummapi.azurewebsites.net/player/signin/500");
            //var player = await HttpClient.Get<Player>("https://localhost:7042/player/signin/500");
            
            //catch
            if (player == null)
            {
                Debug.LogError("Service not running or internet connection not working - please try again");
                return;
            }

            // need to assign to individual field 
            Debug.Log("QR URL : " + player.qrurl.ToString());
            Debug.Log("player : " + player.ToString());

            //load that qr code and show it
            StartCoroutine(LoadQRCode(player.qrurl.ToString()));
            StartCoroutine(CheckForCallback(player.uuid));
        }

        /// <summary>
        /// iEnum for loading the QR code
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IEnumerator LoadQRCode(string url) {
            
            //send the request
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            
            //handle connection errors
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.Log(request.error);
            else
            {
                //get hte texture
                Texture2D qrtex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                if (qrtex != null)
                {
                    qrImage.sprite = Sprite.Create(qrtex, new Rect(0, 0, qrtex.width, qrtex.height), new Vector2(0, 0));
                }
                //set texture
                qrImage.material.mainTexture = qrtex;

                Debug.Log(qrtex.dimension);
            }
        }

        /// <summary>
        /// Checks for any callbacks for the specified UUID
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        private IEnumerator CheckForCallback(string uuid)
        {
            //check for a specified amount of times before timing out.
            var isWaiting = true;
            while (isWaiting && _retryCounter<_maxRetriesForXUMM)
            {
                //keep track of retries
                _retryCounter++;

                //send the request
                UnityWebRequest request = UnityWebRequest.Get($"https://unityxummapi.azurewebsites.net/player/getstatus/{uuid}");
                //UnityWebRequest request = UnityWebRequest.Get($"https://localhost:7042/player/getstatus/{uuid}");
                yield return request.SendWebRequest();
                
                Debug.Log("Request was :" + request);
                
                //catch connection errors
                if (request.result==UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("Network error: " + request.error);
                }
                else
                {
                    //check response code is valid and that there's data
                    if (request.responseCode == 200 && request.downloadHandler.data != null)
                    {
                        Debug.Log("Callback received!");
                        
                        //check for acc details
                        var account = request.downloadHandler.text.ToString();
                        if (!string.IsNullOrEmpty(account))
                        {
                            //stop checking for validated account cause we found onee
                            isWaiting = false;

                            //account found, lets get the NFT's
                            UnityWebRequest nftRequest = UnityWebRequest.Get($"https://unityxummapi.azurewebsites.net/player/nfts/{account}");
                            //UnityWebRequest nftRequest = UnityWebRequest.Get($"https://localhost:7042/player/nfts/{account}");
                            yield return nftRequest.SendWebRequest();

                            Debug.Log("NFT Request data was :" + nftRequest);
                            
                            //handle connection errors again
                            if (nftRequest.result== UnityWebRequest.Result.ConnectionError)
                            {
                                Debug.LogError("Network error: " + nftRequest.error);
                            }
                            else
                            {
                                //check if a response was received and that there's data.
                                if (nftRequest.responseCode == 200 && nftRequest.downloadHandler.data != null)
                                {
                                    //deserialize
                                    var nftList = JsonConvert.DeserializeObject<List<NftDnaResponse>>(nftRequest.downloadHandler.text);
                                    
                                    //get all the images of the nft's
                                    foreach (var nft in nftList)
                                    {
                                        UnityWebRequest imageResult = UnityWebRequestTexture.GetTexture(nft.image);
                                        yield return imageResult.SendWebRequest();

                                        //handle errors
                                        if (imageResult.result == UnityWebRequest.Result.ConnectionError)
                                        {
                                            Debug.LogError("Network error: " + nftRequest.error);
                                        }
                                        else
                                        {
                                            //get the image texture from the web
                                            Texture2D imtex = ((DownloadHandlerTexture)imageResult.downloadHandler).texture;
                                            if (imtex != null)
                                            {
                                                qrImage.sprite = Sprite.Create(imtex, new Rect(0, 0, imtex.width, imtex.height), new Vector2(0, 0));
                                            }

                                            qrImage.material.mainTexture = imtex;
                                            
                                            //adding each of the images to a list
                                            imageList.Add(qrImage);

                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No callback received.");
                    }
                }
                yield return new WaitForSeconds(15f);
            }

            if (_retryCounter >= _maxRetriesForXUMM)
            {
                isWaiting = false;
                Debug.Log("Max retries for checking - aborting");
            }
        }
        

    }
}

