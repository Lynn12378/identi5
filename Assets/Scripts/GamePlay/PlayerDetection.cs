using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Identi5.GamePlay.Player;

namespace Identi5.GamePlay
{
    public class PlayerDetection : NetworkBehaviour
    {
        public List<PlayerController> playerInCollider = new List<PlayerController>();
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.CompareTag("Player"))
            {
                var player = collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    playerInCollider.Add(player);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if(collider.CompareTag("Player"))
            {
                var player = collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    playerInCollider.Remove(player);
                }
            }
        }
    }
}