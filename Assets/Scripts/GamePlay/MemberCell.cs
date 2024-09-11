using UnityEngine;
using TMPro;

namespace Identi5.GamePlay
{
    public class MemberCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameTxt = null;
        public string playerName = null;

        public void SetInfo(string playerName)
        {
            this.playerName = playerName;
            playerNameTxt.text = this.playerName;
        }
    }
}