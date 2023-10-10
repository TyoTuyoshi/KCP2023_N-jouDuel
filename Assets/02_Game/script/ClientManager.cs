using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using BaseSystem.Utility;
using Debug = UnityEngine.Debug;

namespace KCP2023
{
    /// <summary>
    /// クライアントマネージャー
    /// </summary>
    public class ClientManager : SingletonBase<ClientManager>
    {
        //接続先ホスト 0:ローカルホスト　1:本選サーバー
        public int hostType = 0;

        /// <summary>
        /// GET: curl "localhost:3000/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521">debug.json
        /// POST: curl -i -X POST -H "Content-Type: application/json" -d "{\"turn\": 69,\"actions\": [{\"type\":2,\"dir\":4},{\"type\":2,\"dir\":4}]}" localhost:3000/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521
        /// </summary>
        private Process m_curlProcess = null;

        [NonSerialized] public string mainUrl =
            "https://www.procon.gr.jp/matches/id?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";

        public string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        //public const string url ="https://www.procon.gr.jp/matches/?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";

        /// <summary>
        /// CurlコマンドProcess群
        /// </summary>
        [NonSerialized] public const string curl = "curl.exe";

        [NonSerialized] public string mainHost = "https://www.procon.gr.jp";
        [NonSerialized] public string localHost = "localhost:3000";
        [NonSerialized] public int matchID = 10;
        [NonSerialized] public string outputPath = "C:/Users/futur/Desktop/KCP2023";
        [NonSerialized] public string outputJson = "state.json";

        /// <summary>
        /// .batファイル群
        /// </summary>
        [NonSerialized] public string getSampleServerBat = "/getSampleServer.bat";

        [NonSerialized] public string getSampleServerBatPath = "C:/Users/futur/Desktop/KCP2023/server/bat";

        // 現状の試合
        private Matches nowMatches = null;

        /// <summary>
        /// Curlコマンド引数取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>Curlコマンド引数</returns>
        private string GetCurlArgs(int type)
        {
            string host = (type == 0) ? localHost : mainHost;
            //return $"\"{host}/matches/{matchID}?token={token}\">{outputPath}/{outputJson}";
            return $"\"{GetUrl(type)}\">{outputPath}/{outputJson}";
        }

        /// <summary>
        /// Curl Postコマンド引数取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>Curlコマンド引数</returns>
        private string PostCurlArgs(int type)
        {
            string host = (type == 0) ? localHost : mainHost;
            //return $"\"{host}/matches/{matchID}?token={token}\">{outputPath}/{outputJson}";
            return $"\"{GetUrl(type)}\">{outputPath}/{outputJson}";
        }

        /// <summary>
        /// サーバーURL取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>接続先サーバーURL</returns>
        private string GetUrl(int type)
        {
            string host = (type == 0) ? localHost : mainHost;
            string http = (type == 0) ? "http://" : "https://";
            return $"{http}{host}/matches/{matchID}?token={token}";
        }

        /// <summary>
        /// Curlコマンド直接実行で取得(非推奨)
        /// </summary>
        private void CurlGetJson()
        {
            using (m_curlProcess = new Process()
                   {
                       StartInfo = new ProcessStartInfo(curl)
                       {
                           Arguments = GetCurlArgs(0),
                           Verb = "RunAs"
                       }
                   })
            {
                m_curlProcess.Start();
                m_curlProcess.WaitForExit();
                m_curlProcess.Close();
            }
        }

        /// <summary>
        /// Webリクエストでjson取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        public void WebRequestGetJson(int type)
        {
            try
            {
                WebRequest req = WebRequest.Create(GetUrl(type));
                WebResponse res = req.GetResponse();
                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                Stream st = res.GetResponseStream();
                StreamReader sr = new StreamReader(st, enc);
                string json = sr.ReadToEnd();
                //DebugEx.Log($"get:{json}");
                //jsonをmatchesクラスへ変換して格納
                nowMatches = KCP2023.Utility.MatchFromJson(json);
                DebugEx.ShowArrayLog(nowMatches.board.territories);

                sr.Close();
                st.Close();
            }
            catch (Exception e)
            {
                DebugEx.Log(e.Message);
                DebugEx.Log("サーバー接続拒否");
            }
        }

        /// <summary>
        /// .batファイルから実行して取得(UACの観点から非推奨)
        /// </summary>
        private void BatGetJson()
        {
            DebugEx.Log(getSampleServerBatPath + getSampleServerBat);
            using (m_curlProcess = new Process()
                   {
                       StartInfo = new ProcessStartInfo("C:/Users/futur/Desktop/KCP2023/server/bat/getSampleServer")
                       {
                           FileName = "C:/Users/futur/Desktop/KCP2023/server/bat/getSampleServer.bat",
                           Verb = "RunAs"
                       }
                   })
            {
                m_curlProcess.EnableRaisingEvents = true;
                m_curlProcess.Exited += (object sender, System.EventArgs e) => { DebugEx.Log("end"); };

                m_curlProcess.Start();
                m_curlProcess.WaitForExit();
                m_curlProcess.Close();
            }
        }

        /// <summary>
        /// バッチファイルから実行(推奨)
        /// </summary>
        private void BatPostJson()
        {
            DebugEx.Log(getSampleServerBatPath + getSampleServerBat);

            //Dictionary<int, int>[] test = new Dictionary<int, int>() { { 1, 1 }, { 2, 4 } };
            
            //デバッグ用コマンド群
            Command[] cmd = new[]
            {
                new Command { act = 2, dir = 4 },
                new Command { act = 2, dir = 4 } 
            };
            DebugEx.Log($"{Utility.EncodeCommandJson(cmd)}");

            using (m_curlProcess = new Process()
                   {
                       StartInfo = new ProcessStartInfo(
                           "C:/Users/futur/Desktop/KCP2023/server/bat/postSampleServer_args")
                       {
                           FileName = "C:/Users/futur/Desktop/KCP2023/server/bat/postSampleServer_args.bat",
                           //Arguments = "1 q3q1q6typeq6:2q5q6dirq6:4q2q5{q6typeq6:2q5q6dirq6:4q2q4",
                           Arguments = $"1 {Utility.EncodeCommandJson(cmd)}",

                           CreateNoWindow = false,
                           UseShellExecute = true
                           //Verb = "RunAs"
                       }
                   })
            {
                m_curlProcess.EnableRaisingEvents = true;
                //m_curlProcess.Exited += (object sender, System.EventArgs e) => { DebugEx.Log("end"); };
                m_curlProcess.Start();
                m_curlProcess.WaitForExit();
                m_curlProcess.Close();
            }
        }

        /// <summary>
        /// WebリクエストからJsonをPost非推奨
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        private void WebRequestPostJson(int type, string json)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(GetUrl(0));
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";

            using (var client = new HttpClient())
            {
                var result = client.PostAsync(
                    GetUrl(type),
                    new StringContent(json, Encoding.UTF8, "application/json"));
                DebugEx.Log(result.ToString());
            }

            DebugEx.Log("post");
        }

        private void Start()
        {
            StartCoroutine(AsyncUpdate());
        }

        private IEnumerator AsyncUpdate()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    WebRequestGetJson(hostType);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    BatPostJson();
                }

                yield return null;
            }
        }
    }
}