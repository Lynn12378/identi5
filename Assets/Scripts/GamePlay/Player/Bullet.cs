using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private AudioSource source;
        [Networked] private TickTimer life { get; set; }
        [Networked] public PlayerRef playerRef { get; private set; }

        public void Init(Vector2 mousePosition)
        {
            source.Play();
            SetPlayerRef_RPC();
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
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRef = Runner.LocalPlayer;
		}

        #region - OnTrigger -
        [SerializeField] private PlayerController playert;
        private void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.GetComponent<PlayerController>();
            var zombie = collider.GetComponent<Zombie>();
            var livings = collider.GetComponent<Livings>();

            if(collider.CompareTag("MapCollision") || collider.CompareTag("Building"))
            {
                if(playerRef == Runner.LocalPlayer)
                {
                    GameMgr.playerOutputData.bulletOnCollisions++;
                }
                Runner.Despawn(Object);
            }
            if(player != null)
            {
                playert= player;
                if(player.GetPND().playerRef == playerRef){return;}
                if(player.GetPND().teamID < 1 || player.GetPND().teamID != GameMgr.Instance.PNDList[playerRef].teamID)
                {
                    player.TakeDamage(10);
                }
                Runner.Despawn(Object);
            }
            else if(zombie != null)
            {
                zombie.SetZombieHP_RPC(zombie.Hp - 10);
                if(zombie.Hp <= 0)
                {
                    GameMgr.Instance.PNDList[playerRef].AddKillNo_RPC();
                    zombie.DespawnZombie_RPC();
                }
                Runner.Despawn(Object);
            }
            else if(livings != null)
            {
                livings.SetLivingsHP_RPC(livings.Hp - 10);
                if(playerRef == Runner.LocalPlayer)
                {
                    GameMgr.playerOutputData.bulletOnLiving++;
                }
                if(livings.Hp <= 0)
                {
                    livings.DespawnLivings_RPC();
                }
                Runner.Despawn(Object);
            }
            
        }
        #endregion
    }
}