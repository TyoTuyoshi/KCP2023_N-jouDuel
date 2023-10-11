using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KCP2023
{
    /// <summary>
    /// ゲームフィールドのデータを格納するクラスです。
    /// </summary>
    public class GameField : MonoBehaviour
    {
        /// <summary>
        /// 平地　　 : 0
        /// 池　　　 : 1
        /// 城　　　 : 2
        /// 先手職人 : a
        /// 後手職人 : b
        /// </summary>

        public string fieldName = "";

        public char fieldType = ' ';
        public int fieldSize = 0;
        public char[,] field = null;

        //public 
        //フィールドの先手職人
        public List<Mason> m_craftsmenA = new List<Mason>();

        //フィールドの後手職人
        public List<Mason> m_craftsmenB = new List<Mason>();

        private int r = 0;

        public Mason[] GetCraftsmen(int num)
        {
            return (num == 0) ? m_craftsmenA.ToArray() : m_craftsmenB.ToArray();
        }

        public static GameField ToGameField(char[,] data)
        {
            GameField gameField = new GameField();
            gameField.field = data;
            gameField.fieldSize = data.GetLength(0);
            gameField.r = 10;

            //m_craftsmenA.Clear();
            //m_craftsmenB.Clear();

            for (int i = 0; i < gameField.fieldSize; i++)
            {
                for (int j = 0; j < gameField.fieldSize; j++)
                {
                    switch (gameField.field[i, j])
                    {
                        //平地
                        case '0':
                            break;
                        //池
                        case '1':
                            break;
                        //城
                        case '2':
                            break;
                        //先手職人
                        case 'a':
                            Mason masonA = new Mason();
                            masonA.SetPos(i, j);
                            gameField.m_craftsmenA.Add(masonA);
                            break;
                        //後手職人
                        case 'b':
                            Mason masonB = new Mason();
                            masonB.SetPos(i, j);
                            gameField.m_craftsmenB.Add(masonB);
                            break;
                        default:
                            break;
                    }
                }
            }
            return gameField;
        }
    }
}