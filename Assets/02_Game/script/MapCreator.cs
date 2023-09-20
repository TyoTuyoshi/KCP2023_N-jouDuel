using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using BaseSystem.Utility;
using UnityEngine;
using UnityEngine.Serialization;

public class MapCreator : SingletonBase<MapCreator>
{
    private char[,] m_fieldData = new char[,]{};

    private GameObject[,] m_field = new GameObject[,]{};
    
    [SerializeField] private GameObject s;
    [SerializeField] private Vector3 m_offsetPos = new Vector3();
    void Start()
    {
        m_fieldData = GameManager.Instance.fieldData;
        SetGameField();
        //m_fieldData = GameManager.Instance.fieldData;
    }

    private void SetGameField()
    {
        for (int i = 0; i < m_fieldData.GetLength(0); i++)
        {
            for (int j = 0; j < m_fieldData.GetLength(1); j++)
            {
                //城、平地の場合
                if (m_fieldData[i, j] != '1') 
                {
                    GameObject chip = Instantiate(s);
                    chip.transform.position = m_offsetPos + new Vector3(i, 0, j);
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
