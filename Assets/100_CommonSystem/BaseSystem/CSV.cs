using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using KCP2023;
using UnityEngine;

namespace BaseSystem.Utility.File
{
    public class CSV
    {
        private string m_directoryPath = "";//読み込むディレクトリのパス
        private string m_filePath = ""; //読み込むファイルのパス

        public static int row = 0;//行
        public static int column = 0;//列
        
        public static string fileName = "";//ファイル名
        
        //private static StreamReader csvFile = null;//CSVファイル

        public static char[,] csvData = null;
        /// <summary>
        /// 読み込みファイルのパスを変更
        /// </summary>
        /// <param name="path"></param>
        public void SetFilePath(string path) { m_filePath = path; }

        public void LoadCSVFile()
        {
            
        }

        public void ReadCSVFile(string path, ref int[][] table)
        {
      
        }

        /// <summary>
        /// csvファイルからデータテーブルを取り出します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public static char[,] GetCharTable(string path, string name = null)
        {
            CSV c = new CSV();
            c.m_directoryPath = "deddedefe";
            try
            {
                //ファイル読み込み
                StreamReader csvFile = new StreamReader(@path);
                //ファイル名取得
                name ??= path.Substring(path.LastIndexOf("/") + 1);
                fileName = name;

                column = 0;
                row = 0;

                //読み取り保存用のcsvテーブル
                List<char[]> csvTable = new List<char[]>();

                //csvからデータ読み取り、テーブルに格納
                while (!csvFile.EndOfStream)
                {
                    string readData = csvFile.ReadLine().Replace(",", "");
                    char[] values = readData.ToCharArray();
                    row++;
                    column = values.Length;

                    csvTable.Add(values);
                }

                csvFile.Close();

                char[,] csvValue = new char[row, column];
                //戻り値用char二次元配列へデータをコピー
                ArrayUtility.CopyListToTwoDim(csvTable, ref csvValue);

                csvData = csvValue;
                return csvValue;
            }

            catch (Exception e)
            {
                Debug.LogError("ファイルが他のアプリケーションで編集中，もしくはアクセス権がありません。");
                throw;
            }
        }

        public static string[,] GetStringTable(string path)
        {
            //string[] lineValue = csvFile.ReadLine().Split(",");
            return new string[,] { };
        }


        public void GetTwoDimArray()
        {
            
        }

        public void ConvertTwoDimArray()
        {
            
        }

        /// <summary>
        /// CSVファイルのエクスポート
        /// </summary>
        /// <param name="exportPath"></param>
        public void ExportCSVFormat(string exportPath)
        {
            
        }

        /// <summary>
        /// CSVファイルの上書き
        /// </summary>
        /// <param name="filePath"></param>
        public void OverwriteCSVFormat(string filePath)
        {
            
        }
    }
}