using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class ShopCell : MonoBehaviour
    {
        private Item item;
        private Item.ItemType itemType;
        public Image itemImage;
        public SpriteLibraryAsset spriteLibraryAsset;

        public Button leftButton;
        public Button rightButton;
        public Button buyButton;
        public TMP_InputField inputField;
        public TextMeshProUGUI costTxt;
        private int buyQuantity = 1;
        [SerializeField] private int buyCost;
        private int totalCost;

        private PlayerNetworkData playerNetworkData;

        private void Start(Item item)
        {
            itemType = (Item.ItemType)item.itemId;
            itemImage.sprite = spriteLibraryAsset.GetSprite("item", itemType.ToString());
            inputField.text = buyQuantity.ToString();
            UpdateTotalCost();
        }
        public void Initialize(PlayerNetworkData playerNetworkData)
        {
            this.playerNetworkData = playerNetworkData;
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
                inputField.text = buyQuantity.ToString(); // If input not valid, back to latest buyQuantity
            }
        }

        public void OnInputFieldEndEdit(string value)
        {
            if (!int.TryParse(value, out int result) || result <= 0)
            {
                inputField.text = buyQuantity.ToString(); // Ensure input valid
            }
            else
            {
                UpdateTotalCost(); // Ensure total cost is updated when editing end
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
            if(playerNetworkData.coinAmount >= totalCost)
            {
                item = new Item
                {
                    itemId = (int)itemType,
                    quantity = buyQuantity
                };

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