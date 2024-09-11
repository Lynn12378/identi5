using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        private PlayerRef shooterPlayerRef;
        private float bulletSpeed = 1f;
        private float bulletTime = 1f;
        private int damage = 10;
        [Networked] private TickTimer life { get; set; }

        // private GamePlayManager gamePlayManager;

        // private void Start()
        // {
        //     gamePlayManager = GamePlayManager.Instance;
        // }

        public void Init(Vector2 mousePosition, PlayerRef shooterPlayerRef)
        {
            this.shooterPlayerRef = shooterPlayerRef;
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

        // #region - OnTrigger -
        // private void OnTriggerEnter2D(Collider2D collider)
        // {
        //     if(collider.CompareTag("MapCollision"))
        //     {
        //         foreach (var kvp in gamePlayManager.playerOutputList)
        //         {
        //             PlayerRef playerRefKey = kvp.Key;
        //             PlayerOutputData playerOutputDataValue = kvp.Value;

        //             if (shooterPlayerRef == playerRefKey)
        //             {
        //                 playerOutputDataValue.bulletCollision++;
        //             }
        //         }

        //         AudioManager.Instance.Play("Hit");
        //         Runner.Despawn(Object);
        //     }

        //     var enemy = collider.GetComponent<Enemy>();
        //     var player = collider.GetComponent<PlayerController>();
        //     var livings = collider.GetComponent<Livings>();

        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage(damage, shooterPlayerRef);
        //         AudioManager.Instance.Play("Hit");
        //         Runner.Despawn(Object);
        //     }
        //     else if(player != null)                  ////////////////////////// team will not shoot each other
        //     {
        //         if(player.GetPlayerNetworkData().playerRef != shooterPlayerRef)
        //         {
        //             if(player.GetPlayerNetworkData().teamID != shooter.GetPlayerNetworkData().teamID || shooter.GetPlayerNetworkData().teamID == -1)
        //             {
        //                 player.TakeDamage(damage, shooterPlayerRef);
        //             }
        //             AudioManager.Instance.Play("Hit");
        //             Runner.Despawn(Object);
        //         }
        //     }
        //     else if(collider.CompareTag("Livings"))
        //     {
        //         livings.TakeDamage(damage, shooterPlayerRef);

        //         foreach (var kvp in gamePlayManager.playerOutputList)
        //         {
        //             PlayerRef playerRefKey = kvp.Key;
        //             PlayerOutputData playerOutputDataValue = kvp.Value;

        //             if (shooterPlayerRef == playerRefKey)
        //             {
        //                 playerOutputDataValue.bulletCollisionOnLiving++;
        //             }
        //         }
        //         AudioManager.Instance.Play("Hit");
        //         Runner.Despawn(Object);
        //     }
        // }
        // #endregion
    }
}