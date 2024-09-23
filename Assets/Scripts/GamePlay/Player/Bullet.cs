using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        private GameMgr gameMgr;
        private PlayerRef shooterPlayerRef;
        [SerializeField] private AudioClip clip;
        [Networked] private TickTimer life { get; set; }
        [Networked, OnChangedRender(nameof(OnPlayerChange))]
        public PlayerRef playerRef { get; private set; }

        public void Init(Vector2 mousePosition)
        {
            gameMgr = GameMgr.Instance;
            gameMgr.source.clip = clip;
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

        private void OnPlayerChange()
        {
            shooterPlayerRef = playerRef;
        }

        #region - OnTrigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.GetComponent<PlayerController>();
            var zombie = collider.GetComponent<Zombie>();
            var livings = collider.GetComponent<Livings>();

            if(shooterPlayerRef == null){return;}
            if(collider.CompareTag("MapCollision"))
            {
                if(shooterPlayerRef == Runner.LocalPlayer)
                {
                    GameMgr.playerOutputData.bulletOnCollisions++;
                }
                DespawnBullet_RPC();
            }
            else if(player != null)
            {
                if(player.GetPND().playerRef == shooterPlayerRef){return;}
                if(player.GetPND().teamID < 1 || player.GetPND().teamID != GameMgr.Instance.PNDList[shooterPlayerRef].teamID)
                {
                    player.TakeDamage(10);
                    if(shooterPlayerRef == Runner.LocalPlayer)
                    {
                        GameMgr.playerOutputData.bulletOnPlayer++;
                    }
                }
                DespawnBullet_RPC();
            }
            else if(zombie != null)
            {
                zombie.SetZombieHP_RPC(zombie.HP - 10);
                if(zombie.HP <= 0)
                {
                    GameMgr.Instance.PNDList[shooterPlayerRef].AddKillNo_RPC();
                    zombie.DespawnZombie_RPC();
                }
                DespawnBullet_RPC();
            }
            else if(livings != null)
            {
                livings.SetLivingsHP_RPC(livings.HP - 10);
                if(shooterPlayerRef == Runner.LocalPlayer)
                {
                    GameMgr.playerOutputData.bulletOnLiving++;
                }
                if(livings.HP <= 0)
                {
                    livings.DespawnLivings_RPC();
                }
                DespawnBullet_RPC();
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRef = Runner.LocalPlayer;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnBullet_RPC()
        {
            Runner.Despawn(Object);
        }
        #endregion
    }
}