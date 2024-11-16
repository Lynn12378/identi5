using UnityEngine;
using Fusion;

namespace Identi5
{
    public class EndGameMgr : MonoBehaviour
    {
        private string baseUrl = "https://directly-precious-egret.ngrok-free.app/DEMO/BFI-15.php";
        public void GoToQuestion()
        {
            string fullUrl = $"{baseUrl}?player_id={GameMgr.playerInfo.Player_id}";
            Application.OpenURL(fullUrl);
        }
        public void Start()
        {
            GoToQuestion();
        }
    }
}