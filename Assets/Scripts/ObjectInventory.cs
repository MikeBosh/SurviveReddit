using System.Collections.Generic;
using UnityEngine;

public class ObjectInventory : MonoBehaviour
{
    private readonly List<Item> mSlots = new List<Item>();
    public bool IsInventoryShowing;
    public List<Vector2> ItemsContained;
    public int KeyID;
    public bool Locked;
    public int ScreenPosX;
    public int ScreenPosY;
    public GUISkin Skin;
    public int SlotDimension;
    public int SlotsX;
    public int SlotsY;
    private Inventory mInventory;
    private bool mIsShowing;

    private ItemDatabase mItemDatabase;
    private GameObject mPlayer;
    private PlayerController mPlayerController;
    private bool mShowTooltip;
    private string tooltip;

    private void Start()
    {
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mPlayerController = mPlayer.GetComponent<PlayerController>();
        mInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        mItemDatabase = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();

        for (int i = 0; i < ItemsContained.Count; i++)
        {
            for (int j = 0; j < mItemDatabase.items.Count; j++)
            {
                if (mItemDatabase.items[j].itemID == ItemsContained[i].x)
                {
                    mSlots.Add(mItemDatabase.items[j]);

                    if (mItemDatabase.items[j].TheItemType == Item.ItemType.Ammo)
                    {
                        for (int k = 0; k < mSlots.Count; k++)
                        {
                            if (mSlots[k].itemID == ItemsContained[i].x)
                            {
                                mSlots[k].ItemQuantity = (int) ItemsContained[i].y;
                            }
                        }
                    }
                }
            }
        }
    }


    private void Update()
    {
        if (Vector3.Distance(mPlayer.transform.position, gameObject.transform.position) > mPlayerController.UseDistance)
        {
            IsInventoryShowing = false;
        }

        if (Input.GetButtonDown("Inventory"))
        {
            IsInventoryShowing = false;
        }
    }

    private void OnGUI()
    {
        //initialize tooltip

        tooltip = "";

        //initialize GUI Skin

        GUI.skin = Skin;

        //Calls the inventory draw method when showInventory is true

        if (IsInventoryShowing)
        {
            DrawInventory();

            if (mShowTooltip)
            {
                GUI.Box(new Rect(Event.current.mousePosition.x + 15, Event.current.mousePosition.y, 200, 200), tooltip, Skin.GetStyle("Tooltip"));
            }
        }
    }

    private void DrawInventory()
    {
        Event e = Event.current;

        //variable initialized to represent current inventory slot within the loop

        int i = 0;

        //loops through until all rows have been created

        for (int y = 0; y < SlotsY; y++)
        {
            //loops through untill all the collumns have been created, this draws the inventory out from
            //left to right, top to bottom, as is the general standard in games

            for (int x = 0; x < SlotsX; x++)
            {
                //Rect that defines the position and dimensions of each inventory slot

                var slotRect = new Rect(ScreenPosX + (x*SlotDimension), ScreenPosY + (y*SlotDimension), SlotDimension, SlotDimension);

                //inventory slots are created and skinned, no text is added within them

                GUI.Box(slotRect, "", Skin.GetStyle("Slot"));

                if (mSlots[i].ItemName != null)
                {
                    //draws the items icon within the currect slot

                    GUI.DrawTexture(slotRect, mSlots[i].ItemIcon);

                    //add quantity text to box

                    if (mSlots[i].ItemQuantity != 0)
                    {
                        string quantityText = mSlots[i].ItemQuantity.ToString();

                        GUI.Label(slotRect, quantityText);
                    }
                    //checks if the player is currently moused over the slot

                    if (slotRect.Contains(e.mousePosition))
                    {
                        //creates the tooltip based on the item within the slot, then sets it to display

                        tooltip = mInventory.CreateTooltip(mSlots[i]);
                        mShowTooltip = true;

                        if (e.button == 1 && e.type == EventType.mouseDown)
                        {
                            mInventory.AddItem(mSlots[i].itemID, mSlots[i].ItemQuantity);
                            mSlots[i] = new Item();
                        }
                    }
                }
                if (tooltip == "")
                {
                    mShowTooltip = false;
                }
                i++;
            }
        }
    }

    //Draw the inventory
    //put items in the slots
    //on right click, add the item to the inventory and remove the item from the slot
    //Close the inventory on Tab or if the distance between this object and the player becomes too great.
}