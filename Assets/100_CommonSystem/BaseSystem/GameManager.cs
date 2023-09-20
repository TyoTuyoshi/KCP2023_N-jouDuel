using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using BaseSystem.Utility.File;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    //デフォルトのマップ
    enum Map {
        A11,A13,A15,A17,A21,A25,
        B11,B13,B15,B17,B21,B25,
        C11,C13,C15,C17,C21,C25,
        CUSTOM
    }

    public string csvDirectoryPath = "C:/Users/futur/Desktop/KCP2023/fielddata";
    public string csvFileName = "A11";
    
    public string csvFilePath {
        get { return $"{csvDirectoryPath}/{csvFileName}.csv"; }
    }

    public char[,] fieldData
    {
        get { return CSV.GetCharTable(csvFilePath); }
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


    private void Start()
    {
    }

    private void Update()
    {
        
    }
}
