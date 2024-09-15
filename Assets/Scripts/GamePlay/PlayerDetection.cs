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
                var PND = collider.GetComponent<PlayerController>();
                if (PND != null)
                {
                    playerInCollider.Add(PND);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if(collider.CompareTag("Player"))
            {
                var PND = collider.GetComponent<PlayerController>();
                if (PND != null)
                {
                    playerInCollider.Remove(PND);
                }
            }
        }
    }
}