using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int m_hp = 100;
    public int Power = 10;

    //宣言したクラスのメンバ
    private Enemy m_enemy = null;

    void Start()
    {
        m_enemy = new Enemy();
        m_enemy.m_hp = 150; //privateな値
        m_enemy.Power = 100;//publicな値
    }
}