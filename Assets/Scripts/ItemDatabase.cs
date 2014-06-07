using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    //List of all available items within the game

    public List<Item> items = new List<Item>();

    //contains all item declarations to be used within the game, it should be noted that the items
    //icon must be named exactly the same way the item itself is named

    private void Start()
    {
        var m4a1SopMod = new Item
        {
            itemID = 2,
            ItemName = "M4",
            ItemDesc = "An M4 Assault Rifle. Aim and Fire.",
            TheItemType = Item.ItemType.Weapon,
            GamePrefab = Resources.Load("M4A1 Sopmod"),
            WeaponKeyBind = 1,
        };
        var ak47 = new Item
        {
            itemID = 3,
            ItemName = "AK",
            ItemDesc = "An AK Assault Rifle. Aim and Fire.",
            TheItemType = Item.ItemType.Weapon,
            GamePrefab = Resources.Load("Ak-47"),
            WeaponKeyBind = 2,
        };
        var testAmmo = new Item
        {
            itemID = 4,
            ItemName = "Ammo",
            ItemDesc = "Ammo for a gun, shoot it at zombies.",
            TheItemType = Item.ItemType.Ammo,
            GamePrefab = Resources.Load("AK47Mag")
        };

        var medKit = new Item
        {
            itemID = 5,
            ItemName = "MedKit",
            ItemDesc = "Contains numerous different medical supplies.",
            TheItemType = Item.ItemType.Healing,
            ItemRecovAmount = 30,
            GamePrefab = Resources.Load("MedKit")
        };

        var woodPlank = new Item
        {
            itemID = 6,
            ItemName = "WoodPlank",
            ItemDesc = "A plank of wood.",
            TheItemType = Item.ItemType.Crafting,
            GamePrefab = Resources.Load("WoodPlank")
        };

        items.AddRange(new[]
        {
            m4a1SopMod,
            ak47,
            testAmmo,
            medKit,
            woodPlank
        });
    }
}