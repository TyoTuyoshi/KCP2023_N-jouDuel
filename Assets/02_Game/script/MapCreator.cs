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

        [SerializeField] private Vector3 m_offsetPos = new Vector3();

        void Start()
        {
            m_fieldData = GameManager.Instance.fieldData;
            //SetGameField();
        }

        /// <summary>
        /// 第零層フィールドのプレハブリスト
        /// 0:平地
        /// 1:池
        /// 2:城
        /// </summary>
        public List<GameObject> Layer0FieldChips = new List<GameObject>();
        /// <summary>
        /// 第一層フィールドのプレハブリスト
        /// 0:城壁
        /// 1:職人
        /// </summary>
        public List<GameObject> Layer1FieldChips = new List<GameObject>();

        //フィールドを形成するチップのリスト
        private List<GameObject> m_fieldObjs = new List<GameObject>();

        //陣地の配色
        private Color[] territoriesColor = new[]
        {
            Color.red,      //自チームの陣地
            Color.white,    //中立
            Color.blue,     //相手チームの陣地
            Color.yellow,   //両チームの陣地
        };

        //城壁の配色
        private Color[] wallColor = new[]
        {
            Color.white,//自チームの色
            Color.black //相手チームの色
        };
        
        /// <summary>
        /// ゲームフィールドをシーンに反映させる。
        /// </summary>
        public void SetGameFieldNow()
        {
            //現状のフィールドを取得
            Board field = GameSceneManager.Instance.nowMatches.board;
            //DebugEx.Log(matchesInfo);

            ClearFieldObjs();
            m_fieldObjs = new List<GameObject>();
            for (int i = 0; i < field.width; i++)
            {
                for (int j = 0; j < field.height; j++)
                {
                    //プレハブのインデックスに代用
                    int index = field.structures[i, j];
                    GameObject chip = Instantiate(Layer0FieldChips[index]) as GameObject;
                    //並べる
                    SetPosition(ref chip, i, j);
                    //陣地に着色
                    int color = field.territories[i, j];
                    SetTerritoriesColor(ref chip, color);

                    //GameObject chipOver = Instantiate(Layer1FieldChips[index]);

                    //chip.transform.position = m_offsetPos + new Vector3(i, 0, j);
                    m_fieldObjs.Add(chip);
                }
            }
        }
        
        /// <summary>
        /// ゲームフィールドをシーンに反映させる。(初期化)
        /// </summary>
        public void SetGameFieldInit()
        {
            //nullリターン
            if (GameSceneManager.Instance.matchesInfo.matches.board == null)
            {
                GameSceneManager.Instance.ShowLogMessage($"フィールド情報取得不能", Utility.Level.Error);
                return;
            }
            //現状のフィールドを取得
            Board field = GameSceneManager.Instance.matchesInfo.matches.board;
            //DebugEx.Log(matchesInfo);

            ClearFieldObjs();
            m_fieldObjs = new List<GameObject>();
            for (int i = 0; i < field.width; i++)
            {
                for (int j = 0; j < field.height; j++)
                {
                    //第零層
                    //プレハブのインデックスに代用
                    int index = field.structures[i, j];
                    GameObject chip = Instantiate(Layer0FieldChips[index]) as GameObject;
                    //地形を並べる
                    SetPosition(ref chip, i, j,0);

                    //第一層のインデックス
                    int index_layer1 = field.masons[i, j];
                    if (index_layer1 != 0)
                    {
                        //職人の配置
                        GameObject chipOver = Instantiate(Layer1FieldChips[0]);
                        
                        
                        SetPosition(ref chipOver, i, j,1);
                    }

                    //chip.transform.position = m_offsetPos + new Vector3(i, 0, j);
                    m_fieldObjs.Add(chip);
                }
            }

            GameSceneManager.Instance.ShowLogMessage($"フィールド配置完了");
        }

        /// <summary>
        /// フィールドチップのリストの破棄と初期化
        /// </summary>
        private void ClearFieldObjs()
        {
            GameSceneManager.Instance.ShowLogMessage("マップリスト開放", Utility.Level.PopUp);
            if (m_fieldObjs.Count == 0 || m_fieldObjs == null) return;
            foreach (var chip in m_fieldObjs) Destroy(chip);
            m_fieldObjs.Clear();
        }

        private void SetPosition(ref GameObject obj, float x, float z, float y=0.0f)
        {
            obj.transform.position = m_offsetPos + new Vector3(x, y, z);
        }
        
        private void SetTerritoriesColor(ref GameObject obj,int type)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.material.SetColor("_Color", territoriesColor[type]);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}