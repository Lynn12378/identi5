using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class ItemCell : MonoBehaviour
    {
        private Item item;
        [SerializeField] private Image img = null;
        [SerializeField] private TMP_Text quantityTxt = null;

        public void SetInfo(Item item)
        {
            this.item = item;
            item.transform.SetParent(transform, false);
            img.sprite = item.spriteRenderer.sprite;
            quantityTxt.text = $"{item.quantity}";
        }

        public void OnBtnClicked()
        {
            FindObjectOfType<GPMgr>().SetItemAction(item);
        }

        public void OnUseClicked()
        {
            GameMgr.Instance.UpdatedItemList();
        //     bool validItem = true;

        //     switch (itemType)
        //     {
        //         default:
        //         case ItemType.Bullet:
        //             playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + bulletAdd);
        //             AudioManager.Instance.Play("Use");
        //             break;
        //         case ItemType.Food:
        //             playerNetworkData.SetPlayerFood_RPC(playerNetworkData.foodAmount + foodAdd);
        //             AudioManager.Instance.Play("Eat");
        //             break;
        //         case ItemType.Health:
        //             playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + boostHealth);
        //             AudioManager.Instance.Play("Heal");
        //             break;
        //         case ItemType.Wood:
        //             if(playerNetworkData.shelter != null)
        //             {
        //                 playerNetworkData.shelter.SetDurability_RPC(playerNetworkData.shelter.durability + 2);
        //                 playerNetworkData.GetPlayerOutputData().repairQuantity++;
        //                 AudioManager.Instance.Play("Use");
        //             }
        //             else
        //             {
        //                 gamePlayManager.ShowWarningBox("這個物品只能在基地使用。");
        //                 validItem = false;
        //             }
        //             break;
        //         case ItemType.Badge_LiberalArts:
        //         case ItemType.Badge_Science:
        //         case ItemType.Badge_Engineer:
        //         case ItemType.Badge_Management:
        //         case ItemType.Badge_EECS:
        //         case ItemType.Badge_Earth:
        //         case ItemType.Badge_Hakka:
        //         case ItemType.Badge_HST:
        //         case ItemType.Badge_TeachingCenters:
        //             if (IsPlayerInCorrectLocation(playerNetworkData, itemType, out Building building))
        //             {
        //                 building.AddBadge_RPC();
        //                 playerNetworkData.GetPlayerOutputData().usePlaceholderNo++;
        //                 AudioManager.Instance.Play("BadgeUse"); ///////////////////////// Add sound
        //             }
        //             else
        //             {
        //                 gamePlayManager.ShowWarningBox("這個物品不能在這裡使用。");
        //                 validItem = false;
        //             }
        //             break;
        //     }

        //     if(validItem)
        //     {
        //         DecreaseQuantityOrRemove(playerNetworkData.itemList);
        //     }
        }

    }
}