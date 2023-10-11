using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;

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
        
        public static T[,] ToTowDimensional<T>(T[] src, int width, int heigth)
        {
            var dest = new T[heigth, width];
            int len = width * heigth;
            len = src.Length < len ? src.Length : len;
            for (int y = 0, i = 0; y < heigth; y++)
            {
                for (int x = 0; x < width; x++, i++)
                {
                    if (i >= len)
                    {
                        return dest;
                    }
                    dest[y, x] = src[i];
                }
            }

            return dest;
        }

        /// <summary>
        /// 文字列から整数値を抽出して一次元配列で返す(正負の値を保障)
        /// </summary>
        /// <param name="source">抽出元の文字列</param>
        /// <returns>sourceから抽出された文字列の一次元配列</returns>
        public static int[] PuckStrToIntArray(string source)
        {
            //TODO 文字列内に数値がない場合は、からの配列を返す
            Func<string, string, string> RemoveStringDust = (string source, string dustSign) =>
            {
                foreach (var dust in dustSign) {
                    source = source.Replace(dust.ToString(), "");
                }
                return source;
            };
            var str_array = RemoveStringDust(source, "{[]}").Split(",");
            Array.Resize(ref str_array, str_array.Length - 1);
            var intArray =str_array.Select(int.Parse).ToArray();
            return intArray;
        }

        /// <summary>
        /// 文字列から正の整数値を抽出して一次元配列で返す(正の数のみ保障)
        /// </summary>
        /// <param name="source">抽出元の文字列</param>
        /// <returns>sourceから抽出された文字列の一次元配列</returns>
        public static int[] PuckStrToIntArrayEx(string source)
        {
            var intArray = Regex.Matches( source, "[0-9]+")
                .Cast<Match>()
                .Select( m => int.Parse( m.Value ) )
                .ToArray();

            //DebugEx.ShowArrayLog(intArray);
            //文字列から数値がない場合0の配列を返す
            if (intArray == null || intArray.Length == 0)
            {
                intArray = new[] { 0 };
            }
            //DebugEx.ShowArrayLog(intArray);

            return intArray;
        }

        public static T[,] ToTowDimensionalPrimitives<T>(T[] src, int width, int heigth)
        {
            var dest = new T[heigth, width];
            int len = width * heigth;
            len = src.Length < len ? src.Length : len;

            var size = Marshal.SizeOf(typeof(T));
            Buffer.BlockCopy(src, 0, dest, 0, len * size);
            return dest;
        }
    }
}