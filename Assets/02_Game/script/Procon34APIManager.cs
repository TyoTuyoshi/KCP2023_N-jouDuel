using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KCP2023
{
    [Serializable]
    public class Log
    {
        [Serializable]
        public class Action
        {
            public bool succeeded;
            public int type;
            public int dir;
        }

        public int turn;
        public Action[] actions;
    }

    [Serializable]
    public class Board
    {
        public int width;
        public int height;
        public int mason;
        public int[,] structures;
        public int[,] masons;
        public int[,] walls;
        public int[,] territories;
    }
    
    [Serializable]
    public class Bonus
    {
        public int wall;
        public int territory;
        public int castle;
    }
    
    /// <summary>
    /// 試合状況取得APIのレスポンス格納用クラス
    /// </summary>
    [Serializable]
    public class Matches
    {
        public int id; //対戦ID
        public int turn; //ターン数
        public Board board; //フィールドの状況
        public object logs; //試合ログ
    }

    /// <summary>
    /// 試合開始前の試合一覧取得
    /// </summary>
    [Serializable]
    public class MatchesInfo
    {
        [Serializable]
        public class Matches
        {
            public int id; //対戦ID
            public int turns; //試合ターン数
            public int turnSeconds; //ターン時間
            public Bonus bonus; //ボーナス
            public Board board; //フィールド情報
            public string opponent; //相手の名前
            public bool first; //true:自チームが先手　false:自チームが後手
        }

        public Matches matches;
    }

    /// <summary>
    /// コマンド
    /// </summary>
    public struct Command
    {
        public int actType { get; set; }
        public int dir { get; set; }
    }

    
    /// <summary>
    /// 外部に保存しておく設定json
    /// </summary>
    [Serializable]
    public struct Config
    {
        /// <summary>
        /// ユーザー設定
        /// </summary>
        [Serializable]
        public struct UserConfig
        {
            public float volume; //音量
            public string csvLoadPath; //csvの読み込み元のパス
            public string displayResolution; //表示モニターの解像度
            public string appResolution; //アプリの解像度(WXGA)
            public bool appFullDisplay; //フルウィンドウ設定フラグ(ウィンドウ)
        }

        /// <summary>
        /// クライアント(通信する際)の設定
        /// 書き換え保存が可能
        /// </summary>
        [Serializable]
        public struct ClientConfig
        {
            public string header; //ヘッダ
            public string token; //トークン
            public string name; //チーム名
            [FormerlySerializedAs("connectHost")] public int hostType; //接続先ホスト

            public float getIntervalSec; //GETリクエスト頻度(sec)
            public float postIntervalSec; //POSTリクエスト頻度(sec)

            public string mainHost; //本選サーバー
            public string localHost; //ローカルホスト

            public string postBatchPath; //postするbatファイルのパス
            public string postBatchName; //postするbatファイルの名前
            public string localMatchesJsonPath;//ローカルホストデバッグ用のjsonファイルの参照パス
        }
        public UserConfig user;
        public ClientConfig client;
    }


}