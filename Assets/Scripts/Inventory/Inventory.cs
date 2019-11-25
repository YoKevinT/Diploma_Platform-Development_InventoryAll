using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Linear
{
    public class Inventory : MonoBehaviour
    {
        #region Variables
        public KeyCode OpenInventory = KeyCode.Q;

        public GUISkin invSkin;
        public GUIStyle boxStyle;
        public GUIStyle buttonStyle;

        public List<Item> inv = new List<Item>(); // List of Items
        public Item selectedItem;
        public bool showInv;

        public Vector2 scr;
        public Vector2 scrollPos;

        public int money;
        public string sortType = "";
        public Transform dropLocation;
        private int[] randomItemID = { 0, 1, 2, 100, 101, 102, 200, 201, 202, 300, 301, 302, 400, 401, 402,
                                     500, 501, 502, 600, 601, 602, 700, 701, 702, 800,801,802,900,901,902 };

        [System.Serializable]
        public struct equipment
        {
            public string name;
            public Transform location;
            public GameObject curItem;
        };
        public equipment[] equipmentSlots;
        #endregion
        #region Start
        private void Start()
        {
            ///showInv = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Have them in the inventory already
            inv.Add(ItemData.CreateItem(0));
            inv.Add(ItemData.CreateItem(100));
            inv.Add(ItemData.CreateItem(200));
            inv.Add(ItemData.CreateItem(300));
            inv.Add(ItemData.CreateItem(400));
            inv.Add(ItemData.CreateItem(500));
            inv.Add(ItemData.CreateItem(600));
            inv.Add(ItemData.CreateItem(700));
            inv.Add(ItemData.CreateItem(800));
            inv.Add(ItemData.CreateItem(900));
        }
        #endregion
        #region Update
        private void Update()
        {
            if (Input.GetKey(KeyCode.I))
            {
                inv.Add(ItemData.CreateItem(randomItemID[Random.Range(0, randomItemID.Length)]));
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                inv[4].AMOUNT += 3;
            }

            if (Input.GetKeyDown(OpenInventory))
            {
                if (showInv)
                {
                    showInv = false;
                    Time.timeScale = 1;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    return;
                }
                else
                {
                    showInv = true;
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    return;
                }
            }
        }
        #endregion
        #region ONGUI
        private void OnGUI()
        {
            if (showInv)
            {
                // Make that more efficient by only calling when you need to
                scr.x = Screen.width / 16;
                scr.y = Screen.height / 9;

                GUI.Box(new Rect(0, 0, scr.x * 8, Screen.height), "");
                if (GUI.Button(new Rect(4f * scr.x, 0, scr.x, 0.25f * scr.y), "All"))
                {
                    sortType = "All";
                }
                if (GUI.Button(new Rect(5f * scr.x, 0, scr.x, 0.25f * scr.y), "Food"))
                {
                    sortType = "Food";
                }
                if (GUI.Button(new Rect(6f * scr.x, 0, scr.x, 0.25f * scr.y), "Armour"))
                {
                    sortType = "Armour";
                }
                if (GUI.Button(new Rect(7f * scr.x, 0, scr.x, 0.25f * scr.y), "Weapon"))
                {
                    sortType = "Weapon";
                }
                if (GUI.Button(new Rect(8f * scr.x, 0, scr.x, 0.25f * scr.y), "Ingredient"))
                {
                    sortType = "Ingredient";
                }
                Display();
                if (selectedItem != null)
                {
                    // Position of Name, Box for Picture, Picture and Description
                    GUI.Box(new Rect(4.375f * scr.x, 0.25f * scr.y, 3 * scr.y, 0.25f * scr.y), selectedItem.NAME, boxStyle);
                    GUI.skin = invSkin;
                    GUI.Box(new Rect(4.375f * scr.x, 0.55f * scr.y, 3 * scr.y, 3 * scr.y), selectedItem.ICON);
                    GUI.Box(new Rect(4.375f * scr.x, 3.6f * scr.y, 3 * scr.y, 3 * scr.y), selectedItem.DESCRIPTION);
                    GUI.skin = null;
                    ItemUse();
                }
                else
                {
                    { return; }
                }
            }
        }
        #endregion
        #region Display
        void Display()
        {
            if (!(sortType == "All" || sortType == ""))
            {
                ItemType type = (ItemType)System.Enum.Parse(typeof(ItemType), sortType);
                int a = 0; // amount of that type
                int s = 0;//slot position
                for (int i = 0; i < inv.Count; i++)
                {
                    if (inv[i].TYPE == type)//find our type
                    {
                        a++;//increase for each item it finds
                    }
                }
                if (a <= 34)
                {
                    for (int i = 0; i < inv.Count; i++)
                    {
                        if (inv[i].TYPE == type)
                        {
                            if (GUI.Button(new Rect
                        (0.5f * scr.x, 0.25f * scr.y + s * (0.25f * scr.y),
                        3 * scr.x, 0.25f * scr.y), inv[i].NAME))
                            {
                                selectedItem = inv[i];
                            }
                            s++;
                        }
                    }
                }
                else
                {
                    scrollPos = GUI.BeginScrollView(new Rect(0, 0.25f * scr.y, 3.75f * scr.x, 8.5f * scr.y), scrollPos, new Rect(0, 0, 0, 8.5f * scr.y + ((a - 34) * (0.25f * scr.y))), false, true);

                    for (int i = 0; i < inv.Count; i++)
                    {
                        if (inv[i].TYPE == type)
                        {
                            if (GUI.Button(new Rect
                        (0.5f * scr.x, 0 * scr.y + s * (0.25f * scr.y),
                        3 * scr.x, 0.25f * scr.y), inv[i].NAME))
                            {
                                selectedItem = inv[i];
                            }
                            s++;
                        }
                    }
                    GUI.EndScrollView();
                }
            }
            else
            {
                if (inv.Count <= 34)//if we have 34 or less (space at top and bottom)
                {
                    for (int i = 0; i < inv.Count; i++)
                    {
                        if (GUI.Button(new Rect
                        (0.5f * scr.x, 0.25f * scr.y + i * (0.25f * scr.y),
                        3 * scr.x, 0.25f * scr.y), inv[i].NAME))
                        {
                            selectedItem = inv[i];
                        }
                    }
                }
                else//more than 34 items
                {
                    //our movable scroll position
                    scrollPos =
                    //the start of our viewable area
                    GUI.BeginScrollView(
                    //our View Window
                    new Rect(0, 0.25f * scr.y, 3.75f * scr.x, 8.5f * scr.y),
                    //our current scroll position
                    scrollPos,
                    //scroll zone (extra space)
                    new Rect(0, 0, 0, 8.5f * scr.y + ((inv.Count - 34) * (0.25f * scr.y))),
                    //can we see the horizontal bar?
                    false,
                    //can we see the vertical bar?
                    true);

                    for (int i = 0; i < inv.Count; i++)
                    {
                        if (GUI.Button(new Rect
                        (0.5f * scr.x, 0 * scr.y + i * (0.25f * scr.y),
                        3 * scr.x, 0.25f * scr.y), inv[i].NAME))
                        {
                            selectedItem = inv[i];
                        }
                    }

                    //End the scroll space
                    GUI.EndScrollView();
                }
            }
        }
        #endregion
        #region ItemUse
        void ItemUse()
        {
            switch (selectedItem.TYPE)
            {
                case ItemType.Ingredient:
                    break;
                case ItemType.Potion:
                    break;
                case ItemType.Food:
                    break;
                case ItemType.Scroll:
                    break;
                case ItemType.Armour:
                    if (equipmentSlots[0].curItem == null || selectedItem.NAME != equipmentSlots[0].curItem.name)
                    {
                        if (GUI.Button(new Rect(4.75f * scr.x, 8 * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                        {
                            if (equipmentSlots[0].curItem != null)
                            {
                                Destroy(equipmentSlots[1].curItem);
                            }
                            GameObject curItem = Instantiate(selectedItem.ITEMMESH, equipmentSlots[0].location);
                            equipmentSlots[0].curItem = curItem;
                            curItem.name = selectedItem.NAME;
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(4.75f * scr.x, 8 * scr.y, scr.x, 0.25f * scr.y), "Unequip"))
                        {
                            Destroy(equipmentSlots[0].curItem);
                        }
                    }
                    break;
                case ItemType.Weapon:
                    if (equipmentSlots[1].curItem == null || selectedItem.NAME != equipmentSlots[1].curItem.name)
                    {
                        if (GUI.Button(new Rect(4.75f * scr.x, 8 * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                        {
                            if (equipmentSlots[1].curItem != null)
                            {
                                Destroy(equipmentSlots[1].curItem);
                            }
                            GameObject curItem = Instantiate(selectedItem.ITEMMESH, equipmentSlots[1].location);
                            equipmentSlots[1].curItem = curItem;
                            curItem.name = selectedItem.NAME;
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(4.75f * scr.x, 8 * scr.y, scr.x, 0.25f * scr.y), "Unequip"))
                        {
                            Destroy(equipmentSlots[1].curItem);
                        }
                    }
                    break;
                case ItemType.Craftable:
                    break;
                case ItemType.Money:
                    break;
                case ItemType.Quest:
                    break;
                case ItemType.Misc:
                    break;
            }
            if (GUI.Button(new Rect(scr.x * 5.3f, scr.y * 6.8f, scr.x, scr.y), "Discard", buttonStyle))
            {
                for (int i = 0; i < equipmentSlots.Length; i++)
                {
                    // Check equiped items, if yea delete
                    if (equipmentSlots[i].curItem != null && selectedItem.NAME == equipmentSlots[i].curItem.name)
                    {
                        Destroy(equipmentSlots[i].curItem);
                    }
                }
                // Spawn in front
                GameObject droppedItem = Instantiate(selectedItem.ITEMMESH, dropLocation.position, Quaternion.identity);
                // Set Name
                droppedItem.name = selectedItem.NAME;
                // Give Gravity
                droppedItem.AddComponent<Rigidbody>().useGravity = true;
                // Reduce or delete
                if(selectedItem.AMOUNT > 1)
                {
                    selectedItem.AMOUNT--;
                }
                else
                {
                    inv.Remove(selectedItem);
                    selectedItem = null;
                }
                return;
            }
        }
        #endregion
    }
}