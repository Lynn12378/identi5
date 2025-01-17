using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

namespace Identi5.GamePlay
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        private GameMgr gameMgr;
        private ChangeDetector changes;
        public UIManager uIManager;
        public GameObject minimapIcon;
        private PlayerOutputData POD;
        
        #region - max -
        public int MaxHP = 100;
        public int MaxFood = 100;
        public int MaxBullet = 50;
        #endregion

        public List<Item> itemList = new List<Item>();

        [SerializeField] private Item item;
        [SerializeField] public PlayerOutfitsHandler playerOutfitsHandler;
        [SerializeField] private TMP_Text playerNameTxt;
        [SerializeField] Slider HPSlider;
        
        #region - Networked -
        [Networked] public int playerId { get; private set; }
        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int foodAmount { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int coinAmount { get; set; }
        [Networked] public int teamID { get; private set; }
        [Networked] public int itemID { get; set; } = -1;
        [Networked][Capacity(2)] public NetworkArray<Color> colorList => default;
        [Networked][Capacity(10)] public NetworkArray<string> outfits => default;

        [Networked] public int contribution { get; private set; }
        [Networked] public int killNo { get; private set; }
        [Networked] public int deathNo { get; private set; }
        [Networked] public float surviveTime { get; private set; }
        #endregion

        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            POD = GameMgr.playerOutputData;
            transform.SetParent(Runner.transform);
            gameMgr.PNDList.Add(Object.InputAuthority, this);

            playerNameTxt.text = playerName;
            if (Object.HasStateAuthority)
            {
                Init();
                SetPlayerTeamID_RPC(-1);
                SetPlayerRef_RPC();
                SetPlayerCoin_RPC(100);
                if(outfits.Get(0) != ""){
                    uIManager.playerImg.Init();
                    ChangeOufitsLocal();
                }
                minimapIcon.GetComponent<SpriteRenderer>().color = Color.green;
                HPSlider.fillRect.GetComponent<Image>().color = Color.green;
                minimapIcon.SetActive(true);
            }
            if(outfits.Get(0) != ""){
                playerOutfitsHandler.Init();
                ChangeOufits();
            }
            gameMgr.UpdatedPNDList();
        }
        private void ChangeOufitsLocal()
        {
            uIManager.UpdatedOutfits(outfits);
            uIManager.UpdatedColor(colorList);
        }
        private void ChangeOufits()
        {
            uIManager.UpdatedOutfits(playerOutfitsHandler, outfits);
            uIManager.UpdatedColor(playerOutfitsHandler, colorList);
        }
        public void Init()
        {
            SetPlayerHP_RPC(MaxHP);
            SetPlayerBullet_RPC(MaxBullet);
            SetPlayerFood_RPC(MaxFood);
            
        }

        private void ReceiveItem()
        {
            if(itemID != -1 && itemList.Count < 12)
            {
                var itemReceive = Instantiate(item);
                itemReceive.itemId = itemID;
                itemReceive.quantity = 1;
                itemReceive.SetSprite();
                itemList.Add(itemReceive);
                gameMgr.UpdateItemList();
                gameMgr.dialogCell.SetInfo("收到贈禮");
            }
            SetItem_RPC(-1);
        }

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;
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
            foodAmount = (amount < MaxFood ? amount :MaxFood);
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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetItem_RPC(int itemID)
        {
            this.itemID = itemID;
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
                    case nameof(HP):
                        HPSlider.value = HP;
                        break;
                    case nameof(teamID):
                        gameMgr.UpdatedTeamList();
                        gameMgr.UpdatedPlayerMinimap();
                        break;
                    case nameof(outfits):
                    case nameof(colorList):
                        ChangeOufits();
                        break;
                    case nameof(killNo):
                    case nameof(contribution):
                    case nameof(deathNo):
                    case nameof(surviveTime):
                        gameMgr.UpdateRankList();
                        break;
                }

                if(!Object.HasStateAuthority){return;}
                switch (change)
                {
                    case nameof(playerName):
                        playerNameTxt.text = playerName;
                        break;
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
                    case nameof(colorList):
                        ChangeOufitsLocal();
                        break;
                    case nameof(itemID):
                        ReceiveItem();
                        break;
                    case nameof(killNo):
                        POD.killNo = killNo;
                        break;
                }
            }
        }
        #endregion
    }
}