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

        /// 本選
        /// GET: curl 172.28.0.1:8080/matches?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521 -o test.json
        /// "http://172.28.0.1:8080/matches?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521"
        /// "http://172.28.0.1:8080/matches/ID?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521"

        /// "http://172.28.0.1:8080/matches?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521"
        private Process m_curlProcess = null;

        public const string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";

        /// <summary>
        /// TODO:修正
        /// </summary>
        [NonSerialized] public string mainHost = "172.28.0.1:8080";
        [NonSerialized] public string localHost = "localhost:3000";
        //試合ID
        [NonSerialized] public int matchesID;
        //[NonSerialized] public string outputPath = "C:/Users/futur/Desktop/KCP2023";
        //[NonSerialized] public string outputJson = "state.json";

        /// <summary>
        /// .batファイル群
        /// </summary>
        [NonSerialized] public string getSampleServerBat = "/getSampleServer.bat";

        [NonSerialized] public string getSampleServerBatPath = "C:/Users/futur/Desktop/KCP2023/server/bat";
        
        //getリクエスト周期(sec)
        private float m_getIntervalSec = 0.2f;
        //周期カウンタ
        private float m_getIntervalCnt = 0.0f;

        //POST可能判定フラグ
        public bool ablePost = false;
        //GET成功フラグ
        public bool isStart = false;
        //終了フラグ
        public bool isEnd = false;

        /// <summary>
        /// サーバーURL取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <param name="isInfo">試合開始前の情報を要求しているか　デフォルト:false</param>
        /// <returns>接続先のURL</returns>
        private string GetUrl(int type)
        {
            string host = (type == 0) ? localHost : mainHost;
            //初期状態を要求/現在の状態を要求
            return $"http://{host}/matches/{matchesID}?token={token}";
        }

        /// <summary>
        /// JSONファイルの取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>true:取得に成功した false:取得に失敗した</returns>
        private string GetJson(int type)
        {
            WebRequest req = WebRequest.Create(GetUrl(type));
            WebResponse res = req.GetResponse();
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream st = res.GetResponseStream();
            StreamReader sr = new StreamReader(st, enc);
            //レスポンスログ表示
            string json = sr.ReadToEnd();
            //GameSceneManager.Instance.ShowLogMessage(res.ToString());
            sr.Close();
            st.Close();
            //DebugEx.Log(json);

            return json;
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
                //jsonをmatchesクラスへ変換して試合状況更新
                GameSceneManager.Instance.nowMatches = Utility.MatchFromJson(GetJson(type));
                GameSceneManager.Instance.ShowLogMessage("試合状況が取得成功");
                //初回に接続成功フラグを立てる
                if (!isStart) isStart = true;
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
        /// バッチファイルから実行(推奨)
        /// </summary>
        public bool PostCommandJson(int turn, Command[] cmd)
        {
            string batPath = GameManager.Instance.gameConfig.client.postBatchPath;
            using (m_curlProcess = new Process()
                   {
                       StartInfo = new ProcessStartInfo(batPath)
                       {
                           FileName = batPath,
                           Arguments = $"{turn} {Utility.EncodeCommandJson(cmd)} {matchesID} {hostType}",
                           CreateNoWindow = true,
                           UseShellExecute = false
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

        /// <summary>
        /// Start()以前に実行される
        /// config.jsonパラメータの設定
        /// </summary>
        protected override void Init()
        {
            //ホストタイプ 0:ローカルホスト 1:本選サーバー
            hostType = GameManager.Instance.gameConfig.client.connectHost;
            //GET更新頻度設定
            m_getIntervalSec = GameManager.Instance.gameConfig.client.getIntervalSec;
            //ID取得
            //matchID = 112;
            //TODO 修正
            mainHost = GameManager.Instance.gameConfig.client.mainHost;
            localHost = GameManager.Instance.gameConfig.client.localHost;
            matchesID = GameManager.Instance.gameConfig.nowMatches.id;

            //DebugEx.Log(m_getIntervalSec);
        }

        private void Start()
        {
            //DebugEx.Log(GetUrl(hostType,true));

            //GetMatchesInfoJson(1);
            //非同期アップデート
            StartCoroutine(AsyncUpdate());
        }
        
        /// <summary>
        /// 非同期アップデート
        /// </summary>
        /// <returns></returns>
        private IEnumerator AsyncUpdate()
        {
            //スペースキーを押して開始
            while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
            GameSceneManager.Instance.ShowLogMessage($"起動完了：接続プロセス開始", Utility.Level.PopUp);
            //float t = 0.0f;
            //const float wait_t = 5.0f;
            //開始前の試合情報を要求
            //while (!GetMatchesInfoJson(hostType))
            //{
            //    //時間超過時にエラーログ表示
            //    t += Time.deltaTime;
            //    if (t > wait_t)
            //    {
            //        GameSceneManager.Instance.ShowLogMessage($"エラー：接続待機時間超過", Utility.Level.Error);
            //    }
            //    yield return null;
            //} 
            //接続後に初期マップの配置
            //MapCreator.Instance.SetGameFieldInit();
            GameSceneManager.Instance.ShowLogMessage($"接続完了：プロセス開始", Utility.Level.PopUp);

            //POST可フラグ成立
            ablePost = true;

            //終了待ち
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
                //if (GameSceneManager.Instance.nowMatches.turn
                //    == GameSceneManager.Instance.matchesInfo.matches.turns)
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

            DebugEx.Log("matches end!");
            GameSceneManager.Instance.ShowLogMessage("Matches END!", Utility.Level.Failed);
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