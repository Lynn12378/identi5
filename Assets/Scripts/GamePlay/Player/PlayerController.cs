using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private GameMgr gameMgr;
        private NetworkButtons buttonsPrevious;
        private float surviveTime = 0f;
        private PlayerOutputData POD;
        private Item item;
        private Livings livings;
        private Building building;

        [SerializeField] private PlayerNetworkData PND;
        [SerializeField] private Transform trans;
        [SerializeField] private SpriteRenderer weapon;
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerVoiceDetection voiceDetection = null;
        // private MapInteractionManager mapInteractionManager;
        // private Item itemInRange = null;
        // private IInteractable interactableInRange = null;
        // private bool isInteracting = false;
        // [SerializeField] private bool shopInRange = false;

        // [SerializeField] private PlayerOutputData POD;
        [Networked] private TickTimer HPTimer { get; set; }
        [Networked] private TickTimer foodTimer { get; set; }
        [Networked, OnChangedRender(nameof(Flip))]
        private bool isFlip { get; set; }
        
        public PlayerNetworkData GetPND()
        {
            return PND;
        }
        public PlayerVoiceDetection GetPlayerVoiceDetection()
        {
            return voiceDetection;
        }
        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;
            POD = GameMgr.playerOutputData;
            // mapInteractionManager = FindObjectOfType<MapInteractionManager>();
            PND.uIManager = FindObjectOfType<UIManager>();
            foodTimer = TickTimer.CreateFromSeconds(Runner, 20);
        }

        private void Respawn() 
        {
            PND.AddDeathNo_RPC();
            POD.deathNo++;

            if(surviveTime > PND.surviveTime)
            {
                PND.SetSurviveTime_RPC(surviveTime);
                POD.surviveTime = surviveTime;
            }
            PND.Init();
            surviveTime = 0f;
            transform.position = Vector3.zero;
        }

        public override void FixedUpdateNetwork()
        {
            if(foodTimer.Expired(Runner))
            {
                PND.SetPlayerFood_RPC(PND.foodAmount - 10);
                foodTimer = TickTimer.CreateFromSeconds(Runner, 20);
            }
            surviveTime += Runner.DeltaTime;
            if(PND.HP <= 0 || PND.foodAmount <= 0)
            {
                Respawn();
            }

            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            // if(PND.playerRef == Runner.LocalPlayer)
            // {
            //     uIManager.UpdateMinimapArrow(gameObject.transform);
            // }

            if (gameMgr.shelter != null)
            {
                if (HPTimer.Expired(Runner))
                {
                    PND.SetPlayerHP_RPC(PND.HP + 10);
                    HPTimer = TickTimer.CreateFromSeconds(Runner, 5);
                }
            }
            voiceDetection.AudioCheck();
        }

        #region - Input -
        private async void ApplyInput(NetworkInputData data)
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(buttonsPrevious);
            buttonsPrevious = buttons;

            movementHandler.Move(data);
            movementHandler.SetRotation(data.mousePosition);
            SetIsFlip_RPC((data.mousePosition.x - trans.position.x) < 0);

            if (pressed.IsSet(InputButtons.FIRE))
            {
                if(PND.bulletAmount > 0)
                {
                    attackHandler.Shoot(data.mousePosition);
                    PND.SetPlayerBullet_RPC(PND.bulletAmount - 1);
                }
                else
                {
                    gameMgr.dialogCell.SetInfo("請補充子彈");
                }
            }

            if (pressed.IsSet(InputButtons.SPACE))
            {
                if(item != null)
                {
                    if(item.itemId == (int)Item.ItemType.Coin)
                    {
                        PND.SetPlayerCoin_RPC(PND.coinAmount);
                        item.DespawnItem_RPC();
                    }
                    else if(PND.itemList.Count < 12)
                    {
                        var itemPicked = Instantiate(item.gameObject).GetComponent<Item>();
                        PND.itemList.Add(itemPicked);
                        gameMgr.UpdateItemList();
                        item.DespawnItem_RPC();
                    }
                    else
                    {
                        gameMgr.dialogCell.SetInfo("物品欄位已滿");
                        POD.fullNo++;
                    }
                }
                if(livings != null)
                {
                    livings.Interact();
                    POD.interactNo++;
                }
                if(building != null)
                {
                    gameMgr.docCell.SetInfo(building.GetDoc());
                }
            }
            if (pressed.IsSet(InputButtons.TALK))
            {
                voiceDetection.rec.TransmitEnabled = !voiceDetection.rec.TransmitEnabled;
            }
        }
        #endregion

        #region - On Trigger -
        private void OnTriggerStay2D(Collider2D collider)
        {
            gameMgr.shelter = collider.GetComponent<Shelter>();
            item = collider.GetComponent<Item>();
            livings = collider.GetComponent<Livings>();
            building = collider.GetComponent<Building>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("MapCollision"))
            {
                POD.collisionNo++;
            }
        }
        #endregion

        public void TakeDamage(int damage)
        {
            PND.SetPlayerHP_RPC(PND.HP - damage);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetIsFlip_RPC(bool isFlip)
        {
            this.isFlip = isFlip;
		}

        private void Flip()
        {
            trans.rotation = isFlip ? new Quaternion(0, 0 , 0, 1) : new Quaternion(0, 180 , 0, 1);
            weapon.flipY = isFlip;
        }
    }
}