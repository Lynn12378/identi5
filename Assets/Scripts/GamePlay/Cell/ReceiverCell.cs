using UnityEngine;
using Fusion;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class ReceiverCell : MonoBehaviour
    {
        private GPMgr gpMgr;
        private PlayerRef playerRef;
        private string playerName;
        [SerializeField] private TMP_Text receiverTxt = null;

        public void SetInfo(PlayerRef playerRef, string playerName)
        {
            this.playerRef = playerRef;
            receiverTxt.text = $"{playerName}";
        }
        public void OnReceiverBtnClicked()
        {
            gpMgr.SetReceiver(playerRef);
        }

        private void Start()
        {
            gpMgr = FindObjectOfType<GPMgr>();
        }
    }
}