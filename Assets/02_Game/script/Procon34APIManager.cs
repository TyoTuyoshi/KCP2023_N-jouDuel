using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KCP2023
{
    /// <summary>
    /// 試合状況取得APIのレスポンス格納用クラス
    /// </summary>
    [System.Serializable]
    public class Matches
    {
        public class Log
        {
            public class Action
            {
                public bool succeeded;
                public int type;
                public int dir;
            }

            public int turn;
            public Action[] actions;
        }
        public int id;      //対戦ID
        public int turn;    //ターン数
        public Board board; //フィールドの状況
        public Log[] logs;  //試合ログ
    }
    public class Board
    {
        public enum Wall
        {
            None,       //陣地なし
            Mine,       //自陣地の城壁
            Opponent    //対戦相手陣地の城壁
        }
        public enum Territory
        {
            Neutral,    //中立
            Mine,       //自陣地
            Opponent,   //対戦相手陣地
            Both        //両陣地
        }
        public enum Structure
        {
            None,
            Pond,
            Castle
        }
        public enum MasonID
        {
            
        }

        public int[] walls;
        public int[] territories;

        public int width;
        public int height;
        public int mason;
        public int[] structures;
        public int[] masons;
    }

    public class Procon34APIManager : SingletonBase<Procon34APIManager>
    {
        private Board m_board = new Board();
        //public string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        public const string url ="https://www.procon.gr.jp/matches/?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        /// https://www.procon.gr.jp/matches/ID?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521
        public int matchID = 0;
        public string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";

        [SerializeField] private Button button;

        // Start is called before the first frame update
        void Start()
        {
            
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

                var response = JsonUtility.FromJson<Matches>(webRequest.downloadHandler.text);
                Debug.Log(response.id);
                
                yield return null;
            }
        }
    }

}