using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using TMPro;

using DEMO.DB;
using DEMO.UI;

namespace DEMO.Manager
{
    public class GamePlayManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public Dictionary<PlayerRef, PlayerNetworkData> gamePlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();
		[SerializeField] private NetworkRunner networkInstance = null;
        private void Start()
        {
            networkInstance.AddCallbacks(this);
        }

        public void UpdatedGamePlayer()
        {
            //組隊、排行榜 應該類似RoomManager 的 UpdatePlayerList()

            //列印所有玩家資料
            foreach(var player in gamePlayerList)
            {
                Debug.Log(player.Value.playerRef);
            }
        }


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
            public void OnSceneLoadDone(NetworkRunner runner){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}