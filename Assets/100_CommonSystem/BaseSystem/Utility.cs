using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BaseSystem.Utility;
using KCP2023;
using UnityEngine;

namespace KCP2023
{
    public static class Utility
    {
        /// <summary>
        /// stringのjsonを解析してMatchesクラスで返す
        /// </summary>
        /// <param name="json">変換元のjsonソース</param>
        /// <returns>jsonから変換したmatches</returns>
        public static KCP2023.Matches MatchFromJson(string json)
        {
            Matches matches = JsonUtility.FromJson<KCP2023.Matches>(json);

            int w = matches.board.width;
            int h = matches.board.height;
            //文字列データ切り出し
            string[] s = json.Split("\"");
            const int offset = 2;

            //ボードの情報取得
            Func<string, int, int, int[,]> GetBoardInfo = (string source, int w, int h) =>
            {
                return ArrayUtility.ToTowDimensional(
                    ArrayUtility.PuckStrToIntArray(source),
                    w, h);
            };
            const int idx_structure = 14;
            const int idx_masons = 16;
            const int idx_walls = 18;
            const int idx_territories = 20;
            //ボード情報を解析して取得
            matches.board.structures = GetBoardInfo(s[idx_structure].Substring(offset), w, h);
            matches.board.masons = GetBoardInfo(s[idx_masons].Substring(offset), w, h);
            matches.board.walls = GetBoardInfo(s[idx_walls].Substring(offset), w, h);
            matches.board.territories = GetBoardInfo(s[idx_territories].Substring(offset), w, h);
            return matches;
        }
        
        public static MatchesInfo MatchInfoFromJson(string json)
        {
            MatchesInfo matchesInfo = JsonUtility.FromJson<MatchesInfo>(json);

            int w = matchesInfo.matches.board.width;
            int h = matchesInfo.matches.board.height;
            //文字列データ切り出し
            string[] s = json.Split("\"");
            const int offset = 2;

            //int i = 0;
            //
            //foreach (var a in s)
            //{
            //    DebugEx.Log($"{i} {a}");
            //    i++;
            //}
            
            //ボードの情報取得
            Func<string, int, int, int[,]> GetBoardInfo = (string source, int w, int h) =>
            {
                return ArrayUtility.ToTowDimensional(
                    ArrayUtility.PuckStrToIntArray(source),
                    w, h);
            };
            const int idx_structure = 26;
            const int idx_masons = 28;
            //ボード情報を解析して取得
            matchesInfo.matches.board.structures = GetBoardInfo(s[idx_structure].Substring(offset), w, h);
            matchesInfo.matches.board.masons = GetBoardInfo(s[idx_masons].Substring(offset), w, h);
            return matchesInfo;
        }

        /// <summary>
        /// コマンド命令をjsonStringに変換して返す
        /// </summary>
        /// <param name="cmd">コマンド群</param>
        /// <returns>コマンド群のjsonエンコード</returns>
        public static string EncodeCommandJson(Command[] cmd)
        {
            //１番　２番　３番...
            //{type:2,dir:4},{type:2,dir:4}
            //cmd アクション0-3 方向0-7
            //例：　右上に移動 1 5
            string jsonCmd = "q3";
            for (int i = 0; i < cmd.Length; i++)
            {
                jsonCmd += $"q1q6typeq6:{cmd[i].actType}q5" +
                           $"q6dirq6:{cmd[i].dir}q2";
                if (i < cmd.Length - 1) jsonCmd += "q5";
            }

            jsonCmd += "q4";

            return jsonCmd;
        }

        public static bool isMainHost()
        {
            return ClientManager.Instance.hostType == 1;
        }

        public enum Level
        {
            Success,//成功
            Failed, //サーバー接続が成功しているがデータが変だったときとかの軽度エラー
            Error,  //重大エラー
            PopUp   //注目レベル
        }
    }
}