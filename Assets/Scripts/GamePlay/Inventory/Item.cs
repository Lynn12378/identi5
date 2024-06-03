using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Fusion;

using DEMO.Manager;

namespace DEMO.GamePlay.Inventory
{
    public class Item : NetworkBehaviour
    {
        public enum ItemType
        {
            Bullet,
            Coin,
            Food,
            Health,
            Wood,
        }

        [SerializeField] private SpriteResolver spriteResolver;
        [Networked] public int itemID { get; set; }
        [Networked] public int amount { get; set; }
        public ItemType itemType;

        public override void Spawned()
        {
            var itemWorld = Runner.transform.Find("itemWorld");
            transform.SetParent(itemWorld.transform, false);

            Init(itemID);
            GamePlayManager.Instance.itemList.Add(this);
        }

        public void Init(int itemID)
        {
            this.itemType = (ItemType) itemID;
            spriteResolver.SetCategoryAndLabel("item", this.itemType.ToString());

            SetItemID_RPC(itemID);
            SetAmount_RPC(1);
		}

        public void SetType()
        {
            this.itemType = (ItemType) itemID;
            spriteResolver.SetCategoryAndLabel("item", this.itemType.ToString());
        }

        #region - Pick Up Item -

        public void OnPickUp() // Method to handle when the item is picked up
        {
            DespawnItem_RPC();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetItemID_RPC(int itemID)
        {
            this.itemID = itemID;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetAmount_RPC(int amount)
        {
            this.amount = amount;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void DespawnItem_RPC()
        {
            Runner.Despawn(Object);
		}

        #endregion

        /*public void RemoveFromInventory()
        {
            Inventory.instance.Remove(this);
        }*/
    }
}