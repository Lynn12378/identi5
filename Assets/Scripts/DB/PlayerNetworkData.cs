using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.Manager;
using DEMO.GamePlay.Inventory;

namespace DEMO.DB
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        private GameManager gameManager = null;
        private ChangeDetector changes;
        private UIManager uIManager = null;

        [SerializeField] public PlayerOutfitsHandler playerOutfitsHandler = null;

        [Networked] public int playerId { get; private set; }
        [Networked] public string playerRef { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int teamID { get; set; }
        [Networked][Capacity(2)] public NetworkArray<Color> colorList => default;
        [Networked][Capacity(10)] public NetworkArray<string> outfits => default;

        public int MaxHP = 100;
        public int MaxBullet = 50;
        public List<Item> itemList = new List<Item>();

        public void SetUIManager(UIManager uIManager)
        {
            this.uIManager = uIManager;
        }

        public void UpdatedOutfits()
        {
            var i = 0;
            foreach(var resolver in playerOutfitsHandler.resolverList)
            {
                playerOutfitsHandler.ChangeOutfit(resolver.GetCategory(),outfits[i]);
                i+=1;
            }
        }

        public override void Spawned()
        {
            gameManager = GameManager.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
  
            if (Object.HasStateAuthority)
            {
                SetPlayerHP_RPC(MaxHP);
                SetPlayerBullet_RPC(MaxBullet);
                SetPlayerTeamID_RPC(-1);
            }

            transform.SetParent(Runner.transform);
            gameManager.gamePlayerList.Add(Object.InputAuthority, this);

            playerOutfitsHandler.Init();

            if(outfits.Get(0) != ""){UpdatedOutfits();}
            playerOutfitsHandler.SetSkinColor(colorList[0]);
            playerOutfitsHandler.SetHairColor(colorList[1]);
		}

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;
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

		public void SetColorList(List<Color> colors)
        {
            colorList.Clear();
            colorList.Set(0, colors[0]);
            colorList.Set(1, colors[1]);
		}

        public void SetOutfits(List<string> outfits)
        {
            this.outfits.Clear();

            for(int i = 0; i < outfits.Count; i++)
            {
                this.outfits.Set(i, outfits[i]);
            }
		}

        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                if(!Object.HasStateAuthority){return;}
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(HP):
                            uIManager.UpdateHPSlider(HP, MaxHP);
                            break;
                        case nameof(bulletAmount):
                            uIManager.UpdateBulletAmountTxt(bulletAmount, MaxBullet);
                            break;
                        case nameof(teamID):
                            //call UIManager change Team
                            break;
                        case nameof(outfits):
                            UpdatedOutfits();
                            break;
                        case nameof(colorList):
                            playerOutfitsHandler.SetSkinColor(colorList[0]);
                            playerOutfitsHandler.SetHairColor(colorList[1]);
                            break;
                    }
                }
            }
        #endregion
    }
}