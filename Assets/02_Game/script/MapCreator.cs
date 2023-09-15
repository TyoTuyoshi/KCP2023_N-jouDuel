using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using BaseSystem.Utility;
using UnityEngine;

[System.Serializable]
public class MapChip
{
    public enum State
    {
        Plain,
        Catstle,
        Pond
    }

    public State fieldState = State.Plain;
    private int raw = 0;
    private int column = 0;
}

public class MapCreator : SingletonBase<MapCreator>
{
    private char[,] m_fieldData = new char[,]{};

    private MapChip[,] m_field = new MapChip[,]{};
    
    [SerializeField] private MapChip s;
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
                //MapChip chip = Instantiate(s) as MapChip;
                //chip.SetPosition(i, 0, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
