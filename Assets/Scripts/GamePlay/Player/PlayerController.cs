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
        private GameObject icon;

        [SerializeField] private PlayerNetworkData PND;
        [SerializeField] private AudioClip clip;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform trans;
        [SerializeField] private SpriteRenderer weapon;
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerVoiceDetection voiceDetection = null;
        [Networked] private TickTimer RefillTimer { get; set; }
        [Networked] private TickTimer foodTimer { get; set; }
        [Networked, OnChangedRender(nameof(Flip))]
        private bool isFlip { get; set; }
        [Networked, OnChangedRender(nameof(MicOpen))]
        private bool micOpen { get; set; }
        [Networked, OnChangedRender(nameof(Animation))]
        private int animationID { get; set; }
        
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
            PND.uIManager = FindObjectOfType<UIManager>();
            foodTimer = TickTimer.CreateFromSeconds(Runner, 20);
            icon = voiceDetection.icon;
        }

        private void Respawn() 
        {
            PND.AddDeathNo_RPC();
            POD.deathNo++;
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
                if(surviveTime > PND.surviveTime)
                {
                    PND.SetSurviveTime_RPC(surviveTime);
                    POD.surviveTime = surviveTime;
                }
            }
            surviveTime += Runner.DeltaTime;
            if(PND.HP <= 0 || PND.foodAmount <= 0)
            {
                SetAnimationID_RPC(2);
                Respawn();
            }

            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            if(PND.playerRef == Runner.LocalPlayer)
            {
                PND.uIManager.UpdateMinimapArrow(gameObject.transform);
            }

            if (gameMgr.shelter != null)
            {
                if (RefillTimer.Expired(Runner))
                {
                    PND.SetPlayerHP_RPC(PND.HP + 10);
                    PND.SetPlayerBullet_RPC(PND.bulletAmount + 5);
                    RefillTimer = TickTimer.CreateFromSeconds(Runner, 1);
                }
            }
            voiceDetection.AudioCheck();
            SetIsMic_RPC(icon.activeSelf);
        }

        #region - Input -
        private async void ApplyInput(NetworkInputData data)
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(buttonsPrevious);
            buttonsPrevious = buttons;

            movementHandler.Move(data);
            movementHandler.SetRotation(data.mousePosition);
            animationID = (data.movementInput != Vector2.zero) ? 1 : 0;
            SetAnimationID_RPC(animationID);
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
                    gameMgr.source.clip = clip;
                    gameMgr.source.Play();

                    if(item.itemId == (int)Item.ItemType.Money)
                    {
                        PND.SetPlayerCoin_RPC(PND.coinAmount + 20);
                        item.DespawnItem_RPC();
                    }
                    else if(PND.itemList.Count < 12)
                    {
                        var itemPicked = Instantiate(item.gameObject).GetComponent<Item>();
                        PND.itemList.Add(itemPicked);
                        gameMgr.UpdateItemList();
                        if(itemPicked.itemId > 5)
                        {
                            POD.placeholderNo++;
                        }
                        item.DespawnItem_RPC();
                    }
                    else
                    {
                        gameMgr.dialogCell.SetInfo("物品欄位已滿");
                        POD.fullNo++;
                    }
                }
                
                else if(gameMgr.shelter != null)
                {
                    gameMgr.shelter.SetIsOpen_RPC();
                    if(gameMgr.shelter.IsOpen)
                    {
                        gameMgr.shelter.SetPlayerRef_RPC(Runner.LocalPlayer);
                    }
                }

                else if(livings != null)
                {
                    livings.Interact();
                    POD.interactNo++;
                }
                
                else if(building != null)
                {
                    gameMgr.docCell.SetInfo(building.GetDoc());
                    if(!POD.buildingVisit.Contains(building.buildingID))
                    {
                        POD.buildingVisit.Add(building.buildingID);
                    }
                }
            }
            if (pressed.IsSet(InputButtons.TALK))
            {
                voiceDetection.rec.TransmitEnabled = !voiceDetection.rec.TransmitEnabled;
            }
        }
        #endregion

        #region - On Trigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.GetComponent<Shelter>() != null)
            {
                RefillTimer = TickTimer.CreateFromSeconds(Runner, 0);
                gameMgr.shelter = collider.GetComponent<Shelter>();
            }
            if(collider.GetComponent<Building>() != null)
            {
                building = collider.GetComponent<Building>();
            }
            if(collider.GetComponent<Item>() != null)
            {
                item = collider.GetComponent<Item>();
            }
            if(collider.GetComponent<Livings>() != null)
            {
                livings = collider.GetComponent<Livings>();
            }
        }
        private void OnTriggerExit2D(Collider2D collider)
        {
            if(gameMgr.shelter != null && collider.GetComponent<Shelter>() == gameMgr.shelter)
            {
                gameMgr.shelter = null;
            }
            if(building != null && collider.GetComponent<Building>() == building)
            {
                building = null;
            }
            if(item != null && collider.GetComponent<Item>() == item)
            {
                item = null;
            }
            if(livings != null && collider.GetComponent<Livings>() == livings)
            {
                livings = null;
            }
            
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("MapCollision"))
            {
                POD.collisionMapNo++;
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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetIsMic_RPC(bool micOpen)
        {
            this.micOpen = micOpen;
		}

        private void MicOpen()
        {
            icon.SetActive(micOpen);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetAnimationID_RPC(int animationID)
        {
            this.animationID = animationID;
		}

        private void Animation()
        {
            animator.SetInteger("animationID", animationID);
        }
    }
}