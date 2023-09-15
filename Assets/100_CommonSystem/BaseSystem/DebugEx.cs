using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem.Utility
{
    /// <summary>
    /// たいそうな名前のDebugLogの拡張
    /// </summary>
    public sealed class DebugEx : Debug
    {
        public static void ShowArrayLog<T>(T[] array)
        {
            Log(string.Join(",", array));
        }

        public static void ShowArrayLog<T>(List<T> array)
        {
            Log(string.Join(",", array));
        }

        public static void ShowArrayLog<T>(T[,] array)
        {
            int row = array.GetLength(0);
            int colum = array.GetLength(1);
            Log($"row:{row} colum:{colum}");

            for (int i = 0; i < row; i++)
            {
                List<T> line = new List<T>();
                for (int j = 0; j < colum; j++)
                {
                    line.Add(array[i, j]);
                }

                ShowArrayLog(line);
            }
        }

        public static void ShowGameView<T>(T t)
        {

        }
    }
}