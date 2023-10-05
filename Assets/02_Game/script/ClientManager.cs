using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using BaseSystem.Utility;
using Debug = System.Diagnostics.Debug;

namespace KCP2023
{
    public class ClientManager : SingletonBase<ClientManager>
    {
        /// <summary>
        /// GET:
        /// curl "localhost:3000/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521">debug.json
        /// POST:
        /// curl -i -X POST -H "Content-Type: application/json" -d "{\"turn\": 69,\"actions\": [{\"type\":2,\"dir\":4},{\"type\":2,\"dir\":4}]}" localhost:3000/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521
        /// </summary>
        private Process m_curlProcess = null;

        private Board m_board = new Board();

        [NonSerialized]public string mainUrl =
            "https://www.procon.gr.jp/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";

        public string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        //public const string url ="https://www.procon.gr.jp/matches/?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        //"curl \"localhost:3000/matches/10?token=\" > debug.json";

        /// <summary>
        /// CurlコマンドProcess群
        /// </summary>
        public const string curl = "curl.exe";
        public string mainHost = "https://www.procon.gr.jp";
        public string localHost = "localhost:3000";
        public int matchID = 10;
        public string outputPath = "C:/Users/futur/Desktop/KCP2023";
        public string outputJson = "state.json";
        
        //"C:\Users\futur\Desktop\KCP2023\getSampleServer.bat"
        public string getSampleServerBat = "/getSampleServer.bat";
        public string getSampleServerBatPath = "C:/Users/futur/Desktop/KCP2023/server/bat";

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
            using (m_curlProcess= new Process()
                   {
                       StartInfo = new ProcessStartInfo(curl)
                       {
                           Arguments = GetCurlArgs(0),
                           Verb = "RunAs"
                       }
                   })
            {
                m_curlProcess.Start();
                DebugEx.Log($"{curl} {GetCurlArgs(0)}");
                m_curlProcess.WaitForExit();
                m_curlProcess.Close();
            }
        }

        /// <summary>
        /// Webリクエストで取得(推奨)
        /// </summary>
        private void WebRequestGetJson()
        {
            WebRequest req = WebRequest.Create(GetUrl(0));
            WebResponse res = req.GetResponse();
            Encoding enc = Encoding.GetEncoding("Shift_JIS");

            try
            {
                Stream st = res.GetResponseStream();
            
                StreamReader sr = new StreamReader(st, enc);
                string html = sr.ReadToEnd();
                sr.Close();
                st.Close();
                DebugEx.Log(html);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// .batファイルから実行して取得(UACの観点から非推奨)
        /// </summary>
        private void RunGetBat()
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
                m_curlProcess.Exited += (object sender, System.EventArgs e) =>
                {
                    DebugEx.Log("end");
                };
                
                m_curlProcess.Start();
                m_curlProcess.WaitForExit();
                m_curlProcess.Close();
            }
        }

        private IEnumerator POSTJson()
        {
            yield return null;
        }

        private void Start()
        {
            WebRequestGetJson();
            StartCoroutine(AsyncUpdate());
        }

        private IEnumerator AsyncUpdate()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    WebRequestGetJson();
                }
                yield return null;
            }
        }

    }
}

