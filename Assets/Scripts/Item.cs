using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Item
{
    //used to define the type of the item
    public enum ItemType
    {
        Weapon,
        Food,
        Drink,
        Healing,
        Armor,
		Ammo,
		Crafting
    }

    public Object GamePrefab;
    public string ItemDesc;
    public Texture2D ItemIcon;
    public int ItemQuantity;
    public int ItemRecovAmount;
    public ItemType TheItemType;
    public int itemID;
    private string mItemName;
    public int WeaponKeyBind;

    public string ItemName
    {
        get { return mItemName; }
        set
        {
            mItemName = value;
            ItemIcon = Resources.Load<Texture2D>("Item Icons/" + value);
        }
    }
}