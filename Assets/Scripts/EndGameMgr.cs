using UnityEngine;
using Fusion;

namespace Identi5
{
    public class EndGameMgr : NetworkBehaviour
    {
        private string baseUrl = "http://localhost/DEMO/BFI-15.php";
        public void GoToQuestion()
        {
            string fullUrl = $"{baseUrl}?player_id={GameMgr.playerInfo.Player_id}";
            Application.OpenURL(fullUrl);
        }
    }
}