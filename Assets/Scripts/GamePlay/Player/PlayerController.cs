using UnityEngine;
using Fusion;
using TMPro;

namespace Identi5.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private GameMgr gameMgr;
        // private Shelter shelter;
        private NetworkButtons buttonsPrevious;
        private float surviveTime = 0f;

        [SerializeField] private TMP_Text playerNameTxt = null;
        [SerializeField] private PlayerNetworkData playerNetworkData;
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        // [SerializeField] private PlayerVoiceDetection voiceDetection = null;
        
        // [SerializeField] private PlayerOutputData playerOutputData;
        // private MapInteractionManager mapInteractionManager;

        // private Item itemInRange = null;
        // private IInteractable interactableInRange = null;
        // private bool isInteracting = false;
        // [SerializeField] private bool shopInRange = false;
        [Networked] private TickTimer shelterTimer { get; set; }

        
        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;
            
            // mapInteractionManager = FindObjectOfType<MapInteractionManager>();
            playerNetworkData.uIManager = FindObjectOfType<UIManager>();
            playerNetworkData.uIManager.SetPlayerNameTxt(playerNameTxt);
        }

        private void Respawn() 
        {
            surviveTime = 0f;
            playerNetworkData.Init();
            transform.position = Vector3.zero;
        }

        // public void Restart()
        // {
        //     playerOutputData.restartNo++;
        //     playerOutputData.AddDeathNo_RPC();

        //     transform.position = Vector3.zero;

        //     playerNetworkData.Restart();
        //     playerOutputData.Restart();
            
        //     surviveTime = 0f;
        // }

        public override void FixedUpdateNetwork()
        {
            // surviveTime += Runner.DeltaTime;
            // if(surviveTime > playerOutputData.surviveTime)
            // {
            //     playerOutputData.SetSurviveTime_RPC(surviveTime);
            // }

            // if(playerNetworkData.HP <= 0 || playerNetworkData.foodAmount <= 0)
            // {
            //     playerOutputData.AddDeathNo_RPC();
            //     Respawn();
            // }

            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            // if(playerNetworkData.playerRef == Runner.LocalPlayer)
            // {
            //     uIManager.UpdateMinimapArrow(gameObject.transform);
            // }

            // if (shelter != null)
            // {
            //     if (shelterTimer.Expired(Runner))
            //     {
            //         playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + 10);
            //         shelterTimer = TickTimer.CreateFromSeconds(Runner, 5);
            //     }
            // }

            // voiceDetection.AudioCheck();
        }

        // #region - Getter -
        // public PlayerVoiceDetection GetPlayerVoiceDetection()
        // {
        //     return voiceDetection;
        // }

        // public PlayerNetworkData GetPlayerNetworkData()
        // {
        //     return playerNetworkData;
        // }
        // #endregion

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
                if(playerNetworkData.bulletAmount > 0)
                {
                    attackHandler.Shoot(data.mousePosition);
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount - 1);
                }
                // else
                // {
                //     gamePlayManager.ShowWarningBox("請填充子彈。");
                // }
            }

            // if (pressed.IsSet(InputButtons.SPACE))
            // {
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
            // }

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
            //     playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + 5);
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
        //         playerNetworkData.SetPlayerCoin_RPC(playerNetworkData.coinAmount + 10);
        //         AudioManager.Instance.Play("Pickup");
        //         itemInRange.DespawnItem_RPC();
        //     }

        //     // If item not coin and enough space    
        //     if (playerNetworkData.itemList.Count < 12 && item.itemType != Item.ItemType.Coin)
        //     {
        //         playerNetworkData.itemList.Add(item);
        //         playerNetworkData.UpdateItemList();

        //         if (item.itemId >= 5 && item.itemId <= 13)
        //         {
        //             playerOutputData.placeholderNo++;
        //         }

        //         AudioManager.Instance.Play("Pickup");
        //         itemInRange.DespawnItem_RPC();
        //     }
        //     else if(playerNetworkData.itemList.Count >= 12)
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

        // #region - On Trigger -
        // private void OnTriggerEnter2D(Collider2D collider)
        // {
        //     // Check for interactable objects
        //     IInteractable interactable = collider.GetComponent<IInteractable>();
        //     if (interactable != null)
        //     {
        //         interactableInRange = interactable;
        //     }

        //     // Check for items
        //     Item item = collider.GetComponent<Item>();
        //     if (item != null)
        //     {
        //         itemInRange = item;
        //     }

        //     // Check for shelter
        //     Shelter shelterCollider = collider.GetComponent<Shelter>();
        //     if (shelterCollider != null)
        //     {
        //         shelterTimer = TickTimer.CreateFromSeconds(Runner, 0);
        //         shelter = shelterCollider;
        //         playerNetworkData.SetShelter(shelter);
        //     }
        // }

        // private void OnTriggerExit2D(Collider2D collider)
        // {
        //     // Check for interactable objects
        //     IInteractable interactable = collider.GetComponent<IInteractable>();
        //     if (interactable != null && interactable == interactableInRange)
        //     {
        //         interactableInRange = null;
        //     }

        //     // Check for items
        //     Item item = collider.GetComponent<Item>();
        //     if (item != null && item == itemInRange)
        //     {
        //         itemInRange = null;
        //     }

        //     // Check for shelter
        //     Shelter shelterCollider = collider.GetComponent<Shelter>();
        //     if (shelterCollider != null && shelterCollider == shelter)
        //     {
        //         shelter = null;
        //         playerNetworkData.SetShelter(shelter);
        //     }
        // }
        // #endregion

        // #region - OnCollision -
        // private void OnCollisionEnter2D(Collision2D collision)
        // {
        //     if(collision.collider.CompareTag("MapCollision"))
        //     {
        //         playerOutputData.collisionNo++;
        //         Debug.Log(playerNetworkData.playerRefString + "'s collision no. is: " + playerOutputData.collisionNo.ToString());
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
        // #endregion

        #region - Player HP -
        public void TakeDamage(int damage)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
        }

        // public void TakeDamage(int damage, PlayerRef shooter)
        // {
        //     playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
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
        #endregion
    }
}