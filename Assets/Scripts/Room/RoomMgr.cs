using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using TMPro;

using Identi5.DB;

namespace Identi5.Room
{
    public class RoomMgr : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameMgr gameMgr;
        private NetworkRunner runner;
        [SerializeField] private PlayerCell playerPrefab = null;
        [SerializeField] private Transform contentTrans = null;
		[SerializeField] private TMP_InputField roomName = null;

        private PlayerInfo playerInfo;
        private List<PlayerCell> playerCells = new List<PlayerCell>();

        private void Start()
        {
            gameMgr = GameMgr.Instance;
            runner = gameMgr.Runner;
            runner.AddCallbacks(this);
            
			roomName.text = runner.SessionInfo.Name;
            gameMgr.OnPIFListUpdated += UpdatePIFList;
        }

        private void OnDestroy()
        {
            gameMgr.OnPIFListUpdated -= UpdatePIFList;
        }

        public void UpdatePIFList()
        {
            var allReady = true;
            foreach(var cell in playerCells)
            {
                Destroy(cell.gameObject);
            }
            playerCells.Clear();
            foreach(var player in gameMgr.PIFList)
            {
                var cell = Instantiate(playerPrefab, contentTrans);
                var playerInfo = player.Value;

                cell.SetInfo(playerInfo.playerName, playerInfo.isReady);
                playerCells.Add(cell);

                if(!playerInfo.isReady)
                {
                    allReady = false;
                }
            }

            if(allReady)
            {
                StartGamePlay();
            }
        }

		public async void StartGamePlay()
        {
			if (runner.IsSceneAuthority) {
				await runner.UnloadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Room")));
  				await runner.LoadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("GamePlay")), LoadSceneMode.Additive);
			}
        }

        public void OnReadyBtnClicked()
        {
            if (gameMgr.PIFList.TryGetValue(runner.LocalPlayer, out PlayerInfo playerInfo))
            {
                playerInfo.SetIsReady_RPC();
            }
        }

        private void Leave()
        {
            if (gameMgr.PIFList.TryGetValue(runner.LocalPlayer, out PlayerInfo playerInfo))
            {
                runner.Despawn(playerInfo.Object);
            }

            runner.Shutdown();
            SceneManager.LoadScene("Lobby");
        }

        public void OnLeaveBtnClicked()
        {
            Leave();
        }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Leave();
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            gameMgr.UpdatePIFList();
        }
        
		#region /-- Unused Function --/
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
            
            public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnInput(NetworkRunner runner, NetworkInput input){}
            public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
            public void OnConnectedToServer(NetworkRunner runner){}
            public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,byte[] token){}
            public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
            public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
            public void OnSceneLoadDone(NetworkRunner runner){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}