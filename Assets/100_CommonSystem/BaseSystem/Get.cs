using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BaseSystem.Utility;
using KCP2023;
using UnityEngine;

namespace KCP2023.Utility
{
    public static class Get
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
            const int idx_structure = 14; const int idx_masons = 16;
            const int idx_walls = 18; const int idx_territories = 20;
            //ボード情報を解析して取得
            matches.board.structures = GetBoardInfo(s[idx_structure].Substring(offset), w, h);
            matches.board.masons = GetBoardInfo(s[idx_masons].Substring(offset), w, h);
            matches.board.walls = GetBoardInfo(s[idx_walls].Substring(offset), w, h);
            matches.board.territories = GetBoardInfo(s[idx_territories].Substring(offset), w, h);
            return matches;
        }
    }
}