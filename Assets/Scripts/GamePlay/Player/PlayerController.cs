using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerNetworkData playerNetworkDataPrefab;
        // [SerializeField] private PlayerCanvasController playerCanvasController = null;
        private Shelter shelter;
        private PlayerNetworkData playerNetworkData;
        private NetworkButtons buttonsPrevious;

        public override void Spawned()
        {
            shelter = FindObjectOfType<Shelter>();
            playerNetworkData = playerNetworkDataPrefab;
        }

        private void Respawn() 
        {
            transform.position = Vector3.zero;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }
        }

        private void ApplyInput(NetworkInputData data)
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(buttonsPrevious);
            buttonsPrevious = buttons;

            movementHandler.Move(data);
            movementHandler.SetRotation(data.mousePosition);

            if (pressed.IsSet(InputButtons.FIRE))
            {
                if(playerNetworkData.bulletAmount > 0)
                {
                    attackHandler.Shoot(data.mousePosition);
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount - 1);
                }
                else
                {
                    Debug.Log("Not enough bullet!");
                }
            }

            if (pressed.IsSet(InputButtons.TESTDAMAGE))
            {
                Debug.Log($"TESTDAMAGE");
                playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - 10);
                Debug.Log(playerNetworkData.HP);
            }

            if (pressed.IsSet(InputButtons.REPAIR) && shelter != null && shelter.IsPlayerInRange())
            {
                shelter.Repair(20);
            }
        }
    }
}

