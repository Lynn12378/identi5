using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using Fusion;
using TMPro;

namespace Identi5.GamePlay
{
    public class UIManager : MonoBehaviour
    {
        #region - Outfits -
        [SerializeField] public PlayerOutfitsHandler playerImg;
        public void UpdatedOutfits(NetworkArray<string> outfits)
        {
            playerImg.Init();
            UpdatedOutfits(playerImg, outfits);
        }

        public void UpdatedOutfits(PlayerOutfitsHandler outfitsHandler, NetworkArray<string> outfits)
        {
            var i = 0;
            foreach(var resolver in outfitsHandler.resolverList)
            {
                outfitsHandler.ChangeOutfit(resolver.GetCategory(),outfits[i]);
                i+=1;
            }
        }

        public void UpdatedColor(NetworkArray<Color> colorList)
        {
            UpdatedColor(playerImg, colorList);
        }

        public void UpdatedColor(PlayerOutfitsHandler outfitsHandler, NetworkArray<Color> colorList)
        {
            outfitsHandler.SetSkinColor(colorList[0]);
            outfitsHandler.SetHairColor(colorList[1]);
        }
        #endregion

        #region - PlayerNetworkData UI -
        [SerializeField] Slider HPSlider;
        [SerializeField] private TMP_Text HPTxt;
        public void UpdateHPSlider(int HP, int maxHP)
        {
            HPSlider.value = HP;
            HPTxt.text = $"HP: {HP}/{maxHP}";
        }

        [SerializeField] Slider durabilitySlider;
        [SerializeField] private TMP_Text durabilityTxt;
        public void UpdateDurabilitySlider(int durability, int maxDurability)
        {
            durabilitySlider.value = durability;
            durabilityTxt.text = $"耐久度: {durability}/{maxDurability}";
        }

        [SerializeField] Slider foodSlider;
        [SerializeField] private TMP_Text foodTxt;
        public void UpdateFoodSlider(int food, int maxFood)
        {
            foodSlider.value = food;
            foodTxt.text = $"食物: {food}/{maxFood}";
        }

        [SerializeField] private TMP_Text bulletAmountTxt;
        public void UpdateBulletAmountTxt(int bulletAmount, int maxbulletAmount)
        {
            bulletAmountTxt.text = $"{bulletAmount}";
        }

        [SerializeField] private TextMeshProUGUI playerCoinAmount;
        public void UpdateCoinAmountTxt(int coinAmount)
        {
            playerCoinAmount.SetText(coinAmount.ToString());
        }
        #endregion

        #region - Minimap -
        public Transform baseTransform;
        public RectTransform arrowRectTransform;
        public float initialAngleOffset = 350f;

        public void UpdateMinimapArrow(Transform playerTransform)
        {
            Vector3 direction = playerTransform.position - baseTransform.position;
        
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - initialAngleOffset;

            arrowRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        #endregion
    }
}