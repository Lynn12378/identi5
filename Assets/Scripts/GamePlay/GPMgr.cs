using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using TMPro;

using Identi5.GamePlay.Cell;

namespace Identi5.GamePlay
{
    public class GPMgr : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameMgr gameMgr;
		private NetworkRunner runner;
        private PlayerRef localPlayer;
        private PlayerNetworkData PND;
        private PlayerOutputData POD;
        public DialogCell dialogCell;
        public DocCell docCell;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PanelMgr panelMgr;

        private void Start()
        {
            gameMgr = GameMgr.Instance;
            runner = gameMgr.Runner;
            runner.AddCallbacks(this);
            panel.Add(TeamListPanel);
        }

        private void OnDestroy()
        {
            gameMgr.OnInPNDListUpdated -= UpdatedPNDList;
            gameMgr.OnTeamListUpdated -= UpdatedTeamList;
            gameMgr.OnTeamListUpdated -= UpdatedPlayerMinimap;
            gameMgr.OnMessageListUpdated -= UpdatedMessageList;
            gameMgr.OnItemListUpdated -= UpdateItemList;
            gameMgr.OnRankListUpdated -= UpdateRankList;
            gameMgr.OnOutfitsUpdated -= UpdateOutfits;
        }

        #region - Start Game -
        private void Init()
        {
            var PIF = gameMgr.PIFList[localPlayer];
            PND = gameMgr.PNDList[localPlayer];
            POD = GameMgr.playerOutputData;

            PND.SetPlayerInfo_RPC(PIF.playerId, PIF.playerName);
            PND.SetColorList(PIF.colorList);
            PND.SetOutfits(PIF.outfits);
            GameMgr.playerNetworkData = PND;

            PIF.Despawned();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            gameMgr.OnInPNDListUpdated += UpdatedPNDList;
            gameMgr.OnTeamListUpdated += UpdatedTeamList;
            gameMgr.OnTeamListUpdated += UpdatedPlayerMinimap;
            gameMgr.OnMessageListUpdated += UpdatedMessageList;
            gameMgr.OnItemListUpdated += UpdateItemList;
            gameMgr.OnRankListUpdated += UpdateRankList;
            gameMgr.OnOutfitsUpdated -= UpdateOutfits;

            localPlayer = runner.LocalPlayer;
            var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, localPlayer);

            runner.SetPlayerObject(localPlayer, playerObject);
            Camera.main.transform.SetParent(playerObject.transform);
            Init();
            SortKill();
            gameMgr.UpdatedPNDList();
            gameMgr.dialogCell = dialogCell;
            gameMgr.docCell = docCell;
            playerOutfitsHandler.Init();
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            gameMgr.UpdatedPNDList();
        }

        #endregion
        #region - PND -
            private List<ReceiverCell> receiverCells = new List<ReceiverCell>();
            [SerializeField] private ReceiverCell receiverCell = null;
            [SerializeField] private Transform receiverContentTrans = null;
            public void UpdatedPNDList()
            {
                foreach(var cell in receiverCells)
                {
                    Destroy(cell.gameObject);
                }
                receiverCells.Clear();
                foreach(var player in gameMgr.PNDList.Values)
                {
                    if(player.playerRef == localPlayer){continue;}
                    var cell = Instantiate(receiverCell);
                    cell.SetInfo(player.playerRef, player.playerName);
                    cell.transform.SetParent(receiverContentTrans, false);
                    receiverCells.Add(cell);
                }
            }
        #endregion
        
        #region - Messages - 
        [SerializeField] private TMP_Text messageTxt = null;
        [SerializeField] private GameObject messageCellPrefab = null;
        [SerializeField] private Transform messageContentTrans = null;

        public void CreateMessage()
        {
            var cell = runner.Spawn(messageCellPrefab, Vector3.zero, Quaternion.identity);
            cell.GetComponent<MessageCell>().SetMessage_RPC(PND.playerName, messageTxt.text);
            gameMgr.UpdatedMessageList();
            messageTxt.text = "";
            POD.messageSent++;
        }

        public void UpdatedMessageList()
        {
            foreach(var message in gameMgr.messageList)
            {
                message.transform.SetParent(messageContentTrans, false);
            }
        }
        #endregion

        #region - Team -
        private List<GameObject> panel = new List<GameObject>();
        private List<MemberCell> memberCells = new List<MemberCell>();
        [SerializeField] private TMP_Text teamTxt = null;
        [SerializeField] private GameObject teamCellPrefab;
        [SerializeField] private MemberCell memberCell;
        [SerializeField] private Transform teamContentTrans;
        [SerializeField] private Transform memberContentTrans;
        [SerializeField] private GameObject TeamListPanel;
        [SerializeField] private GameObject MemberListPanel;

        public void CreateTeam()
        {
            var cell = runner.Spawn(teamCellPrefab, Vector3.zero, Quaternion.identity);
            var teamCell = cell.GetComponent<TeamCell>();
            teamCell.SetCount_RPC(++teamCell.count);
            teamCell.SetInfo(++gameMgr.newTeamID);
            PND.SetPlayerTeamID_RPC(gameMgr.newTeamID);
            POD.teamCreated++;
            OnActivePanel();
        }

        public void LeaveTeam()
        {
            foreach(var team in gameMgr.teamList)
            {
                if(team.teamID == PND.teamID)
                {
                    team.SetCount_RPC(--team.count);
                }
            }
            PND.SetPlayerTeamID_RPC(0);
            POD.quitTeamNo++;
            OnActivePanel();
        }

        public void OnActivePanel()
        {
            panel = new List<GameObject>();
            if(PND.teamID > 0)
            {
                panel.Add(MemberListPanel);
            }
            else
            {
                panel.Add(TeamListPanel);
            }
            panelMgr.OnActivePanel(panel);
        }

        public void UpdatedTeamList()
        {
            foreach(var cell in memberCells)
            {
                Destroy(cell.gameObject);
            }
            memberCells.Clear();
            foreach(var team in gameMgr.teamList)
            {
                team.transform.SetParent(teamContentTrans, false);
                team.SetInfo(team.teamID);
            }
            if(PND.teamID > 0)
            {
                teamTxt.text = $"隊伍 {PND.teamID}";
                foreach(var player in gameMgr.PNDList.Values)
                {
                    if(player.teamID == PND.teamID)
                    {
                        var cell = Instantiate(memberCell);
                        memberCells.Add(cell);
                        cell.transform.SetParent(memberContentTrans, false);
                        cell.SetInfo(player.playerName);
                    }
                }
            }
            panel[0].SetActive(false);
        }
        public void UpdatedPlayerMinimap()
        {
            foreach (var player in gameMgr.PNDList.Values)
            {
                if(player == PND || PND.teamID == -1) continue;
                if (player.teamID == PND.teamID)
                {
                    player.minimapIcon.SetActive(true);
                }
                else
                {
                    player.minimapIcon.SetActive(false);
                }
            }
        }
        #endregion

        #region - Inventory - 
        [SerializeField] private GameObject itemCellPrefab = null;
        [SerializeField] private Transform itemContentTrans = null;
        private List<ItemCell> itemCells = new List<ItemCell>();
        
        public void UpdateItemList()
        {
            var tempList = new List<Item>();
            foreach(var cell in itemCells)
            {
                Destroy(cell.gameObject);
            }
            itemCells.Clear();
            foreach(var item in PND.itemList)
            {
                if(item.quantity < 1)
                {
                    tempList.Add(item);
                    continue;
                }
                var cell = Instantiate(itemCellPrefab).GetComponent<ItemCell>();
                itemCells.Add(cell);
                cell.transform.SetParent(itemContentTrans, false);
                cell.SetInfo(item);
            }
            foreach(var item in tempList)
            {
                PND.itemList.Remove(item);
                Destroy(item.gameObject);
            }
        }

        public void ArrangeItem()
        {
            Dictionary<int, Item> tempList = new Dictionary<int, Item>();
            foreach(var item in PND.itemList)
            {
                if(tempList.ContainsKey(item.itemId))
                {
                    tempList[item.itemId].quantity++;
                    Destroy(item.gameObject);
                }
                else
                {
                    tempList.Add(item.itemId, item);
                }
            }
            PND.itemList.Clear();
            foreach(var item in tempList.Values)
            {
                PND.itemList.Add(item);
            }
            gameMgr.UpdateItemList();
            POD.organizeNo++;
        }
        #endregion

        #region - Action -
            public Item itemAction;
            private List<string> outfits;
            private List<Color> colorList;
            [SerializeField] private PlayerRef receiver;
            [SerializeField] private PlayerOutfitsHandler playerOutfitsHandler;
            [SerializeField] private GameObject actionListPanel;
            [SerializeField] private GameObject givenPanel;
            [SerializeField] private GameObject outfitPanel;
            [SerializeField] private GameObject outfitCamera;

            public void SetItemAction(Item itemAction)
            {
                this.itemAction = itemAction;
                actionListPanel.SetActive(true);
            }

            public void SetReceiver(PlayerRef receiver)
            {
                this.receiver = receiver;
                itemAction.quantity--;
                gameMgr.UpdateItemList();
                CloseActionPanel();
            }

            public void GiveItem()
            {
                gameMgr.source.clip = clips[3];
                gameMgr.source.Play();
                gameMgr.PNDList[receiver].SetItem_RPC(itemAction.itemId);
                itemAction.quantity--;
                CloseGivenPanel();
                POD.giftNo++;
                gameMgr.dialogCell.SetInfo("已送出該物品");
            }
            
            [SerializeField] private AudioClip[] clips;
            public void UseItem()
            {
                var info = "該物品無法使用";
                gameMgr.source.clip = clips[4];

                switch((Item.ItemType)itemAction.itemId)
                {
                    case Item.ItemType.BulletBox:
                        info = "子彈已增加";
                        POD.remainBullet.Add(PND.bulletAmount);
                        PND.SetPlayerBullet_RPC(PND.bulletAmount + 10);
                        break;
                    case Item.ItemType.FoodCan:
                        info = "食物值已增加";
                        gameMgr.source.clip = clips[2];
                        PND.SetPlayerFood_RPC(PND.foodAmount + 20);
                        break;
                    case Item.ItemType.MedicalKit:
                        info = "血量已增加";
                        gameMgr.source.clip = clips[1];
                        POD.remainHP.Add(PND.HP);
                        PND.SetPlayerHP_RPC(PND.HP + 20);                    
                        break;
                    case Item.ItemType.Cerement:
                        if(gameMgr.shelter == null)
                        {
                            info = "該物品需要在基地內使用!";
                            gameMgr.dialogCell.SetInfo(info);
                            return;
                        }
                        info = "基地耐久度已增加";
                        gameMgr.shelter.SetDurability_RPC(gameMgr.shelter.durability + 10);
                        PND.AddContribution_RPC();
                        POD.contribution = PND.contribution;
                        break;
                    case Item.ItemType.OutfitChangeCard:
                        if(gameMgr.shelter == null)
                        {
                            info = "該物品需要在基地內使用";
                            gameMgr.dialogCell.SetInfo(info);
                            return;
                        }
                        info = "換裝卡已使用";
                        outfitPanel.SetActive(true);
                        outfitCamera.SetActive(true);
                        break;
                    case Item.ItemType.IDcard:
                        info = "被遺失的學生證，有明顯的凹折";
                        break;
                    case Item.ItemType.Kettle:
                        info = "便宜好用的運動水壺";
                        break;
                    case Item.ItemType.Key:
                        info = "掉落在路上的鑰匙，看起來是學生宿舍的";
                        break;
                    case Item.ItemType.Purse:
                        info = "在月底撿到的錢包，裡面空無一物";
                        break;
                    case Item.ItemType.Quiz:
                        info = "一張不及格的微積分考卷";
                        break;
                    case Item.ItemType.Umbrella:
                        info = "路邊無主的愛心傘";
                        break;
                }
                gameMgr.source.Play();
                gameMgr.dialogCell.SetInfo(info);
                itemAction.quantity--;
                gameMgr.UpdateItemList();
                CloseActionPanel();
            }

            public void DiscardItem()
            {
                gameMgr.source.clip = clips[7];
                gameMgr.source.Play();
                itemAction.quantity--;
                gameMgr.UpdateItemList();
                CloseActionPanel();
            }

            #region -Outfit -
            public void ChangeOutfit()
            {
                GameMgr.playerOutputData.oufitChangedNo++;
                itemAction.quantity--;
                gameMgr.UpdateItemList();
                UpdateOutfits();
                outfitPanel.SetActive(false);
                outfitCamera.SetActive(false);
            }

            public void UpdateOutfits()
            {
                SetOufits();
                PND.SetOutfits(outfits);
                PND.SetColorList(colorList);
            }

            private void SetOufits()
            {
                outfits = new List<string>();
                colorList = GameMgr.playerInfo.colorList;
                foreach(var resolver in playerOutfitsHandler.resolverList)
                {
                    outfits.Add(resolver.GetLabel().ToString());
                }
            }
            #endregion
            #region - Panel - 
            public void CloseActionPanel()
            {
                actionListPanel.SetActive(false);
            }

            public void ActiveGivenPanel()
            {
                givenPanel.SetActive(true);
            }

            public void CloseGivenPanel()
            {
                givenPanel.SetActive(false);
            }
            #endregion
            
        #endregion

        #region - Rank -
            private int sortID = 0;
            List<PlayerNetworkData> sortedList;
            [SerializeField] private RankCell rankCell;
            [SerializeField] private Transform rankContentTrans = null;
            private List<RankCell> rankCells = new List<RankCell>();

            public void OnRankBtnCliked()
            {
                POD.rankClikedNo++;
            }
    
            public void SortKill()
            {
                sortID = 0;
                gameMgr.UpdateRankList();
            }
            public void SortDeath()
            {
                sortID = 1;
                gameMgr.UpdateRankList();
            }
            public void SortSuvivetime()
            {
                sortID = 2;
                gameMgr.UpdateRankList();
            }
            public void SortContribution()
            {
                sortID = 3;
                gameMgr.UpdateRankList();
            }

            private void sortByID()
            {
                switch(sortID)
                {
                    case 0:
                        CalculateRank(gameMgr.PNDList.Values, (a, b) => b.killNo.CompareTo(a.killNo));
                        break;
                    case 1:
                        CalculateRank(gameMgr.PNDList.Values, (a, b) => a.deathNo.CompareTo(b.deathNo));
                        break;
                    case 2:
                        CalculateRank(gameMgr.PNDList.Values, (a, b) => b.surviveTime.CompareTo(a.surviveTime));
                        break;
                    case 3:
                        CalculateRank(gameMgr.PNDList.Values, (a, b) => b.contribution.CompareTo(a.contribution));
                        break;
                } 
            }

            private void CalculateRank(IEnumerable<PlayerNetworkData> PNDList, Comparison<PlayerNetworkData> comparison)
            {
                sortedList = new List<PlayerNetworkData>(PNDList);
                sortedList.Sort(comparison);
            }

            public void UpdateRankList()
            {
                int i = 0;
                sortByID();
                foreach(var cell in rankCells)
                {
                    Destroy(cell.gameObject);
                }
                rankCells.Clear();
                foreach(var PND in sortedList)
                {
                    
                    var cell = Instantiate(rankCell, rankContentTrans);
                    switch(sortID)
                    {
                        case 0:
                            cell.SetInfo(PND.playerName, PND.killNo, i+1);
                            break;
                        case 1:
                            cell.SetInfo(PND.playerName, PND.deathNo, i+1);
                            break;
                        case 2:
                            cell.SetInfo(PND.playerName, (float)Math.Round(PND.surviveTime, 2), i+1);
                            break;
                        case 3:
                            cell.SetInfo(PND.playerName, PND.contribution, i+1);
                            break;
                    }
                    i++;
                    rankCells.Add(cell);
                }
            }
        #endregion 
       
		#region /-- Unused Function --/
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
            public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnInput(NetworkRunner runner, NetworkInput input){}
            public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
            public void OnConnectedToServer(NetworkRunner runner){}
            public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
            public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,byte[] token){}
            public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
            public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}