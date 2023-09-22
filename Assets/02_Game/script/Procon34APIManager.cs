using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KCP2023
{
    [System.Serializable]
    public class Response
    {
        public string publishingOffice;
        public string reportDatetime;
        public string targetArea;
        public string headlineText;
        public string text;
    }

    public class TestResp
    {
        public string id;
    }

    public class Procon34APIManager : SingletonBase<Procon34APIManager>
    {
        //public string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        public string url ="https://www.procon.gr.jp/matches/?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        /// https://www.procon.gr.jp/matches?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521
        /// </summary>
        
        [SerializeField] private Button button;

        // Start is called before the first frame update
        void Start()
        {
            button?.onClick.AddListener(OnClickedButton);
        }

        private void OnClickedButton()
        {
            //StartCoroutine(GetRequest("https://www.jma.go.jp/bosai/forecast/data/overview_forecast/130000.json"));
            StartCoroutine(GetRequest(url));
            
        }

        private IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    // Error.
                    switch (webRequest.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                            Debug.LogError("Error: " + webRequest.error);
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            Debug.LogError("HTTP Error: " + webRequest.error);
                            break;
                        default:
                            // ここにはこない.
                            break;
                    }
                    yield break;
                }

                //var response = JsonUtility.FromJson<Response>(webRequest.downloadHandler.text);
                //Debug.Log("データ配信元: " + response.publishingOffice);
                //Debug.Log("報告日時: " + response.reportDatetime);
                //Debug.Log("対象の地域: " + response.targetArea);
                //Debug.Log("ヘッドライン: " + response.headlineText);
                //Debug.Log("詳細: " + response.text);

                var response = JsonUtility.FromJson<TestResp>(webRequest.downloadHandler.text);
                Debug.Log(response.id);
                
                yield return null;
            }
        }
    }

}