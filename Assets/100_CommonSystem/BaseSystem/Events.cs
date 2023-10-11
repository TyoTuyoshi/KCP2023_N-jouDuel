using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;

namespace KCP2023
{
    public class Events : SingletonBase<Events>
    {
        private string m_masonsAct = "";

        private List<Command> m_cmd = new List<Command>();

        private void Start()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            
        }

        public void UpdateField()
        {
            
        }

        private void GetMasonsAction()
        {
            
        }

        public void PostActionData()
        {
            Command[] cmd = new[]
            {
                new Command { act = 2, dir = 4 },
                new Command { act = 2, dir = 4 } 
            };
            ClientManager.Instance.PostCommandJson(cmd);
        }
    }
}