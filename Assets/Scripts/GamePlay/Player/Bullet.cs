using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        private PlayerRef playerRef;
        [SerializeField] private AudioSource source;
        [Networked] private TickTimer life { get; set; }

        // private GamePlayManager gamePlayManager;

        // private void Start()
        // {
        //     gamePlayManager = GamePlayManager.Instance;
        // }

        public void Init(Vector2 mousePosition)
        {
            source.Play();
            playerRef = Runner.LocalPlayer;
            life = TickTimer.CreateFromSeconds(Runner, 1f);
            mousePosition = mousePosition.normalized;
            transform.Translate(Vector2.zero);
        }

        public override void FixedUpdateNetwork()
        {
            transform.Translate(Vector2.right * 1f);
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }

        #region - OnTrigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.GetComponent<PlayerNetworkData>();
            // var enemy = collider.GetComponent<Enemy>();
            // var livings = collider.GetComponent<Livings>();

            if(player != null)
            {
                if(player.teamID == -1 || player.teamID != GameMgr.Instance.PNDList[playerRef].teamID)
                {
                    player.SetPlayerHP_RPC(player.HP - 10);
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