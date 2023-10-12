using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using BaseSystem.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace KCP2023
{
    public class MapCreator : SingletonBase<MapCreator>
    {
        private char[,] m_fieldData = new char[,] { };

        private GameObject[,] m_field = new GameObject[,] { };

        [SerializeField] private GameObject s;
        [SerializeField] private Vector3 m_offsetPos = new Vector3();

        void Start()
        {
            m_fieldData = GameManager.Instance.fieldData;
            SetGameField();
        }

        private List<GameObject> fieldObjs = new List<GameObject>();

        /// <summary>
        /// ゲームフィールドをシーンに反映させる。
        /// </summary>
        public void SetGameField()
        {
            //現状のフィールドを取得
            Board field = GameSceneManager.Instance.nowMatches.board;
            fieldObjs = new List<GameObject>();
            
            for (int i = 0; i < field.width; i++)
            {
                for (int j = 0; j < field.height; j++)
                {
                    GameObject chip = new GameObject(); 
                    if (m_fieldData[i, j] != '1')
                    {
                        chip = Instantiate(s);
                        chip.transform.position = m_offsetPos + new Vector3(i, 0, j);
                    }
                    fieldObjs.Add(chip);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}