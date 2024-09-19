using UnityEngine;
using UnityEngine.U2D.Animation;
using Fusion;

namespace Identi5.GamePlay
{
    [System.Serializable]
    public class Item : NetworkBehaviour
    {
        public enum ItemType
        {
            BulletBox,
            Cerement,
            FoodCan,
            MedicalKit,
            Money,
            OutfitChangeCard,
            IDcard,
            Kettle,
            Key,
            Purse,
            Quiz,
            Umbrella
        }

        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public SpriteRenderer spriteRenderer;

        [Networked, OnChangedRender(nameof(OnItemIDChange))]
        public int itemID { get; set; }
        public int itemId;
        public int quantity = 1;

        public override void Spawned()
        {
            var itemWorld = GameObject.Find("GPManager/itemWorld");
            transform.SetParent(itemWorld.transform, false);
            SetItemID_RPC(itemID);
            OnItemIDChange();
        }

        public void OnItemIDChange()
        {
            itemId = itemID;
            SetSprite();
		}

        public void SetSprite()
        {
            var type = (ItemType)itemId;
            spriteResolver.SetCategoryAndLabel("item", type.ToString());
		}
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void DespawnItem_RPC()
        {
            Runner.Despawn(Object);
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetItemID_RPC(int itemID)
        {
            this.itemID = itemID;
		}
    }
}