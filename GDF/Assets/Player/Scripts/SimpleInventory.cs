using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
    [SerializeField]
    private Dictionary<string, InventoryItem> _inventoryItems = new Dictionary<string, InventoryItem>();
    [SerializeField]
    private GameObject _panel;

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="inventoryItem">The item in question</param>
    public void AddItem(InventoryItem inventoryItem)
    {
        //If we already have one, we just add the quantity, useless for this game, but will probably reuse this later
        if (_inventoryItems.ContainsKey(inventoryItem.itemName))
        {
            _inventoryItems[inventoryItem.itemName].quantity += inventoryItem.quantity;
        }
        else
        {
            _inventoryItems.Add(inventoryItem.itemName, inventoryItem);
            UpdateUI();
        }
    }


    /// <summary>
    /// Takes an item from the player (like a baby)
    /// </summary>
    /// <param name="itemName">The item name you are looking for. (Not the droids)</param>
    public void RemoveItem(string itemName)
    {
        if (_inventoryItems.ContainsKey(itemName))
        {
            _inventoryItems.Remove(itemName);
            UpdateUI();
        }
    }

    /// <summary>
    /// Checks if the player has a specific item
    /// </summary>
    /// <param name="itemName">The item name you are looking for. (Not the droids)</param>
    /// <returns></returns>
    public bool CheckItem(string itemName)
    {
        if(_inventoryItems.ContainsKey(itemName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateUI()
    {
        if(_panel != null)
        {
            RectTransform panelrt = _panel.GetComponent<RectTransform>();

            //Clear all the childre, not the right way, but the quick and dirty way. 
            if (panelrt.childCount > 0)
            {
                int items = panelrt.childCount;

                RectTransform[] rts = panelrt.GetComponentsInChildren<RectTransform>();
                
                foreach(RectTransform rt in rts)
                {
                    if (rt.gameObject.tag != "UI")
                    {
                        Destroy(rt.gameObject);
                    }
                }
            }

            //Loop through the items and add them to the UI
            foreach(KeyValuePair<string, InventoryItem> kvp in _inventoryItems)
            {
                GameObject go = new GameObject("InventoryItem");

                //remove the base transform, I assume new gameobjects have it. 
                if (go.GetComponent<Transform>() != null)
                {
                    Destroy(go.GetComponent<Transform>());
                }

                //add the shit we need to the gameobject
                go.AddComponent(typeof(RectTransform));
                go.AddComponent(typeof(CanvasRenderer));
                go.AddComponent(typeof(Image));

                //set the sprite
                Image image = go.GetComponent<Image>();
                image.sprite = kvp.Value.itemIcon;

                go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                go.GetComponent<RectTransform>().SetParent(panelrt, false);
            }
        }
    }
}
