using UnityEngine;

namespace BaseSystem
{
    /// <summary>
    /// シングルトン化のテンプレート
    /// </summary>
    /// <typeparam name="T">シングルトン化させるクラス</typeparam>
    public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        //参照用インスタンス
        private static T instance = null;

        static public T Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Createオバロ
        /// </summary>
        static public void Create()
        {
            GameObject obj = new GameObject();
            //Debug.Log("Create");
            Create(obj);
        }

        /// <summary>
        /// インスタンスの生成
        /// </summary>
        /// <param name="obj">アタッチ先のゲームオブジェクト</param>
        static public void Create(GameObject obj)
        {
            if (instance == null && obj)
            {
                T component = obj.AddComponent<T>();
                //オブジェクトにコンポーネント名で名づける
                obj.name = component.GetType().Name;
            }
        }

        private void Awake()
        {
            //既にシーンに存在する場合は削除
            if (instance)
            {
                Destroy(gameObject);
                return;
            }

            //インスタンス登録
            instance = this as T;
            Init();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// オブジェクト破棄
        /// </summary>
        public void Destoroy()
        {
            if (instance)
            {
                instance = null;
                Destroy(gameObject);
            }
        }
    }
}