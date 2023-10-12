using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KCP2023
{
    /// <summary>
    /// 試合状況取得APIのレスポンス格納用クラス
    /// </summary>
    [Serializable]
    public class Matches
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
       
        public int id; //対戦ID
        public int turn; //ターン数
        public Board board; //フィールドの状況
        public object logs; //試合ログ
    }

    [Serializable]
    public class MatchesInfo: Matches
    {
        //public int id; //対戦ID
        //public int turn; //ターン数
        //public Board board; //フィールドの状況
        //public object logs; //試合ログ
        [Serializable]
        public class Bonus
        {
            public int wall;
            public int territory;
            public int castle;
        }

        public int turns;
        public int turnSeconds;
        public Bonus bonus;
        public string opponent;
        public bool first;//true:先手　false:後手
    }
    
    /// <summary>
    /// コマンド
    /// </summary>
    public struct Command
    {
        public int actType { get; set; }
        public int dir { get; set; }
    }
}