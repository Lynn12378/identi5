using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

using Fusion;
using Fusion.Sockets;

using FusionHelpers;
using Unity.VisualScripting;

namespace DEMO.Player
{
    public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameManager gameManager = null;
        private NetworkRunner networkRunner = null;

        [SerializeField] public GameObject cameraPrefab; //prefab of the camera you want to instance
        private Camera playerCam; //this is gonna be a reference to the camera that looks at the player

        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private Inventory playerInventoryPrefab = null;

        private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();

        private async void Start()
        {
            gameManager = GameManager.Instance;

            networkRunner = gameManager.Runner;

            networkRunner.AddCallbacks(this);

            await SpawnAllPlayers();
        }

        private async Task SpawnAllPlayers()
        {
            foreach(var player in gameManager.playerList.Keys)
            {
                if (player == networkRunner.LocalPlayer){
                    playerCam = Instantiate(cameraPrefab).GetComponent<Camera>(); 
                    playerCam.transform.position = Vector3.back * 10;//transform.LookAt((Vector3.back * 10));
                }

                Vector3 spawnPosition = Vector3.zero;
                NetworkObject networkPlayerObject = await networkRunner.SpawnAsync(playerPrefab, spawnPosition, Quaternion.identity, player);

                networkRunner.SetPlayerObject(player, networkPlayerObject);

                Inventory playerInventory = Instantiate(playerInventoryPrefab);
                PlayerInventoryManager.instance.SetPlayerInventory(player, playerInventory);
                Debug.Log("Inventory for player "+ player +" created.");
                PlayerInventoryManager.instance.InitializeInventory();
                Debug.Log("Inventory slot and UI initialized.");

                playerList.Add(player, networkPlayerObject);
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Vector3 spawnPosition = Vector3.zero;
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            runner.SetPlayerObject(player, networkPlayerObject);

            playerList.Add(player, networkPlayerObject);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (playerList.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                playerList.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();

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