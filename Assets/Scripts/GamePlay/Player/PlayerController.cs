using UnityEngine;
using Fusion;
using TMPro;

namespace Identi5.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private GameMgr gameMgr;
        private NetworkButtons buttonsPrevious;
        private float surviveTime = 0f;
        // private Shelter shelter;

        [SerializeField] private PlayerNetworkData PND;
        [SerializeField] private TMP_Text playerNameTxt = null;
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        // [SerializeField] private PlayerVoiceDetection voiceDetection = null;
        // private MapInteractionManager mapInteractionManager;
        // private Item itemInRange = null;
        // private IInteractable interactableInRange = null;
        // private bool isInteracting = false;
        // [SerializeField] private bool shopInRange = false;

        // [SerializeField] private PlayerOutputData POD;
        [Networked] private TickTimer HPTimer { get; set; }
        [Networked] private TickTimer foodTimer { get; set; }

        
        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;

            // mapInteractionManager = FindObjectOfType<MapInteractionManager>();
            PND.uIManager = FindObjectOfType<UIManager>();
            PND.uIManager.SetPlayerNameTxt(playerNameTxt);
            playerNameTxt.text = PND.playerName;
        }

        private void Respawn() 
        {
            PND.AddDeathNo_RPC();
            if(surviveTime > PND.surviveTime)
            {
                PND.SetSurviveTime_RPC(surviveTime);
            }
            surviveTime = 0f;
            PND.Init();
            transform.position = Vector3.zero;
        }

        public override void FixedUpdateNetwork()
        {
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

            // if (shelter != null)
            // {
            //     if (shelterTimer.Expired(Runner))
            //     {
            //         PND.SetPlayerHP_RPC(PND.HP + 10);
            //         shelterTimer = TickTimer.CreateFromSeconds(Runner, 5);
            //     }
            // }

            // voiceDetection.AudioCheck();
        }

        #region - Input -
        private async void ApplyInput(NetworkInputData data)
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(buttonsPrevious);
            buttonsPrevious = buttons;

            movementHandler.Move(data);
            movementHandler.SetRotation(data.mousePosition);

            if (pressed.IsSet(InputButtons.FIRE))
            {
                if(PND.bulletAmount > 0)
                {
                    attackHandler.Shoot(data.mousePosition);
                    PND.SetPlayerBullet_RPC(PND.bulletAmount - 1);
                }
                // else
                // {
                //     gamePlayManager.ShowWarningBox("請填充子彈。");
                // }
            }

            if (pressed.IsSet(InputButtons.SPACE))
            {
            //     if(itemInRange != null)
            //     {
            //         Pickup();
            //     }
            //     else if(shopInRange)
            //     {
            //         uIManager.OnOpenShopButton();
            //     }
            //     else if(interactableInRange != null && isInteracting == false)
            //     {
            //         Interact();
            //         isInteracting = true;
            //     }
            //     else if ( (interactableInRange != null && isInteracting) || interactableInRange == null)
            //     {
            //         EndInteract();
            //     }
            //     else
            //     {
            //         return;
            //     }
            }

            // if (pressed.IsSet(InputButtons.PET))
            // {
            //     if (isInteracting && mapInteractionManager.currentInteraction.interactionType == InteractionType.Pet)
            //     {
            //         mapInteractionManager.Pet(gameObject);
            //         playerOutputData.petNo++;
            //     }
            // }

            // if (pressed.IsSet(InputButtons.TALK))
            // {
            //     voiceDetection.rec.TransmitEnabled = !voiceDetection.rec.TransmitEnabled;
            // }

            // if (pressed.IsSet(InputButtons.RELOAD) && shelter != null)
            // {
            //     PND.SetPlayerBullet_RPC(PND.bulletAmount + 5);
            // }
        }
        #endregion

        // #region - Pickup Item -
        // private void Pickup()
        // {
        //     var item = itemInRange.GetComponent<Item>();

        //     // If item is coin, then just add to coinAmount
        //     if(item.itemType == Item.ItemType.Coin)
        //     {
        //         PND.SetPlayerCoin_RPC(PND.coinAmount + 10);
        //         AudioManager.Instance.Play("Pickup");
        //         itemInRange.DespawnItem_RPC();
        //     }

        //     // If item not coin and enough space    
        //     if (PND.itemList.Count < 12 && item.itemType != Item.ItemType.Coin)
        //     {
        //         PND.itemList.Add(item);
        //         PND.UpdateItemList();

        //         if (item.itemId >= 5 && item.itemId <= 13)
        //         {
        //             playerOutputData.placeholderNo++;
        //         }

        //         AudioManager.Instance.Play("Pickup");
        //         itemInRange.DespawnItem_RPC();
        //     }
        //     else if(PND.itemList.Count >= 12)
        //     {
        //         playerOutputData.fullNo++;
        //         // gameMgr.ShowWarningBox("背包已滿，不能撿起物品。");
        //     }
        // }
        // #endregion

        // #region - Interact -
        // private void Interact()
        // {
        //     var interactable = interactableInRange;
        //     interactable.Interact();
        // }

        // private void EndInteract()
        // {
        //     mapInteractionManager.EndInteraction();
        //     isInteracting = false;
        // }
        // #endregion

        #region - On Trigger -
        private void OnTriggerStay2D(Collider2D collider)
        {
            // IInteractable interactable = collider.GetComponent<IInteractable>();
            // Item item = collider.GetComponent<Item>();
            // Shelter shelter = collider.GetComponent<Shelter>();
        }

        // #region - OnCollision -
        // private void OnCollisionEnter2D(Collision2D collision)
        // {
        //     if(collision.collider.CompareTag("MapCollision"))
        //     {
        //         playerOutputData.collisionNo++;
        //         Debug.Log(PND.playerRefString + "'s collision no. is: " + playerOutputData.collisionNo.ToString());
        //     }

        //     if(collision.collider.CompareTag("Shop"))
        //     {
        //         shopInRange = true;
        //     }
        // }

        // private void OnCollisionExit2D(Collision2D collision)
        // {
        //     if(collision.collider.CompareTag("Shop"))
        //     {
        //         shopInRange = false;
        //         uIManager.CloseShopPanel();
        //     }
        // }
        #endregion

        public void TakeDamage(int damage)
        {
            PND.SetPlayerHP_RPC(PND.HP - damage);
        }

        // public void TakeDamage(int damage, PlayerRef shooter)
        // {
        //     PND.SetPlayerHP_RPC(PND.HP - damage);
        //     AudioManager.Instance.Play("Hit");

        //     foreach (var kvp in gameMgr.playerOutputList)
        //     {
        //         PlayerRef playerRefKey = kvp.Key;
        //         PlayerOutputData playerOutputDataValue = kvp.Value;

        //         if (shooter == playerRefKey)
        //         {
        //             playerOutputDataValue.bulletCollisionOnLiving++;
        //         }
        //     }
        // }
    }
}