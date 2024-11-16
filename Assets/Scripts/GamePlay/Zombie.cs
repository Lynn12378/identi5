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
        private int maxHP = 105;
        private int directDamage = 3;
        private int damageOverTime = 1;
        private float damageInterval = 0.5f;
        private float moveSpeed = 1f;
        private Vector2 direction;
        private bool isMoving;
        public static float deltaTime;
        private float damageTimer = 0;
        private float moveTimer = 0;
        [SerializeField] private PlayerController target;
        [SerializeField] private NetworkRigidbody2D ZombieNetworkRigidbody = null;
        [SerializeField] private SpriteResolver SRV1;
        [SerializeField] private SpriteResolver SRV2;
        [SerializeField] private SpriteResolver SRV3;
        [SerializeField] private Transform trans;
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
            direction = new Vector2(Random.insideUnitCircle.x, Random.insideUnitCircle.y);
        }

        public void Init()
        {
            zombieType = (ZombieType) ZombieID;
            switch (zombieType)
            {
                case ZombieType.HighDamage:
                    maxHP = 55;
                    directDamage = 5;
                    damageOverTime = 3;
                    SRV1.SetCategoryAndLabel("Zombie1", zombieType.ToString());
                    SRV2.SetCategoryAndLabel("Zombie2", zombieType.ToString());
                    SRV3.SetCategoryAndLabel("Zombie3", zombieType.ToString());
                    break;
                case ZombieType.HighHP:
                    maxHP = 155;
                    SRV1.SetCategoryAndLabel("Zombie1", zombieType.ToString());
                    SRV2.SetCategoryAndLabel("Zombie2", zombieType.ToString());
                    SRV3.SetCategoryAndLabel("Zombie3", zombieType.ToString());
                    break;
                case ZombieType.HighSpeed:
                    maxHP = 85;
                    moveSpeed = 1.5f;
                    damageOverTime = 1;
                    SRV1.SetCategoryAndLabel("Zombie1", zombieType.ToString());
                    SRV2.SetCategoryAndLabel("Zombie2", zombieType.ToString());
                    SRV3.SetCategoryAndLabel("Zombie3", zombieType.ToString());
                    break;
                case ZombieType.Normal:
                    SRV1.SetCategoryAndLabel("Zombie1", zombieType.ToString());
                    SRV2.SetCategoryAndLabel("Zombie2", zombieType.ToString());
                    SRV3.SetCategoryAndLabel("Zombie3", zombieType.ToString());
                    break;
            }
            HPSlider.maxValue = maxHP;
            SetZombieHP_RPC(maxHP);
        }
        #endregion

        #region - Movement -
        public override void FixedUpdateNetwork()
        {
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
            trans.rotation = (direction.x > 0) ? new Quaternion(0, 0 , 0, 1) : new Quaternion(0, 180 , 0, 1);
            ZombieNetworkRigidbody.Rigidbody.velocity = direction * moveSpeed;

            if(target != null)
            {
                damageTimer += Time.deltaTime;
                if(damageTimer > damageInterval)
                {
                    target.TakeDamage(damageOverTime);
                    damageTimer = 0;
                }
            }
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
        private void OnTriggerStay2D(Collider2D collider)
        {
            var shelter = collider.GetComponent<Shelter>();
            if(shelter != null)
            {
                shelter.SetIsZombieInShelter_RPC(true);
            }
        } 

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                var player = collision.collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    target = player;
                    target.TakeDamage(directDamage);
                    damageTimer = 0;
                }
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                var player = collision.collider.GetComponent<PlayerController>();
                if (target != null && target == player)
                {
                    target = null;
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
                for (int i = 0; i < 3; i++)
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
                        HPSlider.maxValue = HP > HPSlider.maxValue ? HP : HPSlider.maxValue;
                        HPSlider.value = HP;
                        break;
                }
            }
        }
        #endregion
    }
}