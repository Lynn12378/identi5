using System.Collections;
using UnityEngine;
using Fusion;

using DEMO.Manager;

namespace DEMO.DB
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        // private GameManager gameManager = null;
        private ChangeDetector changes;
        // private GameObject obj;
<<<<<<< HEAD
        [SerializeField] private Slider healthPointSlider = null;
        [SerializeField] private CanvasManager canvasManager = null;
=======
>>>>>>> parent of 3544c30 (5/27 merge xuan)

        [Networked] public int playerId { get; private set; }
        [Networked] public string playerRef { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int teamID { get; set; }

        public int MaxHP = 100;
        public int MaxBullet = 50;


        public override void Spawned()
        {
			// gameManager = GameManager.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            // gameManager.gamePlayerList.Add(Object.InputAuthority, this);
            // transform.SetParent(GameManager.Instance.transform);
            transform.SetParent(Runner.transform);

            if (Object.HasStateAuthority)
            {
                SetPlayerInfo_RPC(0,"TEST");
                SetPlayerHP_RPC(MaxHP);
                SetPlayerBullet_RPC(MaxBullet);
                SetPlayerTeamID_RPC(-1);
            }

            // gameManager.UpdatedGamePlayer();
		}

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;
            // obj.name = "LocalPlayer";
            playerRef = Runner.LocalPlayer.ToString();
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerHP_RPC(int hp)
        {
            HP = hp;
		}
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerBullet_RPC(int amount)
        {
            bulletAmount = amount;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerTeamID_RPC(int id)
        {
            teamID = id;
		}

        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(HP):
<<<<<<< HEAD
                            
                            // UIManager.Instance?.UpdateHealthSlider(playerRef, HP);
                            UpdateHealthPointSlider(HP);
                            break;

                        case nameof(bulletAmount):
                            // UIManager.Instance?.UpdateBulletAmount(playerRef, bulletAmount);
=======
                            //call UIManager change slider
                            break;

                        case nameof(bulletAmount):
                            //call UIManager change amount
>>>>>>> parent of 3544c30 (5/27 merge xuan)
                            break;

                        case nameof(teamID):
                            //call UIManager change Team
                            break;
                    }
                }
            }
        #endregion
    }
}