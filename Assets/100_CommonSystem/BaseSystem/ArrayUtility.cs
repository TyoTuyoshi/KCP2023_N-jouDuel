using System.Collections;
using System.Collections.Generic;

namespace BaseSystem.Utility
{
    /// <summary>
    /// 二次元リストや二次元配列の変換
    /// </summary>
    public sealed class ArrayUtility
    {
        public static void ListToTwoDim<T>()
        {
            //for (int i = 0; i < row; i++) {
            //    for (int j = 0; j < column; j++) {
            //        csvValue[i, j] = csvTable[i][j];
            //    }
            //}
            //return new T[,]{};
        }

        /// <summary>
        /// リスト内配列を二次元配列へコピーする。
        /// </summary>
        /// <param name="source">コピー元のリスト内配列</param>
        /// <param name="pasteArray">コピー先二次元配列</param>
        /// <typeparam name="T"></typeparam>
        public static void CopyListToTwoDim<T>(List<T[]> source, ref T[,] pasteArray)
        {
            int row = source.Count;
            int column = source[0].Length;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    pasteArray[i, j] = source[i][j];
                }
            }
        }
    }
}