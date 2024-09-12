using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using Fusion;
using TMPro;

namespace Identi5.GamePlay
{
    public class UIManager : MonoBehaviour
    {
        private GameMgr gameMgr;
        // [SerializeField] private GameObject micIcon;
        // [SerializeField] private TMP_Text micTxt = null;

        // [SerializeField] private GameObject shopPanel = null;
        
        // private ShopItemSlot[] itemSlots;
        // [SerializeField] private GameObject rankPanel = null;

        // [SerializeField] private GameObject inventoryPanel = null;
        // [SerializeField] private Transform slotsBackground = null;
        // private InventorySlot[] inventorySlots;
        // private List<Item> tempItemList;

        // public Transform baseTransform;
        // public RectTransform arrowRectTransform;
        // public float initialAngleOffset = 90f;

        // private PlayerRef playerRef;

        # region - Outfits -
        [SerializeField] public PlayerOutfitsHandler playerImg;
        public void UpdatedOutfits(NetworkArray<string> outfits)
        {
            playerImg.Init();
            UpdatedOutfits(playerImg, outfits);
        }

        public void UpdatedOutfits(PlayerOutfitsHandler outfitsHandler, NetworkArray<string> outfits)
        {
            var i = 0;
            foreach(var resolver in outfitsHandler.resolverList)
            {
                outfitsHandler.ChangeOutfit(resolver.GetCategory(),outfits[i]);
                i+=1;
            }
        }

        public void UpdatedColor(NetworkArray<Color> colorList)
        {
            UpdatedColor(playerImg, colorList);
        }

        public void UpdatedColor(PlayerOutfitsHandler outfitsHandler, NetworkArray<Color> colorList)
        {
            outfitsHandler.SetSkinColor(colorList[0]);
            outfitsHandler.SetHairColor(colorList[1]);
        }
        #endregion
        
        private void Start()
        {
            gameMgr = GameMgr.Instance;
            // inventorySlots = slotsBackground.GetComponentsInChildren<InventorySlot>();
        }

        // public void InitializeItemSlots(PlayerNetworkData playerNetworkData)
        // {
        //     itemSlots = shopPanel.GetComponentsInChildren<ShopItemSlot>();
        //     foreach (var slot in itemSlots)
        //     {
        //         slot.Initialize(playerNetworkData);
        //     }
        // }

        // #region - Minimap -
        // public void UpdateMinimapArrow(Transform playerTransform)
        // {
        //     Vector3 direction = playerTransform.position - baseTransform.position;
        
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - initialAngleOffset;

        //     arrowRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // }
        // #endregion

        #region - PlayerNetworkData UI -
        [SerializeField] Slider HPSlider;
        [SerializeField] private TMP_Text HPTxt;
        public void UpdateHPSlider(int HP, int maxHP)
        {
            HPSlider.value = HP;
            HPTxt.text = $"HP: {HP}/{maxHP}";
        }

        [SerializeField] Slider durabilitySlider;
        [SerializeField] private TMP_Text durabilityTxt;
        public void UpdateDurabilitySlider(int durability, int maxDurability)
        {
            durabilitySlider.value = durability;
            durabilityTxt.text = $"耐久度: {durability}/{maxDurability}";
        }

        [SerializeField] Slider foodSlider;
        [SerializeField] private TMP_Text foodTxt;
        public void UpdateFoodSlider(int food, int maxFood)
        {
            foodSlider.value = food;
            foodTxt.text = $"食物: {food}/{maxFood}";
        }

        [SerializeField] private TMP_Text bulletAmountTxt;
        public void UpdateBulletAmountTxt(int bulletAmount, int maxbulletAmount)
        {
            bulletAmountTxt.text = $"{bulletAmount}";
        }

        [SerializeField] private TextMeshProUGUI playerCoinAmount;
        public void UpdateCoinAmountTxt(int coinAmount)
        {
            // playerCoinAmount.SetText(coinAmount.ToString());
        }

        // public void UpdateMicIconColor(int isSpeaking)
        // {
        //     Image micIconImage = micIcon.GetComponent<Image>();

        //     if (isSpeaking == 0)
        //     {
        //         micIconImage.color = Color.green; // Local player speaking
        //     }
        //     else if (isSpeaking == 1)
        //     {
        //         micIconImage.color = Color.blue; // Other player speaking
        //     }
        //     else
        //     {
        //         micIconImage.color = Color.gray; // Default color when not speaking
        //     }
        // }

        // public void UpdateMicTxt(string playerName)
        // {
        //     if(playerName == "none")
        //     {
        //         micTxt.text = " ";
        //     }
        //     else
        //     {
        //         micTxt.text = $"說話中: {playerName}";
        //     }
        // }
        #endregion

        // #region - Open panels -
        // public void OnOpenInventoryButton()
        // {
        //     inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        // }

        // public void OnOpenShopButton()
        // {
        //     shopPanel.SetActive(!shopPanel.activeSelf);
        // }

        // public void CloseShopPanel()
        // {
        //     shopPanel.SetActive(false);
        // }

        // public void OnOpenTeamListButton()
        // {
        //     teamListPanel.SetActive(!teamListPanel.activeSelf);
        // }

        // public void OnOpenRankButton()
        // {
        //     rankPanel.SetActive(!rankPanel.activeSelf);

        //     if(rankPanel.activeInHierarchy)
        //     {
        //         gamePlayManager.AddRankNo(playerRef);
        //     }
        // }

        // public void OnOrganizeButton()
        // {
        //     OrganizeInventory(tempItemList);
        // }
        // #endregion 

        // #region - Inventory -

        // public void SetItemList(List<Item> items)
        // {
        //     tempItemList = items;
        // }

        // public void OrganizeInventory(List<Item> items)
        // {
        //     // Create dictionary to store item and amount in stack
        //     Dictionary<Item.ItemType, int> stackedItems = new Dictionary<Item.ItemType, int>();
        //     List<Item> placeholderItems = new List<Item>();

        //     foreach (Item item in items)
        //     {
        //         // If item is a Placeholder, add it to the placeholderItems list
        //         if (item.itemId >= 5 && item.itemId <= 13)
        //         {
        //             placeholderItems.Add(item);
        //         }
        //         else
        //         {
        //             // If item in dict, add amount
        //             if (stackedItems.ContainsKey(item.itemType))
        //             {
        //                 stackedItems[item.itemType] += item.quantity;
        //             }
        //             else
        //             {
        //                 // Else, add item in dict with its amount
        //                 stackedItems.Add(item.itemType, item.quantity);
        //             }
        //         }
        //     }

        //     // Clear items list
        //     items.Clear();

        //     // Add stackedItems into items
        //     foreach (KeyValuePair<Item.ItemType, int> kvp in stackedItems)
        //     {
        //         Item.ItemType itemType = kvp.Key;
        //         int quantity = kvp.Value;

        //         // Create new item
        //         // Slightly error here
        //         Item stackedItem = new Item
        //         {
        //             itemType = itemType,
        //             itemId = (int)itemType,
        //             quantity = quantity
        //         };

        //         // Add into items
        //         items.Add(stackedItem);
        //     }

        //     // Add placeholder items to the final items list
        //     foreach (Item placeholderItem in placeholderItems)
        //     {
        //         items.Add(placeholderItem);
        //     }

        //     UpdateInventoryUI(items);

        //     gamePlayManager.AddOrganizeNo(playerRef);
        // }

        // public void UpdateInventoryUI(List<Item> items)
        // {
        //     // Loop through all the slots
        //     for (int i = 0; i < inventorySlots.Length; i++)
        //     {
        //         // Add item if there is an item to add
        //         if (i < items.Count)
        //         {
        //             inventorySlots[i].AddItem(items[i]);

        //             if (items[i].quantity > 1)
        //             {
        //                 inventorySlots[i].ShowAmountText();
        //             }
        //             else
        //             {
        //                 inventorySlots[i].itemAmount.text = " ";
        //             }
        //         } 
        //         else
        //         {
        //             // Otherwise clear the slot
        //             inventorySlots[i].ClearSlot();
        //         }
        //     }
        // }

        // // Test for debug
        // public string ShowList(List<Item> items)
        // {
        //     string result = "Inventory: ";

        //     for(int i=0; i < items.Count; i++)
        //     {
        //         result += "ItemType: " + items[i].itemType + "; Quantity: " + items[i].quantity;
        //     }

        //     return result;
        // }

        // #endregion
    }
}