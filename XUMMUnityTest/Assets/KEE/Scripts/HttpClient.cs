using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace KEEGames {

    /// <summary>
    /// Wrapper for Unity WebRequest - makes it easier to use and Async
    /// </summary>
    public static class HttpClient
    {
        /// <summary>
        /// send async get request to specified endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static async Task<T> Get<T>(string endpoint) {
            var getRequest = CreateRequest(endpoint);
            getRequest.SendWebRequest();

            // okay so changed the use of Newtonsoft to use the built in Json utilty for unity
            while (!getRequest.isDone) await Task.Delay(10);

            return JsonUtility.FromJson<T>(getRequest.downloadHandler.text);
        }

        /// <summary>
        /// send async post request to specified endpoint with given payload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static async Task<T> Post<T>(string endpoint, object payload) {
            var postRequest = CreateRequest(endpoint, RequestType.POST, payload);
            postRequest.SendWebRequest();

            while (!postRequest.isDone) await Task.Delay(10);
            return JsonUtility.FromJson<T>(postRequest.downloadHandler.text);
        }

        /// <summary>
        /// wrapped unitywebrequest function
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null) {
            var request = new UnityWebRequest(path, type.ToString());

            if (data != null) {
                var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;

        }

        /// <summary>
        /// helper to attach header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static void AttachHeader(UnityWebRequest request, string key, string value)
        {
            request.SetRequestHeader(key, value);
        }


    }

    /// <summary>
    /// enum for the different request types
    /// </summary>
    public enum RequestType
    {
        GET=0,
        POST=1,
        PUT=2
    }
}

