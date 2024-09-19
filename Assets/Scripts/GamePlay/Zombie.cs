using System.Collections.Generic;
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
        #region - statics -
        public enum ZombieType
        {
            HighDamage,
            HighHP,
            HighSpeed,
            Normal,
        }
        private ChangeDetector changes;
        public ZombieType zombieType;
        private Vector2 direction;
        private int maxHp = 50;
        private int directDamage = 10;
        private int damageOverTime = 5;
        private float damageInterval = 3f;
        private float moveSpeed = 1f;
        [SerializeField] private NetworkRigidbody2D ZombieNetworkRigidbody = null;
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] private PlayerDetection playerDetection;
        [SerializeField] private ItemSpawner itemSpawner;
        [SerializeField] public Slider HpSlider;
        [Networked] public int ZombieID { get; set;}
        [Networked] public int Hp { get; set; }
        [Networked] private TickTimer damageTimer { get; set; }
        #endregion

        #region - Initialize -
        public override void Spawned() 
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(GameObject.Find("GPManager/Zombies").transform, false);
            SetZombieID_RPC(ZombieID);
            Init();
            damageTimer = TickTimer.CreateFromSeconds(Runner, 0.5f);
        }

        public void Init()
        {
            zombieType = (ZombieType) ZombieID;
            spriteResolver.SetCategoryAndLabel("Zombie", zombieType.ToString());
            switch (zombieType)
            {
                case ZombieType.HighDamage:
                    maxHp = 40;
                    directDamage = 20;
                    damageOverTime = 10;
                    Hp = maxHp;
                    break;
                case ZombieType.HighHP:
                    maxHp = 80;
                    Hp = maxHp;
                    break;
                case ZombieType.HighSpeed:
                    maxHp = 30;
                    Hp = maxHp;
                    moveSpeed = 1.5f;
                    damageOverTime = 0;
                    break;
                case ZombieType.Normal:
                    break;
            }
            HpSlider.maxValue = maxHp;
            SetZombieHP_RPC(maxHp);
        }
        #endregion

        #region - Patrol & Player Detect -
        public override void FixedUpdateNetwork()
        {
            if(Random.value > 0.2 && playerDetection.playerInCollider.Count > 0)
            {
                FollowDirection();
            }
            else
            {
                if(damageTimer.Expired(Runner))
                {
                    RandomDirection();
                    damageTimer = TickTimer.CreateFromSeconds(Runner, 0.5f);
                }
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
        private void RandomDirection()
        {
            direction = new Vector2(Random.insideUnitCircle.x, Random.insideUnitCircle.y);
        }
        #endregion

        #region - Collision - 
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.CompareTag("Shelter"))
            {
                var shelter = collider.GetComponent<Shelter>();
                if(shelter.playerRef == Runner.LocalPlayer)
                {
                    FindObjectOfType<PlayerOutputData>().zombieInShelteNo++;
                }
            }
        }
        public void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                var player = collision.collider.GetComponent<PlayerController>();
                if(player != null && damageTimer.Expired(Runner))
                {
                    player.TakeDamage(damageOverTime);
                    damageTimer = TickTimer.CreateFromSeconds(Runner, 2f);
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
            Hp = hp;
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
                    case nameof(Hp):
                        HpSlider.value = Hp;
                        break;
                }
            }
        }
        #endregion
    }
}