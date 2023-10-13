using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseSystem;
using BaseSystem.Utility;
using BaseSystem.Utility.File;
using UnityEngine;

namespace KCP2023
{
    public class GameManager : SingletonBase<GameManager>
    {
        public string csvDirectoryPath = "C:/Users/futur/Desktop/KCP2023/fielddata";
        public string csvFileName = "A11";

        public string csvFilePath
        {
            get { return $"{csvDirectoryPath}/{csvFileName}.csv"; }
        }

        public char[,] fieldData
        {
            get { return CSV.GetCharTable(csvFilePath); }
        }

        public GameField filedData_KAI
        {
            get { return new GameField(); }
        }

        //フィールド名
        public string fieldName
        {
            get { return csvFileName; }
        }

        //フィールドタイプ(A,B,C)
        public char fileType
        {
            get { return csvFileName[0]; }
        }

        //フィールドサイズ(11,13,15,17,21,25)
        public int fieldSize
        {
            get { return int.Parse(csvFileName.Substring(1, 2)); }
        }

        public int fieldSizeIndex
        {
            get
            {
                int[] fsize = { 11, 13, 15, 17, 21, 25 };
                return Array.IndexOf(fsize, fieldSize);
            }
        }
        
        private List<Mason> playAbleCraftsmen = new List<Mason>();

        public List<Mason> GetPlayAbleCraftsmen()
        {
            return playAbleCraftsmen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="craftsmen"></param>
        public void SetPlayAbleCraftsmen(Mason[] craftsmen)
        {
            playAbleCraftsmen = craftsmen.ToList();
        }

        public void SetPlayAbleCraftsmen(List<Mason> craftsmen)
        {
            playAbleCraftsmen = craftsmen;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        //設定jsonファイルのパス
        public string configJsonPath = "C:/Users/futur/Desktop/KCP2023/config/config.json";

        //簡易デバッグ
        public string JsonPath = "C:/config/config.json";
        
        public Config gameConfig = new Config();
        /// <summary>
        /// 設定jsonの読み取り
        /// </summary>
        /// <returns>true:成功 false:失敗</returns>
        private bool ReadConfigJson()
        {
            try
            {
                StreamReader configJson = new StreamReader(configJsonPath);
                string configStr = configJson.ReadToEnd();
                gameConfig = JsonUtility.FromJson<Config>(configStr);
                DebugEx.Log(configStr);
                DebugEx.Log(gameConfig.client.name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
                throw;
            }
            return true;
        }

        private bool GetMatchesInfoJson()
        {
            StreamReader configJson = new StreamReader(JsonPath);
            string configStr = configJson.ReadToEnd();
            //MatchesInfo matchesInfo = JsonUtility.FromJson<MatchesInfo>(configStr);
            MatchesInfo matchesInfo = Utility.MatchInfoFromJson(configStr);
            //DebugEx.Log(configStr);
            //DebugEx.Log(matchesInfo.matches.bonus.wall);
            //DebugEx.ShowArrayLog(matchesInfo.matches.board.masons);
            return true;
        }

        protected override void Init()
        {
            Base.Instance.AddToBase(this);
            FadeManager.Create();

            //設定ファイルの読み取り
            ReadConfigJson();
            //OptionManager.Create();
        }
    }
}