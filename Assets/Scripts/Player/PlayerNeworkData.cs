using System.Collections;
using UnityEngine;

using Fusion;

namespace DEMO.Player
{
    public class PlayerNetworkData : NetworkBehaviour
    {
		private GameManager gameManager = null;
        private ChangeDetector changes;

		[Networked] public string PlayerName { get; set; }
        // [Networked] public string HP { get; set; }
		[Networked] public NetworkBool IsReady { get; set; }
	
        public override void Spawned()
        {
			gameManager = GameManager.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

			transform.SetParent(GameManager.Instance.transform);

			gameManager.playerList.Add(Object.InputAuthority, this);
			gameManager.UpdatePlayerList();

			if (Object.HasInputAuthority)
			{
                SetPlayerName_RPC(gameManager.PlayerName);
				// SetHP_RPC(gameManager.HP);
			}
		}

        #region - RPCs -

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetPlayerName_RPC(string name)
        {
			PlayerName = name;
		}

        // public void SetHP_RPC(int newHP)
        // {
		// 	HP = newHP;
		// }

		[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetReady_RPC(bool isReady)
		{
			IsReady = isReady;
		}
        #endregion

        #region - OnChanged Events -

        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(PlayerName):
                        GameManager.Instance.UpdatePlayerList();
                        break;
                    // case nameof(HP):
                    //     GameManager.Instance.UpdatePlayerList();
                    //     break;
                    case nameof(IsReady):
                        GameManager.Instance.UpdatePlayerList();
                        break;
                }
            }
        }
        #endregion
    }
}