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
        }

        #region - Start Game -
        private void Init()
        {
            var PIF = gameMgr.PIFList[localPlayer];
            PND = gameMgr.PNDList[localPlayer];
            // var POD = gameMgr.playerOutputList[player];

            PND.SetPlayerInfo_RPC(PIF.playerId, PIF.playerName);
            PND.SetColorList(PIF.colorList);
            PND.SetOutfits(PIF.outfits);
            // POD.SetPlayerId_RPC(PIF.playerId);

            PIF.Despawned();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            gameMgr.OnInPNDListUpdated += UpdatedPNDList;
            gameMgr.OnTeamListUpdated += UpdatedTeamList;
            gameMgr.OnMessageListUpdated += UpdatedMessageList;

            localPlayer = runner.LocalPlayer;
            var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, localPlayer);

            runner.SetPlayerObject(localPlayer, playerObject);
            Camera.main.transform.SetParent(playerObject.transform);
            Init();
            
            // var spawner = FindObjectOfType<Spawner>();
            // spawner.StartSpawners();
        }

        #endregion
        #region - PND -
                public void UpdatedPNDList()
                {
                    // foreach (var gamePlayer in gameMgr.PNDList.Values)
                    // {
                    //     if(gamePlayer == PND || PND.teamID == -1) continue;

                    //     if (gamePlayer.teamID == PND.teamID)
                    //     {
                    //         gamePlayer.minimapIcon.SetActive(true);
                    //     }
                    //     else
                    //     {
                    //         gamePlayer.minimapIcon.SetActive(false);
                    //     }
                    // }
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

            // foreach(var player in gameMgr.playerOutputList)
            // {
            //     if(player.Key == runner.LocalPlayer) player.Value.sendMessageNo++;
            // }
            messageTxt.text = "";
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
        }

        public void LeaveTeam()
        {
            PND.SetPlayerTeamID_RPC(0);
            OnActivePanel();
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
        #endregion
       
		#region /-- Unused Function --/
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
            public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
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