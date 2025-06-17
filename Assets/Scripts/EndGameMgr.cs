using UnityEngine;
using Fusion;

namespace Identi5
{
    public class EndGameMgr : NetworkBehaviour
    {
        private string baseUrl = "https://identi5-1073915238949.asia-east1.run.app/BFI-15.php";
        public void GoToQuestion()
        {
            string fullUrl = $"{baseUrl}?player_id={GameMgr.playerInfo.Player_id}";
            Application.OpenURL(fullUrl);
        }
        public void Start()
        {
            Debug.Log($"Player_id:{GameMgr.playerInfo.Player_id}");
            GoToQuestion();
        }
    }
}