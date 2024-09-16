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
        }

        private void OnDestroy()
        {
            gameMgr.OnInPNDListUpdated -= UpdatedPNDList;
            gameMgr.OnTeamListUpdated -= UpdatedTeamList;
            gameMgr.OnMessageListUpdated -= UpdatedMessageList;
            gameMgr.OnItemListUpdated -= UpdateItemList;
            gameMgr.OnTeamListUpdated -= UpdatedPlayerMinimap;
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

            PIF.Despawned();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            gameMgr.OnInPNDListUpdated += UpdatedPNDList;
            gameMgr.OnTeamListUpdated += UpdatedTeamList;
            gameMgr.OnMessageListUpdated += UpdatedMessageList;
            gameMgr.OnItemListUpdated += UpdateItemList;
            gameMgr.OnTeamListUpdated += UpdatedPlayerMinimap;
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
                rankList.Clear();
                foreach(var cell in receiverCells)
                {
                    Destroy(cell.gameObject);
                }
                receiverCells.Clear();
                foreach(var player in gameMgr.PNDList.Values)
                {
                    rankList.Add(player);
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

            List<string> memberList = new List<string>();
            memberList.Add(PND.playerName);

            teamCell.SetMember(memberList);
            teamCell.SetInfo(++gameMgr.newTeamID);
            PND.SetPlayerTeamID_RPC(gameMgr.newTeamID);
            OnActivePanel();
            POD.teamCreated++;
        }

        public void LeaveTeam()
        {
            PND.SetPlayerTeamID_RPC(0);
            OnActivePanel();
            POD.quitTeamNo++;
        }

        public void OnActivePanel()
        {
            List<GameObject> panel = new List<GameObject>();
            if(PND.teamID == 0)
            {
                panel.Add(TeamListPanel);
            }else{
                panel.Add(MemberListPanel);
            }
            panelMgr.OnActivePanel(panel);
        }

        public void UpdatedTeamList()
        {
            if(PND.teamID == 0){return;}
            foreach(var cell in memberCells)
            {
                Destroy(cell.gameObject);
            }
            memberCells.Clear();
            foreach(var team in gameMgr.teamList)
            {
                if(team.teamID == PND.teamID);
                {
                    var members = new List<string>();
                    teamTxt.text = $"隊伍 {PND.teamID}";
                    foreach(var member in team.member)
                    {
                        if(member == ""){break;}
                        var cell = Instantiate(memberCell);
                        memberCells.Add(cell);
                        cell.transform.SetParent(memberContentTrans, false);
                        cell.SetInfo(member);
                        members.Add(memberCell.playerName);
                    }
                    team.SetMember(members);
                }
                team.transform.SetParent(teamContentTrans, false);
                team.SetInfo(team.teamID);
            }
        }
        public void UpdatedPlayerMinimap()
        {
            foreach (var gamePlayer in gameMgr.PNDList.Values)
            {
                if(gamePlayer == PND || PND.teamID == -1) continue;

                if (gamePlayer.teamID == PND.teamID)
                {
                    gamePlayer.minimapIcon.SetActive(true);
                }
                else
                {
                    gamePlayer.minimapIcon.SetActive(false);
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
            public PlayerRef receiver;
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
                    case Item.ItemType.Bullet:
                        POD.remainBullet.Add(PND.bulletAmount);
                        PND.SetPlayerBullet_RPC(PND.bulletAmount + 10);
                        break;
                    case Item.ItemType.Food:
                        PND.SetPlayerFood_RPC(PND.foodAmount + 20);
                        break;
                    case Item.ItemType.Health:
                        POD.remainHP.Add(PND.HP);
                        PND.SetPlayerHP_RPC(PND.HP + 20);                    
                        break;
                    case Item.ItemType.Wood:
                        if(gameMgr.shelter != null)
                        {
                            gameMgr.shelter.SetDurability_RPC(gameMgr.shelter.durability + 10);
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
            [SerializeField] private RankCell rankCell;
            [SerializeField] private Transform rankContentTrans = null;
            List<PlayerNetworkData> rankList = new List<PlayerNetworkData>();
             List<float> scoreList = new List<float>();
            private List<RankCell> rankCells = new List<RankCell>();

            public void OnRankBtnCliked()
            {
                POD.rankClikedNo++;
            }
    
            public void SortKill()
            {
                rankList.Sort((a, b) => b.killNo.CompareTo(a.killNo));
                scoreList.Clear();
                foreach(var rank in rankList)
                {
                    scoreList.Add(rank.killNo);
                }
                gameMgr.UpdateRankList();
            }
            public void SortDeath()
            {
                rankList.Sort((b, a) => b.deathNo.CompareTo(a.deathNo));
                scoreList.Clear();
                foreach(var rank in rankList)
                {
                    scoreList.Add(rank.deathNo);
                }
                gameMgr.UpdateRankList();
            }
            public void SortSuvivetime()
            {
                rankList.Sort((a, b) => b.surviveTime.CompareTo(a.surviveTime));
                scoreList.Clear();
                foreach(var rank in rankList)
                {
                    scoreList.Add(rank.surviveTime);
                }
                gameMgr.UpdateRankList();
            }
            public void SortContribution()
            {
                rankList.Sort((a, b) => b.contribution.CompareTo(a.contribution));
                scoreList.Clear();
                foreach(var rank in rankList)
                {
                    scoreList.Add(rank.contribution);
                }
                gameMgr.UpdateRankList();
            }
            
            public void UpdateRankList()
            {
                foreach(var cell in rankCells)
                {
                    Destroy(cell.gameObject);
                }
                rankCells.Clear();
                foreach(var PND in rankList)
                {
                    var i = 1;
                    var cell = Instantiate(rankCell, rankContentTrans);
                    cell.SetInfo(PND.playerName,scoreList[i-1],i);
                    rankCells.Add(cell);
                    i++;
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