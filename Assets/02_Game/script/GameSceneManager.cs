using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;

namespace KCP2023
{
    public class GameSceneManager : SingletonBase<GameSceneManager>
    {
        //自身が先手か否か。(先手A,後手B)
        public bool isFirst = false;
        
        [SerializeField] private Camera m_camera = null;
        [SerializeField] private Vector3[] m_cameraOffset = null;

        //現在の試合状況
        public Matches nowMatches = new Matches();

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
        }

        private void Update()
        {
            if (isFirst) ;
        }
    }
}