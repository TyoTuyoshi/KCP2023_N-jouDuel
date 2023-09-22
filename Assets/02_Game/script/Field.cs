using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCP2023
{
    /// <summary>
    /// フィールドのマップ単品
    /// ゲームフィールドはフィールドを繋げたもの
    /// </summary>
    public class Field : MonoBehaviour
    {
        //フィールドの状態
        public enum FieldState
        {
            Neutral, //中立地区
            AField, //Aの陣地
            BField, //Bの陣地
            Wall //城壁が置かれている
        }

        //フィールドのモデル
        public GameObject fieldModel = null;

        //フィールドのポイント
        public int point = 0;

        //フィールド上の状況
        public FieldState fieldState = FieldState.Neutral;

        //フィールドの座標
        public int posX = 0;
        public int posY = 0;
    }
}