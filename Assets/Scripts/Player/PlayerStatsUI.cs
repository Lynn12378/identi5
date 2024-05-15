using UnityEngine;
using TMPro;

namespace DEMO
{
    public class PlayerStatsUI : MonoBehaviour
    {
        public TextMeshProUGUI bulletAmountText;

        public void UpdateBulletAmount(int amount)
        {
            if (bulletAmountText != null)
            {
                bulletAmountText.text = amount.ToString();
            }
        }
    }
}
