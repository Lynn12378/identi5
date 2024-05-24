using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

using DEMO.Manager;

namespace DEMO.Player
{
    public class PlayerInputHandler : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameManager gameManager = null;
        private NetworkRunner networkRunner = null;
        private Camera playerCam;

        private async void Start()
        {
            gameManager = GameManager.Instance;
            networkRunner = gameManager.Runner;

            playerCam = FindObjectOfType<Camera>();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new PlayerInputData();

            float xInput = Input.GetAxisRaw("Horizontal");
            float yInput = Input.GetAxisRaw("Vertical");

            Vector2 mousePosition = playerCam.ScreenToWorldPoint(Input.mousePosition); // mouseInput
            mousePosition = mousePosition - new Vector2(playerCam.transform.position.x, playerCam.transform.position.y);

            data.movementInput = new Vector2(xInput, yInput);
            data.mousePosition = mousePosition;
            data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0)); //Set NetworkButton
            data.buttons.Set(InputButtons.PICKUP, Input.GetKey(KeyCode.Mouse1)); // Set NetworkButton
            data.buttons.Set(InputButtons.TESTDAMAGE, Input.GetKey(KeyCode.Space)); // Set NetworkButton
    
            input.Set(data);
        }

        #region /-- Unused Function --/
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}

            public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
            public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
            public void OnConnectedToServer(NetworkRunner runner) { }
            public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
            public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
            public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
            public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
            public void OnSceneLoadDone(NetworkRunner runner) { }
            public void OnSceneLoadStart(NetworkRunner runner) { }
            public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
            public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        #endregion
    }
}
