using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseSystem;
using BaseSystem.Utility.File;
using KCP2023;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public string csvDirectoryPath = "C:/Users/futur/Desktop/KCP2023/fielddata";
    public string csvFileName = "A11";
    
    public string csvFilePath {
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
    public string fieldName { get { return csvFileName; } }
    //フィールドタイプ(A,B,C)
    public char fileType { get { return csvFileName[0]; } }
    //フィールドサイズ(11,13,15,17,21,25)
    public int fieldSize {
        get { return int.Parse(csvFileName.Substring(1, 2)); }
    }
    public int fieldSizeIndex {
        get
        {
            int[] fsize = { 11, 13, 15, 17, 21, 25 };
            return Array.IndexOf(fsize, fieldSize);
        }
    }

    
    private List<Craftsman> playAbleCraftsmen = new List<Craftsman>();
    public List<Craftsman> GetPlayAbleCraftsmen() { return playAbleCraftsmen; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="craftsmen"></param>
    public void SetPlayAbleCraftsmen(Craftsman[] craftsmen) { playAbleCraftsmen = craftsmen.ToList(); }
    public void SetPlayAbleCraftsmen(List<Craftsman> craftsmen) { playAbleCraftsmen = craftsmen; }
    
    private void Start()
    {
    }

    private void Update()
    {
        
    }
}
