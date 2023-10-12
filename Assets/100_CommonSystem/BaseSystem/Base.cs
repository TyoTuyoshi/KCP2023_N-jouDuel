using UnityEngine;

namespace BaseSystem
{
    /// <summary>
    /// シーン共通機能
    /// </summary>
    public class Base : SingletonBase<Base>
    {
        protected override void Init()
        {
            DontDestroyOnLoad(gameObject);
            //GameManager生成
            KCP2023.GameManager.Create();
        }
        
        /// <summary>
        /// ゲーム内で共通して使用するものをBaseSystemとして追加
        /// シーン遷移でも破棄されない
        /// ゲームマネージャ
        /// サウンドマネージャ等
        /// </summary>
        public void AddToBase<T>(T singletonClass) where T : SingletonBase<T>
        {
            if (singletonClass)
            {
                singletonClass.transform.parent = transform;
            }
        }
    }
}