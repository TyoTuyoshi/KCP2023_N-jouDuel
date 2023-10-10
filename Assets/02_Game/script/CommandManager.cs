using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseSystem;
using TMPro;
using UnityEngine;

namespace KCP2023
{
    /// <summary>
    /// 指揮官
    /// </summary>
    public class CommandManager : SingletonBase<CommandManager>
    {
        public List<TMP_InputField> cmdInputFields = new List<TMP_InputField>();

        private string m_jsonCmds = "";
        private void Start()
        {
            var t = cmdInputFields.Select(x => x.text);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //post
            }
        }
    }
}