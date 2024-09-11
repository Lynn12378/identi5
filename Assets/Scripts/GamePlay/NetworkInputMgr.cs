using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

namespace Identi5.GamePlay
{
    public class NetworkInputMgr : MonoBehaviour ,INetworkRunnerCallbacks
    {
        public void Start()
        {
            var runner = GameMgr.Instance.Runner;
            runner.AddCallbacks(this);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();
            float xInput = Input.GetAxisRaw("Horizontal");
            float yInput = Input.GetAxisRaw("Vertical");

            data.mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            data.movementInput = new Vector2(xInput, yInput);

            // Set NetworkButton
            data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse1));
            data.buttons.Set(InputButtons.SPACE, Input.GetKey(KeyCode.Space));
            data.buttons.Set(InputButtons.TALK, Input.GetKey(KeyCode.Tab));
            data.buttons.Set(InputButtons.RELOAD, Input.GetKey(KeyCode.T));
            data.buttons.Set(InputButtons.PET, Input.GetKey(KeyCode.P));
    
            input.Set(data);
        }

        #region /-- Unused Function --/
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
            public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
            public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
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