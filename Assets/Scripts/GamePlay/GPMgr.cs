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

        public void Test()
        {
            GameMgr.Instance.ODHandler.UpdateOD();
        }

        private void OnDestroy()
        {
            gameMgr.OnInPNDListUpdated -= UpdatedPNDList;
            gameMgr.OnTeamListUpdated -= UpdatedTeamList;
            gameMgr.OnTeamListUpdated -= UpdatedPlayerMinimap;
            gameMgr.OnMessageListUpdated -= UpdatedMessageList;
            gameMgr.OnItemListUpdated -= UpdateItemList;
            gameMgr.OnRankListUpdated -= UpdateRankList;
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

            localPlayer = runner.LocalPlayer;
            var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, localPlayer);

            runner.SetPlayerObject(localPlayer, playerObject);
            Camera.main.transform.SetParent(playerObject.transform);
            Init();
            SortKill();
            gameMgr.UpdatedPNDList();
            gameMgr.dialogCell = dialogCell;
            gameMgr.docCell = docCell;
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
            [SerializeField] public PlayerRef receiver;
            public Item itemAction;
            [SerializeField] private GameObject actionListPanel;
            [SerializeField] private GameObject givenPanel;
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
                gameMgr.PNDList[receiver].SetItem_RPC(itemAction);
                itemAction.quantity--;
                CloseGivenPanel();
                POD.giftNo++;
                gameMgr.dialogCell.SetInfo("已送出該物品");
            }
            
            public void UseItem()
            {
                switch((Item.ItemType)itemAction.itemId)
                {
                    case Item.ItemType.BulletBox:
                        POD.remainBullet.Add(PND.bulletAmount);
                        PND.SetPlayerBullet_RPC(PND.bulletAmount + 10);
                        break;
                    case Item.ItemType.FishCan:
                        PND.SetPlayerFood_RPC(PND.foodAmount + 20);
                        break;
                    case Item.ItemType.NedicalKit:
                        POD.remainHP.Add(PND.HP);
                        PND.SetPlayerHP_RPC(PND.HP + 20);                    
                        break;
                    case Item.ItemType.Cerement:
                        if(gameMgr.shelter != null)
                        {
                            gameMgr.shelter.SetDurability_RPC(gameMgr.shelter.durability + 10);
                            PND.AddContribution_RPC();
                            POD.contribution = PND.contribution;
                        }
                        else
                        {
                            gameMgr.dialogCell.SetInfo("請在基地內使用");
                            return;
                        }
                        break;
                    default:
                        break;
                }
                itemAction.quantity--;
                gameMgr.UpdateItemList();
                CloseActionPanel();
            }

            public void DiscardItem()
            {
                itemAction.quantity--;
                gameMgr.UpdateItemList();
                CloseActionPanel();
            }

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
                            cell.SetInfo(PND.playerName, PND.killNo, i);
                            break;
                        case 1:
                            cell.SetInfo(PND.playerName, PND.deathNo, i);
                            break;
                        case 2:
                            cell.SetInfo(PND.playerName, (float)Math.Round(PND.surviveTime, 2), i);
                            break;
                        case 3:
                            cell.SetInfo(PND.playerName, PND.contribution, i);
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