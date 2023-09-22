using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;

namespace KCP2023
{
    public class GameSceneManager : SingletonBase<GameSceneManager>
    {
        [SerializeField] private Camera m_camera = null;
        [SerializeField] private Vector3[] m_cameraOffset = null;
        /// <summary>
        /// カメラをフィールド中央に設置
        /// </summary>
        public void SetFieldCenterCameraPosition()
        {
            int fieldSize = GameManager.Instance.fieldSize;
            Vector3 cameraPos = m_camera.transform.position;
            float chipHalf = 0.5f;

            //X軸方向の位置調整
            m_camera.transform.position = ((float)fieldSize / 2.0f - chipHalf) * Vector3.right;
            //Y-Z軸方向の位置調整
            m_camera.transform.position += m_cameraOffset[GameManager.Instance.fieldSizeIndex];
        }

        void Start()
        {
            SetFieldCenterCameraPosition();
            Debug.Log("de");
        }
    }
}