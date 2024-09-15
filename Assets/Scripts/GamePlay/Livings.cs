using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;

namespace Identi5.GamePlay
{
    public class Livings : NetworkBehaviour
    {
        public enum LivingsType
        {
            Cat1,
            Cat2,
            Cat3,
            Cat4,
            Dog1,
            Dog2,
            Dog3,
            Dog4,
            Goose,
            Squirrel,
            Frog,
        }
        private ChangeDetector changes;
        public LivingsType livingsType;
        [SerializeField] private NetworkRigidbody2D livingsNetworkRigidbody = null;

        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public Slider HpSlider;
        [SerializeField] GameObject love;
    
        [Networked] public int livingsID { get; set;}
        [Networked] public int Hp { get; set; }

        private Vector3 moveDirection;
        private bool isMoving = false;
        public bool isInteracting = false;

        #region - Initialize -

        public override void Spawned() 
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            var livingsTransform = GameObject.Find("GPManager/Livings");
            transform.SetParent(livingsTransform.transform, false);
            SetLivingsID_RPC(livingsID);
            Init();
            SetLivingsHP_RPC(30);
            StartCoroutine(RandomMovement());
        }

        public void Init()
        {
            livingsType = (LivingsType) livingsID;
            spriteResolver.SetCategoryAndLabel("livings", livingsType.ToString());            
        }
        #endregion

        #region - Movement -
        private IEnumerator RandomMovement()
        {
            while (true)
            {
                moveDirection = Random.insideUnitCircle.normalized;
                isMoving = true;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f)); // Move for a random duration
                isMoving = false;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f)); // Wait before next movement
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                Vector3 newPosition = transform.position + moveDirection * 0.2f * Time.fixedDeltaTime;
                livingsNetworkRigidbody.Rigidbody.MovePosition(newPosition);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MapCollision"))
            {
                moveDirection = -moveDirection;
            }
        }
        #endregion

        #region - Interact -
        public static float deltaTime;
        private float timer = 0;
        public void Interact()
        {
            timer = 0;
            love.SetActive(true);
        }
        void Update()
        {
            timer += Time.deltaTime;
            if(timer > 5 && love.activeSelf)
            {
                love.SetActive(false);
                timer = 0;
            }
        }
        #endregion

        #region - RPC -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetLivingsID_RPC(int livingsID)
        {
            this.livingsID = livingsID;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetLivingsHP_RPC(int hp)
        {
            Hp = hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnLivings_RPC()
        {
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
                    case nameof(livingsID):
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