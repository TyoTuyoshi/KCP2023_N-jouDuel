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
        [NonSerialized] public List<Mason> craftsmen = new List<Mason>();

        private void Start()
        {
            //SpawnCraftsman(GameManager.Instance.GetPlayAbleCraftsmen());
        }

        private void SpawnCraftsman(List<Mason> craftsmen)
        {
            foreach (var _ in craftsmen)
            {
                Mason mason = Instantiate(_);
                //craftsman.SetPos();
                craftsmen.Add(mason);
            }
        }

        private void SetCraftsmanPos()
        {
            
        }
    }
}