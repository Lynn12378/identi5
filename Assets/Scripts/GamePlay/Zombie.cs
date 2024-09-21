using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;

using Identi5.GamePlay.Player;
using Identi5.GamePlay.Spawner;

namespace Identi5.GamePlay
{
    public class Zombie : NetworkBehaviour
    {
        #region -Properties -
        public enum ZombieType
        {
            HighDamage,
            HighHP,
            HighSpeed,
            Normal,
        }
        public ZombieType zombieType;
        private ChangeDetector changes;
        private int maxHP = 100;
        private int directDamage = 10;
        private int damageOverTime = 1;
        private float damageInterval = 1.5f;
        private float moveSpeed = 1f;
        private Vector2 direction;
        private bool isMoving;
        public static float deltaTime;
        private float damageTimer = 0;
        private float moveTimer = 0;
        [SerializeField] private PlayerController player;
        [SerializeField] private NetworkRigidbody2D ZombieNetworkRigidbody = null;
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] private PlayerDetection playerDetection;
        [SerializeField] private ItemSpawner itemSpawner;
        [SerializeField] public Slider HPSlider;
        [Networked] public int ZombieID { get; set;}
        [Networked] public int HP { get; set; }
        #endregion

        #region - Initialize -
        public override void Spawned() 
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(GameObject.Find("SpawnSpace/Zombies").transform, false);
            SetZombieID_RPC(ZombieID);
            Init();
            damageTimer = 0;
            moveTimer = 0;
            direction = new Vector2(Random.insideUnitCircle.x, Random.insideUnitCircle.y);
        }

        public void Init()
        {
            zombieType = (ZombieType) ZombieID;
            spriteResolver.SetCategoryAndLabel("Zombie", zombieType.ToString());
            switch (zombieType)
            {
                case ZombieType.HighDamage:
                    maxHP = 50;
                    directDamage = 20;
                    damageOverTime = 3;
                    break;
                case ZombieType.HighHP:
                    maxHP = 150;
                    break;
                case ZombieType.HighSpeed:
                    maxHP = 80;
                    moveSpeed = 1.5f;
                    damageOverTime = 0;
                    break;
                case ZombieType.Normal:
                    break;
            }
            HPSlider.maxValue = maxHP;
            SetZombieHP_RPC(maxHP);
        }
        #endregion

        #region - Movement -
        public override void FixedUpdateNetwork()
        {
            damageTimer += Time.deltaTime;
            moveTimer += Time.deltaTime;
            if(playerDetection.playerInCollider.Count > 0)
            {
                FollowDirection();
            }
            else if(moveTimer > 3)
            {
                direction = new Vector2(Random.insideUnitCircle.x, Random.insideUnitCircle.y);
                moveTimer = 0;
            }

            ZombieNetworkRigidbody.Rigidbody.velocity = direction * moveSpeed;
 
        }
        private void FollowDirection()
        {
            if(playerDetection.playerInCollider[0]!=null)
            {
                direction = playerDetection.playerInCollider[0].transform.position - transform.position;
            }
        }
        #endregion

        #region - Collision - 
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.GetComponent<Shelter>() != null)
            {
                if(collider.GetComponent<Shelter>().playerRef == Runner.LocalPlayer)
                {
                    GameMgr.playerOutputData.zombieInShelteNo++;
                }
            }
        }
        public void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                player = collision.collider.GetComponent<PlayerController>();
                if(player != null && damageTimer < 2)
                {
                    player.TakeDamage(damageOverTime);
                    damageTimer = 0;
                }
            }
        }
        #endregion

        #region - RPC -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetZombieID_RPC(int ZombieID)
        {
            this.ZombieID = ZombieID;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetZombieHP_RPC(int hp)
        {
            HP = hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnZombie_RPC()
        {
            if (Random.value < 0.5f)
            {
                itemSpawner.gameObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    itemSpawner.RandomSpawn();
                }
            }
            Runner.Despawn(Object);
        }
        #endregion

        #region - OnChanged Events -
        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(ZombieID):
                        Init();
                        break;
                    case nameof(HP):
                        HPSlider.value = HP;
                        break;
                }
            }
        }
        #endregion
    }
}