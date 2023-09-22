using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;

namespace KCP2023
{

    public class CraftsmanManager : SingletonBase<CraftsmanManager>
    {
        //職人たち
        [NonSerialized] public List<Craftsman> craftsmen = new List<Craftsman>();

        private void Start()
        {
            SpawnCraftsman(GameManager.Instance.GetPlayAbleCraftsmen());
        }

        private void SpawnCraftsman(List<Craftsman> craftsmen)
        {
            foreach (var _ in craftsmen)
            {
                Craftsman craftsman = Instantiate(_);
                //craftsman.SetPos();
                craftsmen.Add(craftsman);
            }
        }

        private void SetCraftsmanPos()
        {
            
        }
    }
}