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
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace KCP2023
{
    /// <summary>
    /// クライアントマネージャー
    /// 一定時間ごとに試合状況をGETする。
    /// サーバーとのGET/POSTのやり取りの専門クラス
    /// POSTはGameSceneManagerのPostCommandData()から行う
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

        [NonSerialized] public string mainHost = "www.procon.gr.jp";
        [NonSerialized] public string localHost = "localhost:3000";
        [NonSerialized] public int matchID = 10;
        //[NonSerialized] public string outputPath = "C:/Users/futur/Desktop/KCP2023";
        //[NonSerialized] public string outputJson = "state.json";

        /// <summary>
        /// .batファイル群
        /// </summary>
        [NonSerialized] public string getSampleServerBat = "/getSampleServer.bat";

        [NonSerialized] public string getSampleServerBatPath = "C:/Users/futur/Desktop/KCP2023/server/bat";

        /// <summary>
        /// サーバーURL取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <param name="isInfo">試合開始前の情報を要求しているか　デフォルト:false</param>
        /// <returns>接続先のURL</returns>
        private string GetUrl(int type, bool isInfo = false)
        {
            string http = (type == 0) ? "http://" : "https://";
            string host = (type == 0) ? localHost : mainHost;
            return isInfo
                ? $"{http}{host}/matches?token={token}"
                : $"{http}{host}/matches/{matchID}?token={token}";
        }

        /// <summary>
        /// Webリクエストでjson取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>true:取得に成功した false:取得に失敗した</returns>
        public bool GetMatchesJson(int type)
        {
            try
            {
                WebRequest req = WebRequest.Create(GetUrl(type));
                WebResponse res = req.GetResponse();
                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                Stream st = res.GetResponseStream();
                StreamReader sr = new StreamReader(st, enc);
                string json = sr.ReadToEnd();

                //jsonをmatchesクラスへ変換して試合状況更新
                GameSceneManager.Instance.nowMatches = KCP2023.Utility.MatchFromJson(json);
                //DebugEx.ShowArrayLog(GameSceneManager.Instance.nowMatches.board.masons);
                //DebugEx.Log(GameSceneManager.Instance.nowMatches.turn);
                sr.Close();
                st.Close();
                GameSceneManager.Instance.ShowLogMessage("試合状況が取得成功");
                return true;
            }
            catch (Exception e)
            {
                DebugEx.Log(e.Message);
                DebugEx.Log("サーバー接続拒否");
                GameSceneManager.Instance.ShowLogMessage("試合状況が取得失敗", Utility.Level.Error);
                return false;
            }
        }

        /// <summary>
        /// 試合開始前に取得する試合情報
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>true:取得に成功した false:取得に失敗した</returns>
        public bool GetMatchesInfoJson(int type)
        {
            try
            {
                //試合前の状態を要求
                WebRequest req = WebRequest.Create(GetUrl(type, true));
                WebResponse res = req.GetResponse();
                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                Stream st = res.GetResponseStream();
                StreamReader sr = new StreamReader(st, enc);
                string json = sr.ReadToEnd();

                //jsonをMatchesInfoクラスへ変換して試合情報取得
                GameSceneManager.Instance.matchesInfo = Utility.MatchInfoFromJson(json);
                sr.Close();
                st.Close();
                GameSceneManager.Instance.ShowLogMessage("試合情報が取得成功");
                return true;
            }
            catch (Exception e)
            {
                DebugEx.Log(e.Message);
                DebugEx.Log("サーバー接続拒否");
                GameSceneManager.Instance.ShowLogMessage("試合情報が取得失敗", Utility.Level.Error);
                return false;
            }
        }

        /// <summary>
        /// バッチファイルから実行(推奨)
        /// </summary>
        public bool PostCommandJson(int turn, Command[] cmd)
        {
            //DebugEx.Log(getSampleServerBatPath + getSampleServerBat);
            //Dictionary<int, int>[] test = new Dictionary<int, int>() { { 1, 1 }, { 2, 4 } };
            DebugEx.Log($"POST called : {Utility.EncodeCommandJson(cmd)}");

            using (m_curlProcess = new Process()
                   {
                       StartInfo = new ProcessStartInfo(
                           "C:/Users/futur/Desktop/KCP2023/server/bat/postSampleServer_args")
                       {
                           FileName = "C:/Users/futur/Desktop/KCP2023/server/bat/postSampleServer_args.bat",
                           Arguments = $"{turn} {Utility.EncodeCommandJson(cmd)}",
                           CreateNoWindow = true,
                           UseShellExecute = false
                           //Verb = "RunAs"
                       }
                   })
            {
                m_curlProcess.EnableRaisingEvents = true;
                //m_curlProcess.Exited += (object sender, System.EventArgs e) => { DebugEx.Log("end"); };
                try
                {
                    m_curlProcess.Start();
                }
                catch (Exception e)
                {
                    DebugEx.Log(e);
                    return false;
                }
                finally
                {
                    m_curlProcess.WaitForExit();
                    m_curlProcess.Close();
                }

                return true;
            }
        }

        private void Start()
        {
            StartCoroutine(AsyncUpdate());
        }

        private float m_getIntervalSec = 0.2f;
        private float m_getIntervalCnt = 0.0f;

        public bool ablePost = false;
        public bool isEnd = false;

        private IEnumerator AsyncUpdate()
        {
            //スペースキーを押して開始
            while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
            //開始前の試合情報を要求(本選のみ)
            while (hostType == 1 && !GetMatchesInfoJson(hostType)) yield return null;

            ablePost = true;

            while (!isEnd)
            {
                //一定時間ごとにGETリクエスト
                if (m_getIntervalCnt > m_getIntervalSec)
                {
                    m_getIntervalCnt = 0.0f;
                    GetMatchesJson(hostType);
                }
                m_getIntervalCnt += Time.deltaTime;

                //本選のみ上限ターン到達時に終了フラグ
                if (GameSceneManager.Instance.nowMatches.turn
                    == GameSceneManager.Instance.matchesInfo.matches.turns
                    && hostType == 1)
                {
                    isEnd = true;
                }

                //デバッグ用2ターンで終了フラグ
                //if (GameSceneManager.Instance.nowMatches.turn == 2)
                //{
                //    isEnd = true;
                //}

                //Aキー強制GETリクエスト
                if (Input.GetKeyDown(KeyCode.A))
                {
                    GetMatchesJson(hostType);
                }

                yield return null;
            }

            DebugEx.Log("end");
        }
        /// <summary>
        /// WebリクエストからJsonをPost非推奨
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        //private void WebRequestPostJson(int type, string json)
        //{
        //    var httpRequest = (HttpWebRequest)WebRequest.Create(GetUrl(0));
        //    httpRequest.Method = "POST";
        //    httpRequest.Accept = "application/json";
        //    httpRequest.ContentType = "application/json";
        //    using (var client = new HttpClient())
        //    {
        //        var result = client.PostAsync(
        //            GetUrl(type),
        //            new StringContent(json, Encoding.UTF8, "application/json"));
        //        DebugEx.Log(result.ToString());
        //    }
        //    DebugEx.Log("post");
        //}
        
        /// <summary>
        /// .batファイルから実行して取得(UACの観点から非推奨)
        /// </summary>
        //private void BatGetJson()
        //{
        //    DebugEx.Log(getSampleServerBatPath + getSampleServerBat);
        //    using (m_curlProcess = new Process()
        //           {
        //               StartInfo = new ProcessStartInfo("C:/Users/futur/Desktop/KCP2023/server/bat/getSampleServer")
        //               {
        //                   FileName = "C:/Users/futur/Desktop/KCP2023/server/bat/getSampleServer.bat",
        //                   Verb = "RunAs"
        //               }
        //           })
        //    {
        //        m_curlProcess.EnableRaisingEvents = true;
        //        m_curlProcess.Exited += (object sender, System.EventArgs e) => { DebugEx.Log("end"); };
        //        m_curlProcess.Start();
        //        m_curlProcess.WaitForExit();
        //        m_curlProcess.Close();
        //    }
        //}
        
        /// <summary>
        /// Curlコマンド直接実行で取得(非推奨)
        /// </summary>
        //private void CurlGetJson()
        //{
        //    using (m_curlProcess = new Process()
        //           {
        //               StartInfo = new ProcessStartInfo(curl)
        //               {
        //                   Arguments = GetCurlArgs(0),
        //                   Verb = "RunAs"
        //               }
        //           })
        //    {
        //        m_curlProcess.Start();
        //        m_curlProcess.WaitForExit();
        //        m_curlProcess.Close();
        //    }
        //}
        
        /// <summary>
        /// Curlコマンド引数取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>Curlコマンド引数</returns>
        //private string GetCurlArgs(int type)
        //{
        //    string host = (type == 0) ? localHost : mainHost;
        //    //return $"\"{host}/matches/{matchID}?token={token}\">{outputPath}/{outputJson}";
        //    return $"\"{GetUrl(type)}\">{outputPath}/{outputJson}";
        //}

        /// <summary>
        /// Curl Postコマンド引数取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>Curlコマンド引数</returns>
        //private string PostCurlArgs(int type)
        //{
        //    string host = (type == 0) ? localHost : mainHost;
        //    //return $"\"{host}/matches/{matchID}?token={token}\">{outputPath}/{outputJson}";
        //    return $"\"{GetUrl(type)}\">{outputPath}/{outputJson}";
        //}
    }
}