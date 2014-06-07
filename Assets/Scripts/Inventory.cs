using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private ItemDatabase database; //current item database being used
    private Item draggedItem; //holds the current dragged items information
    private bool draggingItem; //true when an item is being dragged within the inventory
    public List<Item> inventory = new List<Item>(); //Players current inventory
    private bool inventoryFull; //boolean value to represent all slots being filled with non-null items
    private PlayerController mPlayerController;
    private int maxSlots; //maximum number of slots within the grid
    private CameraSmooth mouseLook;
    private int prevIndex; //holds the current dragged items previous index location for swapping
    public int screenPosX;
    public int screenPosY;
    private bool showInventory; //shows the inventory on the screen when true
    private bool showTooltip; //shows the current items tooltip when true
    public GUISkin skin; //GUISkin to be chosen through the inspector, used to select UI Skin prefab
    public int slotDimention;
    public List<Item> slots = new List<Item>(); //Players inventory slots
    public int slotsX, slotsY; //number of slots within the grid on the x and y axis
    private string tooltip; //item tooltip information
    private VitalsController vitalsController;

    internal bool IsInventoryShowing
    {
        get { return showInventory; }
        set
        {
            showInventory = value;
            mouseLook.enabled = !showInventory;
            Screen.lockCursor = !Screen.lockCursor;
        }
    }

    // Use this for initialization
    private void Start()
    {
        mouseLook = Camera.main.GetComponent<CameraSmooth>();
        vitalsController = GetComponent<VitalsController>();

        maxSlots = slotsX*slotsY; //finds the current maximum slots

        //Adds null items to both the slots and inventory list, until filled

        for (int i = 0; i < (slotsX*slotsY); i++)
        {
            slots.Add(new Item());
            inventory.Add(new Item());
        }

        //initialize database

        database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();
    }

    private void Update()
    {
        //calls up the inventory screen

        if (Input.GetButtonDown("Inventory"))
        {
            IsInventoryShowing = !IsInventoryShowing;
        }
    }


    private void OnGUI()
    {
        //initialize tooltip

        tooltip = "";

        //initialize GUI Skin

        GUI.skin = skin;

        //Calls the inventory draw method when showInventory is true

        if (IsInventoryShowing)
        {
            DrawInventory();

            if (showTooltip)
            {
                GUI.Box(new Rect(Event.current.mousePosition.x + 15, Event.current.mousePosition.y, 200, 200), tooltip, skin.GetStyle("Tooltip"));
            }
        }

        //Places the item icon on the mouse cursor when an item is being dragged

        if (draggingItem)
        {
            GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 50, 50), draggedItem.ItemIcon);
        }
    }

    //Draws the inventory on the screen

    private void DrawInventory()
    {
        Event e = Event.current;

        //variable initialized to represent current inventory slot within the loop

        int i = 0;

        //loops through until all rows have been created

        for (int y = 0; y < slotsY; y++)
        {
            //loops through untill all the collumns have been created, this draws the inventory out from
            //left to right, top to bottom, as is the general standard in games

            for (int x = 0; x < slotsX; x++)
            {
                //Rect that defines the position and dimensions of each inventory slot

                var slotRect = new Rect(screenPosX + (x*slotDimention), screenPosY + (y*slotDimention), slotDimention, slotDimention);

                //inventory slots are created and skinned, no text is added within them

                GUI.Box(slotRect, "", skin.GetStyle("Slot"));

                //the slots list is set as a reference to the inventory list

                slots[i] = inventory[i];

                //checks if the current slot has a non-null item within it

                if (slots[i].ItemName != null)
                {
                    //draws the items icon within the currect slot

                    GUI.DrawTexture(slotRect, slots[i].ItemIcon);

                    //add quantity text to box

                    if (slots[i].TheItemType == Item.ItemType.Ammo)
                    {
                        if (slots[i].ItemQuantity <= 0)
                        {
                            inventory[i] = new Item();
                        }
                    }

                    if (slots[i].ItemQuantity != 0)
                    {
                        string quantityText = slots[i].ItemQuantity.ToString();

                        GUI.Label(slotRect, quantityText);
                    }
                    //checks if the player is currently moused over the slot

                    if (slotRect.Contains(e.mousePosition))
                    {
                        //creates the tooltip based on the item within the slot, then sets it to display

                        tooltip = CreateTooltip(slots[i]);
                        showTooltip = true;

                        //checks if the left mouse button has been pressed down and the mouse has been dragged
                        //and if no item is currently being dragged

                        if (e.button == 0 && e.type == EventType.mouseDrag && !draggingItem)
                        {
                            //sets draggingitem to true and records the index of the slot the item was in
                            //sets the draggedItem variable to the item in the currently selected slot
                            //changes the current slot to a null item

                            draggingItem = true;
                            prevIndex = i;
                            draggedItem = slots[i];
                            inventory[i] = new Item();
                        }

                        //checks if the mouse button has been released and if an item is being dragged

                        if (e.type == EventType.mouseUp && draggingItem)
                        {
                            //sets the item within the previous slot to the item in the current slot
                            //then sets the current slot to the item being dragged
                            //resets dragging item to false and the current dragged item to null

                            inventory[prevIndex] = inventory[i];
                            inventory[i] = draggedItem;
                            draggingItem = false;
                            draggedItem = null;
                        }

                        if (e.button == 1 && e.type == EventType.mouseDown)
                        {
                            switch (slots[i].TheItemType)
                            {
                                case Item.ItemType.Healing:
                                    if (vitalsController.CoreVitals.Blood < 100)
                                    {
                                        vitalsController.CoreVitals.Blood += slots[i].ItemRecovAmount;
                                        inventory[i] = new Item();
                                    }
                                    break;
                                default:
                                    print("Unknown item type case detected!");
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    //if the item is null, checks if the current mouse position is over the current slot

                    if (slotRect.Contains(e.mousePosition))
                    {
                        //places the dragged item into the current slot and resets the dragging and dragged items
                        //the previous slot is already a new null item, so it does not need to be set

                        if (e.type == EventType.mouseUp && draggingItem)
                        {
                            inventory[i] = draggedItem;
                            draggingItem = false;
                            draggedItem = null;
                        }
                    }
                }


                //checks if the current tooltip is empty and sets it to no longer show

                if (tooltip == "")
                {
                    showTooltip = false;
                }

                //increments 'i' with the 'x', to move to the next item in the list

                i++;
            }
        }

        //if an item is dragged out of the inventory, it is then dropped or destroyed

        if (draggingItem && e.type == EventType.mouseUp)
        {
            //draggedItem can now be passed into a 'dropped item' method, to throw the item into the game world
            mPlayerController.DropItem(draggedItem.GamePrefab, draggedItem.ItemQuantity);
            draggingItem = false;
            draggedItem = null;
        }
    }

    //method for creating the tooltip for the current item

    public string CreateTooltip(Item item)
    {
        tooltip = "<color=#ffffff>" + item.ItemName + "</color>\n\n" + item.ItemDesc;
        return tooltip;
    }

    public void RemoveOneItemFromStack(int id)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemID == id)
            {
                inventory[i].ItemQuantity -= 1;
            }
        }
    }

    //method for removing an item from the inventory based on the items id

    public void RemoveItem(int id)
    {
        //loop to search for the first item containing the items id and replace it with a null item

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemID == id)
            {
                inventory[i] = new Item();
                break;
            }
        }
    }

    //method to add a new item to the inventory

    public void AddItem(int id, int quantity)
    {
        //checks if the inventory is currently full

        if (!InventoryFilled())
        {
            if (InventoryContains(id) && quantity > 0)
            {
                for (int k = 0; k < inventory.Count; k++)
                {
                    if (inventory[k].itemID == id && inventory[k].TheItemType == Item.ItemType.Ammo)
                    {
                        inventory[k].ItemQuantity += quantity;
                        break;
                    }
                }
            }
            else
            {
                //searched for the first null item in the inventory and replaces it with an item matching the provided id

                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].ItemName == null)
                    {
                        for (int j = 0; j < database.items.Count; j++)
                        {
                            if (database.items[j].itemID == id)
                            {
                                inventory[i] = database.items[j];

                                if (inventory[i].TheItemType == Item.ItemType.Ammo)
                                {
                                    inventory[i].ItemQuantity = quantity;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            //inventory is full message, item is not added
            print("Inventory is full");
        }
    }

    public void SetPlayer(PlayerController playerController)
    {
        mPlayerController = playerController;
    }

    //method to check if an item is within the inventory by the item id

    public bool InventoryContains(int id)
    {
        bool result = false;

        for (int i = 0; i < inventory.Count; i++)
        {
            result = inventory[i].itemID == id;
            if (result)
            {
                break;
            }
        }
        return result;
    }

    //method to check if the iventory is current filled

    private bool InventoryFilled()
    {
        int curFilled = 0;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemName != null)
            {
                curFilled++;
            }
        }
        return curFilled >= maxSlots;
    }
}