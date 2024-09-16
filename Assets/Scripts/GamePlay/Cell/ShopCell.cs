using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class ShopCell : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] public Item.ItemType itemType;
        [SerializeField] private int buyCost;
        public Image itemImage;
        public SpriteLibraryAsset spriteLibraryAsset;
        public TMP_InputField inputField;
        public TMP_Text costTxt;
        private int buyQuantity = 1;
        private int totalCost;

        private PlayerNetworkData playerNetworkData;

        private void Start()
        {
            playerNetworkData = GameMgr.playerNetworkData;
            itemImage.sprite = spriteLibraryAsset.GetSprite("item", itemType.ToString());
            inputField.text = buyQuantity.ToString();
            UpdateTotalCost();
        }

        public void OnInputFieldValueChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                buyQuantity = result;
                UpdateTotalCost();
            }
            else
            {
                inputField.text = buyQuantity.ToString();
            }
        }

        public void OnInputFieldEndEdit(string value)
        {
            if (!int.TryParse(value, out int result) || result <= 0)
            {
                inputField.text = buyQuantity.ToString();
            }
            else
            {
                UpdateTotalCost();
            }
        }

        public void OnLeftButton()
        {
            if (buyQuantity > 1)
            {
                buyQuantity--;
                inputField.text = buyQuantity.ToString();
                UpdateTotalCost();
            }
        }

        public void OnRightButton()
        {
            buyQuantity++;
            inputField.text = buyQuantity.ToString();
            UpdateTotalCost();
        }

        private void UpdateTotalCost()
        {
            totalCost = buyQuantity * buyCost;
            costTxt.text = $"$ {totalCost}";
        }

        public void OnBuyButton()
        {
            if(playerNetworkData.itemList .Count > 11)
            {
                GameMgr.Instance.dialogCell.SetInfo("背包已滿");
                return;
            }

            if(playerNetworkData.coinAmount >= totalCost)
            {
                item.itemId = (int)itemType;
                item.quantity = buyQuantity;
                Instantiate(item);

                playerNetworkData.itemList.Add(item);
                GameMgr.Instance.UpdateItemList();
                playerNetworkData.SetPlayerCoin_RPC(playerNetworkData.coinAmount - totalCost);
            }
            else
            {
                GameMgr.Instance.dialogCell.SetInfo("你沒有足夠金幣來購買物品");
            }
        }
    }
}