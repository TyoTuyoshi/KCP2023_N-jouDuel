using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseSystem;
using BaseSystem.Utility;
using BaseSystem.Utility.File;
using UnityEngine;
using UnityEngine.Serialization;

namespace KCP2023
{
    public class GameManager : SingletonBase<GameManager>
    {
        //設定jsonファイルのパス
        public string configJsonPath = "C:/config/config.json";

        //簡易デバッグ用の試合Jsonのパス
        //public string LocalMatchesJsonPath = "C:/Users/futur/Desktop/KCP2023/server/json/getMatchesInfo.json";
        
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
                //DebugEx.Log(gameConfig.client.name);
            }
            catch (Exception e)
            {
                GameSceneManager.Instance.ShowLogMessage("設定ファイル読み取り不能", Utility.Level.Error);
                return false;
                throw;
            }
            return true;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        protected override void Init()
        {
            Base.Instance.AddToBase(this);
            FadeManager.Create();

            //設定ファイルの読み取り
            ReadConfigJson();
        }
    }
}