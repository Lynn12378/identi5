using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;
using TMPro;

namespace Identi5.GamePlay
{
    public class Livings : NetworkBehaviour
    {
        public enum LivingsType
        {
            Dog1,
            Dog2,
            Dog3,
            Dog4,
            Cat1,
            Cat2,
            Cat3,
            Cat4,
            Squirrel,
            Frog,
            Goose,
        }
        private ChangeDetector changes;
        public LivingsType livingsType;
        private Vector2 direction;
        private bool isMoving;
        [SerializeField] private NetworkRigidbody2D livingsNetworkRigidbody = null;
        [SerializeField] private SpriteResolver SRV1;
        [SerializeField] private SpriteResolver SRV2;
        [SerializeField] private SpriteResolver SRV3;
        [SerializeField] private GameObject obj;
        [SerializeField] private Transform trans;
        [SerializeField] private TMP_Text Txt = null;
        [SerializeField] public Slider HpSlider;
        [Networked] public int livingsID { get; set;}
        [Networked] public int Hp { get; set; }
        
        #region - Initialize -
        public override void Spawned() 
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(GameObject.Find("GPManager/Livings").transform, false);
            SetLivingsID_RPC(livingsID);
            Init();
            SetLivingsHP_RPC(30);
            StartCoroutine(RandomMovement());
        }

        public void Init()
        {
            livingsType = (LivingsType) livingsID;
            Txt.text = GetTxt();
            SRV1.SetCategoryAndLabel("livings1", livingsType.ToString());
            SRV2.SetCategoryAndLabel("livings2", livingsType.ToString());
            SRV3.SetCategoryAndLabel("livings3", livingsType.ToString());
        }

        public string GetTxt()
        {
            switch(livingsType)
            {
                case LivingsType.Dog1:
                case LivingsType.Dog2:
                case LivingsType.Dog3:
                case LivingsType.Dog4:
                    return "汪!";
                case LivingsType.Cat1:
                case LivingsType.Cat2:
                case LivingsType.Cat3:
                case LivingsType.Cat4:
                    return "喵嗚";
                case LivingsType.Squirrel:
                    return "唧唧";
                case LivingsType.Goose:
                    return "嘎嘎";
                default:
                    return "";
            }
        }
        #endregion

        #region - Movement -
        private IEnumerator RandomMovement()
        {
            while (true)
            {
                direction = new Vector2(Random.insideUnitCircle.x, Random.insideUnitCircle.y);
                isMoving = true;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
                isMoving = false;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                livingsNetworkRigidbody.Rigidbody.velocity = direction * 1.0f;
                trans.rotation = (direction.x > 0) ? new Quaternion(0, 180 , 0, 1) : new Quaternion(0, 0 , 0, 1);
            }
        }
        #endregion

        #region - Interact -
        public static float deltaTime;
        private float timer = 0;
        public void Interact()
        {
            timer = 0;
            obj.SetActive(true);
        }
        void Update()
        {
            timer += Time.deltaTime;
            if(timer > 5)
            {
                obj.SetActive(false);
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