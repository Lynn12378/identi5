using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

// using DEMO.GamePlay.Inventory;
// using DEMO.Gameplay;
// using DEMO.UI;

namespace Identi5.GamePlay
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        private GameMgr gameMgr;
        private ChangeDetector changes;
        public UIManager uIManager;
        
        #region - max -
        public int MaxHP = 100;
        public int MaxFood = 100;
        public int MaxBullet = 50;
        #endregion

        [SerializeField] public PlayerOutfitsHandler playerOutfitsHandler;
        // [SerializeField] private PlayerOutputData playerOutputData;
        
        [Networked] public int playerId { get; private set; }
        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int foodAmount { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int coinAmount { get; set; }
        [Networked] public int teamID { get; private set; }
        [Networked][Capacity(2)] public NetworkArray<Color> colorList => default;
        [Networked][Capacity(10)] public NetworkArray<string> outfits => default;

        [Networked] public int contribution { get; private set; }
        [Networked] public int killNo { get; private set; }
        [Networked] public int deathNo { get; private set; }
        [Networked] public float surviveTime { get; private set; }

        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);
            gameMgr.PNDList.Add(Object.InputAuthority, this);

            if (Object.HasStateAuthority)
            {
                Init();
                SetPlayerRef_RPC();
                SetPlayerCoin_RPC(100);
                if(outfits.Get(0) != ""){
                    uIManager.UpdatedOutfits(outfits);
                }
                uIManager.UpdatedColor(colorList);
            }

            playerOutfitsHandler.Init();
            uIManager.playerImg.Init();
            if(outfits.Get(0) != "")
            {
                uIManager.UpdatedOutfits(playerOutfitsHandler, outfits);
            }
            
            uIManager.UpdatedColor(playerOutfitsHandler, colorList);
        }

        public void Init()
        {
            SetPlayerHP_RPC(MaxHP);
            SetPlayerBullet_RPC(MaxBullet);
            SetPlayerFood_RPC(MaxFood);
            SetPlayerTeamID_RPC(0);
        }


            // if (Object.HasInputAuthority)
            // {
            //     // Change color of color code, if failed then color = white
            //     localColor = ColorUtility.TryParseHtmlString("#00C800", out Color color) ? color : Color.white;
            //     hpSlider.fillRect.GetComponent<Image>().color = localColor;
            //     minimapIcon.GetComponent<SpriteRenderer>().color = localColor;

            //     uIManager.InitializeItemSlots(this);
            // }
            // else
            // {
            //     minimapIcon.SetActive(false);
            // }

            // uIManager.UpdateMicTxt("none");
            // uIManager.SetPlayerRef(playerRef);

        // #region - Restart -
        // public void Restart()
        // {
        //     SetPlayerHP_RPC(MaxHP);
        //     SetPlayerBullet_RPC(MaxBullet);
        //     SetPlayerFood_RPC(MaxFood);
        //     SetPlayerCoin_RPC(0);
        //     SetPlayerTeamID_RPC(-1);

        //     itemList.Clear();
        //     UpdateItemList();
        // }
        // #endregion

        // #region - Update UI -
        // public void UpdateHPSlider(int health)
        // {
        //     hpSlider.value = health;
        // }

        // public void UpdateItemList()
        // {
        //     uIManager.SetItemList(itemList);
        //     uIManager.UpdateInventoryUI(itemList);
        // }

        
        // #endregion

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;
            uIManager.UpdatePlayerName(playerName);
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRef = Runner.LocalPlayer;
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void SetPlayerHP_RPC(int hp)
        {
            HP = (hp < MaxHP ? hp :MaxHP);
		}
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerBullet_RPC(int amount)
        {
            bulletAmount = (amount < MaxBullet ? amount :MaxBullet);
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerFood_RPC(int amount)
        {
            foodAmount = amount;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerCoin_RPC(int amount)
        {
            coinAmount = amount;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerTeamID_RPC(int id)
        {
            teamID = id;
		}

        // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        // public void ReceiveGift_RPC(Item.ItemType itemType)
        // {
        //     Item item = new Item
        //     {
        //         itemType = itemType,
        //         quantity = 1
        //     };
        //     itemList.Add(item);

        //     UpdateItemList();
        // }

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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddKillNo_RPC()
        {
            killNo++;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddContribution_RPC()
        {
            contribution++;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddDeathNo_RPC()
        {
            deathNo++;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetSurviveTime_RPC(float longestTime)
        {
            surviveTime = longestTime;
		}

        #endregion

        #region - OnChanged Events -
        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(teamID):
                        gameMgr.UpdatedPNDList();
                        gameMgr.UpdatedTeamList();
                        break;
                    case nameof(outfits):
                        uIManager.UpdatedOutfits(playerOutfitsHandler, outfits);
                        break;
                    case nameof(colorList):
                        uIManager.UpdatedColor(playerOutfitsHandler, colorList);
                        break;
                }

                if(!Object.HasStateAuthority){return;}
                switch (change)
                {
                    case nameof(HP):
                        uIManager.UpdateHPSlider(HP, MaxHP);
                        break;
                    case nameof(bulletAmount):
                        uIManager.UpdateBulletAmountTxt(bulletAmount, MaxBullet);
                        break;
                    case nameof(coinAmount):
                        uIManager.UpdateCoinAmountTxt(coinAmount);
                        break;
                    case nameof(foodAmount):
                        uIManager.UpdateFoodSlider(foodAmount, MaxFood);
                        break;
                    case nameof(outfits):
                        uIManager.UpdatedOutfits(outfits);
                        break;
                    case nameof(colorList):
                        uIManager.UpdatedColor(colorList);
                        break;
                }
            }
        }
        #endregion
    }
}