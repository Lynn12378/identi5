using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        private PlayerRef playerRef;
        private float bulletSpeed = 1f;
        private float bulletTime = 1f;
        [Networked] private TickTimer life { get; set; }

        // private GamePlayManager gamePlayManager;

        // private void Start()
        // {
        //     gamePlayManager = GamePlayManager.Instance;
        // }

        public void Init(Vector2 mousePosition)
        {
            playerRef = Runner.LocalPlayer;
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            mousePosition = mousePosition.normalized;
            transform.Translate(Vector2.zero);
        }

        public override void FixedUpdateNetwork()
        {
            transform.Translate(Vector2.right * bulletSpeed);
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }

        #region - OnTrigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.GetComponent<PlayerController>();
            // var enemy = collider.GetComponent<Enemy>();
            // var livings = collider.GetComponent<Livings>();

            if(player != null)
            {
                if(player.PND.teamID == -1 || player.PND.teamID != GameMgr.Instance.PNDList[playerRef].teamID)
                {
                    player.TakeDamage(10);
                    
                }
            }
            else
            {
                return;
            }
            Runner.Despawn(Object);
            // else if()
        //     if(collider.CompareTag("MapCollision"))
        //     {
        //         foreach (var kvp in gamePlayManager.playerOutputList)
        //         {
        //             PlayerRef playerRefKey = kvp.Key;
        //             PlayerOutputData playerOutputDataValue = kvp.Value;

        //             if (playerRef == playerRefKey)
        //             {
        //                 playerOutputDataValue.bulletCollision++;
        //             }
        //         }

        //         AudioManager.Instance.Play("Hit");
        //         Runner.Despawn(Object);
        //     }

        //     

        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage(damage, playerRef);
        //         AudioManager.Instance.Play("Hit");
        //         Runner.Despawn(Object);
        //     }
        //     else if(player != null)                  ////////////////////////// team will not shoot each other
        //     {
        //         if(player.GetPlayerNetworkData().playerRef != playerRef)
        //         {
        //             if(player.GetPlayerNetworkData().teamID != shooter.GetPlayerNetworkData().teamID || shooter.GetPlayerNetworkData().teamID == -1)
        //             {
        //                 player.TakeDamage(damage, playerRef);
        //             }
        //             AudioManager.Instance.Play("Hit");
        //             Runner.Despawn(Object);
        //         }
        //     }
        //     else if(collider.CompareTag("Livings"))
        //     {
        //         livings.TakeDamage(damage, playerRef);

        //         foreach (var kvp in gamePlayManager.playerOutputList)
        //         {
        //             PlayerRef playerRefKey = kvp.Key;
        //             PlayerOutputData playerOutputDataValue = kvp.Value;

        //             if (playerRef == playerRefKey)
        //             {
        //                 playerOutputDataValue.bulletCollisionOnLiving++;
        //             }
        //         }
        //         AudioManager.Instance.Play("Hit");
        //         Runner.Despawn(Object);
        //     }
        }
        #endregion
    }
}