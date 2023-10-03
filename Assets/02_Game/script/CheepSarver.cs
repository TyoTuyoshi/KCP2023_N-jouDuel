using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using System.Diagnostics;
using BaseSystem.Utility;
using Debug = System.Diagnostics.Debug;

namespace KCP2023
{
    public class CheepSarver : SingletonBase<CheepSarver>
    {
        /// <summary>
        /// GET:
        /// curl "localhost:3000/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521">debug.json
        /// POST:
        /// curl -i -X POST -H "Content-Type: application/json" -d "{\"turn\": 69,\"actions\": [{\"type\":2,\"dir\":4},{\"type\":2,\"dir\":4}]}" localhost:3000/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521
        /// </summary>
        private Process m_curlProcess = null;

        private Board m_board = new Board();

        public const string url =
            "https://www.procon.gr.jp/matches/10?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";

        public string token = "takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        //public const string url ="https://www.procon.gr.jp/matches/?token=takuma65a0e120ec5279be88be37b54b40a9dd5a364a0b8113cd36aa99c05521";
        //"curl \"localhost:3000/matches/10?token=\" > debug.json";

        public const string curl = "C:/Windows/System32/curl.exe";
        public string localHost = "localhost:3000";
        public string mainHost = "https://www.procon.gr.jp";

        public int matchID = 10;
        public string outputPath = "C:/Users/futur/Desktop/KCP2023";
        public string outputJson = "state.json";
        
        /// <summary>
        /// Curlコマンド引数取得
        /// </summary>
        /// <param name="type">0:ローカルホスト　1:本選サーバー</param>
        /// <returns>Curlコマンド引数</returns>
        public string GetCurlArgs(int type)
        {
            string host = (type == 0) ? localHost : mainHost;
            return $" \"{host}/matches/{matchID}?token = {token}\" > {outputPath}/{outputJson}";
        }

        public void GETJson()
        {
            m_curlProcess = new Process();

            using (var process = new Process()
                   {
                       StartInfo = new ProcessStartInfo(curl)
                       {
                           UseShellExecute = false,
                           RedirectStandardOutput = false,
                           Arguments = GetCurlArgs(0)
                       }
                   })
            {
                process.Start();
                DebugEx.Log($"{curl} {GetCurlArgs(0)}");
                //process.WaitForExit();
            }
        }

        private void Start()
        {
            GETJson();
        }
    }
}