using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;

/// <summary>
/// Base生成のエントリ
/// </summary>
public class InitializeOnLoad : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Base.Create();
    }
}
