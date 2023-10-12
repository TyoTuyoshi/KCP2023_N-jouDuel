using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KCP2023;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BaseSystem.Utility
{
    /// <summary>
    /// たいそうな名前のDebugLogの拡張
    /// </summary>
    public sealed class DebugEx : Debug
    {
        /// <summary>
        /// セグメントのタイプを返す
        /// </summary>
        /// <param name="type">0:スペース 1:カンマ 2:アンダーバー 3:アスタリスク</param>
        /// <returns>セグメント</returns>
        private static string SegmentType(int type)
        {
            string seg = " ,_*";
            type = type > seg.Length - 1 ? 0 : type;
            return seg[type].ToString();
        }

        public static void ShowArrayLog<T>(T[] array,int type=0)
        {
            Log(string.Join(SegmentType(type), array));
        }

        public static void ShowArrayLog<T>(List<T> array,int type=0)
        { 
            Log(string.Join(SegmentType(type), array));
        }

        /// <summary>
        /// 二次元配列のログ表示
        /// </summary>
        /// <param name="array">表示対象</param>
        /// <typeparam name="T"></typeparam>
        public static void ShowArrayLog<T>(T[,] array,int type=0)
        {
            int row = array.GetLength(0);
            int colum = array.GetLength(1);
            string message = $"row:{row} colum:{colum}\n";
            //TODO:改行挿入 
            for (int i = 0; i < row; i++)
            {
                List<T> line = new List<T>();
                for (int j = 0; j < colum; j++)
                {
                    line.Add(array[i, j]);
                }
                message += String.Join(SegmentType(type), line) + "\n";
                //ShowArrayLog(line,type);
            }
            Log(message);
        }

        public static void ShowGameView<T>(T t)
        {

        }
    }
}