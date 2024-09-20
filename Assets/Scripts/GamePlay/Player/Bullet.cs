using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        private GameMgr gameMgr;
        [SerializeField] private AudioClip[] clips;
        [Networked] private TickTimer life { get; set; }
        [Networked] public PlayerRef playerRef { get; private set; }

        public void Init(Vector2 mousePosition)
        {
            gameMgr = GameMgr.Instance;
            gameMgr.source.clip = clips[0];
            gameMgr.source.Play();
            SetPlayerRef_RPC();
            life = TickTimer.CreateFromSeconds(Runner, 1f);
            mousePosition = mousePosition.normalized;
            transform.Translate(Vector2.zero);
        }

        public override void FixedUpdateNetwork()
        {
            transform.Translate(Vector2.right * 0.5f);
            if (life.Expired(Runner))
            {
                DespawnBullet_RPC();
            }
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRef = Runner.LocalPlayer;
		}

        #region - OnTrigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.GetComponent<PlayerController>();
            var zombie = collider.GetComponent<Zombie>();
            var livings = collider.GetComponent<Livings>();

            if(collider.CompareTag("MapCollision"))
            {
                if(playerRef == Runner.LocalPlayer)
                {
                    GameMgr.playerOutputData.bulletOnCollisions++;
                }
                gameMgr.source.clip = clips[1];
                gameMgr.source.Play();
                Runner.Despawn(Object);
            }
            if(player != null)
            {
                if(player.GetPND().playerRef == playerRef){return;}
                if(player.GetPND().teamID < 1 || player.GetPND().teamID != GameMgr.Instance.PNDList[playerRef].teamID)
                {
                    player.TakeDamage(10);
                    if(playerRef == Runner.LocalPlayer)
                    {
                        GameMgr.playerOutputData.bulletOnPlayer++;
                    }
                }
                gameMgr.source.clip = clips[1];
                gameMgr.source.Play();
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
                gameMgr.source.clip = clips[1];
                gameMgr.source.Play();
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
                gameMgr.source.clip = clips[1];
                gameMgr.source.Play();
                Runner.Despawn(Object);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnBullet_RPC()
        {
            Runner.Despawn(Object);
        }
        #endregion
    }
}