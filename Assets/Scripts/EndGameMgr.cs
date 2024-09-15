using UnityEngine;
using Fusion;

namespace Identi5
{
    public class EndGameMgr : NetworkBehaviour
    {
        private string baseUrl = "http://localhost/DEMO/BFI-15.php";
        public void GoToQuestion()
        {
            int playerId = 0;

            foreach (var player in GameMgr.Instance.PNDList)
            {
                if(player.Key == Runner.LocalPlayer) playerId = player.Value.playerId;
            }

            string fullUrl = baseUrl + "?player_id=" + playerId.ToString();
            if(playerId != 0)
            {
                Application.OpenURL(fullUrl);
            }
        }
    }
}