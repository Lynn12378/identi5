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
            GameMgr.Instance.UpdateItemList();
        }
    }
}